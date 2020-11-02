using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace ServiceDeskToolsCore.Windows
{
    public class Gpupdate
    {
        /// <summary>
        /// Permet de lancer la commande : "GPUPDATE /force" sur une machine.
        /// </summary>
        /// <param name="machinename"></param>
        /// <param name="loginAdmin"></param>
        /// <param name="pass"></param>
        public void UpdateGPO(string machinename, string loginAdmin, string pass)
        {
            try
            {
                ConnectionOptions connectionOptions = new ConnectionOptions();

                connectionOptions.Username = loginAdmin;
                connectionOptions.Password = pass;
                connectionOptions.Impersonation = ImpersonationLevel.Impersonate;

                ManagementScope scope = new ManagementScope(@$"\\{machinename}\root\CIMV2", connectionOptions);

                scope.Connect();

                ManagementClass clas = new ManagementClass(scope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                ManagementBaseObject inparams = clas.GetMethodParameters("Create");
                inparams["CommandLine"] = "cmd /c GPUpdate /force";

                ManagementBaseObject outparam = clas.InvokeMethod("Create", inparams, null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ResultGPO(string machinename, string loginAdmin, string pass)
        {
            try
            {
                ConnectionOptions connectionOptions = new ConnectionOptions();

                connectionOptions.Username = loginAdmin;
                connectionOptions.Password = pass;
                connectionOptions.Impersonation = ImpersonationLevel.Impersonate;

                ManagementScope scope = new ManagementScope(@$"\\{machinename}\root\CIMV2", connectionOptions);

                scope.Connect();

                ManagementClass clas = new ManagementClass(scope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                ManagementBaseObject inparams = clas.GetMethodParameters("Create");
                inparams["CommandLine"] = @"cmd /c GPResult /r /user c.lenert > c:\temp\gpresult.txt";

                ManagementBaseObject outparam = clas.InvokeMethod("Create", inparams, null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
