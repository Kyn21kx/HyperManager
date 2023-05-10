using HyperManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.DTOs {
	public enum FindType {
		Process,
		Blockers
	}
	public struct FindData : IParseable {

		public FindType type;
		public string target;

		private const string TYPE_MARKER = "--type=";

		public IParseable SetFromCommand(CommandBuilder command) {
			//Check the arguments
			int argc = command.ArgsCount;
			this.type = FindType.Process;
			if (argc < 1) {
				throw new Exception("Not enough arguments found for Find operation!\nRun with: Help Find");
			}

			for (int i = 0; i < argc; i++) {
				string normalizedArg = command.GetArgument(i).ToLower();
				//Check if the argument is the type
				if (normalizedArg.StartsWith(TYPE_MARKER)) {
					//Add it to either the process or the blockers
					string typeTarget = normalizedArg.Substring(TYPE_MARKER.Length);
					this.type = ParseTypeFromString(typeTarget);
					continue;
				}
				//Otherwise, the arg is the file path, regex or PID of the process
				this.target = normalizedArg;
			}
			return this;
		}

		private FindType ParseTypeFromString(string type) {
			bool parsed = Enum.TryParse(type, true, out FindType result);
			if (parsed) {
				return result;
			}
			throw new Exception($"Operation Find has no type with the name {type}!\nRun with: Help Find");
		}
	}
}
