using HyperManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.DTOs {
	public struct KillData : IParseable {

		private const string FORCE_MARKER = "--force";

		public string target;
		public bool force;

		public IParseable SetFromCommand(CommandBuilder command) {
			int argc = command.ArgsCount;
			if (argc < 1) {
				throw new Exception("Not enough arguments provided for Kill command! Run with: Help Kill");
			}
			for (int i = 0; i < argc; i++) {
				string normalizedArg = command.GetArgument(i).ToLower();
				if (normalizedArg.StartsWith(FORCE_MARKER)) {
					this.force = true;
					continue;
				}
				this.target = normalizedArg;
			}
			return this;
		}
	}
}
