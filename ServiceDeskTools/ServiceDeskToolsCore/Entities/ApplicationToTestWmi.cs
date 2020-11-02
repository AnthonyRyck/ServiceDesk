using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Entities.Metier
{
    public class ApplicationToTestWmi
    {
        public string ApplicationName { get; set; }

        public string Version { get; set; }

        public string WqlRequest { get; set; }
    }
}
