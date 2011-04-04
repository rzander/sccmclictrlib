//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections;
using System.Management;
using System.Runtime.InteropServices;
using System.Collections.Generic;

[assembly: CLSCompliant(false)]
namespace smsclictr.automation
{
    /// <summary>
    /// SMSClient automation main class
    /// </summary>
    public class SMSClient : IDisposable
    {
        #region Internal Settings

        private WMIProvider oWMIProvider;
        private ManagementObject mo_SMS_Authority;
        private string sSiteCode;
        private string sMP;
        private SMSSchedules oSMSSchedules;
        private WMIRegistry oWMIRegistry;
        private WMIService oWMIService;
        private CCMSoftwareDistribution oCCMSoftwareDistribution;
        private WindowsInstaller oMSI;
        private WMIFileIO oWMIFileIO;
        private WMIComputerSystem oCompSys;
        private SMSComponents oSMSComponents;
        private SCCMDCM oDCM;
        private ManagementObject oSMS_Client;
        private ManagementObject oCCM_Client;
        private ManagementObject oCacheConfig;
        internal string pHostname;
        internal string pUsername;
        internal string pPassword;
        internal string sSMSVersion = "";
        internal string sLocalSMSPath = "";

        #endregion

        #region Constructors

        /// <summary>
        /// SMSClient Constructor
        /// </summary>
        /// <param name="hostname">Hostname or IP-Address of the remote host</param>
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
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// </code>
        /// </example>
        public SMSClient(string hostname)
        {
            connect(hostname, null, null);
        }

        /// <summary>
        /// SMSClient Constructor
        /// </summary>
        /// <param name="hostname">Hostname or IP-Address of the remote host</param>
        /// <param name="username">Username to Logon (Domain\User)</param>
        /// <param name="password">Password of the specified Username</param>
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
        ///                SMSClient oClient = new SMSClient("workstation01", "Domain\SMSAdmin", "password");
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01", "Domain\SMSAdmin", "password")
        /// </code>
        /// </example>
        public SMSClient(string hostname, string username, string password)
        {
            connect(hostname, username, password);
        }

        /// <summary>
        /// SMSClient Constructor
        /// </summary>
        /// <param name="wmiProvider">WMIProvider</param>
        public SMSClient(WMIProvider wmiProvider)
        {
            connect(wmiProvider);
        }

        /// <summary>
        /// Connect the WMI Namespace root\cimv2 on a remote system
        /// </summary>
        /// <param name="sHostname">Hostname</param>
        protected void connect(string sHostname)
        {
            connect(sHostname, null, null);
        }

        /// <summary>
        /// Connect the WMI Namespace root\cimv2 on a remote system
        /// </summary>
        /// <param name="sHostname"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        protected void connect(string sHostname, string username, string password)
        {
            oWMIProvider = new WMIProvider(@"\\" + sHostname + @"\ROOT\cimv2", username, password);
            connect(oWMIProvider);
            pHostname = sHostname;
            pUsername = username;
            pPassword = password;
        }

        /// <summary>
        /// Connect the WMI Namespace root\cimv2 on a remote system
        /// </summary>
        /// <param name="oProvider">WMI Provider</param>
        protected void connect(WMIProvider oProvider)
        {
            oWMIProvider = oProvider;
            oSMSSchedules = new SMSSchedules(oWMIProvider);
            oWMIRegistry = new WMIRegistry(oWMIProvider);
            oWMIService = new WMIService(oWMIProvider);
            oCCMSoftwareDistribution = new CCMSoftwareDistribution(oWMIProvider);
            oMSI = new WindowsInstaller(oWMIProvider);
            oWMIFileIO = new WMIFileIO(oWMIProvider);
            oCompSys = new WMIComputerSystem(oWMIProvider);
            oSMSComponents = new SMSComponents(oWMIProvider);
            oDCM = new SCCMDCM(oWMIProvider);
        }

        /// <summary>
        /// SMSSchedules Class
        /// </summary>
        public SMSSchedules Schedules { get { return oSMSSchedules; } }

        /// <summary>
        /// CCM_SoftwareDistribution Class
        /// </summary>
        public CCMSoftwareDistribution SoftwareDistribution { get { return oCCMSoftwareDistribution; } }

        /// <summary>
        /// WindowsInstaller Class
        /// </summary>
        public WindowsInstaller MSI { get { return oMSI; } }

        /// <summary>
        /// WMIFileIO Class
        /// </summary>
        public WMIFileIO FileIO { get { return oWMIFileIO; } }

        /// <summary>
        /// WMIComputerSystem Class
        /// </summary>
        public WMIComputerSystem ComputerSystem { get { return oCompSys; } }

        /// <summary>
        /// SMSComponents Class
        /// </summary>
        public SMSComponents Components { get { return oSMSComponents; } }

        /// <summary>
        /// SCCMDCM Class
        /// </summary>
        public SCCMDCM DCM { get { return oDCM; } }

