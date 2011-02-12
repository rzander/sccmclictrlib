//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;


namespace smsclictr.automation
{
    /// <summary>
    /// SMS Agent Installation Functions
    /// </summary>
    public class CCMSetup
    {
        #region Internal

        internal WMIProvider oWMIProvider;
        internal SMSClient oSMSClient;
        internal string sHostname;

        /// <summary>
        /// Authenticate/Logon a User
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="domain"></param>
        /// <param name="password"></param>
        /// <param name="logonType">LOGON_TYPE_INTERACTIVE = 2</param>
        /// <param name="logonProvider">LOGON_TYPE_PROVIDER_DEFAULT = 0</param>
        /// <param name="accessToken">IntPtr accessToken</param>
        /// <returns>true = success; false = failure</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static public extern bool LogonUser(string userName, string domain, string password, int logonType, int logonProvider, ref IntPtr accessToken); 


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SMSCli">Instance of smsclictr.automation.SMSClient</param>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///         {
        ///            static void Main(string[] args)
        ///               {
        ///                   SMSClient oClient = new SMSClient("smsserver");
        ///                   CCMSetup oCCMSetup = new CCMSetup(oClient);
        ///               }
        ///         }
        /// }
        /// </code>
        /// </example>
        public CCMSetup(SMSClient SMSCli)
        {
            if (SMSCli != null)
            {
                oSMSClient = SMSCli;
                oWMIProvider = SMSCli.Connection;
                sHostname = oWMIProvider.mScope.Path.Server;
            }
        }

        /// <summary>
        /// Constructor (requires Integrated Authentication)
        /// </summary>
        /// <param name="oProvider">Instance of smsclictr.automation.WMIProvider</param>
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
        ///     class Program
        ///         {
        ///            static void Main(string[] args)
        ///               {
        ///                   CCMSetup oCCMSetup = new CCMSetup(new WMIProvider(@"\\servername\root\cimv2"));
        ///               }
        ///         }
        /// }
        /// </code>
        /// </example>
        public CCMSetup(WMIProvider oProvider)
        {
            oSMSClient = new SMSClient(oProvider.mScope.Path.Server);
            oWMIProvider = oProvider;
            sHostname = oWMIProvider.mScope.Path.Server;
        }

        /// <summary>
        /// empty Constructor if no WMI connection is possible.
        /// </summary>
        public CCMSetup(string Hostname)
        {
            sHostname = Hostname;
        }

        #endregion

        #region External

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, SC_MANAGER_ACCESS dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateService(IntPtr hSCManager,
            string lpServiceName,
            string lpDisplayName,
            SC_MANAGER_ACCESS dwDesiredAccess,
            SC_SERVICE_TYPE dwServiceType,
            SC_START_TYPE dwStartType,
            SC_ERROR_CONTROL dwErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword
            );


