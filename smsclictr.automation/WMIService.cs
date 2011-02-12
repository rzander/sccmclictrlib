//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics; 


namespace smsclictr.automation
{
    /// <summary>
    /// Manage Services over WMI
    /// </summary>
    public class WMIService : IDisposable
    {
        #region Internal

        private WMIProvider oWMIProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// WMIService Constructor
        /// </summary>
        /// <param name="oProvider"></param>
        public WMIService(WMIProvider oProvider)
        {
            oWMIProvider = oProvider;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        #region External
        // found at http://groups.google.ch/group/microsoft.public.dotnet.languages.csharp/tree/browse_frm/thread/675fee6d2244348d/36cf2d76a49596de?rnum=11&hl=de&q=SeDebugPrivilege+c%23&_done=%2Fgroup%2Fmicrosoft.public.dotnet.languages.csharp%2Fbrowse_frm%2Fthread%2F675fee6d2244348d%2F36cf2d76a49596de%3Flnk%3Dst%26q%3DSeDebugPrivilege+c%23%26rnum%3D7%26hl%3Dde%26#doc_36cf2d76a49596de
        
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen); 

        #endregion

        #region Constants

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_DEBUG_NAME = "SeDebugPrivilege"; 


        #endregion

        #region Functions

        private ManualResetEvent MRE = new ManualResetEvent(false);

        private void ServiceEventArrivedHandler(object sender, EventArrivedEventArgs arg)
        {
            try
            {
                ManagementBaseObject MBO = (ManagementBaseObject)arg.NewEvent["TargetInstance"];
            }
            catch
            {
            }
            finally
            {
                MRE.Set();
            }
        }

        /// <summary>
        /// Send a stop command to a Service and wait until the Service is stopped
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns>Returns an ArrayList of Services which has stopped because of service dependencies </returns>
        public ArrayList StopService(string ServiceName)
        {
            try
            {
                ManagementObjectCollection Dependencies;
                ManagementObject Service;
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\cimv2";
                oProv.mScope.Options.EnablePrivileges = true;
                Service = oProv.GetObject("Win32_Service.Name='" + ServiceName + "'");
                Dependencies = oProv.ExecuteQuery("Associators of {Win32_Service.Name='" + ServiceName + "'} Where AssocClass=Win32_DependentService Role=Antecedent");

                ArrayList Result = new ArrayList();
                foreach (ManagementObject MO in Dependencies)
                {
                    if (MO.GetPropertyValue("State").ToString().ToLower() == "running")
                    {
                        Result.AddRange(StopService(MO.GetPropertyValue("Name").ToString()));
                    }
                }
                Result.Add(ServiceName);
                bStopService(Service);
                return Result;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private bool bStopService(ManagementObject Service)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\cimv2";

                WqlEventQuery query = new WqlEventQuery("__InstanceModificationEvent");
                query.Condition = "TargetInstance ISA 'Win32_Service' AND TargetInstance.Name='" + Service.GetPropertyValue("Name").ToString() + "' AND TargetInstance.State='Stopped'";
                query.WithinInterval = new TimeSpan(0, 0, 2);
                ManagementEventWatcher watcher = new ManagementEventWatcher(oProv.mScope, query);
                watcher.EventArrived += new EventArrivedEventHandler(ServiceEventArrivedHandler);
                //watcher.Options.Timeout = new TimeSpan(0, 0, 15);
                watcher.Start();
                Object result = Service.InvokeMethod("StopService", null);
                if ((UInt32)result == 0)
                {
                    MRE.WaitOne(new TimeSpan(0, 0, 60), true);
                }
                watcher.Stop();
                watcher.Dispose();
                Service.Get();

                if (Service["State"].ToString() == "Stopped")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }


        }

