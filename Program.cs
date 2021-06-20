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
			FIND
		}

		static void Main(string[] args) {
			if (args.Length < 1) {
				Console.WriteLine("Help: HyperManager Help");
				return;
			}

			string selection = args[0];
			Commands cmd;

			if (!Enum.TryParse(selection.ToUpper(), out cmd)) {
				Console.WriteLine("Help: HyperManager Help");
				return;
			}

			switch (cmd) {
				case Commands.FINDLOCKERS:
					if (args.Length < 2) {
						Console.WriteLine("Path needed");
						break;
					}

					List<Process> blockers = HyperManager.FindLockers(args[1]);

					if (blockers.Count == 0) {
						Console.WriteLine("There are no processes locking this file!");
						break;
					}

					Console.WriteLine($"There are {blockers.Count} processes blocking the file/directory: {args[1]}");

					for (int i = 0; i < blockers.Count; i++) {
						string info = HyperManager.FormattedProcessString(blockers[i]);
						Console.WriteLine(info);
					}
					break;

				case Commands.FIND:
					if (args.Length < 2) {
						Console.WriteLine("Please provide a search string");
						break;
					}

					List<Process> foundProcesses = HyperManager.FindProcesses(args[1]);

					if (foundProcesses.Count == 0) {
						Console.WriteLine("There were no results for your search...");
						break;
					}

					Console.WriteLine($"Found {foundProcesses.Count} processes that match your search: {args[1]}");

					for (int i = 0; i < foundProcesses.Count; i++) {
						string info = HyperManager.FormattedProcessString(foundProcesses[i]);
						Console.WriteLine(info);
					}
					break;
			}
		}
	}
}
