using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Entities.Metier
{
    public class Shem
    {
        /// <summary>
        /// Nom du SHEM
        /// </summary>
        public string NomShem { get; set; }

        /// <summary>
        /// Collection des OU contenues dans la SHEM.
        /// </summary>
        public IEnumerable<string> OuCollection { get; set; }
    }
}
