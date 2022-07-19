# -*- coding: utf-8 -*-
import atexit
import tools.cli as cli
from pyVim import connect
from pyVmomi import vmodl
from pyVmomi import vim
import pymysql

db = pymysql.connect(host='localhost', user='root', passwd='drsteel1', db='webbr', charset='utf8')
curs = db.cursor()

def get_vm_info(vm):
    args = cli.get_args()
    summary = vm.summary

    vmUuid = summary.config.instanceUuid  # Идентификатор VM
    if vmUuid:
        hypervisorIp = args.host
        vmCluster = vm.runtime.host.parent.name
        vmName = summary.config.name  # Имя VM
        vmGuestos = summary.config.guestFullName  # Примерная версия ОС VM
        vmState = summary.runtime.powerState
        vmHost = vm.runtime.host.name  # IP-адрес хоста, на котором расположена VM
        vmAnnotation = summary.config.annotation  # notes, описание VM

        summary = vm.summary

        if summary.guest is not None:  # Информация о VM
            vmIpaddress = summary.guest.ipAddress  # IP-адрес VM
            # vmToolsstatus = summary.guest.toolsStatus  # Состояние vmWare Tools на VM
            # vmToolsversion = vm.guest.toolsVersion  # Версия vmWare Tools на VM
            vmDnsname = summary.guest.hostName  # DNS имя VM

        vmMemory = summary.config.memorySizeMB # Количество выделенной оперативной памяти для VM
        vmCpu = summary.config.numCpu # Количество выделенных ядер CPU для VM

        try:
            sql = "INSERT INTO `virtualservers` (`vmUuid`, `hypervisorIp`, `vmCluster`, `vmName`, `vmGuestos`, `vmState`, `vmHost`, `vmAnnotation`, `vmIpaddress`, `vmDnsname`, `vmCpu`, `vmMemory`) " + \
                "VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s) " + \
                "ON DUPLICATE KEY UPDATE `hypervisorIp` = %s, `vmCluster` = %s, `vmName` = %s, `vmGuestos` = %s, `vmState` = %s, `vmHost` = %s, `vmAnnotation` = %s, `vmIpaddress` = %s, `vmDnsname` = %s, `vmCpu` = %s, `vmMemory` = %s"
            curs.execute(sql, (
                vmUuid, hypervisorIp, vmCluster, vmName, vmGuestos, vmState, vmHost, vmAnnotation, vmIpaddress, vmDnsname, vmCpu, vmMemory,
                hypervisorIp, vmCluster, vmName, vmGuestos, vmState, vmHost, vmAnnotation, vmIpaddress, vmDnsname, vmCpu, vmMemory))
        except Exception as e:
            print(str(e))



def main():
    args = cli.get_args()

    sql = "DELETE FROM virtualservers WHERE `hypervisorIp`=%s"
    curs.execute(sql, (args.host))

    try:
        service_instance = connect.SmartConnectNoSSL(host=args.host, user=args.user, pwd=args.password, port=int(args.port))
        atexit.register(connect.Disconnect, service_instance)
        content = service_instance.RetrieveContent()
        container = content.rootFolder
        viewType = [vim.VirtualMachine]
        recursive = True
        containerView = content.viewManager.CreateContainerView(container, viewType, recursive)

        children = containerView.view
        for child in children:
            get_vm_info(child)

        db.commit()
        curs.close()

    except vmodl.MethodFault as error:
        print("Caught vmodl fault : " + error.msg)
        return -1
    return 0


if __name__ == "__main__":
    main()