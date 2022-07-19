using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;

namespace Webbr.Extensions
{
    public interface IWebbrDatabase
    {
        Task<List<T>> QueryAsync<T>(string query, object param = null); // QueryAsync - возвращает значение IEnumerable - используется для Select
        Task ExecuteAsync(string query, object param = null); // ExecuteAsync - возвращает значение integer - используется для Insert, Update, Delete
        Task TransactionAsync(string query, IEnumerable list = null); // TransactionAsync - используется для проведения транзакций
        
        Task<List<T>> OracleQueryAsync<T>(string query); // OracleQueryAsync - запрос Oracle
        Task<List<T>> OracleQueryAsyncConnection<T>(string conn, string query); // OracleQueryAsyncConnection - не стандартный запрос Oracle
    }

    public class WebbrDatabase : IWebbrDatabase
    {
        #region Environments
        private static readonly string EnvMysqlConnectionString = Environment.GetEnvironmentVariable("WEBBR_MYSQL_CONN", EnvironmentVariableTarget.Process);
        private static readonly string EnvOracleConnectionString = Environment.GetEnvironmentVariable("WEBBR_ORACLE_CONN", EnvironmentVariableTarget.Process);
        #endregion

        
        #region QueryAsync
        public async Task<List<T>> QueryAsync<T>(string query, object param = null)
        {
            using (var db = new MySqlConnection(EnvMysqlConnectionString))
            {
                var rec = await db.QueryAsync<T>(query, param);
                var record = rec.ToList();
                await db.CloseAsync();
                return record;
            }
        }
        #endregion

        #region ExecuteAsync
        public async Task ExecuteAsync(string query, object param = null)
        {
            using (var db = new MySqlConnection(EnvMysqlConnectionString))
            {
                try
                {
                    await db.ExecuteAsync(query, param);
                    await db.CloseAsync();
                }
                catch
                {
                    // ignored
                }
            }
        }
        #endregion

        #region TransactionAsync
        public async Task TransactionAsync(string query, IEnumerable value = null)
        {
            using (var db = new MySqlConnection(EnvMysqlConnectionString))
            {
                try
                {
                    await db.OpenAsync();
                    var transaction = await db.BeginTransactionAsync();
                    if (value != null) await db.ExecuteAsync(query, value, transaction);
                    else await db.ExecuteAsync(query, transaction);
                    transaction.Commit();
                    await db.CloseAsync();
                }
                catch
                {
                    // ignored
                }
            }
        }
        #endregion


        #region OracleQueryAsync
        public async Task<List<T>> OracleQueryAsync<T>(string query)
        {
            using (var db = new OracleConnection(EnvOracleConnectionString))
            {
                var rec = await db.QueryAsync<T>(query);
                var record = rec.ToList();
                db.Close();
                db.Dispose();
                return record;
            }
        }
        #endregion
        
        #region OracleQueryAsyncConnection
        public async Task<List<T>> OracleQueryAsyncConnection<T>(string conn, string query)
        {
            using (var db = new OracleConnection(conn))
            {
                var rec = await db.QueryAsync<T>(query);
                var record = rec.ToList();
                db.Close();
                db.Dispose();
                return record;
            }
        }
        #endregion
    }
}