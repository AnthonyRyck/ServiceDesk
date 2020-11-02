using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.ActiveDirectory
{
    public class HomeDirectory
    {
        /// <summary>
        /// C'est le chemin d'accès
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Lettre utilisé pour l'accès au réseau.
        /// doit être affiché.
        /// </summary>
        public string LettreReseau
        {
            get { return _lettreReseau; }
            set
            {
                if (_lettreReseau != value)
                {
                    _lettreReseau = value;
                    IndexLetter = GetIndexLetter(value);
                }
            }
        }
        private string _lettreReseau;

        public int IndexLetter { get; set; }

        #region Public Methods

        /// <summary>
        /// Permet de changer la valeur de la lettre sans notification
        /// </summary>
        /// <param name="lettre"></param>
        public void SetLettreReseauWithNoNotify(string lettre)
        {
            _lettreReseau = lettre;
        }

        /// <summary>
        /// Permet de changer la valeur du chemin sans notification.
        /// </summary>
        /// <param name="path"></param>
        public void SetDirectoryWithNoNotify(string path)
        {
            Directory = path;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Retourne l'index de la lettre.
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        private int GetIndexLetter(string letter)
        {
            int index;

            switch (letter)
            {
                case "G:":
                    index = 1;
                    break;
                case "H:":
                    index = 2;
                    break;
                case "I:":
                    index = 3;
                    break;
                case "J:":
                    index = 4;
                    break;
                case "K:":
                    index = 5;
                    break;
                case "L:":
                    index = 6;
                    break;
                case "M:":
                    index = 7;
                    break;
                case "N:":
                    index = 8;
                    break;
                case "O:":
                    index = 9;
                    break;
                case "P:":
                    index = 10;
                    break;
                case "Q:":
                    index = 11;
                    break;
                case "R:":
                    index = 12;
                    break;
                case "S:":
                    index = 13;
                    break;
                case "T:":
                    index = 14;
                    break;
                case "U:":
                    index = 15;
                    break;
                case "V:":
                    index = 16;
                    break;
                case "W:":
                    index = 17;
                    break;
                case "X:":
                    index = 18;
                    break;
                case "Y:":
                    index = 19;
                    break;
                case "Z:":
                    index = 20;
                    break;
                default:
                    index = 0;
                    break;
            }

            return index;
        }

        #endregion
    }
}
