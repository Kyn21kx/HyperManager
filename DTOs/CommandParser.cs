using HyperManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.DTOs {
	public static class CommandParser<T> where T : struct, IParseable {
		public static T ParseFromCommand(CommandBuilder command) {
			T result = new T();
			result.SetFromCommand(command);
			return result;
		}
	}
}
