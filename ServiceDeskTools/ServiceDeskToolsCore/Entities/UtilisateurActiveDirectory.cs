using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Entities.Metier
{
    public class UtilisateurActiveDirectory : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Nom de l'utilisateur (prenom.nom ou p.nom)
        /// </summary>
        public string NomUtilisateur { get; set; }

        /// <summary>
        /// Nom du groupe OU.
        /// </summary>
        public string NomGroupeOU { get; set; }

        /// <summary>
        /// Chemin d'accès du lecteur réseau. Peut être null.
        /// </summary>
        public string LecteurReseau { get; set; }

        /// <summary>
        /// Affiche le résultat.
        /// </summary>
        public string Resultat
        {
            get { return _resultat; }
            set
            {
                _resultat = value;
                OnNotifyPropertyChanged();
            }
        }
        private string _resultat;

        /// <summary>
        /// Donne le statut du résultat.
        /// </summary>
        public ResultatStatut ResultatStatut
        {
            get { return _resultatStatut; }
            set
            {
                _resultatStatut = value;
                OnNotifyPropertyChanged();
            }
        }
        private ResultatStatut _resultatStatut;

        #endregion

        #region Constructeur

        public UtilisateurActiveDirectory()
        {
            ResultatStatut = ResultatStatut.Info;
        }

        #endregion

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnNotifyPropertyChanged([CallerMemberName] string memberName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }

        #endregion
    }


    public enum ActionActiveDirectory
    {
        Ajouter,
        Supprimer,
        AucuneSelection
    }

    public enum ResultatStatut
    {
        Error,
        Warn,
        Ok,
        Info
    }
}
