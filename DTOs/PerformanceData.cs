using HyperManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.DTOs {

	public enum PerformanceTarget {
		Cpu,
		Ram,
		Gpu,
		All
	}

	public struct PerformanceData : IParseable {

		public PerformanceTarget hardwareTarget;

		public IParseable SetFromCommand(CommandBuilder command) {
			int argc = command.ArgsCount;
			if (argc < 1) {
				throw new Exception("Not enough arguments for Performance command! Run with: Help Performance");
			}
			string arg = command.GetArgument(0);
			this.hardwareTarget = ParseTargetFromString(arg);
			return this;
		}

		private PerformanceTarget ParseTargetFromString(string target) {
			bool parsed = Enum.TryParse(target, true, out PerformanceTarget result);
			if (parsed)
				return result;
			throw new Exception($"No option for Performance command matches {target}! Run with Help Performance");
		}
	}
}
