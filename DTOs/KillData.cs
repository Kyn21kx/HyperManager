using HyperManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.DTOs {
	public struct KillData : IParseable {

		public string target;
		public bool force;

		public IParseable SetFromCommand(CommandBuilder command) {
			throw new NotImplementedException();
		}
	}
}
