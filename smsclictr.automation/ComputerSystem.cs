//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Management;

namespace smsclictr.automation
{
    /// <summary>
    /// System Informations independent of SMS
    /// </summary>
    public class WMIComputerSystem
    {
        #region Internal

        private WMIProvider oWMIProvider;
        private ManagementObject oWin32_OperatingSystem;
        private ManagementObject oWin32_ComputerSystem;
        private ManagementObjectCollection oWin32_SystemEnvironment;
        private List<string> oLoggedOnUsers;

        #endregion

        #region Constructor

        /// <summary>
        /// WMI ComputerSystem Constructor
        /// </summary>
        /// <param name="oProvider"></param>
        public WMIComputerSystem(WMIProvider oProvider)
        {
            oWMIProvider = oProvider;
        }

        #endregion

        #region Properties & Functions

        /// <summary>
        /// Do not used the cached Object. Reload the Object once (Flag wil be reset).
        /// </summary>
        public bool Reload
        {
            get;
            set;
        }

        /// <summary>
        /// Get the Win32_OperatingSystem ManagementObject
        /// </summary>
        public ManagementObject Win32_OperatingSystem
        {
            get
            {
                if ((oWin32_OperatingSystem == null) | Reload)
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    oProv.mScope.Options.EnablePrivileges = true;
                    ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM Win32_OperatingSystem");
                    foreach (ManagementObject MO in MOC)
                    {
                        oWin32_OperatingSystem = MO;
                        Reload = false;
                        return MO;
                    }
                    return null;
                }
                else
                {
                    return oWin32_OperatingSystem;
                }
            }

        }

