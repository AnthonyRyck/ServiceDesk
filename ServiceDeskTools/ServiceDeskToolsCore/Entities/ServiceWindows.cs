using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Entities
{
	public class ServiceWindows
	{
		/// <summary>
		/// Nom du service.
		/// </summary>
		public string ServiceName { get; set; }

		/// <summary>
		/// Etat du service (RUNNING, STOPPED,...)
		/// </summary>
		public string State { get; set; }

		/// <summary>
		/// Indication si le service est démarré
		/// </summary>
		public bool IsStarted { get; set; }

		/// <summary>
		/// Indique comment est le service peut être démarré (DISABLE, AUTO, MANUAL,...)
		/// </summary>
		public string StartMode { get; set; }

	}
}