        /// <summary>
        /// Cache the Object "root\ccm:SMS_Client"
        /// </summary>
        internal ManagementObject mSMS_Client
        {
            get 
            {
                if ((oSMS_Client == null) | Reload)
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                    Reload = false;
                    return oProv.GetObject("SMS_Client=@");
                }
                else
                    return oSMS_Client;
            }

            set
            {
                oSMS_Client = value;
            }
         }

        /// <summary>
        /// Cache the Object "root\ccm:CCM_Client"
        /// </summary>
        internal ManagementObject mCCM_Client
        {
            get
            {
                if ((oCCM_Client == null) | Reload)
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                    Reload = false;
                    return oProv.GetObject("CCM_Client=@");
                }
                else
                    return oCCM_Client;
            }

            set
            {
                oCCM_Client = value;
            }
        }

        /// <summary>
        /// Cache the Object "root\ccm\SoftMgmtAgent:CacheConfig"
        /// </summary>
        internal ManagementObject mCacheConfig
        {
            get
            {
                if ((oCacheConfig == null) | Reload)
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";
                    Reload = false;
                    return oProv.GetObject("CacheConfig.ConfigKey='Cache'");

                }
                else
                    return oCacheConfig;
            }

            set
            {
                oCacheConfig = value;
            }
        }

        /// <summary>
        /// Do not use cached Object. Reload the Object (Once!)
        /// </summary>
        public bool Reload
        {
            get;set;
        }

        #endregion

        #region Puplic Properties

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            oWMIProvider = null;
            GC.Collect();
        }

        /// <summary>
        /// Get or Set the assigned SMS SiteCode
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
        ///                Console.WriteLine(oClient.SiteCode);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.SiteCode
        /// </code>
        /// </example>
        public string SiteCode 
        { 
            get 
            {
                if (sSiteCode != null)
                    return sSiteCode;
                else
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";

                    sSiteCode = oProv.ExecuteMethod("SMS_Client", "GetAssignedSite").GetPropertyValue("sSiteCode").ToString();
                    return sSiteCode;
                }
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                    sSiteCode = null;
                else
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";

                    ManagementBaseObject inParams = oProv.GetClass("SMS_Client").GetMethodParameters("SetAssignedSite");
                    inParams["sSiteCode"] = value;
                    oProv.ExecuteMethod("SMS_Client", "SetAssignedSite", inParams);
                    //sSiteCode = value;
                    sSiteCode = null; //to clear the cached code...
                }
            }
        }

        /// <summary>
        /// Get the assigned Management Point
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
        ///                Console.WriteLine(oClient.ManagementPoint);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ManagementPoint
        /// </code>
        /// </example>
        public string ManagementPoint
        {
            get
            {
                if (!string.IsNullOrEmpty(sMP))
                    return sMP;
                else
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM";

                    mo_SMS_Authority = oProv.GetObject("SMS_Authority.Name='SMS:" + SiteCode + "'");
                    return mo_SMS_Authority.GetPropertyValue("CurrentManagementPoint").ToString();
                }
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                    sMP = null;
            }
        }

        /// <summary>
        /// Get the assigned proxy Management Point (if the client belongs to a secondary site)
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
        ///                Console.WriteLine(oClient.ProxyManagementPoint);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ProxyManagementPoint
        /// </code>
        /// </example>
        public string ProxyManagementPoint
        {
            get
            {
                ManagementObjectCollection MPProxies;
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                MPProxies = oProv.ExecuteQuery("SELECT * FROM SMS_MPProxyInformation Where State = 'Active'");
                foreach (ManagementObject MPProxy in MPProxies)
                {
                    return MPProxy.GetPropertyValue("Name").ToString();
                }
                return null;
            }
        }

        /// <summary>
        /// Get the assigned Internet Management Point
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
        ///                Console.WriteLine(oClient.InternetMP);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.InternetMP
        /// </code>
        /// </example>
        public string InternetMP
        {
            get
            {
                string sPort = oWMIRegistry.GetString(2147483650, @"SOFTWARE\Microsoft\SMS\Client\Internet Facing", "Internet MP Hostname", "");
                if (string.IsNullOrEmpty(sPort))
                {
                    if (this.ComputerSystem.Win32_ComputerSystem["SystemType"].ToString().ToLower().Contains("x64"))
                    {
                        sPort = oWMIRegistry.GetString(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\SMS\Client\Internet Facing", "Internet MP Hostname", "");
                    }
                }

                return sPort;
            }
            set
            {
                if (!this.ComputerSystem.Win32_ComputerSystem["SystemType"].ToString().ToLower().Contains("x64"))
                {
                    oWMIRegistry.SetStringValue(2147483650, @"SOFTWARE\Microsoft\SMS\Client\Internet Facing", "Internet MP Hostname", value);
                }
                else
                {
                    oWMIRegistry.SetStringValue(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\SMS\Client\Internet Facing", "Internet MP Hostname", value);
                }
            }
        }

        /// <summary>
        /// Get the assigned DNS Suffix
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
        ///                Console.WriteLine(oClient.DNSSuffix);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.DNSSuffix
        /// </code>
        /// </example>
        public string DNSSuffix
        {
            get
            {
                string sPort = oWMIRegistry.GetString(2147483650, @"SOFTWARE\Microsoft\CCM\LocationServices", "DnsSuffix", "");
                if (string.IsNullOrEmpty(sPort))
                {
                    if (this.ComputerSystem.Win32_ComputerSystem["SystemType"].ToString().ToLower().Contains("x64"))
                    {
                        sPort = oWMIRegistry.GetString(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\CCM\LocationServices", "DnsSuffix", "");
                    }
                }

                return sPort;
            }
            set
            {
                if (this.ComputerSystem.Win32_ComputerSystem["SystemType"].ToString().ToLower().Contains("x64"))
                {
                    oWMIRegistry.SetStringValue(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\CCM\LocationServices", "DnsSuffix", value);
                }
                else
                {
                    oWMIRegistry.SetStringValue(2147483650, @"SOFTWARE\Microsoft\CCM\LocationServices", "DnsSuffix", value);
                }
            }
        }

        /// <summary>
        /// Get or Set the HTTP port for Client-Server communication
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
        ///                Console.WriteLine(oClient.HttpPort);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.HttpPort
        /// </code>
        /// </example>
        public string HttpPort
        {
            get
            {
                string sPort = oWMIRegistry.GetDWord(2147483650, @"SOFTWARE\Microsoft\CCM", "HttpPort", "");
                if (string.IsNullOrEmpty(sPort))
                {
                    if (this.ComputerSystem.Win32_ComputerSystem["SystemType"].ToString().ToLower().Contains("x64"))
                    {
                        sPort = oWMIRegistry.GetDWord(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\CCM", "HttpPort", "");
                    }
                }

                return sPort;
            }
            set
            {
                if (this.ComputerSystem.Win32_ComputerSystem["SystemType"].ToString().ToLower().Contains("x64"))
                {
                    oWMIRegistry.SetDWord(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\CCM", "HttpPort", value);
                }
                else
                {
                    oWMIRegistry.SetDWord(2147483650, @"SOFTWARE\Microsoft\CCM", "HttpPort", value);
                }
            }
        }

        /// <summary>
        /// Get or Set the SCCM Server Locator Point
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
        ///                Console.WriteLine(oClient.ServerLocatorPoint);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ServerLocatorPoint
        /// </code>
        /// </example>
        public string ServerLocatorPoint
        {
            get
            {
                if (!this.oWMIProvider.isX86)
                {
                    return oWMIRegistry.GetString(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\CCM", "SMSSLP");
                }
                else
                {
                    return oWMIRegistry.GetString(2147483650, @"SOFTWARE\Microsoft\CCM", "SMSSLP");
                }
            }
            set
            {
                if (!this.oWMIProvider.isX86)
                {
                    oWMIRegistry.SetStringValue(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\CCM", "SMSSLP", value);
                }
                else
                {
                    oWMIRegistry.SetStringValue(2147483650, @"SOFTWARE\Microsoft\CCM", "SMSSLP", value);
                }
            }
        }

        /// <summary>
        /// Get or Set the SMS Cache Path
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
        ///                Console.WriteLine(oClient.CachePath);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.CachePath
        /// </code>
        /// </example>
        public string CachePath
        {
            get
            {
                return mCacheConfig.GetPropertyValue("Location").ToString();
            }

            set
            {
                string CachePath = value;
                if (CachePath.Length > 3)
                {
                    ManagementObject MO = mCacheConfig;
                    MO.SetPropertyValue("Location", CachePath);
                    MO.Put();
                    mCacheConfig = MO;
                }

            }
        }

        /// <summary>
        /// Get or Set the SMS Cache Size in Megabyte (MB)
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
        ///                UInt32 CacheSize = UInt32.Parse(oClient.CacheSize);
        ///                if (CacheSize != 600)
        ///                    {
        ///                         //Set the CacheSize to 600MB
        ///                         oClient.CacheSize = "600";
        ///                     }
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.CacheSize = "600"
        /// </code>
        /// </example>
        public string CacheSize
        {
            get
            {
                return mCacheConfig.GetPropertyValue("Size").ToString();
            }

            set
            {
                UInt32 CacheSize = System.Convert.ToUInt32(value);

                if (CacheSize > 0)
                {
                    ManagementObject MO = mCacheConfig;
                    MO.SetPropertyValue("Size", CacheSize);
                    MO.Put();
                    mCacheConfig = MO;
                }

            }
        }

        /// <summary>
        /// Size of the downloaded packages in cache
        /// </summary>
        public Int32 CacheContentSize
        {
            get
            {
                ManagementObjectCollection MOC;
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";

                if (this.SMSVersion.StartsWith("4"))
                {
                    MOC = oProv.ExecuteQuery("SELECT * FROM CacheInfoEx");
                }
                else
                {

                    MOC = oProv.ExecuteQuery("SELECT * FROM CacheInfo");
                }
                Int32 uSize = 0;

                foreach (ManagementObject MO in MOC)
                {
                    uSize = uSize + Int32.Parse(MO.GetPropertyValue("ContentSize").ToString());
                }
                return uSize;

            }
        }

        /// <summary>
        /// Allow the local Admin to override Site Settings
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
        ///                //Prevent an Admin to change SMS Settings (Control Panel)
        ///                oClient.AllowLocalAdminOverride = false;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.AllowLocalAdminOverride = "False"
        /// </code>
        /// </example>
        public bool AllowLocalAdminOverride 
        {
            get
            {
                return Boolean.Parse(mSMS_Client.GetPropertyValue("AllowLocalAdminOverride").ToString());
            }
            set
            {
                ManagementObject MO = mSMS_Client;
                MO.SetPropertyValue("AllowLocalAdminOverride", value);
                MO.Put();
                mSMS_Client = MO;
                MO.Dispose();
            }
        }

        /// <summary>
        /// Get the SMS Agent Type
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>1 = Advanced Client</item>
        /// <item>0 = Legacy Client</item>
        /// </list>
        /// </remarks>
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
        ///                if (oClient.ClientType == 1)
        ///                     {
        ///                         Console.WriteLine("Advanced Client");
        ///                     }
        ///                else
        ///                     {
        ///                         Console.WriteLine("Legacy Client");
        ///                     }
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ClientType
        /// </code>
        /// </example>
        public int ClientType
        {
            get
            {
                return int.Parse(mSMS_Client.GetPropertyValue("ClientType").ToString());
            }
        }

        /// <summary>
        /// Enable automatic Site assignment.
        /// <para>This will force the SMS Client to automatically reassign the SMS Site if the client roams to another SMS Primary Site.</para>
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
        ///                oClient.EnableAutoAssignment = true;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.EnableAutoAssignment = "True"
        /// </code>
        /// </example>
        public bool EnableAutoAssignment {
            get
            {
                return bool.Parse(mSMS_Client.GetPropertyValue("EnableAutoAssignment").ToString());
            }
            set
            {
                //WMIProvider oProv = new WMIProvider(oWMIProvider.mScope);
                //oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                ManagementObject MO = mSMS_Client;
                MO.SetPropertyValue("EnableAutoAssignment", value);
                MO.Put();

                //Refresh cached Object
                mSMS_Client = MO;

                MO.Dispose();
            }
        }

        /// <summary>
        /// Get the SMS GUID
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
        ///                Console.WriteLine(oClient.ClientId);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ClientID
        /// </code>
        /// Output Example:
        /// <code>
        /// GUID:645567CB-D0C5-4A31-8D54-A74A88E565C4
        /// </code>
        /// </example>
        public string ClientId
        {
            get
            {
                return mCCM_Client.GetPropertyValue("ClientId").ToString();
            }
        }

        /// <summary>
        /// Delete the SMS/SCCM GUID on next SMS Agent restart
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
        ///                oClient.DeleteGUID();
        ///                oClient.RestartSMSAgent();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.DeleteGUID()
        /// $SMSClient.RestartSMSAgent()
        /// </code>
        /// </example>
        public void DeleteGUID()
        {
            if (this.SMSVersion.StartsWith("4"))
            {
                //Create an SMSCFG.INI File with a new, random GUID ...
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"\\" + this.Connection.mScope.Path.Server + @"\admin$\SMSCFG.INI", false))
                {
                    sw.WriteLine("[Configuration - Client Properties]");
                    sw.WriteLine("SMS Unique Identifier=GUID:" + System.Guid.NewGuid().ToString().ToUpper());
                    sw.Close();
                }
            }
            else
            {
                //On SMS2003 Agents, delete the smscfg.ini
                string WinDir = ComputerSystem.WindowsDirectory;
                FileIO.DeleteFile(WinDir + @"\smscfg.ini");
            }
        }

        /// <summary>
        /// Get the last GUID change date
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
        ///                Console.WriteLine(oClient.ClientIdChangeDate);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ClientIDChangeDate
        /// </code>
        /// Output Example:
        /// <code>
        /// 12/22/2006 15:18:10
        /// </code>
        /// </example>
        public string ClientIdChangeDate 
        {
            get
            {
                return mCCM_Client.GetPropertyValue("ClientIdChangeDate").ToString();
            }
        }

        /// <summary>
        /// Get the previous SMS GUID
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
        ///                Console.WriteLine(oClient.PreviousClientID);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.PreviousClientID
        /// </code>
        /// Output Example:
        /// <code>
        /// GUID:645567CB-D0C5-4A31-8D54-A74A88E565C4
        /// </code>
        /// </example>
        public string PreviousClientId
        {
            get
            {
                return mCCM_Client.GetPropertyValue("PreviousClientId").ToString();
            }
        }

        /// <summary>
        /// Get the SMS Version (like 2.50 )
        /// </summary>
        /// <value>
        /// Known SMS Versions:
        /// <list type="bullet">
        /// <item>2.50 = SMS 2003</item>
        /// <item>2.00 = SMS 2.0</item>
        /// </list>
        /// </value>
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
        ///                Console.WriteLine(oClient.SMSVersion);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.SMSVersion
        /// </code>
        /// </example>
        public string SMSVersion
        { 
            get
            {
                if (string.IsNullOrEmpty(sSMSVersion))
                {
                    string sVersion = mSMS_Client.GetPropertyValue("ClientVersion").ToString();
                    return sVersion;
                }
                else
                    return sSMSVersion;
            }
            set
            {
                sSMSVersion = "";
                oSMS_Client = null;
            }
        }

        /// <summary>
        /// Get the Log-Directory Path 
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
        ///                Console.WriteLine(oClient.LogDirectory);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.LogDirectory
        /// </code>
        /// Output Example:
        /// <code>
        /// C:\WINDOWS\system32\CCM\Logs
        /// </code>
        /// </example>
        public string LogDirectory
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM\Policy\Machine";
                return oProv.GetObject("CCM_Logging_GlobalConfiguration.DummyKey=1").GetPropertyValue("LogDirectory").ToString();
            }
        }

        /// <summary>
        /// Get the Local SMS Path
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
        ///                Console.WriteLine(oClient.LocalSMSPath);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.LocalSMSPath
        /// </code>
        /// Output Example:
        /// <code>
        /// C:\WINDOWS\system32\CCM
        /// </code>
        /// </example>
        public string LocalSMSPath
        {
            get
            {
                if (string.IsNullOrEmpty(sLocalSMSPath))
                {
                    sLocalSMSPath = oWMIRegistry.GetString(2147483650, @"SOFTWARE\Microsoft\SMS\Client\Configuration\Client Properties", "Local SMS Path");
                }
                return sLocalSMSPath;
            }
        }

        /// <summary>
        /// Get the MSI ProductCode of the installed SMS Client Agent
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
        ///                Console.WriteLine(oClient.ProductCode);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ProductCode
        /// </code>
        /// Output Example:
        /// <code>
        /// {D97113AD-690F-4169-8637-4A046282D8F6}
        /// </code>
        /// </example>
        public string ProductCode
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm";
                ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM CCM_InstalledProduct");
                foreach (ManagementObject MO in MOC)
                {
                    return MO.GetPropertyValue("ProductCode").ToString();
                }
                return "";
            }
        }

        /// <summary>
        /// OutOfBand AutoProvisioning
        /// </summary>
        public Boolean OOBAutoProvision
        {
            get 
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\ActualConfig";
                ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM CCM_OutOfBandManagementSettings");
                foreach (ManagementObject MO in MOC)
                {
                    return Boolean.Parse(MO.GetPropertyValue("AutoProvision").ToString());
                }
                return false;
            }
            set
            {
                try
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\ActualConfig";
                    ManagementObject MO = oProv.GetClass("CCM_OutOfBandManagementSettings").CreateInstance();
                    MO.SetPropertyValue("SiteSettingsKey", "1");
                    MO.SetPropertyValue("AutoProvision", value.ToString());
                    MO.Put();
                }
                catch { }
            }
        }

        /// <summary>
        /// Return List of all approved (from SCCM Site) Software Update ID's
        /// </summary>
        public List<string> ApprovedUpdateIDs
        {
            get
            {
                List<string> sResult = new List<string>();
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\RequestedConfig";

                ManagementClass MC = oProv.GetClass("CCM_UpdateCIAssignment");

                foreach (ManagementObject MO in MC.GetInstances())
                {
                    try
                    {
                        string[] sAssignedCIs = MO.Properties["AssignedCIs"].Value as string[];
                        if (sAssignedCIs != null)
                        {
                            foreach (string sXML in sAssignedCIs)
                            {
                                try
                                {
                                    System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
                                    xDoc.LoadXml(sXML);
                                    foreach (System.Xml.XmlNode xNode in xDoc.SelectNodes(@"/CI/ApplicabilityCondition/ApplicabilityRule/UpdateId"))
                                    {
                                        sResult.Add(xNode.InnerText);
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    catch
                    { }
                }

                return sResult;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Request and download new Machine Policies
        /// </summary>
        /// <remarks>Downloaded policies needs to be evaluated(applied) before they become active</remarks>
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
        ///                oClient.RequestMachinePolicy();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.RequestMachinePolicy()
        /// </code>
        /// </example>
        public void RequestMachinePolicy()
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
            oProv.ExecuteMethod("SMS_Client", "RequestMachinePolicy");
        }

        /// <summary>
        /// Assign all downloaded Machine Policies
        /// </summary>
        /// <remarks>Wait approximately 2 Minutes after a policy request before evaluating policies</remarks>
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
        ///                oClient.EvaluateMachinePolicy();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.EvaluateMachinePolicy()
        /// </code>
        /// </example>
        public void EvaluateMachinePolicy()
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
            oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
        }

        /// <summary>
        /// Start the Reset Policy cycle to cleanup orphaned policies
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
        ///                oClient.ResetPolicy();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ResetPolicy()
        /// </code>
        /// </example>
        public void ResetPolicy()
        {
            ResetPolicy(false);
        }

        /// <summary>
        /// Reset all SMS Policies (Full Reset)
        /// </summary>
        /// <param name="hard">Hard Reset</param>
        /// <remarks>a Hard-Reset will remove all assigned policies.
        /// <para>All pending downloads and executions will be stopped.</para></remarks>
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
        ///                oClient.ResetPolicy(true);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ResetPolicy("True")
        /// </code>
        /// </example>
        public void ResetPolicy(Boolean hard)
        {
            if (hard)
            {
                try
                {
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy", "SELECT * FROM CCM_SoftwareDistribution");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy", "SELECT * FROM CCM_Scheduler_ScheduledMessage");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Scheduler", "SELECT * FROM CCM_Scheduler_History");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Scanagent", "SELECT * FROM CCM_ScanToolHistory");

                    /*oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\ActualConfig", "SELECT * FROM CCM_SoftwareDistribution");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_SoftwareDistribution");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_SoftwareDistributionClientConfig");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy_EmbeddedObject");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy_Config");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy_Policy");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy_Policy2");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy_Policy3");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy_Rules");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy_AuthorityData");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy_AuthorityData2");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy_Assignment");
                    oWMIProvider.DeleteQueryResults(@"root\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_Policy_Assignment2"); */
                }
                catch
                {
                    throw;
                }
            }

            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
            
            ManagementClass WMIClass = oProv.GetClass("SMS_Client");
            WMIClass.Get();
            ManagementBaseObject inParams = WMIClass.GetMethodParameters("ResetPolicy");
            inParams["uFlags"] = 1;
            oProv.ExecuteMethod("SMS_Client", "ResetPolicy", inParams );

            RequestMachinePolicy();
        }

        /// <summary>
        /// Repair the SMS Agent
        /// </summary>
        /// <remarks>
        /// The repair will be triggered by the SMS Agent. 
        /// This is only possible if the SMS Agent Service is running. 
        /// Use the "ReinstallMSI_Name" function to repair the Client.msi even if the SMS Agent is running or not.
        /// </remarks>
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
        ///                oClient.RepairClient();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.RepairClient()
        /// </code>
        /// </example>
        public void RepairClient()
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
            oProv.ExecuteMethod("SMS_Client", "RepairClient");
        }

        /// <summary>
        /// remove the SMS Agent
        /// </summary>
        /// <returns>MSI Exit Code</returns>
        /// <remarks>It's the same like: UninstallMSI_Name("SMS Advanced Client")</remarks>
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
        ///                oClient.UninstallClient();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.UninstallClient()
        /// </code>
        /// </example>
        public UInt32 UninstallClient()
        {
            string msiID = this.ProductCode;
            if (!string.IsNullOrEmpty(msiID))
            {
                return oMSI.UninstallMSI_ID(msiID);
            }
            else
            {
                return 99;
            }
        }

        /// <summary>
        /// Stop the SMS Agent Service (CCMExec)
        /// </summary>
        /// <remarks>Send a stop command and wait until the Service is stopped</remarks>
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
        ///                oClient.StopAgent();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.StopAgent()
        /// </code>
        /// </example>
        public void StopSMSAgent()
        {
            oWMIService.StopService("ccmexec");
        }

        /// <summary>
        /// Start the SMS Agent Service (CCMExec)
        /// </summary>
        /// <returns>Start Code</returns>
        /// <remarks>Send a start command and wait until the Service is started</remarks>
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
        ///                oClient.StartAgent();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.StartAgent()
        /// </code>
        /// </example>
        public int StartSMSAgent()
        {
            return oWMIService.StartService("ccmexec");
        }

        /// <summary>
        /// Restart the SMS Agent Service
        /// </summary>
        /// <remarks>Stop and Start CCMExec and all dependent services</remarks>
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
        ///                oClient.RestartSMSAgent();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.RestartSMSAgent()
        /// </code>
        /// </example>
        public void RestartSMSAgent()
        {
            oWMIService.StopService("ccmexec");
            System.Threading.Thread.Sleep(1000);
            oWMIService.StartService("ccmexec");
        }

        /// <summary>
        /// Delete all Folders in the SMS Cache
        /// </summary>
        /// <remarks>Caution: The SMS Agent Service will be restarted</remarks>
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
        ///                oClient.CacheDelete();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.CacheDelete()
        /// </code>
        /// </example>
        public void CacheDelete()
        {
            //Delete CacheInfo-Items in WMI
            ManagementObjectCollection MOC;
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";

            if (this.SMSVersion.StartsWith("4"))
            {
                MOC = oProv.ExecuteQuery("SELECT * FROM CacheInfoEx");
            }
            else
            {
                MOC = oProv.ExecuteQuery("SELECT * FROM CacheInfo");
            }
            foreach (ManagementObject MO in MOC)
            {
                MO.Delete();
            }

            //Delete Files and Folders
            WMIFileIO FileIO = new WMIFileIO(oWMIProvider);
            ArrayList SubFolders = FileIO.SubFolders(CachePath);
            foreach (string path in SubFolders)
            {
                FileIO.DeleteFolder(path);
            }

            MOC.Dispose();
            RestartSMSAgent();
        }

        /// <summary>
        /// Cleanup all cached Packages where a newer Version exists
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
        ///                oClient.CacheCleanupOldPackages();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.CacheCleanupOldPackages()
        /// </code>
        /// </example>
        public void CacheCleanupOldPackages()
        {
            try
            {
                string sCachePath = CachePath;
                ArrayList CacheFolder = oWMIFileIO.SubFolders(sCachePath);
                CacheFolder.Sort();
                for (int i = 0; i < CacheFolder.Count - 1; i++)
                {
                    string sFolder1 = CacheFolder[i].ToString();
                    string sPkgFolder1 = sFolder1.Replace(sCachePath + @"\", "");
                    string sFolder2 = CacheFolder[i + 1].ToString();
                    string sPkgFolder2 = sFolder2.Replace(sCachePath + @"\", "");
                    string[] aPkg1 = sPkgFolder1.Split('.');
                    string[] aPkg2 = sPkgFolder2.Split('.');

                    //check if the folders are package folders
                    if ((aPkg1.Length == 3) & (aPkg2.Length == 3))
                    {
                        //check if the PkgID and UserID match
                        if ((aPkg1[0] == aPkg2[0]) & (aPkg1[2] == aPkg2[2]))
                        {
                            //compare the PackageVersions and delete the older PackageFolder
                            if (int.Parse(aPkg1[1]) < int.Parse(aPkg2[1]))
                            {
                                oWMIFileIO.DeleteFolder(CacheFolder[i].ToString());
                                try
                                {
                                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                                    oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";
                                    //oProv.DeleteQueryResults("SELECT * FROM CacheInfo where Location='" + CacheFolder[i].ToString().Replace(@"\", @"\\") + "'");
                                    if (this.SMSVersion.StartsWith("4"))
                                    {
                                        oProv.DeleteQueryResults("SELECT * FROM CacheInfoEx where Location='" + CacheFolder[i].ToString().Replace(@"\", @"\\") + "'");
                                    }
                                    else
                                    {
                                        oProv.DeleteQueryResults("SELECT * FROM CacheInfo where Location='" + CacheFolder[i].ToString().Replace(@"\", @"\\") + "'");
                                    }
                                }
                                catch { }
                            }
                            else
                            {
                                try
                                {
                                    oWMIFileIO.DeleteFolder(CacheFolder[i + 1].ToString());
                                }
                                catch { }
                                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                                oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";
                                //oProv.DeleteQueryResults("SELECT * FROM CacheInfo where Location='" + CacheFolder[i + 1].ToString().Replace(@"\", @"\\") + "'");
                                if (this.SMSVersion.StartsWith("4"))
                                {
                                    oProv.DeleteQueryResults("SELECT * FROM CacheInfoEx where Location='" + CacheFolder[i + 1].ToString().Replace(@"\", @"\\") + "'");
                                }
                                else
                                {
                                    oProv.DeleteQueryResults("SELECT * FROM CacheInfo where Location='" + CacheFolder[i + 1].ToString().Replace(@"\", @"\\") + "'");
                                }
                            }
                        }
                    }
                    else
                    {
                        //no sms pkg folder
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Cleanup all Cache Folders where no entry is in the CacheInfo class (orphaned Folders)
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
        ///                oClient.CacheCleanupOrphanedPackages();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.CacheCleanupOrphanedPackages()
        /// </code>
        /// </example>
        public void CacheCleanupOrphanedPackages()
        {
            string sCachePath = CachePath;
            ArrayList CacheFolder = oWMIFileIO.SubFolders(sCachePath);
            foreach (string sFolder in CacheFolder)
            {
                try
                {
                    ManagementObjectCollection MOC;
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";

                    if (this.SMSVersion.StartsWith("4"))
                    {
                        MOC = oProv.ExecuteQuery("SELECT * FROM CacheInfoEx where Location='" + sFolder.Replace(@"\", @"\\") + "'");
                    }
                    else
                    {
                        MOC = oProv.ExecuteQuery("SELECT * FROM CacheInfo where Location='" + sFolder.Replace(@"\", @"\\") + "'");
                    }
                    //Check if Package is listed in the CacheInfo Class
                    if (MOC.Count == 0)
                    {
                        //delete the orphaned Folder
                        try
                        {
                            oWMIFileIO.DeleteFolder(sFolder);
                        }
                        catch
                        { }

                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Cleanup all CacheInfo Objects where no Package-Folder exists
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
        ///                oClient.CacheCleanupOrphanedCacheInfo();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.CacheCleanupOrphanedCacheInfo()
        /// </code>
        /// </example>
        public void CacheCleanupOrphanedCacheInfo()
        {
            string sCachePath = CachePath;
            ArrayList CacheFolder = oWMIFileIO.SubFolders(sCachePath);

            ManagementObjectCollection MOC;
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";
            if (this.SMSVersion.StartsWith("4"))
            {
                MOC = oProv.ExecuteQuery("SELECT * FROM CacheInfoEx");
            }
            else
            {
                MOC = oProv.ExecuteQuery("SELECT * FROM CacheInfo");
            }
            foreach (ManagementObject MO in MOC)
            {
                try
                {
                    if (!CacheFolder.Contains(MO.GetPropertyValue("Location").ToString().ToLower()))
                    {
                        MO.Delete();
                    }
                }
                catch
                { }
            }
        }

        /// <summary>
        /// Cleanup all Software Updates from Cache
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
        ///                oClient.CacheCleanupPatches();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.CacheCleanupPatches()
        /// </code>
        /// </example>
        public void CacheCleanupPatches()
        {
            try
            {
                string sCachePath = CachePath;
                ArrayList CacheFolder = oWMIFileIO.SubFolders(sCachePath);
                CacheFolder.Sort();
                for (int i = 0; i < CacheFolder.Count - 1; i++)
                {
                    string sFolder1 = CacheFolder[i].ToString();
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sFolder1);

                    string sPkgFolder1 = di.Name.ToString();

                    string[] aPkg1 = sPkgFolder1.Split('.');

                    if (aPkg1[0].Length > 8)
                    {
                        oWMIFileIO.DeleteFolder(CacheFolder[i].ToString());

                        try
                        {
                            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                            oProv.mScope.Path.NamespacePath = @"root\ccm\SoftMgmtAgent";
                            if (this.SMSVersion.StartsWith("4"))
                            {
                                oProv.DeleteQueryResults("SELECT * FROM CacheInfoEx where Location='" + CacheFolder[i].ToString().Replace(@"\", @"\\") + "'");
                            }
                            else
                            {
                                oProv.DeleteQueryResults("SELECT * FROM CacheInfo where Location='" + CacheFolder[i].ToString().Replace(@"\", @"\\") + "'");
                            }
                        }
                        catch(Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Cleanup orphaned and old cached packages
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
        ///                oClient.CacheCleanupALL();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.CacheCleanupALL()
        /// </code>
        /// </example>
        public void CacheCleanupALL()
        {
            CacheCleanupOrphanedPackages();
            CacheCleanupOldPackages();
            CacheCleanupOrphanedCacheInfo();
            CacheCleanupPatches();
        }

        /// <summary>
        /// ResetGlobalLoggingConfiguration to the default values
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
        ///                oClient.ResetGlobalLoggingConfiguration();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ResetGlobalLoggingConfiguration()
        /// </code>
        /// </example>
        public void ResetGlobalLoggingConfiguration()
        {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "ResetGlobalLoggingConfiguration");
        }

        /// <summary>
        /// Connect the IPC$ Share of the remote Host
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
        ///                oClient.ConnectIPC();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.ConnectIPC()
        /// </code>
        /// </example>
        public void ConnectIPC()
        {
            CCMSetup CCM = new CCMSetup(this);
            CCM.ConnectIPC();
        }

        /// <summary>
        /// Delete the TrustedRootKey and the MP Certificate
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
        ///                oClient.DeleteTrustedRootKey();
        ///             }
        ///       }
        /// }
        /// </code>
        /// </example>
        public void DeleteTrustedRootKey()
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\CCM\LocationServices";
            try
            {
                ManagementObject MO = oProv.GetObject("TrustedRootKey=@");
                MO.Delete();
            }
            catch
            { }
        }

        /// <summary>
        /// Delete SMS x509 Certificates
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
        ///                oClient.DeleteSMSCertificates();
        ///             }
        ///       }
        /// }
        /// </code>
        /// </example>
        public void DeleteSMSCertificates()
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            try
            {
                WMIRegistry oReg = new WMIRegistry(oProv);
                foreach (string sCert in oReg.RegKeys(2147483650, @"SOFTWARE\Microsoft\SystemCertificates\SMS\Certificates"))
                {
                    try
                    {
                        oReg.DeleteKey(2147483650, @"SOFTWARE\Microsoft\SystemCertificates\SMS\Certificates\" + sCert);
                    }
                    catch { }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Reset paused SoftwareDistribution Flag
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
        ///                oClient.ResetPausedSWDist();
        ///             }
        ///       }
        /// }
        /// </code>
        /// </example>
        public void ResetPausedSWDist()
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());

            try
            {
                
                //Detect if remote System is x86 or x64
                if (oWMIProvider.isX86)
                {
                    WMIRegistry oReg = new WMIRegistry(oProv);

                    oReg.SetDWord(2147483650, @"SOFTWARE\Microsoft\SMS\Mobile Client\Software Distribution\State", "Paused", "0");
                    oReg.SetDWord(2147483650, @"SOFTWARE\Microsoft\SMS\Mobile Client\Software Distribution\State", "PausedCookie", "0");
                }
                else
                {
                    WMIRegistry oReg = new WMIRegistry(oProv);

                    oReg.SetDWord(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\SMS\Mobile Client\Software Distribution\State", "Paused", "0");
                    oReg.SetDWord(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\SMS\Mobile Client\Software Distribution\State", "PausedCookie", "0");
                }
            }
            catch
            { }
        }

        #endregion

        /// <summary>
        /// WMI Connection Settings (WMIProvider Class)
        /// </summary>
        public WMIProvider Connection { get { return oWMIProvider; } }

    }
}
