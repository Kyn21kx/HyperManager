using HyperManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.DTOs {
	public struct HelpData : IParseable {

		public Operation opToConsult;

		public IParseable SetFromCommand(CommandBuilder command) {
			int argc = command.ArgsCount;
			if (argc < 1) {
				this.opToConsult = Operation.Help;
				return this;
			}
			string arg = command.GetArgument(0);
			this.opToConsult = CommandReader.ParseOperationFromString(arg);
			return this;
		}
	}
}
