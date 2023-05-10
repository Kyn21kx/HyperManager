using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.Definitions {
	public class Win32Definitions {

		#region WIN32 API Definitions

		[StructLayout(LayoutKind.Sequential)]
		public struct RM_UNIQUE_PROCESS {
			public int dwProcessId;
			public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
		}

		public const int RmRebootReasonNone = 0;
		public const int CCH_RM_MAX_APP_NAME = 255;
		public const int CCH_RM_MAX_SVC_NAME = 63;

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
		public static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFilenames,
			uint nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, uint nServices,
			string[] rgsServiceNames);

		[DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
		public static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

		[DllImport("rstrtmgr.dll")]
		public static extern int RmEndSession(uint pSessionHandle);

		[DllImport("rstrtmgr.dll")]
		public static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded,
			ref uint pnProcInfo, [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
			ref uint lpdwRebootReasons);

		#endregion
	}
}
