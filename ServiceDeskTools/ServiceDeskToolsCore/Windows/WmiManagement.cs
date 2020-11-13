using ServiceDeskToolsCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Windows
{
    public class WmiManagement
    {
        #region Properties

        //private readonly ManagementScope _scope;
        private readonly ILogger _logger;

        private const string WQL_OPERATING_SYSTEM = "SELECT version FROM Win32_OperatingSystem";

        private readonly ConnectionOptions _connectionOption;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur sans machine.
        /// </summary>
        /// <param name="loginAdmin">Login avec le nom de domaine "DOMAINE\"</param>
        /// <param name="password"></param>
        /// <param name="logger"></param>
        public WmiManagement(string loginAdmin, string password, ILogger logger)
        {
            _connectionOption = new ConnectionOptions();
            _connectionOption.Username = loginAdmin;
            _connectionOption.Password = password;

            _logger = logger;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Vérification de l'application qui répond au critère de présence et de version.
        /// </summary>
        /// <param name="applicationName">Nom de l'application recherché</param>
        /// <param name="queryWql">Requete WQL pour trouver l'application.</param>
        /// <param name="versionSeuil">Version seuil que l'application ne doit pas être en dessous</param>
        /// <returns></returns>
        public Task<bool> VerificationApplicationAsync(string machineName, string applicationName, string queryWql, string versionSeuil)
        {
            return Task.Factory.StartNew(() =>
            {
                string messageRetour = string.Empty;
                bool isAppOk = false;

                try
                {
                    ObjectQuery query = new ObjectQuery(queryWql);
                    
                    var scope = CreateScope(machineName);
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                    
                    using (var tempCollection = searcher.Get())
                    {
                        if (tempCollection.Count == 0)
                        {
                            _logger.Error(applicationName + " n'est pas présent");
                        }
                        else
                        {
                            foreach (var item in tempCollection)
                            {
                                // Test sur la date d'installation.
                                //string dateInstall = item["InstallDate"].ToString().Split('.').First();
                                //DateTime dateInstallation = DateTime.ParseExact(dateInstall,
                                //                                                "yyyyMMddHHmmss",
                                //                                                CultureInfo.InvariantCulture);

                                // Test sur Version
                                // si versionSeuil = NULL - pas de test sur la version.
                                if (!string.IsNullOrEmpty(versionSeuil))
                                {
                                    string versionApplication = item["Version"].ToString();
                                    string message = "--> " + applicationName + " : " + versionApplication;

                                    Version versionInstalle = new Version(versionApplication);
                                    Version versionToTest = new Version(versionSeuil);

                                    if (versionInstalle.CompareTo(versionToTest) < 0)
                                    {
                                        _logger.Error(message);
                                        isAppOk = false;
                                    }
                                    else
                                    {
                                        _logger.Success(message);
                                        isAppOk = true;
                                    }
                                }
                                else
                                {
                                    _logger.Success("--> " + applicationName + " PRESENT");
                                    isAppOk = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Erreur sur le test de l'application : " + applicationName);
                    _logger.Error("Message d'erreur : " + ex.Message);
                    isAppOk = false;
                }

                return isAppOk;
            });
        }

        /// <summary>
        /// Verifie l'existance des répertoires.
        /// </summary>
        /// <param name="queryWql"></param>
        /// <returns></returns>
        public Task<bool> IsDirectoryExistAsync(string machineName, string queryWql)
        {
            return Task.Factory.StartNew(() =>
            {
                string messageRetour = string.Empty;
                bool isDirectoryExists = false;

                try
                {
                    ObjectQuery query = new ObjectQuery(queryWql);

                    var scope = CreateScope(machineName);
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                    using (var tempCollection = searcher.Get())
                    {
                        if (tempCollection.Count == 0)
                        {
                            isDirectoryExists = false;
                        }
                        else
                        {
                            isDirectoryExists = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    isDirectoryExists = false;
                    _logger.Error("Erreur sur la recherche du répertoire.");
                    _logger.Error("Message erreur : " + ex.Message);
                }

                return isDirectoryExists;
            });
        }

        /// <summary>
        /// Permet de récupérer la version de l'OS.
        /// </summary>
        /// <returns></returns>
        public Task<string> GetOperatingSystemAsync(string machineName)
        {
            return Task.Factory.StartNew(() =>
            {
                string versionOs = string.Empty;

                ObjectQuery query = new ObjectQuery(WQL_OPERATING_SYSTEM);

                var scope = CreateScope(machineName);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                using (var tempCollection = searcher.Get())
                {
                    if (tempCollection.Count == 0)
                    {
                        _logger.Error("Impossible de trouver la version de l'OS.");
                    }
                    else
                    {
                        foreach (var item in tempCollection)
                        {
                            versionOs = item["Version"].ToString();
                        }
                    }
                }

                return versionOs;
            });
        }

        /// <summary>
        /// Récupère la liste de logiciel installé.
        /// </summary>
        /// <returns></returns>
        public Task<List<Software>> GetListSofware(string machineName)
        {
            return Task.Factory.StartNew(() =>
            {
                string messageRetour = string.Empty;
                bool isAppOk = false;

                List<Software> softwares = new List<Software>();

                try
                {
                    string queryWql = "Select * from Win32_Product";
                    ObjectQuery query = new ObjectQuery(queryWql);

                    var scope = CreateScope(machineName);
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                    using (var tempCollection = searcher.Get())
                    {
                        if (tempCollection.Count == 0)
                        {
                            _logger.Error("Aucune application n'est présente");
                        }
                        else
                        {
                            foreach (var item in tempCollection)
                            {
                                var temp = item;

                                // Test sur la date d'installation.
                                //string dateInstall = item["InstallDate"].ToString().Split('.').First();
                                //DateTime dateInstallation = DateTime.ParseExact(dateInstall,
                                //                                                "yyyyMMddHHmmss",
                                //                                                CultureInfo.InvariantCulture);

                                //// Test sur Version
                                //// si versionSeuil = NULL - pas de test sur la version.
                                //if (!string.IsNullOrEmpty(versionSeuil))
                                //{
                                //    string versionApplication = item["Version"].ToString();
                                //    string message = "--> " + applicationName + " : " + versionApplication;

                                //    Version versionInstalle = new Version(versionApplication);
                                //    Version versionToTest = new Version(versionSeuil);

                                //    if (versionInstalle.CompareTo(versionToTest) < 0)
                                //    {
                                //        _logger.Error(message);
                                //        isAppOk = false;
                                //    }
                                //    else
                                //    {
                                //        _logger.Success(message);
                                //        isAppOk = true;
                                //    }
                                //}
                                //else
                                //{
                                //    _logger.Success("--> " + applicationName + " PRESENT");
                                //    isAppOk = true;
                                //}
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Erreur sur la récupération de la liste des applications : ");
                }

                return softwares;
            });
        }

        public Task VerificationServiceAsync(string machineName, string serviceName)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string requete = @$"Select Status,State from Win32_Service  where Name='{serviceName}'";
                    ObjectQuery query = new ObjectQuery(requete);

                    var scope = CreateScope(machineName);
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                    using (var tempCollection = searcher.Get())
                    {
                        if (tempCollection.Count == 0)
                        {
                            _logger.Error("Le service : " + serviceName + " n'est pas présent");
                        }
                        else
                        {
                            foreach (var item in tempCollection)
                            {
                                _logger.Success("Service : " + serviceName + " - Status : " + item["Status"].ToString());
                                _logger.Success("Service : " + serviceName + " - Etat : " + item["State"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Erreur sur le test du service : " + serviceName);
                    _logger.Error("Message d'erreur : " + ex.Message);
                }
            });
        }

        

        #endregion

        #region Private methods


        private ManagementScope CreateScope(string machineName)
        {
            return new ManagementScope(@"\\" + machineName + @"\ROOT\cimv2", _connectionOption);
        }

        #endregion
    }
}
