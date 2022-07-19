using System;
using System.Threading;
using System.Threading.Tasks;
using RecurrentTasks;
using Webbr.Extensions;
using Webbr.Models;

namespace Webbr.Tasks
{
    public class NaumenTask : IRunnable
    {
        #region Field
        private readonly IWebbrDatabase _webbrDatabase;
        #endregion

        #region Constructor
        public NaumenTask(IWebbrDatabase webbrDatabase) => _webbrDatabase = webbrDatabase;
        #endregion
        
        
        public async Task RunAsync(ITask currentTask, IServiceProvider scopeServiceProvider, CancellationToken cancellationToken)
        {
            await GetEmployee();
        }


        #region GetEmployee
        private async Task GetEmployee()
        {
            #region Query
            const string naumenQuery = @"
with
    sq_login_group as(
        select sq.login,LISTAGG(sq.caption||' - '||sq.IMPORT_GROUP_ID, '; ') WITHIN GROUP (ORDER BY caption) capname
        from
            (
                select
                    distinct login, dg.caption, dg.IMPORT_GROUP_ID --уникальность у логин группа
                from NC_CORE.MV_REL_PRJ_EMP vrpe
                         join nc_core.REL_GROUP_PROJECTS rgp on rgp.fid_project=vrpe.PROJECT_ID
                         join nc_core.d_groups dg on dg.id_group=rgp.fid_group
                where sysdate between vrpe.active_from and vrpe.active_till --только с активной связкой
                  and dg.import_group_id is not null --только партнеры наумена
            ) sq
        group by sq.login
    ),
    sq_role_employee as
        (
            select login, LISTAGG(vrpe.ROLETITLE, '; ') WITHIN GROUP (ORDER BY vrpe.ROLETITLE) roletitle
            from (select distinct login,ROLETITLE from nc_core.MV_REL_PRJ_EMP where sysdate between active_from and active_till) vrpe
            group by login
        ),
    sq_project as (
        SELECT login, REPLACE(REPLACE(XMLAGG(XMLELEMENT(""a"", decode(vp.name,null,null,vp.name||' - '||vp.direction||';')) ORDER BY vp.name).GETCLOBVAL(),'<a>', ''), '</a>', ' ')
                   AS skills
        FROM NC_CORE.MV_REL_PRJ_EMP skill
                 join nc_core.vw_projects vp on vp.PROJECT_ID=skill.PROJECT_ID and sysdate between skill.active_from and skill.active_till
        GROUP BY login
    )
select emp.id_employee, emp.name AS first_name, emp.patronymic AS middle_name, emp.surname AS last_name, emp.login, emp.department, emp.location_name, pr.profile_name, emp.creation_time, emp.removed,
       capname as projects,
       roletitle AS roles,
       skills
from nc_core.mv_employees emp
         LEFT JOIN nc_core.mv_user_profile pr ON pr.login = emp.login
         LEFT JOIN sq_login_group slg on emp.login=slg.login
         LEFT JOIN sq_role_employee sre on emp.login=sre.login
         LEFT JOIN sq_project sp on emp.login=sp.login";
            #endregion
            
            var naumenEmployeeList = await _webbrDatabase.OracleQueryAsync<NaumenEmployeeDbModel>(naumenQuery);
            
            const string transactionQuery = @"
INSERT naumen_employee (id_employee, first_name, middle_name, last_name, login, creation_time, removed, department, location_name, profile_name, roles, projects, skills)
VALUES(@id_employee, @first_name, @middle_name, @last_name, @login, @creation_time, @removed, @department, @location_name, @profile_name, @roles, @projects, @skills)
ON DUPLICATE KEY UPDATE first_name=@first_name, middle_name=@middle_name, last_name=@last_name, login=@login, creation_time=@creation_time, removed=@removed, department=@department, location_name=@location_name, profile_name=@profile_name, roles=@roles, projects=@projects, skills=@skills";
            await _webbrDatabase.TransactionAsync(transactionQuery, naumenEmployeeList);
        }
        #endregion
    }
}