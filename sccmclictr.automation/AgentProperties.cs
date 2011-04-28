//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;


namespace sccmclictr.automation
{
    /// <summary>
    /// SCCM Agent Properties
    /// </summary>
    public class agentProperties
    {
        #region Initializing

        /// <summary>
        /// Define default Cache Age = 2min
        /// </summary>
        internal static TimeSpan cacheAge = new TimeSpan(0, 2, 0);

        /// <summary>
        /// Define the DebugLevel
        /// </summary>
        private TraceSwitch debugLevel = new TraceSwitch("DebugLevel", "DebugLevel from ConfigFile", "Verbose" );
        
        /// <summary>
        /// Trace Source for PowerShell Commands
        /// </summary>
        private TraceSource tsPSCode { get; set; }

        /// <summary>
        /// Dictionary to store Cache TimeStamps
        /// </summary>
        internal Dictionary<string, DateTime> cacheTimestamp;

        #endregion

        #region Cached Items

        internal string cClientVersion;
        internal Boolean cAllowLocalAdminOverride;
        internal Boolean cEnableAutoAssignment;
        internal string cClientId;
        internal string cPreviousClientId;
        internal string cClientIdChangeDate;
        internal string cClientVersionEx;
        internal UInt32 cClientType;
        internal string cLastRebootDays;

        /// <summary>
        /// Cached Copy of the SMS_Client ManagementObject
        /// </summary>
        internal ManagementObject cSMS_ClientCached;

        /// <summary>
        /// Get the SMS_Client ManagementObject
        /// </summary>
        private ManagementObject cSMS_Client
        {
            get 
            {
                try
                {
                    //Check if SMS_Client is cached?
                    if (!isCached("cSMS_Client", cacheAge))
                    {
                        //Clear the cached Object
                        cSMS_ClientCached = null;
                    }

                    if (cSMS_ClientCached == null)
                    {
                        //Call the PSScript to get the SMS_Client Object
#if DEBUG
                        tsPSCode.TraceInformation(Properties.Resources.SMS_Client);
#endif
                        
                        foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.SMS_Client, remoteRunspace))
                        {
                            //Store the Object in cache
                            cSMS_Client = obj.BaseObject as ManagementObject;
                        }

                        Trace.WriteLineIf(debugLevel.TraceVerbose, @"Cache WMI Object 'SMS_Client' from root\ccm.");
                    }
                    else
                    {
                        Trace.WriteLineIf(debugLevel.TraceVerbose, @"return WMI Object 'SMS_Client' from cache.");
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Trace.TraceError("get cSMS_Client: " + ex.Message);
#endif
                }
                //return the cached Object
                return cSMS_ClientCached;
            }
            set
            {
                cSMS_ClientCached = value;
                
                //update TimeStamp in cache
                updateTimeStamp("cSMS_Client");
            }
        }

        #endregion

        #region Internal functions

