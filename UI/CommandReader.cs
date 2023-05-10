using HyperManager.DTOs;
using HyperManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.UI {
	public class CommandReader {

		private const string FIND_HELP = "Find processes or blockers.\n----Usage----\nFind [name/PID/Regex] --type=process\nFind [path to file] --type=blockers";
		private const string KILL_HELP = "Kills processes.\n----Usage----\nKill [name/PID/Regex] OPTIONAL: --force";
		private const string PERFORMANCE_HELP = "Returns performance information on the specified hardware.\n----Usage----\nPerformance [cpu/ram/gpu]";
		private const string READ_HELP = "Returns the content of a file (similar to cat command).\n----Usage----\nRead [path to file] OPTIONAL: --limit=[lines to read]";
		private const string WRITE_HELP = "Writes content to a file overwriting it by default, or with the desired method.\n----Usage----\nWrite [path to file] OPTIONAL: --method=[append/write]";
		private const string CUSTOM_HELP = "Executes a custom script defined by the user.\n----Usage----\n[name of the script] --custom\nCustom [path to custom scripts folder] --set-path";
		private const string GENERAL_HELP = "Hi, welcome to HyperManager! use any of the commands listed below or type:\n\"Help [name of the command you wish to know more]\"\nTo get some useful info about a particular operation\n----Available commands----";
		
		private static readonly Dictionary<Operation, string> helpDictionary = new Dictionary<Operation, string> {
			{ Operation.Find, FIND_HELP },
			{ Operation.Kill, KILL_HELP},
			{ Operation.Performance, PERFORMANCE_HELP},
			{ Operation.Read, READ_HELP },
			{ Operation.Write, WRITE_HELP },
			{ Operation.Custom, CUSTOM_HELP },
			{ Operation.Help, GENERAL_HELP },

		};

		private CommandBuilder commandBuilder;
		private OperationService operationService;
		
		public CommandReader(string[] args) {
			//The length of the arguments is argc - 1 because args[0] is the base command
			this.commandBuilder = new CommandBuilder(args.Length - 1);
			for (int i = 0; i < args.Length; i++) {
				//This will instantiate and sanitize all command properties.
				this.commandBuilder.Append(args[i]);
			}
			this.operationService = new OperationService();
		}

		public void Execute() {
			//Basically an entry point
			//Map our commands to actions and call them here
			Operation targetOp = ParseOperationFromString(commandBuilder.BaseCommand);
			IParseable parsedData;
			switch (targetOp) {
				case Operation.Find:
					parsedData = CommandParser<FindData>.ParseFromCommand(commandBuilder);
					this.operationService.ExecuteFind((FindData)parsedData);
					break;
				case Operation.Kill:
					parsedData = CommandParser<KillData>.ParseFromCommand(commandBuilder);
					this.operationService.ExecuteKill((KillData)parsedData);
					break;
				case Operation.Performance:
					parsedData = CommandParser<PerformanceData>.ParseFromCommand(commandBuilder);
					this.operationService.ExecutePerformance((PerformanceData)parsedData);
					break;
				case Operation.Read:
					break;
				case Operation.Write:
					break;
				case Operation.Custom:
					break;
				case Operation.Help:
					parsedData = CommandParser<HelpData>.ParseFromCommand(commandBuilder);
					this.DisplayHelp(((HelpData)parsedData).opToConsult);
					break;
			}

		}

		public void DisplayHelp(Operation op = Operation.Help) {
			Console.WriteLine($"----Help for {op}----");
			Console.WriteLine(helpDictionary[op]);
			if (op != Operation.Help) return;
			string[] cmdNames = Enum.GetNames(typeof(Operation));
			for (int i = 0; i < cmdNames.Length; i++) {
				Console.WriteLine(cmdNames[i]);
			}
		}

		private string OperationToString(Operation op) {
			return op.ToString().ToLower();
		}

		public static Operation ParseOperationFromString(string operation) {
			bool parsed = Enum.TryParse(operation, true, out Operation result);
			if (parsed) {
				return result;
			}
			throw new Exception($"Operation with the name: {operation} does not exist!");
		}



	}
}