        [DllImport("advapi32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool StartService(IntPtr hService, int dwNumServiceArgs, IntPtr lpServiceArgVectors);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteService(IntPtr hService);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, SC_MANAGER_ACCESS dwDesiredAccess);

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection3(IntPtr hWndOwner,
            ref NETRESOURCE lpNetResource, string lpPassword,
            string lpUserName, int dwFlags);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseServiceHandle(IntPtr hSCObject);

#endregion

        #region Flags
        [Flags]
        internal enum SC_MANAGER_ACCESS : uint
        {
            CONNECT = 0x0001,
            CREATE_SERVICE = 0x0002,
            ENUMERATE_SERVICE = 0x0004,
            LOCK = 0x0008,
            QUERY_LOCK_STATUS = 0x0010,
            MODIFY_BOOT_CONFIG = 0x0020,

            STANDARD_RIGHTS_REQUIRED = 0x000F0000,

            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000,

            ALL_ACCESS = STANDARD_RIGHTS_REQUIRED |
                                    CONNECT |
                                    CREATE_SERVICE |
                                    ENUMERATE_SERVICE |
                                    LOCK |
                                    QUERY_LOCK_STATUS |
                                    MODIFY_BOOT_CONFIG
        }

        internal enum SC_SERVICE_TYPE : uint
        {
            KERNEL_DRIVER = 0x00000001,
            FILE_SYSTEM_DRIVER = 0x00000002,
            ADAPTER = 0x00000004,
            RECOGNIZER_DRIVER = 0x00000008,

            DRIVER = (KERNEL_DRIVER |
                FILE_SYSTEM_DRIVER |
                RECOGNIZER_DRIVER),

            WIN32_OWN_PROCESS = 0x00000010,
            WIN32_SHARE_PROCESS = 0x00000020,
            WIN32 = (WIN32_OWN_PROCESS |
                WIN32_SHARE_PROCESS),

            INTERACTIVE_PROCESS = 0x00000100,

            TYPE_ALL = (WIN32 |
                ADAPTER |
                DRIVER |
                INTERACTIVE_PROCESS)
        }

        internal enum SC_START_TYPE : int
        {
            SERVICE_BOOT_START = 0x00000000,
            SERVICE_SYSTEM_START = 0x00000001,
            SERVICE_AUTO_START = 0x00000002,
            SERVICE_DEMAND_START = 0x00000003,
            SERVICE_DISABLED = 0x00000004
        }

        internal enum SC_ERROR_CONTROL : int
        {
            SERVICE_ERROR_IGNORE = 0x00000000,
            SERVICE_ERROR_NORMAL = 0x00000001,
            SERVICE_ERROR_SEVERE = 0x00000002,
            SERVICE_ERROR_CRITICAL = 0x00000003
        }

        struct NETRESOURCE
        {
            internal int dwScope;
            internal int dwType;
            internal int dwDisplayType;
            internal int dwUsage;
            internal string LocalName;
            internal string RemoteName;
            internal string Comment;
            internal string Provider;
        }

        internal const int RESOURCETYPE_ANY = 0x0;
        internal const int CONNECT_INTERACTIVE = 0x8;
        internal const int CONNECT_PROMPT = 0x10;
#endregion

        #region Functions

        /// <summary>
        /// Start the CCMSetup Service on the remote System
        /// </summary>
        /// <param name="ccmpath">Path to the ccmsetup.exe (\\SMSServer\SMSClient\i386\ccmsetup.exe)</param>
        /// <param name="sParams">CCMSetup Parameters (like: SMSSITECODE=AUTO)</param>
        /// <exception cref="System.Exception">Installation Failure</exception>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///         {
        ///             static void Main(string[] args)
        ///                 {
        ///                      SMSClient oClient = new SMSClient("localhost");
        ///                      CCMSetup oCCMSetup = new CCMSetup(oClient);
        ///                      oCCMSetup.SMSSetup(@"\\servername\smsclient\i386\ccmsetup.exe", "SMSSITECODE=AUTO");
        ///                 }
        ///          }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("localhost")
        /// $CCMSetup = New-Object -TypeName smsclictr.automation.CCMSetup($SMSClient)
        /// $CCMSetup.SMSSetup("\\servername\smsclient\i386\ccmsetup.exe", "SMSSITECODE=AUTO")
        /// </code>
        /// </example>
        public void SMSSetup(string ccmpath, string sParams)
        {
            try
            {
                InstallService(sHostname, ccmpath, sParams, "CCMSetup");
                //InstallService(oWMIProvider.mScope.Path.Server, ccmpath, " /runservice " + sParams, "CCMSetup");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Start the CCMSetup Service on the remote System
        /// </summary>
        /// <param name="ccmpath">Path to the ccmsetup.exe (\\SMSServer\SMSClient\i386\ccmsetup.exe)</param>
        /// <param name="sParams">CCMSetup Parameters (like: SMSSITECODE=AUTO)</param>
        /// <param name="IntegratedAuthentication">Use Integrated Authentication(True), otherwise connect the IPC$ Share of the remote Host to authenticate</param>
        /// <exception cref="System.Exception">Installation Failure</exception>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///         {
        ///             static void Main(string[] args)
        ///                 {
        ///                      SMSClient oClient = new SMSClient("localhost");
        ///                      CCMSetup oCCMSetup = new CCMSetup(oClient);
        ///                      oCCMSetup.SMSSetup(@"\\servername\smsclient\i386\ccmsetup.exe", "SMSSITECODE=AUTO", True);
        ///                 }
        ///          }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("localhost")
        /// $CCMSetup = New-Object -TypeName smsclictr.automation.CCMSetup($SMSClient)
        /// $CCMSetup.SMSSetup("\\servername\smsclient\i386\ccmsetup.exe", "SMSSITECODE=AUTO", "True")
        /// </code>
        /// </example>
        public void SMSSetup(string ccmpath, string sParams, bool IntegratedAuthentication)
        {
            try
            {
                if (!IntegratedAuthentication)
                {
                    ConnectIPC();
                }
                InstallService(sHostname, ccmpath, sParams, "CCMSetup");
                //InstallService(oWMIProvider.mScope.Path.Server, ccmpath, " /runservice " + sParams, "CCMSetup");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Start the CCMSetup Service on the remote System
        /// </summary>
        /// <param name="ccmpath">Path to the ccmsetup.exe (\\SMSServer\SMSClient\i386\ccmsetup.exe)</param>
        /// <param name="sParams">CCMSetup Parameters (like: SMSSITECODE=AUTO)</param>
        /// <param name="UserName">Username (Doamin\User)</param>
        /// <param name="Password">Password of the specified Username</param>
        /// <exception cref="System.Exception">Installation Failure</exception>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///         {
        ///             static void Main(string[] args)
        ///                 {
        ///                      SMSClient oClient = new SMSClient("localhost");
        ///                      CCMSetup oCCMSetup = new CCMSetup(oClient);
        ///                      oCCMSetup.SMSSetup(@"\\servername\smsclient\i386\ccmsetup.exe", "SMSSITECODE=AUTO", "CORP\SMSAdmin", "password");
        ///                 }
        ///          }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("localhost")
        /// $CCMSetup = New-Object -TypeName smsclictr.automation.CCMSetup($SMSClient)
        /// $CCMSetup.SMSSetup("\\servername\smsclient\i386\ccmsetup.exe", "SMSSITECODE=AUTO", "Corp\SMSAdmin", "password")
        /// </code>
        /// </example>
        public void SMSSetup(string ccmpath, string sParams, string UserName, string Password)
        {
            try
            {
                ConnectIPC(sHostname, UserName, Password);
                InstallService(sHostname, ccmpath, sParams, "CCMSetup");
                //InstallService(oWMIProvider.mScope.Path.Server, ccmpath, " /runservice " + sParams, "CCMSetup");

            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Connect the IPC$ Share of a remote System
        /// </summary>
        /// <returns>System Error Codes (SUCCESS = 0)"
        /// <a href="#" onclick='javascript: window.open("http://msdn2.microsoft.com/en-us/library/ms681381.aspx" );'>Error Codes</a>
        /// </returns>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///         {
        ///            static void Main(string[] args)
        ///               {
        ///                   SMSClient oClient = new SMSClient("smsserver");
        ///                   CCMSetup oCCMSetup = new CCMSetup(oClient);
        ///                   oCCMSetup.ConnectIPC();
        ///               }
        ///         }
        /// }
        /// </code>
        /// </example>
        public int ConnectIPC()
        {
            if (oSMSClient.pUsername != null)
            {
                return ConnectIPC(sHostname, oSMSClient.pUsername, oSMSClient.pPassword);
            }
            else
            {
                return ConnectIPC(oSMSClient);
            }
        }

        /// <summary>
        /// Connect the IPC$ Share of a remote System
        /// </summary>
        /// <param name="oSMSClient">reference to a smsclictr.automation.SMSClient Instance</param>
        /// <returns>System Error Codes (SUCCESS = 0); 
        /// <a href="#" onclick='javascript: window.open("http://msdn2.microsoft.com/en-us/library/ms681381.aspx" );'>Error Codes</a>
        /// </returns>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///         {
        ///            static void Main(string[] args)
        ///               {
        ///                   SMSClient oClient = new SMSClient("smsserver");
        ///                   CCMSetup oCCMSetup = new CCMSetup(oClient);
        ///                   oCCMSetup.ConnectIPC();
        ///               }
        ///         }
        /// }
        /// </code>
        /// </example>
        static public int ConnectIPC(SMSClient oSMSClient)
        {
            return ConnectIPC(oSMSClient.pHostname, oSMSClient.pUsername, oSMSClient.pPassword);
        }

        /// <summary>
        /// Connect the IPC$ Share of a remote System
        /// </summary>
        /// <param name="Hostname">Hostname or IP-Address of the remote System</param>
        /// <param name="UserName">Username (Domain\User)</param>
        /// <param name="Password">Password of the specified Username</param>
        /// <returns>System Error Codes (SUCCESS = 0); 
        /// <a href="#" onclick='javascript: window.open("http://msdn2.microsoft.com/en-us/library/ms681381.aspx" );'>Error Codes</a>
        /// </returns>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///     class Program
        ///         {
        ///            static void Main(string[] args)
        ///               {
        ///                   SMSClient oClient = new SMSClient("smsserver");
        ///                   CCMSetup oCCMSetup = new CCMSetup(oClient);
        ///                   oCCMSetup.ConnectIPC("anotherserver", @"corp\smsadmin", "password");
        ///               }
        ///         }
        /// }
        /// </code>
        /// </example>
        static public int ConnectIPC(string Hostname, string UserName, string Password)
        {
            NETRESOURCE ConnInf = new NETRESOURCE();
            ConnInf.dwType = RESOURCETYPE_ANY;
            ConnInf.RemoteName = @"\\" + Hostname;
            IntPtr hWnd = IntPtr.Zero;
            return WNetAddConnection3(hWnd, ref ConnInf, Password, UserName, 0);
        }

        /// <summary>
        /// Create a remote Service with system privileges...
        /// </summary>
        /// <param name="sClient">Hostname to install the Service</param>
        /// <param name="sServicepath">Path to the Executable</param>
        /// <param name="sParams">Additional Parameters for the executable</param>
        /// <param name="sServiceName">Name of the new Service</param>
        /// <exception cref="System.Exception">Installation Failure</exception>
        static public void InstallService(string sClient, string sServicepath, string sParams, string sServiceName)
        {
            IntPtr SCM = OpenSCManager(sClient, null, SC_MANAGER_ACCESS.ALL_ACCESS);
            IntPtr SHandle = CreateService(SCM, sServiceName, sServiceName, SC_MANAGER_ACCESS.ALL_ACCESS, SC_SERVICE_TYPE.INTERACTIVE_PROCESS | SC_SERVICE_TYPE.WIN32_OWN_PROCESS, SC_START_TYPE.SERVICE_AUTO_START, SC_ERROR_CONTROL.SERVICE_ERROR_NORMAL, (sServicepath + " " + sParams).Trim(), null, (IntPtr)0, null, null, null);
            if (SHandle != IntPtr.Zero)
            {
                bool sResult = StartService(SHandle, 0, IntPtr.Zero);

                if (sResult)
                {
                    return;
                }
                else
                {
                    try
                    {
                        IntPtr SHandle2 = OpenService(SCM, sServiceName, SC_MANAGER_ACCESS.ALL_ACCESS);
                        DeleteService(SHandle2);
                        CloseServiceHandle(SCM);
                        CloseServiceHandle(SHandle2);
                    }
                    catch { }
                    throw (new Exception("Service failed !"));

                }
            }
            else
            {
                try
                {
                    IntPtr SHandle2 = OpenService(SCM, sServiceName, SC_MANAGER_ACCESS.ALL_ACCESS);
                    DeleteService(SHandle2);
                    CloseServiceHandle(SCM);
                    CloseServiceHandle(SHandle2);
                }
                catch { }
                throw (new Exception("unable to create Service...!"));
            }
           
        }

        /// <summary>
        /// Create a WindowsIdentity Object from the specified Credentials
        /// </summary>
        /// <param name="Domain">Domain Name</param>
        /// <param name="UserName">User Name</param>
        /// <param name="Password">Password</param>
        /// <returns></returns>
        static public WindowsIdentity identity(string Domain, string UserName, string Password)
        {
            int LOGON_TYPE_INTERACTIVE = 2; 
            int LOGON_TYPE_PROVIDER_DEFAULT = 0;

            IntPtr accessToken = IntPtr.Zero;
            if (LogonUser(UserName, Domain, Password, LOGON_TYPE_INTERACTIVE, LOGON_TYPE_PROVIDER_DEFAULT, ref accessToken))
            {
                return new WindowsIdentity(accessToken);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