        /// <summary>
        /// Get the Win32_ComputerSystem ManagementObject
        /// </summary>
        public ManagementObject Win32_ComputerSystem
        {
            get
            {
                if ((oWin32_ComputerSystem == null) | Reload)
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    //oWMIProvider.mScope.Options.EnablePrivileges = true;
                    ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM Win32_ComputerSystem");
                    foreach (ManagementObject MO in MOC)
                    {
                        oWin32_ComputerSystem = MO;
                        Reload = false;
                        return MO;
                    }
                    return null;
                }
                else
                {
                    return oWin32_ComputerSystem;
                }
            }
        }

        /// <summary>
        /// Get the Win32_Environment ManagementObject
        /// </summary>
        public ManagementObjectCollection Win32_Environment
        {
            get
            {
                if ((oWin32_SystemEnvironment == null) | Reload)
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    //oWMIProvider.mScope.Options.EnablePrivileges = true;
                    ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM Win32_Environment WHERE SystemVariable='True'");

                    return MOC;
                }
                else
                {
                    return oWin32_SystemEnvironment;
                }
            }
        }

        /// <summary>
        /// Get the Windows Directory (like C:\Windows)
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("Windows Directory: " + oClient.ComputerSystem.WindowsDirectory;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ComputerSystem.WindowsDirectory
        /// </code>
        /// </example>
        public string WindowsDirectory
        {
            get
            {
                ManagementObject MO = Win32_OperatingSystem;
                return MO.GetPropertyValue("WindowsDirectory").ToString();
            }
        }

        /// <summary>
        /// Logoff a loggedon user
        /// </summary>
        /// <returns></returns>
        public UInt32 Logoff()
        {
            ManagementObject MO = Win32_OperatingSystem;
            ManagementBaseObject inParams = MO.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = 4; //4 = forced logoff;
            ManagementBaseObject result = MO.InvokeMethod("Win32Shutdown", inParams, null);
            return UInt32.Parse(result.GetPropertyValue("ReturnValue").ToString());
        }

        /// <summary>
        /// shutdown and restart the remote client
        /// </summary>
        /// <returns></returns>
        public UInt32 Restart()
        {
            ManagementObject MO = Win32_OperatingSystem;
            ManagementBaseObject inParams = MO.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = 6; //6 = force reboot;
            ManagementBaseObject result = MO.InvokeMethod("Win32Shutdown", inParams, null);
            return UInt32.Parse(result.GetPropertyValue("ReturnValue").ToString());
        }

        /// <summary>
        /// Shutdown and poweroff the remote client
        /// </summary>
        /// <returns></returns>
        public UInt32 Shutdown()
        {
            ManagementObject MO = Win32_OperatingSystem;
            ManagementBaseObject inParams = MO.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = 12; //12 = forced power off;
            ManagementBaseObject result = MO.InvokeMethod("Win32Shutdown", inParams, null);
            return UInt32.Parse(result.GetPropertyValue("ReturnValue").ToString());
        }

        /// <summary>
        /// Get the System start Date/Time
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("System start date: " + oClient.ComputerSystem.LastBootUpTime;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ComputerSystem.LastBootUpTime
        /// </code>
        /// </example>
        public DateTime LastBootUpTime
        {
            get
            {
                ManagementObject MO = Win32_OperatingSystem;
                return ManagementDateTimeConverter.ToDateTime(MO.GetPropertyValue("LastBootUpTime").ToString());
            }
        }

        /// <summary>
        /// Get the InstallDate of the OperatingSystem
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("System Installation date: " + oClient.ComputerSystem.InstallDate;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ComputerSystem.InstallDate
        /// </code>
        /// </example>
        public DateTime InstallDate
        {
            get
            {
                ManagementObject MO = Win32_OperatingSystem;
                return ManagementDateTimeConverter.ToDateTime(MO.GetPropertyValue("InstallDate").ToString());
            }
        }

        /// <summary>
        /// Get the SystemDrive like "C:"
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("System Drive: " + oClient.ComputerSystem.SystemDrive;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ComputerSystem.SystemDrive
        /// </code>
        /// </example>
        public string SystemDrive
        {
            get
            {
                ManagementObject MO = Win32_OperatingSystem;
                return MO.GetPropertyValue("SystemDrive").ToString();
            }
        }

        /// <summary>
        /// Get the Caption of the OperatingSystem
        /// </summary>
        public string OSCaption
        {
            get
            {
                ManagementObject MO = Win32_OperatingSystem;
                return MO.GetPropertyValue("Caption").ToString();
            }
        }

        /// <summary>
        /// Get the LoggedOn UserName
        /// </summary>
        public string LoggedOnUser
        {
            get
            {
                ManagementObject MO = Win32_ComputerSystem;
                return MO.GetPropertyValue("UserName").ToString();
            } 
        }

        /// <summary>
        /// Get List of LoggedOnUsers
        /// </summary>
        public List<string> LoggedOnUsers
        {
            get
            {
                List<string> lResult = new List<string>();
                if ((oLoggedOnUsers == null) | Reload)
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    oProv.mScope.Options.EnablePrivileges = true;

                    ManagementObjectSearcher MOSLogonSessions = new ManagementObjectSearcher(oProv.mScope, new ObjectQuery("select __relpath from win32_process where caption = 'explorer.exe'"));
                    //ManagementObjectCollection oLogonSessions = MOSLogonSessions.Get();
                    foreach (ManagementObject MOLogonSession in MOSLogonSessions.Get())
                    {
                        try
                        {
                            ManagementBaseObject oResult = MOLogonSession.InvokeMethod("GetOwner", null, null);
                            lResult.Add(oResult["Domain"].ToString() + @"\" + oResult["User"].ToString());
                        }
                        catch(Exception ex)
                        {
                            ex.ToString();
                        }

                    }

                    return lResult;
                }
                else
                {
                    return oLoggedOnUsers;
                }
            }
        }

        /// <summary>
        /// Get the Manufacturer of the System
        /// </summary>
        public string Manufacturer
        {
            get
            {
                ManagementObject MO = Win32_ComputerSystem;
                return MO.GetPropertyValue("Manufacturer").ToString();
            }
        }

        /// <summary>
        /// Get the HW Model of the System
        /// </summary>
        public string Model
        {
            get
            {
                ManagementObject MO = Win32_ComputerSystem;
                return MO.GetPropertyValue("Model").ToString();
            }
        }

        /// <summary>
        /// Get OS Architecture (x86 or x64)
        /// </summary>
        public string Architecture
        {
            get
            {
                //Check if Environment variables are already cached
                if (Win32_Environment != null)
                {
                    foreach (ManagementObject MO in Win32_Environment)
                    {
                        if (string.Compare(MO["Name"].ToString(), "PROCESSOR_ARCHITECTURE", true) == 0)
                        {
                            string sArchitecture = MO["VariableValue"].ToString();
                            if (sArchitecture.Contains("64"))
                                return "x64";
                            else
                                return "x86";
                        }
                    }
                }
                else
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    EnumerationOptions oEnumOpt = new EnumerationOptions();
                    oEnumOpt.ReturnImmediately = true;
                    oEnumOpt.Rewindable = false;
                    ManagementObject MO = oProv.GetObject("Win32_Environment.Name='PROCESSOR_ARCHITECTURE',UserName='<SYSTEM'");
                    string sArchitecture = MO["VariableValue"].ToString();
                    if (sArchitecture.Contains("64"))
                        return "x64";
                    else
                        return "x86";
                }

                return "x86";
            }
        }

        /// <summary>
        /// Check if Computer is running a x64 OS
        /// </summary>
        public bool isX64
        {
            get
            {
                if(Architecture == "x64")
                    return true;
                else
                return false;
            }
        }

        /// <summary>
        /// Get all Win32_Process as ManagementObjectCollection
        /// </summary>
        public ManagementObjectCollection Win32_Process
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\cimv2";
                EnumerationOptions oEnumOpt = new EnumerationOptions();
                oEnumOpt.ReturnImmediately = true;
                oEnumOpt.Rewindable = false;
                return oProv.ExecuteQuery("SELECT * FROM Win32_Process WHERE ProcessID > 4", oEnumOpt);
            }
        }

        /// <summary>
        /// Create a remote process
        /// </summary>
        /// <param name="CommandLine"></param>
        /// <returns>Returns ProcessID or 0 if failed</returns>
        public UInt32 CreateProcess(string CommandLine)
        {
            return CreateProcess(CommandLine, null, null);
        }

        /// <summary>
        /// Create a remote process
        /// </summary>
        /// <param name="CommandLine"></param>
        /// <param name="CurrentDirectory"></param>
        /// <param name="ProcessStartupInformation"></param>
        /// <returns>Returns ProcessID or 0 if failed</returns>
        public UInt32 CreateProcess(string CommandLine, string CurrentDirectory, ManagementBaseObject ProcessStartupInformation)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\cimv2";
                ManagementClass oProcess = oProv.GetClass("Win32_Process");
                ManagementBaseObject inParams = oProcess.GetMethodParameters("Create");
                inParams["CommandLine"] = CommandLine;
                inParams["CurrentDirectory"] = CurrentDirectory;
                inParams["ProcessStartupInformation"] = ProcessStartupInformation;
                ManagementBaseObject outParams = oProcess.InvokeMethod("Create", inParams, null);
                if (int.Parse(outParams["ReturnValue"].ToString()) == 0)
                    return UInt32.Parse(outParams["ProcessId"].ToString());
                else
                    return 0;
            }
            catch
            {
                throw new Exception("Unable to start command: " + CommandLine);
            }
        }

        #endregion
    }
}
