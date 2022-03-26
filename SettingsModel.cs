using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


	public class SettingsModel
	{
		public string Title { get; set; }

		public string Message { get; set; }

		public int Mode { get; set; }

		public int ProxyMode { get; set; }

		public int Threads { get; set; }

		public string WebHook { get; set; }
	}