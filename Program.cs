using HyperManager.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager {
	class Program {

		enum Commands {
			FINDLOCKERS,
			KILL,
			FINDPROCESSES,
			PERFORMANCE
		}

		static void Main(string[] args) {
			if (args.Length < 1) {
				Console.WriteLine("Help: HyperManager Help");
				return;
			}
			try {
				CommandReader reader = new CommandReader(args);
				reader.Execute();
			}
			catch(Exception err) {
				Console.WriteLine($"[Error]: {err.Message}");
			}
			/*
			switch (cmd) {
				case Commands.FINDLOCKERS:
					if (args.Length < 2) {
						Console.WriteLine("Path needed");
						break;
					}

					
					break;

				case Commands.FINDPROCESSES:
					
					break;

				case Commands.KILL:
					if (args.Length < 2) {
						Console.WriteLine("Please provide the name or id of a process");
						break;
					}

					List<Process> killed;
					bool force = args.Length >= 3 && CheckCommandIgnoreCase("Force", args);
					HyperManager.Kill(args[1], force, out killed);

					if (killed.Count == 0) {
						Console.WriteLine("No processes were found to kill");
						break;
					}

					Console.WriteLine($"Killed {killed.Count} processes with the target: {args[1]}");

					for (int i = 0; i < killed.Count; i++) {
						string info = HyperManager.FormattedProcessString(killed[i]);
						Console.WriteLine(info);
					}
					break;

				case Commands.PERFORMANCE:
					
					break;			
			}*/
		}

		private static bool CheckCommandIgnoreCase(string command, string[] collection) {
			for (int i = 0; i < collection.Length; i++) {
				if (command.ToUpper() == collection[i].ToUpper())
					return true;
			}
			return false;
		}

		private static void Help() {

		}

	}
}