        /// <summary>
        /// Detect if Item is cached
        /// </summary>
        /// <param name="ItemName"></param>
        /// <param name="ItemAge"></param>
        /// <returns></returns>
        internal Boolean isCached(string ItemName, TimeSpan ItemAge)
        {
            try
            {
                if (cacheTimestamp.ContainsKey(ItemName))
                {
                    if (DateTime.Now.Subtract(cacheTimestamp[ItemName]) < ItemAge)
                    {
                        System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceVerbose, "Object: " + ItemName + " is cached.");
                        return true;
                    }
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceVerbose, "Cached Object '" + ItemName + "' is expired.");
                }
            }
            catch { }

            System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceVerbose, "Object: " + ItemName + " is NOT cached.");
            return false;
        }

        internal void updateTimeStamp(string ItemName)
        {
            if (cacheTimestamp.Keys.Contains(ItemName))
                cacheTimestamp[ItemName] = DateTime.Now;
            else
                cacheTimestamp.Add(ItemName, DateTime.Now);
        }

        internal void clearTimeStamp(string ItemName)
        {
            if (cacheTimestamp.Keys.Contains(ItemName))
                cacheTimestamp.Remove(ItemName);
        }

        private Runspace remoteRunspace { get; set; }

        #endregion

        #region Agent Properties

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="RemoteRunspace">PowerShell RunSpace</param>
        /// <param name="PSCode">TraceSource for PowerShell Commands</param>
        public agentProperties(Runspace RemoteRunspace, TraceSource PSCode)
        {
            remoteRunspace = RemoteRunspace;
            cacheTimestamp = new Dictionary<string, DateTime>();
            tsPSCode = PSCode;
        }

        /// <summary>
        /// Return the full SCCM Agent ClientVersion
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// (Get-Wmiobject -class SMS_Client -namespace 'ROOT\CCM').ClientVersion
        /// </code>
        /// </example>
        public string ClientVersion
        {
            get
            {
                if (!isCached("cClientVersion", cacheAge))
                {
                    cClientVersion = "";
                }

                
                if (string.IsNullOrEmpty(cClientVersion))
                {
                    cClientVersion = cSMS_Client.Properties["ClientVersion"].Value.ToString();
                    updateTimeStamp("cClientVersion");
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ClientVersion:" + cClientVersion);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ClientVersion from cache:" + cClientVersion);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.SMS_Client+".ClientVersion");

                //Return result
                return cClientVersion;
            }
        }

        /// <summary>
        /// Get/Set the option if an Administrator can Override Agent Settings from the ControlPanel Applet
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// (Get-Wmiobject -class SMS_Client -namespace 'ROOT\CCM').AllowLocalAdminOverride
        /// </code>
        /// </example>
        public Boolean AllowLocalAdminOverride
        {
            get
            {
                Boolean reload = false;
                if (!isCached("cAllowLocalAdminOverriden", cacheAge))
                {
                    reload = true; ;
                }


                if (reload)
                {
                    cAllowLocalAdminOverride = Boolean.Parse(cSMS_Client.Properties["AllowLocalAdminOverride"].Value.ToString());
                    updateTimeStamp("cAllowLocalAdminOverride");
                    Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get AllowLocalAdminOverride:" + cAllowLocalAdminOverride.ToString());
                }
                else
                {
                    Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get AllowLocalAdminOverride from cache:" + cAllowLocalAdminOverride.ToString());
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.SMS_Client + ".AllowLocalAdminOverride");

                //Return result
                return cAllowLocalAdminOverride;

                //return Boolean.Parse(WSMan.RunPSScriptAsString("(Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM').AllowLocalAdminOverride", remoteRunspace));
            }
            set
            {
                string sCode = "$a = (Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM');" +
                "$a.AllowLocalAdminOverride = $" + value.ToString() + ";" +
                "$a.Put()";
                Trace.WriteLineIf(debugLevel.TraceVerbose, "Set AllowLocalAdminOverride:" + WSMan.RunPSScriptAsString(sCode, remoteRunspace));
                tsPSCode.TraceInformation(sCode);

                cAllowLocalAdminOverride = value;

                //Clear TimeStamp in cache to enforce reload
                clearTimeStamp("cAllowLocalAdminOverride");
                clearTimeStamp("cSMS_Client");
            }
        }

        /// <summary>
        /// Enable Site Code Auto Assignment on next Agent Restart
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// (Get-Wmiobject -class SMS_Client -namespace 'ROOT\CCM').EnableAutoAssignment
        /// </code>
        /// </example>
        public Boolean EnableAutoAssignment
        {
            get
            {
                Boolean reload = false;
                if (!isCached("cEnableAutoAssignment", cacheAge))
                {
                    reload = true; ;
                }


                if (reload)
                {
                    cEnableAutoAssignment = Boolean.Parse(cSMS_Client.Properties["EnableAutoAssignment"].Value.ToString());
                    updateTimeStamp("cEnableAutoAssignment");
                    Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get EnableAutoAssignment:" + cEnableAutoAssignment.ToString());
                }
                else
                {
                    Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get EnableAutoAssignment from cache:" + cEnableAutoAssignment.ToString());
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.SMS_Client + ".EnableAutoAssignment");

                //Return result
                return cEnableAutoAssignment;
                
                //return Boolean.Parse(WSMan.RunPSScriptAsString("(Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM').EnableAutoAssignment", remoteRunspace));
            }
            set
            {
                string sCode = "$a = (Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM');" +
                "$a.EnableAutoAssignment = $" + value.ToString() + ";" +
                "$a.Put()";
                Trace.WriteLineIf(debugLevel.TraceVerbose, "Set EnableAutoAssignmente:" + WSMan.RunPSScript(sCode, remoteRunspace));
                tsPSCode.TraceInformation(sCode);

                cEnableAutoAssignment = value;

                //Clear TimeStamp in cache to enforce reload
                clearTimeStamp("cEnableAutoAssignment");
                clearTimeStamp("cSMS_Client");
            }
        }

//****** Functions below are not up to date ! *************

        /// <summary>
        /// Get the Agent GUID
        /// </summary>
        public string ClientId
        {
            get
            {
                return WSMan.RunPSScriptAsString("(Get-Wmiobject -class CCM_Client -namespace 'ROOT\\CCM').ClientId", remoteRunspace).Trim();
            }
        }

        /// <summary>
        /// Get the previous Agent GUID
        /// </summary>
        public string PreviousClientId
        {
            get
            {
                return WSMan.RunPSScriptAsString("(Get-Wmiobject -class CCM_Client -namespace 'ROOT\\CCM').PreviousClientId", remoteRunspace).Trim();
            }
        }

        /// <summary>
        /// Get last Agent GUID change date as string
        /// </summary>
        public string ClientIdChangeDate
        {
            get
            {
                return WSMan.RunPSScriptAsString("(Get-Wmiobject -class CCM_Client -namespace 'ROOT\\CCM').ClientIdChangeDate", remoteRunspace).Trim();
            }
        }

        /// <summary>
        /// Get the Client Version (SCCM2007 = 2.50); This function seems to be obsolete!!!
        /// </summary>
        public string ClientVersionEx
        {
            get
            {
                return WSMan.RunPSScriptAsString("(Get-Wmiobject -class CCM_Client -namespace 'ROOT\\CCM').ClientVersion", remoteRunspace).Trim();
            }
        }

        /// <summary>
        /// Get the Client Type
        /// </summary>
        public UInt32 ClientType
        {
            get
            {
                return UInt32.Parse(WSMan.RunPSScriptAsString("(Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM').ClientType", remoteRunspace));
            }
        }

        /// <summary>
        /// Return the number of days where the system is up and running
        /// </summary>
        public string LastRebootDays
        {
            get
            {
                return WSMan.RunPSScriptAsString("$wmi = Get-WmiObject -Class Win32_OperatingSystem \n$a = New-TimeSpan $wmi.ConvertToDateTime($wmi.LastBootUpTime) $(Get-Date) \n$a.Days", remoteRunspace).Trim();

            }
        }

//**********************************************************
        #endregion
    }
}
