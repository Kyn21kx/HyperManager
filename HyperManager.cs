using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Management;
using System.Linq;

namespace HyperManager {
	public class HyperManager {

		#region WIN32 API Definitions

		[StructLayout(LayoutKind.Sequential)]
		public struct RM_UNIQUE_PROCESS {
			public int dwProcessId;
			public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
		}

		const int RmRebootReasonNone = 0;
		const int CCH_RM_MAX_APP_NAME = 255;
		const int CCH_RM_MAX_SVC_NAME = 63;

		public enum RM_APP_TYPE {
			RmUnknownApp = 0,
			RmMainWindow = 1,
			RmOtherWindow = 2,
			RmService = 3,
			RmExplorer = 4,
			RmConsole = 5,
			RmCritical = 1000
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct RM_PROCESS_INFO {
			public RM_UNIQUE_PROCESS Process;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)] public string strAppName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)] public string strServiceShortName;

			public RM_APP_TYPE ApplicationType;
			public uint AppStatus;
			public uint TSSessionId;
			[MarshalAs(UnmanagedType.Bool)] public bool bRestartable;
		}

		[DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
		static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFilenames,
			uint nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, uint nServices,
			string[] rgsServiceNames);

		[DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
		static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

		[DllImport("rstrtmgr.dll")]
		static extern int RmEndSession(uint pSessionHandle);

		[DllImport("rstrtmgr.dll")]
		static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded,
			ref uint pnProcInfo, [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
			ref uint lpdwRebootReasons);

		#endregion


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
			path.Replace("\"", "");
			path.Replace(@"\", @"\\");

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
		/// <returns>List of all found processes</returns>
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

		public static void Kill(string target, bool force, out List<Process> killed) {
			killed = new List<Process>();
			int processId;
			if (int.TryParse(target, out processId)) {
				killed.Add(Process.GetProcessById(processId));
			}
			else {
				foreach (var p in Process.GetProcesses()) {
					if (p.ProcessName == target)
						killed.Add(p);
				}
			}

			killed.ForEach((x) => {
				if (force)
					x.Kill();
				else
					x.CloseMainWindow();
			});
		}

		public static float GetCPULevel() {
			try {
				var pc = new PerformanceCounter {
					CategoryName = "Processor",
					CounterName = "% Processor Time",
					InstanceName = "_Total",
					MachineName = Environment.MachineName
				};
				pc.NextValue();
				Thread.Sleep(500);
				return pc.NextValue();
			}
			catch (Exception) {
				//Your performance counters might have been corrupted, make sure to run lodctr /r on C:\Windows\System32 twice to fix it
				return float.NaN;
			}
		}

		public static float GetCPULevel(PerformanceCounter cachedPC) {
			try {
				cachedPC.NextValue();
				Thread.Sleep(500);
				return cachedPC.NextValue();
			}
			catch (Exception) {
				//Your performance counters might have been corrupted, make sure to run lodctr /r on C:\Windows\System32 twice to fix it
				return float.NaN;
			}
		}

		public static CpuInformation GetCPUInfo() {			
			var searcher = new ManagementObjectSearcher("Select * from Win32_Processor");
			ManagementObjectCollection results = searcher.Get();
			var first = results.OfType<ManagementObject>().FirstOrDefault();
			
			CpuInformation cpu = CpuInformation.ParseProperties(first);
			return cpu;
		}

		public static float GetAvailableRAM() {
			try {
				var pc = new PerformanceCounter {
					CategoryName = "Memory",
					CounterName = "Available MBytes",
					MachineName = Environment.MachineName
				};
				pc.NextValue();
				Thread.Sleep(500);
				return pc.NextValue();
			}
			catch (Exception) {
				//Your performance counters might have been corrupted, make sure to run lodctr /r on C:\Windows\System32 twice to fix it
				return float.NaN;
			}
		}

		public static float GetAvailableRAM(PerformanceCounter cachedPC) {
			try {
				cachedPC.NextValue();
				Thread.Sleep(500);
				return cachedPC.NextValue();
			}
			catch (Exception) {
				//Your performance counters might have been corrupted, make sure to run lodctr /r on C:\Windows\System32 twice to fix it
				return float.NaN;
			}
		}

		public static string FormattedProcessString(Process p) {
			return $"Name: {p.ProcessName}\tPID: {p.Id}";
		}

	}

}
