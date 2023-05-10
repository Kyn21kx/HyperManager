using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static HyperManager.Definitions.Win32Definitions;

namespace HyperManager {
	public class ProcessManager {

		//Function provided on https://stackoverflow.com/a/3504251/9854946
		private static List<Process> EnumerateProcesses(uint pnProcInfoNeeded, uint handle, uint lpdwRebootReasons) {
			var processes = new List<Process>(10);
			// Create an array to store the process results
			var processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
			var pnProcInfo = pnProcInfoNeeded;

			// Get the list
			var res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);

			if (res != 0) throw new Exception("Could not list processes locking resource.");
			for (int i = 0; i < pnProcInfo; i++) {
				try {
					processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
				}
				catch (ArgumentException) { } // catch the error -- in case the process is no longer running
			}
			return processes;
		}

		//Function provided on https://stackoverflow.com/a/3504251/9854946
		/// <summary>
		/// Finds all the processes that are currently using the specified directory
		/// </summary>
		/// <param name="path">Directory to check</param>
		/// <returns>List of all found processes</returns>
		public static List<Process> FindLockers(string path) {
			uint handle;
			string key = Guid.NewGuid().ToString();
			int res = RmStartSession(out handle, 0, key);

			//Clean the string
			path = path.Replace("\"", "");
			path = path.Replace(@"\", @"\\");

			if (res != 0) throw new Exception("Could not begin restart session.  Unable to determine file locker.");

			try {
				const int MORE_DATA = 234;
				uint pnProcInfoNeeded, pnProcInfo = 0, lpdwRebootReasons = RmRebootReasonNone;

				string[] resources = { path }; // Just checking on one resource.

				res = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

				if (res != 0) throw new Exception("Could not register resource.");

				//Note: there's a race condition here -- the first call to RmGetList() returns
				//      the total number of process. However, when we call RmGetList() again to get
				//      the actual processes this number may have increased.
				res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

				if (res == MORE_DATA) {
					return EnumerateProcesses(pnProcInfoNeeded, handle, lpdwRebootReasons);
				}
				else if (res != 0) throw new Exception("Could not list processes locking resource. Failed to get size of result.");
			}
			finally {
				RmEndSession(handle);
			}

			return new List<Process>();
		}

		/// <summary>
		/// Finds a process either by its PID, or through a regular expression applied on its name
		/// </summary>
		/// <param name="search">Query to search (numerical for PID, otherwise RegEx)</param>
		/// <returns>A list of all found processes</returns>
		public static List<Process> FindProcesses(string search) {
			//If it is only numbers, then you've got an id, else do contains (or a regex, idk)
			List<Process> results = new List<Process>();
			int processId;

			if (int.TryParse(search, out processId)) {
				//Return the process with the given ID
				results.Add(Process.GetProcessById(processId));
				return results;
			}

			//Run a regular expression to find all matches
			foreach (var p in Process.GetProcesses()) {
				if (Regex.IsMatch(p.ProcessName, search))
					results.Add(p);
			}

			return results;
		}

		/// <summary>
		/// Kills the process(es) with the desired PID or name, then fills a list with the information of the processes killed
		/// </summary>
		/// <param name="target">The PID or name of the process to kill (all numeric for PID, otherwise case sensitive name)</param>
		/// <param name="force">If true, the process will be killed instantly, otherwise, a request will be sent to the main window handle to close it</param>
		/// <param name="killed">The information about the killed processes will be added to this list</param>
		public static void Kill(string target, bool force, out int killCount) {
			int processId;
			killCount = 0;
			if (int.TryParse(target, out processId)) {
				killCount++;
				Process targetP = Process.GetProcessById(processId);
				KillAndLog(targetP, force);
				return;
			}

			List<Process> targetProcesses = FindProcesses(target);
			foreach (var p in targetProcesses) {
				KillAndLog(p, force);
			}
		}

		private static void KillAndLog(Process p, bool force) {
			Console.WriteLine($"Killed process {FormattedProcessString(p)}. Forceful? {force}");
			if (force) {
				p.Kill();
				return;
			}
			p.CloseMainWindow();
		}

		/// <summary>
		/// Gets a simple formatted string with a processe's name and Id
		/// </summary>
		/// <param name="p">The process to be formatted</param>
		/// <returns>A process with the format: Name: {Process' name}\tPID: {Process' Id}</returns>
		public static string FormattedProcessString(Process p) {
			return $"Name: {p.ProcessName}\tPID: {p.Id}";
		}
	}
}
