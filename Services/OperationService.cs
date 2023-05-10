using HyperManager.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.Services {
	public class OperationService {
		private const string DIVIDE_LINE = "***********************";
		public OperationService() { 

		}

		public void ExecuteFind(FindData data) {
			switch (data.type) {
				case FindType.Process:
					List<Process> foundProcesses = ProcessManager.FindProcesses(data.target);

					if (foundProcesses.Count == 0) {
						Console.WriteLine("There were no results for your search...");
						break;
					}

					Console.WriteLine($"Found {foundProcesses.Count} processes that match your search: {data.target}");

					for (int i = 0; i < foundProcesses.Count; i++) {
						string info = ProcessManager.FormattedProcessString(foundProcesses[i]);
						Console.WriteLine(info);
					}
					break;
				case FindType.Blockers:
					List<Process> blockers = ProcessManager.FindLockers(data.target);

					if (blockers.Count == 0) {
						Console.WriteLine("There are no processes locking this file!");
						break;
					}

					Console.WriteLine($"There are {blockers.Count} processes blocking the file/directory: {data.target}");

					for (int i = 0; i < blockers.Count; i++) {
						string info = ProcessManager.FormattedProcessString(blockers[i]);
						Console.WriteLine(info);
					}
					break;
			}
		}

		public void ExecuteKill(KillData data) {
			int killCount;
			ProcessManager.Kill(data.target, data.force, out killCount);

			if (killCount == 0) {
				Console.WriteLine("No processes were found to kill");
				return;
			}

			Console.WriteLine($"Killed {killCount} processes with the target: {data.target}");
		}

		public void ExecutePerformance(PerformanceData data) {
			switch (data.hardwareTarget) {
				case PerformanceTarget.Cpu:
					this.PrintCpuInfo();
					break;
				case PerformanceTarget.Ram:
					this.PrintMemoryInfo();
					break;
				case PerformanceTarget.Gpu:
					this.PrintGpuInfo();
					break;
				case PerformanceTarget.All:
					this.PrintMemoryInfo();
					this.PrintCpuInfo();
					//this.PrintGpuInfo(); //TODO
					break;
			}
			Console.WriteLine(DIVIDE_LINE);

		}

		private void PrintMemoryInfo() {
			Console.WriteLine(DIVIDE_LINE);
			Console.WriteLine("Memory Information:");
			float availableRAM = PerformanceManager.GetAvailableRAM();
			Console.WriteLine($"Available RAM: {availableRAM}MB ({availableRAM / 1024f}GB)");
		}

		private void PrintCpuInfo() {
			Console.WriteLine(DIVIDE_LINE);
			Console.WriteLine("CPU Information:");

			//General hardware info
			CpuInformation cpuInfo = CpuInformation.GetCPUInfo();
			Console.WriteLine(cpuInfo.ToString());

			//Usage level
			Console.WriteLine("Calculating usage level...");
			float usage = PerformanceManager.GetCPULevel();
			Console.WriteLine($"Usage: {usage}%");
		}

		private void PrintGpuInfo() {
			throw new NotImplementedException();
		}

	}
}