        /// <summary>
        /// Start a Service
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns>Return Value</returns>
        public int StartService(String ServiceName)
        {
            ManagementObject Service;
            try
            {
                ManagementEventWatcher watcher = new ManagementEventWatcher();
                lock (oWMIProvider)
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    oProv.mScope.Options.EnablePrivileges = true;
                    Service = oProv.GetObject("Win32_Service.Name='" + ServiceName + "'");

                    WqlEventQuery query = new WqlEventQuery("__InstanceModificationEvent");
                    query.Condition = "TargetInstance ISA 'Win32_Service' AND TargetInstance.Name='" + ServiceName + "' AND TargetInstance.State='Running'";
                    query.WithinInterval = new TimeSpan(0, 0, 2);
                    watcher = new ManagementEventWatcher(oProv.mScope, query);
                    watcher.EventArrived += new EventArrivedEventHandler(ServiceEventArrivedHandler);
                }
                //watcher.Options.Timeout = new TimeSpan(0, 0, 15);
                watcher.Start();

                Object result = Service.InvokeMethod("StartService", null);
                int iResult = int.Parse(result.ToString());
                if (iResult == 0)
                {
                    MRE.WaitOne(new TimeSpan(0, 0, 30), true);
                }
                watcher.Stop();
                watcher.Dispose();

                Service.Get();


                if (Service["State"].ToString() == "Running")
                {
                    return iResult;
                }
                else
                {
                    return iResult;
                }
            }
            catch (Exception ex)
            {
                //0x80080005 wmi stopped
                throw (ex);
            }
        }

        /// <summary>
        /// Start a list of Services
        /// </summary>
        /// <param name="ServiceNames"></param>
        /// <returns>Status if all Service are started</returns>
        public bool StartService(ArrayList ServiceNames)
        {
            Boolean Result = true;
            foreach (string ServiceName in ServiceNames)
            {
                if (StartService(ServiceName) == 0)
                {
                    Result = false;
                }
            }
            return Result;

        }

        /// <summary>
        /// Get the Win32_Service ManagementObject of a Service
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns>Win32_Service ManagementObject</returns>
        public ManagementObject GetService(String ServiceName)
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";

