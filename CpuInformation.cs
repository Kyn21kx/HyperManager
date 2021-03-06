using System;
using System.Text;
using System.Management;
using System.Reflection;

namespace HyperManager {
	public struct CpuInformation {

		/// <summary>
		/// The name as given by the processor's manufacturer
		/// </summary>
		public string Name { get; private set; }
		
		public uint NumberOfCores { get; private set; }

		public uint NumberOfEnabledCore { get; private set; }

		public uint L2CacheSize { get; private set; }

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
