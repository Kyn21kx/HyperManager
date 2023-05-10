using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Management;
using System.Linq;

namespace HyperManager {
	public class PerformanceManager {		

		/// <summary>
		/// Creates a new performance counter to measure the current CPU usage with a delay of 500 milliseconds to avoid faulty readings
		/// </summary>
		/// <returns>The current cpu usage level as a floating point percentage from 0 to 100</returns>
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
				//Note, please make sure to run the command above as an administrator in case you get error #5
				return float.NaN;
			}
		}

		/// <summary>
		/// Returns the value of the provided performance counter (must be well formatted) with a delay of 500 milliseconds to avoid faulty readings
		/// </summary>
		/// <param name="cachedPC">The already initialized performance counter</param>
		/// <returns></returns>
		public static float ParsePerformanceCounter(PerformanceCounter cachedPC) {
			if (cachedPC == null) return float.NaN;
			try {
				cachedPC.NextValue();
				Thread.Sleep(500);
				return cachedPC.NextValue();
			}
			catch (Exception) {
				//Your performance counters might have been corrupted, make sure to run lodctr /r on C:\Windows\System32 twice to fix it
				//Note, please make sure to run the command above as an administrator in case you get error #5
				return float.NaN;
			}
		}

		/// <summary>
		/// Creates a new performance counter to measure the current available RAM (in Mega Bytes) with a delay of 500 milliseconds to avoid faulty readings
		/// </summary>
		/// <returns>The current available RAM in Mega Bytes as a floating point number</returns>
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
				//Note, please make sure to run the command above as an administrator in case you get error #5
				return float.NaN;
			}
		}
	}

}
