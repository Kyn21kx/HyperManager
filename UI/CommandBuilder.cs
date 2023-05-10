using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.UI {
	public class CommandBuilder {

		public string BaseCommand { get; private set; }
		public int ArgsCount => this.arguments.Count;
		private List<string> arguments;

		public CommandBuilder(string initialCommand) { 
			this.BaseCommand = Sanitize(initialCommand);
			this.arguments = new List<string>();
		}

		public CommandBuilder(int argc) {
			this.BaseCommand = null;
			this.arguments = new List<string>(argc);
		}

		public CommandBuilder Append(string input) {
			if (this.BaseCommand == null) {
				this.BaseCommand = Sanitize(input);
				return this;
			}
			//Otherwise, it will go to the args
			this.arguments.Add(Sanitize(input));
			return this;
		}

		public string GetArgument(int index) {
			return this.arguments[index];
		}

		private string Sanitize(string command) {
			//Convert to lowercase
			return command.ToLower();
		}

	}
}
