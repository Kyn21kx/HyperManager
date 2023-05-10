using System;
using System.Text;
using System.Management;
using System.Reflection;
using System.Linq;

namespace HyperManager {
	public struct CpuInformation {

		/// <summary>
		/// The name as given by the processor's manufacturer
		/// </summary>
		public string Name { get; private set; }
		
		/// <summary>
		/// The total number of cores the processor has (or the kernel has access to)
		/// </summary>
		public uint NumberOfCores { get; private set; }

		/// <summary>
		/// The number of currently enabled cores in the processor
		/// </summary>
		public uint NumberOfEnabledCore { get; private set; }

		/// <summary>
		/// Size of the level 2 or external cache in Kilo Bytes
		/// </summary>
		public uint L2CacheSize { get; private set; }

		/// <summary>
		/// Size of the level 3 or specialized cache in Kilo Bytes
		/// </summary>
		public uint L3CacheSize { get; private set; }
	
		public uint MaxClockSpeed { get; private set; }

		public uint CurrentClockSpeed { get; private set; }

		public uint ThreadCount { get; private set; }

		public ushort CurrentVoltage { get; private set; }

		public ushort AddressWidth { get; private set; }

		private void SetMember(string name, object value) {
			object boxed = this;
			PropertyInfo property = typeof(CpuInformation).GetProperty(name);
			property.SetValue(boxed, value, null);
			this = (CpuInformation)boxed;
		}

		/// <summary>
		/// Gets the CPU's information by performing a query to the Management Object Searcher
		/// </summary>
		/// <returns>A structure containing the hardware specifications for the user's CPU</returns>
		public static CpuInformation GetCPUInfo() {
			var searcher = new ManagementObjectSearcher("Select * from Win32_Processor");
			ManagementObjectCollection results = searcher.Get();
			var first = results.OfType<ManagementObject>().FirstOrDefault();

			CpuInformation cpu = CpuInformation.ParseProperties(first);
			return cpu;
		}

		public static CpuInformation ParseProperties(ManagementObject managementObject) {
			PropertyInfo[] internalProperties = typeof(CpuInformation).GetProperties();
			CpuInformation result = new CpuInformation();

			for (int i = 0; i < internalProperties.Length; i++) {
				object value = managementObject.Properties[internalProperties[i].Name].Value;
				result.SetMember(internalProperties[i].Name, value);
			}
			return result;
		}

		public override string ToString() {
			StringBuilder result = new StringBuilder();
			PropertyInfo[] properties = typeof(CpuInformation).GetProperties();

			foreach (var property in properties) {
				result.Append($"{property.Name}: {property.GetValue(this)}\n");
			}
			return result.ToString();
		}

	}
}
