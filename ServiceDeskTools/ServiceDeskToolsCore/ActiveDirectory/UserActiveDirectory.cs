using ServiceDeskToolsCore.Exceptions;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.ActiveDirectory
{
	public class UserActiveDirectory
	{
		#region Properties

		private PrincipalContext _adConnection;

		public readonly string AdressActiveDirectory;

		public ILogger Logger { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Contructeur par défaut.
        /// </summary>
        /// <param name="login">Login sur l'AD</param>
        /// <param name="password">Mot de passe pour avoir accès à l'AD</param>
        public UserActiveDirectory(string adresseAD, string login, string password)
		{
			AdressActiveDirectory = adresseAD;
			_adConnection = new PrincipalContext(ContextType.Domain, AdressActiveDirectory, login, password);
		}

		public UserActiveDirectory(string adresseAD, string login, string password, ILogger logger)
			: this(adresseAD, login, password)
		{
			Logger = logger;
		}

		#endregion

		#region Public methods

		#region User

		/// <summary>
		/// Permet d'avoir le SID d'un utilisateur.
		/// </summary>
		/// <param name="user">UserPrincipal</param>
		/// <returns></returns>
		public Task<string> GetUserSid(UserPrincipal user)
		{
			return Task.Factory.StartNew(() =>
			{
				string sid = string.Empty;

				try
				{
					Logger?.Debug("GetUserSid - utilisateur : " + user.Name);

					if (user != null)
					{
						sid = user.Sid.ToString();
					}
				}
				catch (Exception exception)
				{
					Logger?.Error("Error sur GetUserSid : " + user.Name, exception);
					return sid;
				}

				Logger?.Info("SID : " + sid);
				return sid;
			});
		}

		/// <summary>
		/// Permet d'avoir le SID d'un utilisateur.
		/// </summary>
		/// <param name="userName">Nom de l'utilisateur (ex: jean.dupont)</param>
		/// <returns></returns>
		public Task<string> GetUserSid(string userName)
		{
			return Task.Factory.StartNew(() =>
			{
				string sid = string.Empty;

				try
				{
					Logger?.Debug("GetUserSid - utilisateur : " + userName);

					UserPrincipal user = GetUserPrincipal(userName);
					if (user != null)
					{
						sid = user.Sid.ToString();
					}
				}
				catch (Exception exception)
				{
					Logger?.Error("Error sur GetUserSid : " + userName, exception);
					return sid;
				}

				Logger?.Info("SID : " + sid);
				return sid;
			});
		}

		/// <summary>
		/// Permet d'avoir les informations de l'utilisateur
		/// </summary>
		/// <param name="name">Nom utilisateur : jean.dupont ou j.dupont ou j.dupont1</param>
		public UserPrincipal GetUser(string name)
		{
			UserPrincipal userPrincipal = null;

			try
			{
				Logger?.Debug("GetUser - name : " + name);

				userPrincipal = GetUserPrincipal(name);
			}
			catch (Exception exception)
			{
				Logger?.Error("Error sur GetUser : " + name, exception);
			}

			return userPrincipal;
		}

		/// <summary>
		/// Retourne ou l'utilisateur est.
		/// </summary>
		/// <param name="userName">Nom d'utilisateur</param>
		public IEnumerable<string> GetUserGroups(string userName)
		{
			List<Principal> retourGroups = new List<Principal>();

			try
			{
				UserPrincipal user = GetUserPrincipal(userName);
				if (user != null)
				{
					var tempGroups = user.GetGroups();
					retourGroups = tempGroups.Select(x => x).ToList();
				}
				else
				{
					Logger?.Warn("Aucun utilisateur de trouvé avec : " + userName);
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Erreur sur GetUserGroup - nom utilisateur : " + userName, exception);
				Logger?.Warn("Possible que votre login Admin/MDP ne soit pas bon, ou valide.");
			}

			return retourGroups?.Select(x => x.Name).ToList();
		}

		/// <summary>
		/// Retourne ou l'utilisateur est.
		/// </summary>
		/// <param name="userName">Nom d'utilisateur</param>
		public IEnumerable<string> GetUserGroups(UserPrincipal user)
		{
			List<Principal> retourGroups = new List<Principal>();

			try
			{
				if (user != null)
				{
					var tempGroups = user.GetGroups().ToList();
					//retourGroups = tempGroups.ToList();
				}
				else
				{
					Logger?.Warn("Aucun utilisateur de trouvé avec : " + user.Name);
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Erreur sur GetUserGroup - nom utilisateur : " + user.Name, exception);
				Logger?.Warn("Possible que votre login Admin/MDP ne soit pas bon, ou valide.");
			}

			return retourGroups?.Select(x => x.Name).ToList();
		}

		/// <summary>
		/// Fait un test pour savoir si l'utilisateur est dans le groupe donné.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public bool IsUserMemberOf(string userName, string groupName)
		{
			var groupeUser = GetUserGroups(userName);
			return groupeUser.Any(x => groupName == x);
		}

		/// <summary>
		/// Fait un test pour savoir si l'utilisateur est dans le groupe donné.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public bool IsUserMemberOf(UserPrincipal user, string groupName)
		{
			var groupeUser = GetUserGroups(user);
			return groupeUser.Any(x => groupName == x);
		}

		/// <summary>
		/// Ajout un utilisateur dans un groupe donné.
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="userName"></param>
		public void AddUserToGroup(string groupName, string userName)
		{
			try
			{
				GroupPrincipal group = GroupPrincipal.FindByIdentity(_adConnection, IdentityType.Name, groupName);

				if (group != null)
				{
					using (group)
					{
						UserPrincipal user = GetUserPrincipal(userName);
						if (user != null)
						{
							if (!group.Members.Contains(user))
							{
								Logger?.Info("Ajout de " + userName + " dans le groupe " + groupName);
								group.Members.Add(user);
								group.Save();
							}
							else
							{
								Logger?.Warn(userName + " est déjà dans le groupe " + groupName);
								throw new UserExistInGroupException(userName + " est déjà dans le groupe " + groupName);
							}
						}
						else
						{
							Logger?.Warn("L'utilisateur : " + userName + " n'existe pas !");
							throw new UserNotExistException("L'utilisateur : " + userName + " n'existe pas !");
						}
					}
				}
				else
				{
					Logger?.Warn("Le groupe : " + groupName + " n'existe pas !");
					throw new GroupActiveDirectoryNotExistException("Le groupe : " + groupName + " n'existe pas !");
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Error AddUserToGroup - groupName : " + groupName
				+ " - userName : " + userName, exception);

				throw;
			}
		}

		/// <summary>
		/// Ajout un utilisateur dans un groupe donné.
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="user"></param>
		public void AddUserToGroup(string groupName, UserPrincipal user)
		{
			try
			{
				GroupPrincipal group = GroupPrincipal.FindByIdentity(_adConnection, IdentityType.Name, groupName);

				if (group != null)
				{
					if (!group.Members.Contains(user))
					{
						Logger?.Info("Ajout de " + user.Name+ " dans le groupe " + groupName);
						group.Members.Add(user);
						group.Save();
						group.Dispose();
					}
					else
					{
						Logger?.Warn(user.Name + " est déjà dans le groupe " + groupName);
						throw new UserExistInGroupException(user.Name + " est déjà dans le groupe " + groupName);
					}
				}
				else
				{
					Logger?.Warn("Le groupe : " + groupName + " n'existe pas !");
					throw new GroupActiveDirectoryNotExistException("Le groupe : " + groupName + " n'existe pas !");
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Error AddUserToGroup - groupName : " + groupName
				+ " - userName : " + user.Name, exception);

				throw;
			}
		}

		/// <summary>
		/// Ajout un utilisateur dans un groupe donné.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="user"></param>
		public void AddUserToGroup(GroupPrincipal group, UserPrincipal user)
		{
			try
			{
				if (!group.Members.Contains(user))
				{
					Logger?.Info("Ajout de " + user.Name + " dans le groupe " + group.Name);
					group.Members.Add(user);
					group.Save();
				}
				else
				{
					Logger?.Warn(user.Name + " est déjà dans le groupe " + group.Name);
					throw new UserExistInGroupException(user.Name + " est déjà dans le groupe " + group.Name);
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Error AddUserToGroup - groupName : " + group.Name
				+ " - userName : " + user.Name, exception);

				throw;
			}
		}

		/// <summary>
		/// Retire un utilisateur dans un groupe donné.
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="userName"></param>
		public void RemoveUserToGroup(string groupName, string userName)
		{
			try
			{
				GroupPrincipal group = GroupPrincipal.FindByIdentity(_adConnection, IdentityType.Name, groupName);

				if (group != null)
				{
					UserPrincipal user = GetUserPrincipal(userName);
					if (user != null)
					{
						if (group.Members.Contains(user))
						{
							Logger?.Info("Suppresion de " + userName + " du groupe " + groupName);

							group.Members.Remove(user);
							group.Save();
							group.Dispose();
						}
						else
						{
							Logger?.Warn(userName + " n'est pas dans le groupe " + groupName);
							throw new UserNotInGroupException(userName + " n'est pas dans le groupe " + groupName);
						}
					}
					else
					{
						Logger?.Warn("L'utilisateur : " + userName + " n'existe pas !");
						throw new UserNotExistException("L'utilisateur : " + userName + " n'existe pas !");
					}
				}
				else
				{
					Logger?.Warn("Le groupe : " + groupName + " n'existe pas !");
					throw new GroupActiveDirectoryNotExistException("Le groupe : " + groupName + " n'existe pas !");
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Error RemoveUserToGroup - groupName : " + groupName
				+ " - userName : " + userName, exception);

				throw;
			}
		}

		/// <summary>
		/// Retire un utilisateur dans un groupe donné.
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="user"></param>
		public void RemoveUserToGroup(string groupName, UserPrincipal user)
		{
			try
			{
				GroupPrincipal group = GroupPrincipal.FindByIdentity(_adConnection, IdentityType.Name, groupName);

				if (group != null)
				{
					if (group.Members.Contains(user))
					{
						Logger?.Info("Suppresion de " + user.Name + " du groupe " + groupName);

						group.Members.Remove(user);
						group.Save();
						group.Dispose();
					}
					else
					{
						Logger?.Warn(user.Name + " n'est pas dans le groupe " + groupName);
						throw new UserNotInGroupException(user.Name + " n'est pas dans le groupe " + groupName);
					}
				}
				else
				{
					Logger?.Warn("Le groupe : " + groupName + " n'existe pas !");
					throw new GroupActiveDirectoryNotExistException("Le groupe : " + groupName + " n'existe pas !");
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Error RemoveUserToGroup - groupName : " + groupName
				+ " - userName : " + user.Name, exception);

				throw;
			}
		}

		/// <summary>
		/// Retire un utilisateur dans un groupe donné.
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="user"></param>
		public void RemoveUserToGroup(GroupPrincipal group, UserPrincipal user)
		{
			try
			{
				if (group.Members.Contains(user))
				{
					Logger?.Info("Suppresion de " + user.Name + " du groupe " + group.Name);

					group.Members.Remove(user);
					group.Save();
				}
				else
				{
					Logger?.Warn(user.Name + " n'est pas dans le groupe " + group.Name);
					throw new UserNotInGroupException(user.Name + " n'est pas dans le groupe " + group.Name);
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Error RemoveUserToGroup - groupName : " + group.Name
				+ " - userName : " + user.Name, exception);

				throw;
			}
		}

		#endregion

		/// <summary>
		/// Récupère la liste des groupes pour une machine.
		/// </summary>
		/// <param name="machineName"></param>
		/// <returns></returns>
		public IEnumerable<string> GetComputerGroupes(string machineName)
		{
			IEnumerable<Principal> retourGroups = null;

			try
			{
				ComputerPrincipal computer = GetComputer(machineName);

				if (computer != null)
				{
					var tempGroups = computer.GetGroups();
					retourGroups = tempGroups.Select(x => x).ToList();
				}
			}
			catch (Exception)
			{
				throw;
			}

			return retourGroups?.Select(x => x.Name).ToList();
		}

		/// <summary>
		/// Ajout un utilisateur dans un groupe donné.
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="userName"></param>
		public void AddComputerToGroup(string groupName, string computerName)
		{
			try
			{
				using (GroupPrincipal group = GroupPrincipal.FindByIdentity(_adConnection, IdentityType.Name, groupName))
				{
					if (group != null)
					{
						ComputerPrincipal computer = ComputerPrincipal.FindByIdentity(_adConnection, IdentityType.Name, computerName);
						if (computer != null)
						{
							Logger?.Info("Ajout de PC : " + computerName + " dans le groupe " + groupName);
							group.Members.Add(computer);
							group.Save();
						}
						else
						{
							Logger?.Warn("Le PC : " + computerName + " n'existe pas !");
						}
					}
					else
					{
						Logger?.Warn("Le groupe : " + groupName + " n'existe pas !");
					}
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Error AddComputerToGroup - groupName : " + groupName
				+ " - computerName : " + computerName, exception);
			}
		}

		/// <summary>
		/// Ajout une machine dans un groupe donné.
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="userName"></param>
		public void AddComputerToGroup(string groupName, ComputerPrincipal computer)
		{
			try
			{
				using (GroupPrincipal group = GroupPrincipal.FindByIdentity(_adConnection, IdentityType.Name, groupName))
				{
					if (group != null)
					{
						if (computer != null)
						{
							Logger?.Info("Ajout de PC : " + computer.Name + " dans le groupe " + groupName);
							group.Members.Add(computer);
							group.Save();
						}
						else
						{
							Logger?.Warn("Le PC : " + computer.Name + " n'existe pas !");
						}
					}
					else
					{
						Logger?.Warn("Le groupe : " + groupName + " n'existe pas !");
					}
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Error AddComputerToGroup - groupName : " + groupName
				+ " - computerName : " + computer.Name, exception);
			}
		}

		/// <summary>
		/// Retire une machine dans un groupe donné.
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="computerName"></param>
		public void RemoveComputerToGroup(string groupName, string computerName)
		{
			try
			{
				using (GroupPrincipal group = GroupPrincipal.FindByIdentity(_adConnection, IdentityType.Name, groupName))
				{
					if (group != null)
					{
						ComputerPrincipal computer = ComputerPrincipal.FindByIdentity(_adConnection, IdentityType.Name, computerName);
						if (computer != null)
						{
							Logger?.Info("Ajout de PC : " + computerName + " dans le groupe " + groupName);
							group.Members.Remove(computer);
							group.Save();
						}
						else
						{
							Logger?.Warn("Le PC : " + computerName + " n'existe pas !");
						}
					}
					else
					{
						Logger?.Warn("Le groupe : " + groupName + " n'existe pas !");
					}
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("Error RemoveComputerToGroup - groupName : " + groupName
				+ " - computerName : " + computerName, exception);
			}
		}

		/// <summary>
		/// Vérifie que l'utilisateur existe bien dans l'AD
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public bool IsUserExist(string userName)
		{
			var user = GetUser(userName);
			return user != null;
		}

		/// <summary>
		/// Vérifie que l'utilisateur existe bien dans l'AD.
		/// </summary>
		/// <param name="machineName"></param>
		/// <returns></returns>
		public bool IsComputerExist(string machineName)
		{
			var machine = GetComputer(machineName);
			return machine != null;
		}

		/// <summary>
		/// Supprime le lecteur réseau de l'utilisateur donnée.
		/// </summary>
		public void RemoveLecteurReseau(string userName, string directory)
		{
			var user = GetUser(userName);

			if (user != null)
			{
				if (user.HomeDirectory == directory)
				{
					user.HomeDirectory = null;
					user.HomeDrive = null;
				}
				else
				{
					throw new HomeDirectoryActiveDirectoryDifferentException("Lecteur réseau de l'utilisateur : " + userName + " est : " + (user.HomeDirectory ?? "null"));
				}
			}
			else
			{
				throw new UserNotExistException("Utilisateur " + userName + " n'existe pas");
			}
		}


		public HomeDirectory GetLecteurReseau(string userName)
		{
			var user = GetUser(userName);

			HomeDirectory home = null;

			if (user != null)
			{
				home = new HomeDirectory()
				{
					LettreReseau = user.HomeDrive,
					Directory = user.HomeDirectory
				};
			}
			else
			{
				throw new UserNotExistException("Utilisateur " + userName + " n'existe pas");
			}

			return home;
		}


		public void SetLecteurReseau(string userName, string lettreReseau, string directory)
		{
			var user = GetUser(userName);

			if (user != null)
			{
				user.HomeDrive = lettreReseau;
				user.HomeDirectory = directory;

				user.Save();
			}
			else
			{
				throw new UserNotExistException("Utilisateur " + userName + " n'existe pas");
			}
		}

		/// <summary>
		/// Permet de tester si le groupe en question existe dans l'AD.
		/// </summary>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public Task<bool> IsGroupExist(string groupName)
		{
			return Task.Factory.StartNew(() =>
			{
				GroupPrincipal group = null;

				if (!string.IsNullOrEmpty(groupName))
				{
					group = GroupPrincipal.FindByIdentity(_adConnection, IdentityType.Name, groupName);
				}

				return group != null;
			});
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Permet de chercher un utilisateur dans l'AD sans faire de distinction
		/// entre jean.dupont ou j.dupont
		/// </summary>
		/// <param name="name">Nom de connexion ou son nom d'utilisateur</param>
		/// <returns></returns>
		private UserPrincipal GetUserPrincipal(string name)
		{
			UserPrincipal result = null;

			try
			{
				Logger?.Info("GetUserPrincipal - Recherche de : " + name);

				UserPrincipal userPrincipal = new UserPrincipal(_adConnection);

				// Recherche de l'utilisateur avec son nom (jean.dupont)
				//UserPrincipal user = new UserPrincipal(_adConnection);
				userPrincipal.Name = name;

				using (PrincipalSearcher search = new PrincipalSearcher(userPrincipal))
				{
					result = (UserPrincipal)search.FindOne();
				}

				// Si result est null, c'est pas un nom avec jean.dupont
				if (result == null)
				{
					// Recherche de l'utilisateur avec un nom (j.dupont)
					userPrincipal = new UserPrincipal(_adConnection);
					userPrincipal.SamAccountName = name;

					using (PrincipalSearcher search = new PrincipalSearcher(userPrincipal))
					{
						result = (UserPrincipal)search.FindOne();
					}
				}

				if(result == null)
				{
					// Recherche de l'utilisateur avec un nom (j.dupont)
					userPrincipal = new UserPrincipal(_adConnection);
					userPrincipal.UserPrincipalName = name;

					using (PrincipalSearcher search = new PrincipalSearcher(userPrincipal))
					{
						result = (UserPrincipal)search.FindOne();
					}
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("GetUserPrincipal - name : " + name, exception);
			}

			return result;
		}

		/// <summary>
		/// Récupère la machine.
		/// </summary>
		/// <param name="machineName"></param>
		/// <returns></returns>
		private ComputerPrincipal GetComputer(string machineName)
		{
			ComputerPrincipal result = null;

			try
			{
				Logger?.Info("GetComputer - Recherche de : " + machineName);

				ComputerPrincipal computer = new ComputerPrincipal(_adConnection);
				computer.Name = machineName;

				using (PrincipalSearcher search = new PrincipalSearcher(computer))
				{
					result = (ComputerPrincipal)search.FindOne();
				}
			}
			catch (Exception exception)
			{
				Logger?.Error("GetComputer - machineName : " + machineName, exception);
			}

			return result;
		}



		#endregion
	}
}
