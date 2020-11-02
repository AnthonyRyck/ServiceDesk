using System;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Windows
{
    public class RemoteRegistry
    {
        #region Fields

        private const string KEY_CLASSES = "Classes";
        private const string KEY_DEFAULT = ".DEFAULT";

        private readonly ILogger _logger;

        private readonly ConnectionOptions _connectionOption;

        private uint HKEY_CLASSES_ROOT = 0x80000000;
        private uint HKEY_CURRENT_USER = 0x80000001;
        private uint HKEY_LOCAL_MACHINE = 0x80000002;
        private uint HKEY_USERS = 0x80000003;

        #endregion

        #region Constructeur

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="logger"></param>
        /// <param name="loginAdmin">Inclure le nom de Domaine (NomDeDomaine\login)</param>
        /// <param name="password"></param>
        public RemoteRegistry(string loginAdmin, string password, ILogger logger)
        {
            _logger = logger;

            _connectionOption = new ConnectionOptions();
            _connectionOption.Username = loginAdmin;
            _connectionOption.Password = password;
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Permet de retourner la liste des valeurs dans un chemin donnée.
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public Task<string> GetValueInUserAsync(string hostName, string subKey, string valueName)
        {
            return Task.Factory.StartNew(() =>
            {
                ManagementScope scope = new ManagementScope("\\\\" + hostName + "\\root\\CIMV2", _connectionOption);
                scope.Connect();

                ManagementClass registry = new ManagementClass(scope, new ManagementPath("StdRegProv"), null);

                ManagementBaseObject inParams = registry.GetMethodParameters("GetStringValue");
                inParams.SetPropertyValue("hDefKey", 0x80000003);
                inParams.SetPropertyValue("sSubKeyName", subKey);
                inParams.SetPropertyValue("sValueName", valueName);

                return registry.InvokeMethod("GetStringValue", inParams, null).Properties["sValue"]?.Value?.ToString();
            });
        }

        /// <summary>
        /// Permet de retourner la liste des valeurs dans un chemin donnée.
        /// </summary>
        /// <param name="subKey"></param>
        /// <returns></returns>
        public Task<string[]> GetValuesInUserAsync(string hostName, string subKey)
        {
            return Task.Factory.StartNew(() =>
            {
                ManagementScope scope = new ManagementScope("\\\\" + hostName + "\\root\\CIMV2", _connectionOption);
                scope.Connect();

                ManagementClass registry = new ManagementClass(scope, new ManagementPath("StdRegProv"), null);

                ManagementBaseObject inParams = registry.GetMethodParameters("EnumValues");
                inParams.SetPropertyValue("hDefKey", 0x80000003);
                inParams.SetPropertyValue("sSubKeyName", subKey);

                return (string[])registry.InvokeMethod("EnumValues", inParams, null).Properties["sNames"].Value;
            });
        }

        /// <summary>
        /// Permet de retourner la liste des keys dans un chemin donnée.
        /// </summary>
        /// <param name="subKey"></param>
        /// <returns></returns>
        public Task<string[]> GetKeysInUserAsync(string hostName, string subKey)
        {
            return Task.Factory.StartNew(() =>
            {
                ManagementScope scope = new ManagementScope("\\\\" + hostName + "\\root\\CIMV2", _connectionOption);
                scope.Connect();

                ManagementClass registry = new ManagementClass(scope, new ManagementPath("StdRegProv"), null);

                ManagementBaseObject inParams = registry.GetMethodParameters("EnumKey");
                inParams.SetPropertyValue("hDefKey", 0x80000003);
                inParams.SetPropertyValue("sSubKeyName", subKey);

                return (string[])registry.InvokeMethod("EnumKey", inParams, null).Properties["sNames"].Value;
            });
        }

        /// <summary>
        /// Permet d'ajout une valeur dans le chemin donnée en paramètre.
        /// </summary>
        /// <param name="subKey">Chemin ou mettre la valeur.</param>
        /// <param name="valueName">Nom de la valeur : exemple MANIFEST</param>
        /// <param name="value">Valeur à mettre. Ex : le chemin de Nemo.</param>
        /// <returns></returns>
        public Task AddValueInUserAsync(string hostName, string subKey, string valueName, string value)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    ManagementScope scope = new ManagementScope("\\\\" + hostName + "\\root\\CIMV2", _connectionOption);
                    scope.Connect();

                    ManagementClass registry = new ManagementClass(scope, new ManagementPath("StdRegProv"), null);

                    ManagementBaseObject inParams = registry.GetMethodParameters("SetStringValue");
                    inParams.SetPropertyValue("hDefKey", 0x80000003);
                    inParams.SetPropertyValue("sSubKeyName", subKey);
                    inParams.SetPropertyValue("sValueName", valueName);
                    inParams.SetPropertyValue("sValue", value);

                    registry.InvokeMethod("SetStringValue", inParams, null);

                    _logger.Success("Ajout/Mis à jour de la valeur " + valueName);
                }
                catch (Exception)
                {
                    _logger.Error("Erreur sur l'ajout ou mise à jour de la valeur " + valueName);
                }
            });
        }

        /// <summary>
        /// Vérifie dans le HKEY_USER, pour un utilisateur précis, la présence de la clé donné.
        /// </summary>
        /// <param name="cleUtilisateur"></param>
        /// <param name="sousKeyDeRecherche"></param>
        /// <param name="valueNameToTest"></param>
        /// <returns></returns>
        public async Task<bool> KeyExistInUserAsync(string hostName, string cleUtilisateur, string sousKeyDeRecherche, params string[] valueNameToTest)
        {
            string[] splitKeyPath = sousKeyDeRecherche.Split('\\');
            string pathToTest = cleUtilisateur;
            bool isGood = true;

            // Pour chacun des éléments (Software \ Microsoft \ Office \ Outlook \ Addins \ NeMo.Outlook), test que la key d'en dessous
            // existe.
            for (int i = 0; i < splitKeyPath.Length; i++)
            {
                pathToTest += (@"\" + splitKeyPath[i]);

                if (i + 1 < splitKeyPath.Length)
                {
                    string[] tempKeys = await GetKeysInUserAsync(hostName, pathToTest);

                    string keyToTest = splitKeyPath[i + 1];
                    if (!tempKeys.Any(x => x.Equals(keyToTest, StringComparison.OrdinalIgnoreCase)))
                    {
                        _logger.Error("La chemin " + pathToTest + @"\" + keyToTest + " n'existe pas.");
                        isGood = false;
                        break;
                    }
                }
            }

            if (isGood)
            {
                // Test sur les valeurs.
                string[] allValues = await GetValuesInUserAsync(hostName, pathToTest);

                foreach (var item in valueNameToTest)
                {
                    if (!allValues.Contains(item))
                    {
                        _logger.Error("La valeur " + item + " n'existe pas");
                        isGood = false;
                    }
                }
            }

            if (!isGood)
                _logger.Error("Vérifier l'installation de Nemo.");

            return isGood;
        }

        /// <summary>
        /// Vérifie dans le HKEY_USER, que l'utilisateur est dans la base de registre
        /// </summary>
        /// <param name="sidUser">SID de l'utilisateur. Ex : S-1-5-21-2255225037-4143705525-1198626713-69600</param>
        /// <returns></returns>
        public async Task<bool> IsUserSidKeyExistAsync(string hostName, string sidUser)
        {
            return await GetKeysInUserAsync(hostName, sidUser) != null;
        }


        #endregion

        #region Private Methods

        //NOTE : Nous n'avons pas à modifier la clé de registre !
        //private void UpdateKey(string user, RegistryKey remoteHklm, string subKey, string valueName, string value)
        //{
        //    RegistryKey keyCherche = remoteHklm.OpenSubKey(user)
        //                        .OpenSubKey(subKey, true);

        //    // Dans le cas ou la clé existe.
        //    if (keyCherche != null)
        //    {
        //        SetInformation("sous clé présente");
        //        SetInformation("-->" + keyCherche.Name);

        //        if (keyCherche.GetValueNames().Any(x => x == valueName))
        //        {
        //            string valeurCle = keyCherche.GetValue(valueName).ToString();

        //            SetInformation("Clé --" + valueName + "-- PRESENTE.");
        //            SetInformation("Valeur : " + valeurCle);

        //            _compteRendu.AddCompteRendu("- Clé " + valueName + " présente");
        //        }
        //        else
        //        {
        //            SetError("Clé --" + valueName + "-- absente");
        //            SetError("de " + keyCherche.Name);

        //            SetInfoGood("Ajout de --" + valueName + "-- avec la valeur : " + value);
        //            keyCherche.SetValue(valueName, value);
        //            SetInfoGood("Ajout terminé.");

        //            _compteRendu.AddCompteRendu("- Ajout de " + valueName + " avec la valeur : " + value);
        //        }
        //    }
        //}

        private bool IsUserKeyValid(string key, string ssid)
        {
            // TODO: Voir pour mettre un REGEX pour tester la clé user.
            // ex : S-1-5-21-1713742965-1672955568-1479694994-1001

            if (key == KEY_DEFAULT)
                return false;

            if (key.Contains(KEY_CLASSES))
                return false;

            if (key == ssid)
                return true;

            return false;
        }

        #endregion
    }
}
