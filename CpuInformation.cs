using System;
using System.Text;
using System.Linq;
using System.Management;
using System.Reflection;

namespace HyperManager {
	public struct CpuInformation {

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
			string[] names = GetNamesForProperties();
			CpuInformation result = new CpuInformation();

			for (int i = 0; i < names.Length; i++) {
				object value = managementObject.Properties[names[i]].Value;
				result.SetMember(names[i], value);
			}
			return result;
		}

		private static string[] GetNamesForProperties() {
			PropertyInfo[] properties = typeof(CpuInformation).GetProperties();
			string[] names = new string[properties.Length];
			
			for (int i = 0; i < properties.Length; i++) {
				names[i] = properties[i].Name;
			}
			return names;
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