            return oProv.GetObject("Win32_Service.Name='" + ServiceName + "'");
        }

        /// <summary>
        /// Set the Service start mode
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <param name="StartMode">
        /// <list type="table">
        /// <listheader><term>Value</term><description>Meaning</description></listheader>
        /// <item><term>Boot</term><description>Device driver started by the operating system loader. This value is valid only for driver services.</description></item>
        /// <item><term>System</term><description>Device driver started by the operating system initialization process. This value is valid only for driver services.</description></item>
        /// <item><term>Automatic</term><description>Service to be started automatically by the service control manager during system startup.</description></item>
        /// <item><term>Manual</term><description>Service to be started by the service control manager when a process calls the StartService method.</description></item>
        /// <item><term>Disabled</term><description>Service that can no longer be started.</description></item>
        /// </list>
        /// </param>
        /// <returns></returns>
        public int SetServiceStartMode(string ServiceName, string StartMode)
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            ManagementObject Service = oProv.GetObject("Win32_Service.Name='" + ServiceName + "'");
            ManagementBaseObject inParams = Service.GetMethodParameters("ChangeStartMode");
            inParams["StartMode"] = StartMode;
            ManagementBaseObject Result = Service.InvokeMethod("ChangeStartMode", inParams, null);
            return int.Parse(Result.GetPropertyValue("ReturnValue").ToString());
        }

        /// <summary>
        /// Set the Service start mode
        /// </summary>
        /// <param name="Service">Win32_Service ManagementObject</param>
        /// <param name="StartMode">
        /// <list type="table">
        /// <listheader><term>Value</term><description>Meaning</description></listheader>
        /// <item><term>Boot</term><description>Device driver started by the operating system loader. This value is valid only for driver services.</description></item>
        /// <item><term>System</term><description>Device driver started by the operating system initialization process. This value is valid only for driver services.</description></item>
        /// <item><term>Automatic</term><description>Service to be started automatically by the service control manager during system startup.</description></item>
        /// <item><term>Manual</term><description>Service to be started by the service control manager when a process calls the StartService method.</description></item>
        /// <item><term>Disabled</term><description>Service that can no longer be started.</description></item>
        /// </list>
        /// </param>
        /// <returns></returns>
        public int SetServiceStartMode(ManagementObject Service, string StartMode)
        {
            ManagementBaseObject inParams = Service.GetMethodParameters("ChangeStartMode");
            inParams["StartMode"] = StartMode;
            ManagementBaseObject Result = Service.InvokeMethod("ChangeStartMode", inParams, null);
            return int.Parse(Result.GetPropertyValue("ReturnValue").ToString());
        }

        /// <summary>
        /// Restart a Service
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public bool RestartService(String ServiceName)
        {
            return StartService(StopService(ServiceName));
        }

        /// <summary>
        /// Kill a process by ProcessName
        /// </summary>
        /// <param name="ProcessName"></param>
        /// <returns></returns>
        public int KillProcess(string ProcessName)
        {
            ManagementObjectCollection MOC;
            int Res = 0;
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            oProv.mScope.Options.EnablePrivileges = true;
            MOC = oProv.ExecuteQuery("SELECT * FROM Win32_Process WHERE Name='" + ProcessName + "'");

            foreach (ManagementObject MO in MOC)
            {
                ManagementBaseObject inParams = MO.GetMethodParameters("Terminate");
                Res = int.Parse((MO.InvokeMethod("Terminate", inParams, null)).GetPropertyValue("ReturnValue").ToString());
            }
            return Res;

        }

        /// <summary>
        /// Kill a process by ProcessID
        /// </summary>
        /// <param name="ProcessID"></param>
        /// <returns></returns>
        public int KillProcess(int ProcessID)
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            oProv.mScope.Options.EnablePrivileges = true;
            ManagementObject MO = oProv.GetObject("Win32_Process.Handle='" + ProcessID.ToString() + "'");
            ManagementBaseObject inParams = MO.GetMethodParameters("Terminate");
            ManagementBaseObject Res = MO.InvokeMethod("Terminate", inParams, null);
            return int.Parse(Res.GetPropertyValue("ReturnValue").ToString());
        }

        /// <summary>
        /// Create a new process on a remote system. For security reasons this method cannot be used to start an interactive process remotely.
        /// </summary>
        /// <param name="CommandLine"></param>
        /// <param name="CurrentDirectory"></param>
        /// <param name="ProcessStartupInformation">A Win32_ProcessStartup Object <see href="http://msdn2.microsoft.com/en-us/library/aa394375.aspx"/></param>
        /// <returns>ProcessID</returns>
        public int StartProcess(string CommandLine, string CurrentDirectory, ManagementBaseObject ProcessStartupInformation)
        {

            if (CurrentDirectory == "")
                CurrentDirectory = null;

            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            ManagementClass MC = oProv.GetClass("Win32_Process");
            ManagementBaseObject inParams = MC.GetMethodParameters("Create");
            inParams["CommandLine"] = CommandLine;
            inParams["CurrentDirectory"] = CurrentDirectory;
            inParams["ProcessStartupInformation"] = ProcessStartupInformation;

            ManagementBaseObject Result = MC.InvokeMethod("Create", inParams, null);

            switch (int.Parse(Result.GetPropertyValue("ReturnValue").ToString()))
            {
                case 0: return int.Parse(Result.GetPropertyValue("ProcessID").ToString());
                case 2: throw new System.Security.SecurityException("Access denied");
                case 3: throw new System.Security.SecurityException("Insufficient privilege");
                case 9: throw new Exception("Path not found: " + CommandLine);
                case 21: throw new Exception("Invalid parameter");
                default: throw new Exception("Unknown failure");
            }
        }

        /// <summary>
        /// Set SeDebugPrivilege
        /// </summary>
        /// <returns></returns>
        public bool AdjustDebugPrivileges()
        {
            TokPriv1Luid tp;
            bool Result;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            Result = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            Result = LookupPrivilegeValue(null, SE_DEBUG_NAME, ref tp.Luid);
            Result = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            return Result;
        }

        #endregion
    }
}
