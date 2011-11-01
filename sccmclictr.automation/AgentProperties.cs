﻿//SCCM Client Center Automation Library (SCCMCliCtr.automation)
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

using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using System.ComponentModel;

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
        private TraceSwitch debugLevel = new TraceSwitch("DebugLevel", "DebugLevel from ConfigFile", "Verbose");

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
        internal UInt32 cClientType;
        internal string cADSiteName;
        internal string cCommunicationMode;
        internal string cCertKeyType;

        internal Boolean cMultiUsersLoggedOn;
        internal string cBrandingTitle;
        internal Boolean cRebootPending;
        internal Boolean cIsHardRebootPending;
        internal Boolean cInGracePeriod;
        internal DateTime cRebootDeadline;

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
        /// $a=[wmiclass]"root\ccm\clientsdk:CCM_SoftwareCatalogUtilities";$a.GetClientVersion().ClientVersion
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
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.ClientVersion, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cClientVersion = obj.BaseObject.ToString();
                            }
                            catch
                            {
                                cClientVersion = "";
                            }
                        }
                        else
                        {
                            cClientVersion = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(cClientVersion))
                    {
                        updateTimeStamp("cClientVersion");
                    }
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ClientVersion:" + cClientVersion);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ClientVersion from cache:" + cClientVersion);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.ClientVersion);

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
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.AllowLocalAdminOverride, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cAllowLocalAdminOverride = Boolean.Parse(obj.BaseObject.ToString());
                                updateTimeStamp("cAllowLocalAdminOverride");
                            }
                            catch
                            {
                            }
                        }
                    }

                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get AllowLocalAdminOverride:" + cAllowLocalAdminOverride);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get AllowLocalAdminOverride from cache:" + cAllowLocalAdminOverride);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.AllowLocalAdminOverride);

                //Return result
                return cAllowLocalAdminOverride;

            }
            set
            {
                string sCode = string.Format(Properties.Resources.AllowLocalAdminOverride_Set, value.ToString());
                Trace.WriteLineIf(debugLevel.TraceVerbose, "Set AllowLocalAdminOverride:" + WSMan.RunPSScriptAsString(sCode, remoteRunspace));
                tsPSCode.TraceInformation(sCode);

                cAllowLocalAdminOverride = value;

                //Clear TimeStamp in cache to enforce reload
                clearTimeStamp("cAllowLocalAdminOverride");
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
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.EnableAutoAssignment, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cEnableAutoAssignment = Boolean.Parse(obj.BaseObject.ToString());
                                updateTimeStamp("cEnableAutoAssignment");
                            }
                            catch(Exception ex)
                            {
                                System.Diagnostics.Trace.TraceError(ex.Message);
                            }
                        }
                    }

                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get EnableAutoAssignment:" + cEnableAutoAssignment);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get EnableAutoAssignment from cache:" + cEnableAutoAssignment);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.EnableAutoAssignment);

                //Return result
                return cEnableAutoAssignment;
            }
            set
            {
                string sCode = string.Format(Properties.Resources.EnableAutoAssignment_Set, value.ToString());

                Trace.WriteLineIf(debugLevel.TraceVerbose, "Set EnableAutoAssignmente:" + WSMan.RunPSScript(sCode, remoteRunspace));
                tsPSCode.TraceInformation(sCode);

                cEnableAutoAssignment = value;

                //Clear TimeStamp in cache to enforce reload
                clearTimeStamp("cEnableAutoAssignment");
            }
        }

        /// <summary>
        /// Return the SCCM Agent GUID
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// (Get-Wmiobject -class CCM_Client -namespace 'ROOT\CCM').ClientId
        /// </code>
        /// </example>
        public string ClientId
        {
            get
            {
                if (!isCached("cClientId", cacheAge))
                {
                    cClientId = "";
                }

                if (string.IsNullOrEmpty(cClientId))
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.ClientId, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cClientId = obj.BaseObject.ToString();
                            }
                            catch
                            {
                                cClientId = "";
                            }
                        }
                        else
                        {
                            cClientId = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(cClientId))
                    {
                        updateTimeStamp("cClientId");
                    }
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ClientId:" + cClientId);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ClientId from cache:" + cClientId);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.ClientId);

                //Return result
                return cClientId;
            }
        }

        /// <summary>
        /// Return the previous SCCM Agent GUID
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// (Get-Wmiobject -class CCM_Client -namespace 'ROOT\CCM').PreviousClientId
        /// </code>
        /// </example>
        public string PreviousClientId
        {
            get
            {
                if (!isCached("cPreviousClientId", cacheAge))
                {
                    cPreviousClientId = "";
                }

                if (string.IsNullOrEmpty(cPreviousClientId))
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.PreviousClientId, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cPreviousClientId = obj.BaseObject.ToString();
                            }
                            catch
                            {
                                cPreviousClientId = "";
                            }
                        }
                        else
                        {
                            cPreviousClientId = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(cPreviousClientId))
                    {
                        updateTimeStamp("cPreviousClientId");
                    }
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get PreviousClientId:" + cPreviousClientId);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get PreviousClientId from cache:" + cPreviousClientId);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.PreviousClientId);

                //Return result
                return cPreviousClientId;
            }
        }

        /// <summary>
        /// Return the SCCM Agent GUID creation/change date as string
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// (Get-Wmiobject -class CCM_Client -namespace 'ROOT\CCM').ClientIdChangeDate
        /// </code>
        /// </example>
        public string ClientIdChangeDate
        {
            get
            {
                if (!isCached("cClientIdChangeDate", cacheAge))
                {
                    cClientIdChangeDate = "";
                }

                if (string.IsNullOrEmpty(cClientIdChangeDate))
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.ClientIdChangeDate, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cClientIdChangeDate = obj.BaseObject.ToString();
                            }
                            catch
                            {
                                cClientIdChangeDate = "";
                            }
                        }
                        else
                        {
                            cClientIdChangeDate = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(cPreviousClientId))
                    {
                        updateTimeStamp("cClientIdChangeDate");
                    }
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ClientIdChangeDate:" + cClientIdChangeDate);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ClientIdChangeDate from cache:" + cClientIdChangeDate);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.ClientIdChangeDate);

                //Return result
                return cClientIdChangeDate;

            }
        }

        /// <summary>
        /// Return the SCCM Client Type
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// (Get-Wmiobject -class SMS_Client -namespace 'ROOT\CCM').ClientType
        /// </code>
        /// </example>
        public UInt32 ClientType
        {
            get
            {
                Boolean reload = false;
                if (!isCached("cClientType", cacheAge))
                {
                    reload = true; ;
                }

                if (reload)
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.ClientType, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cClientType =  UInt32.Parse(obj.BaseObject.ToString());
                                updateTimeStamp("cClientType");
                            }
                            catch(Exception ex)
                            {
                                System.Diagnostics.Trace.TraceError(ex.Message);
                            }
                        }
                    }

                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ClientType:" + cClientType);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ClientType from cache:" + cClientType);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.ClientType);

                //Return result
                return cClientType;

            }
        }

        /// <summary>
        /// Return the ActiveDirectory Site-Name (if exist).
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// $a=New-Object -comObject 'CPAPPLET.CPAppletMgr';($a.GetClientProperties() | Where-Object { $_.Name -eq 'ADSiteName' }).Value
        /// </code>
        /// </example>
        public string ADSiteName
        {
            get
            {
                if (!isCached("cADSiteName", cacheAge))
                {
                    cADSiteName = "";
                }

                if (string.IsNullOrEmpty(cADSiteName))
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.ADSiteName, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cADSiteName = obj.BaseObject.ToString();
                            }
                            catch
                            {
                                cADSiteName = "";
                            }
                        }
                        else
                        {
                            cADSiteName = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(cADSiteName))
                    {
                        updateTimeStamp("cADSiteName");
                    }
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ADSiteName:" + cADSiteName);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get ADSiteName from cache:" + cADSiteName);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.ADSiteName);

                //Return result
                return cADSiteName;
            }
        }

        /// <summary>
        /// Return the Agent CommunicationMode (if exist).
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// $a=New-Object -comObject 'CPAPPLET.CPAppletMgr';($a.GetClientProperties() | Where-Object { $_.Name -eq 'CommunicationMode' }).Value
        /// </code>
        /// </example>
        public string CommunicationMode
        {
            get
            {
                if (!isCached("cCommunicationMode", cacheAge))
                {
                    cCommunicationMode = "";
                }

                if (string.IsNullOrEmpty(cCommunicationMode))
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.CommunicationMode, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cCommunicationMode = obj.BaseObject.ToString();
                            }
                            catch
                            {
                                cCommunicationMode = "";
                            }
                        }
                        else
                        {
                            cCommunicationMode = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(cCommunicationMode))
                    {
                        updateTimeStamp("cCommunicationMode");
                    }
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get CommunicationMode:" + cCommunicationMode);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get CommunicationMode from cache:" + cCommunicationMode);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.CommunicationMode);

                //Return result
                return cCommunicationMode;
            }
        }

        /// <summary>
        /// Return the Agent CertKeyType (if exist).
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// $a=New-Object -comObject 'CPAPPLET.CPAppletMgr';($a.GetClientProperties() | Where-Object { $_.Name -eq 'CertKeyType' }).Value
        /// </code>
        /// </example>
        public string CertKeyType
        {
            get
            {
                if (!isCached("cCertKeyType", cacheAge))
                {
                    cCertKeyType = "";
                }

                if (string.IsNullOrEmpty(cCertKeyType))
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.CertKeyType, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cCertKeyType = obj.BaseObject.ToString();
                            }
                            catch
                            {
                                cCertKeyType = "";
                            }
                        }
                        else
                        {
                            cCertKeyType = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(cCertKeyType))
                    {
                        updateTimeStamp("cCertKeyType");
                    }
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get CertKeyType:" + cCertKeyType);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get CertKeyType from cache:" + cCertKeyType);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.CertKeyType);

                //Return result
                return cCertKeyType;
            }
        }

        /// <summary>
        /// Detect if multiple Users are Logged On.
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// $a=[wmiclass]"root\ccm\clientsdk:CCM_ClientInternalUtilities";$a.AreMultiUsersLoggedOn().MultiUsersLoggedOn
        /// </code>
        /// </example>
        public Boolean MultiUsersLoggedOn
        {
            get
            {
                Boolean reload = false;
                if (!isCached("cMultiUsersLoggedOn", cacheAge))
                {
                    reload = true; ;
                }


                if (reload)
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.MultiUsersLoggedOn, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cMultiUsersLoggedOn = Boolean.Parse(obj.BaseObject.ToString());
                                updateTimeStamp("cMultiUsersLoggedOn");
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get MultiUsersLoggedOn:" + cMultiUsersLoggedOn);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get MultiUsersLoggedOn from cache:" + cMultiUsersLoggedOn);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.MultiUsersLoggedOn);

                //Return result
                return cMultiUsersLoggedOn;

            }
        }

        /// <summary>
        /// Return Branding Title (Company Name).
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// (Get-Wmiobject -class CCM_ClientAgentSettings -namespace 'ROOT\CCM\ClientSDK').BrandingTitle
        /// </code>
        /// </example>
        public string BrandingTitle
        {
            get
            {
                if (!isCached("cBrandingTitle", cacheAge))
                {
                    cBrandingTitle = "";
                }

                if (string.IsNullOrEmpty(cBrandingTitle))
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.BrandingTitle, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cBrandingTitle = obj.BaseObject.ToString();
                            }
                            catch
                            {
                                cBrandingTitle = "";
                            }
                        }
                        else
                        {
                            cBrandingTitle = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(cBrandingTitle))
                    {
                        updateTimeStamp("cBrandingTitle");
                    }
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get BrandingTitle:" + cBrandingTitle);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get BrandingTitle from cache:" + cBrandingTitle);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.BrandingTitle);

                //Return result
                return cBrandingTitle;
            }
        }

        /// <summary>
        /// Detect if a reboot is pending.
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// $a=[wmiclass]"root\ccm\clientsdk:CCM_ClientUtilities";$a.DetermineIfRebootPending().RebootPending
        /// </code>
        /// </example>
        public Boolean RebootPending
        {
            get
            {
                Boolean reload = false;
                if (!isCached("cRebootPending", cacheAge))
                {
                    reload = true;
                }

                if (reload)
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.RebootPending, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cRebootPending = Boolean.Parse(obj.Properties["RebootPending"].Value.ToString());
                                updateTimeStamp("cRebootPending");

                                cIsHardRebootPending = Boolean.Parse(obj.Properties["IsHardRebootPending"].Value.ToString());
                                updateTimeStamp("cIsHardRebootPending");
                                cInGracePeriod = Boolean.Parse(obj.Properties["InGracePeriod"].Value.ToString());
                                updateTimeStamp("cInGracePeriod");

                                cRebootDeadline = ManagementDateTimeConverter.ToDateTime(obj.Properties["RebootDeadline"].Value.ToString());
                                updateTimeStamp("cRebootDeadline");
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get RebootPending:" + cRebootPending);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get RebootPending from cache:" + cRebootPending);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.RebootPending + ".RebootPending");

                //Return result
                return cRebootPending;
            }
        }

        /// <summary>
        /// Detect if a hard reboot is pending.
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// $a=[wmiclass]"root\ccm\clientsdk:CCM_ClientUtilities";$a.DetermineIfRebootPending().IsHardRebootPending
        /// </code>
        /// </example>
        public Boolean IsHardRebootPending
        {
            get
            {
                Boolean reload = false;
                if (!isCached("cIsHardRebootPending", cacheAge))
                {
                    reload = true;
                }

                if (reload)
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.RebootPending, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cRebootPending = Boolean.Parse(obj.Properties["RebootPending"].Value.ToString());
                                updateTimeStamp("cRebootPending");

                                cIsHardRebootPending = Boolean.Parse(obj.Properties["IsHardRebootPending"].Value.ToString());
                                updateTimeStamp("cIsHardRebootPending");

                                cInGracePeriod = Boolean.Parse(obj.Properties["InGracePeriod"].Value.ToString());
                                updateTimeStamp("cInGracePeriod");

                                cRebootDeadline = ManagementDateTimeConverter.ToDateTime(obj.Properties["RebootDeadline"].Value.ToString());
                                updateTimeStamp("cRebootDeadline");
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get IsHardRebootPending:" + cIsHardRebootPending);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get IsHardRebootPending from cache:" + cIsHardRebootPending);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.RebootPending + ".IsHardRebootPending");

                //Return result
                return cIsHardRebootPending;
            }
        }

        /// <summary>
        /// Get deadline of pending reboot
        /// Note: This Value will be cached. To get the uncached Version call the 'Initialize' Method from the SCCMAgent Class
        /// </summary>
        /// <example>
        /// PowerShell Code:
        /// <code>
        /// $a=[wmiclass]"root\ccm\clientsdk:CCM_ClientUtilities";$a.DetermineIfRebootPending().RebootDeadline
        /// </code>
        /// </example>
        public DateTime RebootDeadline
        {
            get
            {
                Boolean reload = false;
                if (!isCached("cRebootDeadline", cacheAge))
                {
                    reload = true;
                }

                if (reload)
                {
                    foreach (PSObject obj in WSMan.RunPSScript(Properties.Resources.RebootPending, remoteRunspace))
                    {
                        //Store the Object in cache
                        if (obj != null)
                        {
                            try
                            {
                                cRebootPending = Boolean.Parse(obj.Properties["RebootPending"].Value.ToString());
                                updateTimeStamp("cRebootPending");

                                cIsHardRebootPending = Boolean.Parse(obj.Properties["IsHardRebootPending"].Value.ToString());
                                updateTimeStamp("cIsHardRebootPending");

                                cInGracePeriod = Boolean.Parse(obj.Properties["InGracePeriod"].Value.ToString());
                                updateTimeStamp("cInGracePeriod");

                                cRebootDeadline = ManagementDateTimeConverter.ToDateTime(obj.Properties["RebootDeadline"].Value.ToString());
                                updateTimeStamp("cRebootDeadline");
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get RebootDeadline:" + cRebootDeadline);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceInfo, DateTime.Now.ToString() + " - Get RebootDeadline from cache:" + cRebootDeadline);
                }

                //Trace the PowerShell Command
                tsPSCode.TraceInformation(Properties.Resources.RebootPending + ".RebootDeadline");

                //Return result
                return cRebootDeadline;
            }
        }

        //****** Functions below are not up to date ! *************
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

        public void test()
        {
            using (PowerShell powershell = PowerShell.Create())
            {

                powershell.Runspace = remoteRunspace;
                string sQuery = "$query = 'SELECT * FROM __InstanceModificationEvent WITHIN 10 WHERE TargetInstance ISA \"Win32_Service\"';" +
                    "$a = Register-WmiEvent -Query $query \"WMI.Service.Stopped\" -Forward ;$a";

                /* string sQuery = "$query = 'SELECT * FROM __InstanceModificationEvent WITHIN 10 WHERE TargetInstance ISA \"Win32_Service\"';" +
                     "$a = Register-WmiEvent -Query $query LocalCatchEvent -action {new-event CatchEvent -messageData $event.SourceEventArgs.NewEvent.TargetInstance};"+
                     "Register-EngineEvent CatchEvent -Forward"; */

                //powershell.AddScript(sQuery);

                Pipeline pipe = powershell.Runspace.CreatePipeline("Get-ChildItem c: -Name");
                //pipe.Commands.Add("Out-String -stream");
                //pipe.Commands.AddScript(sQuery);
                //pipe.Input.Write()
                //pipe.Input.Close();
                pipe.InvokeAsync();
                pipe.Output.DataReady += new EventHandler(Output_DataReady);

                /*pipe.InvokeAsync();
                pipe.Output.ToString();

                IAsyncResult async = powershell.BeginInvoke();

                foreach (PSObject result in powershell.EndInvoke(async))
                {

                    powershell.Runspace.Events.ReceivedEvents.ToString();

                    Console.WriteLine(result.ToString());

                } */

                //Collection<PSObject> results = powershell.Invoke(); 

            }

        }

        void Output_DataReady(object sender, EventArgs e)
        {
            PipelineReader<PSObject> output = sender as PipelineReader<PSObject>;
            while(!output.EndOfPipeline)
            {
                foreach (PSObject obj in output.NonBlockingRead())
                {
                    foreach (PSPropertyInfo pi in obj.Properties)
                    {
                        Console.WriteLine(pi.Value.ToString());
                    }
                }
            }
        }

        //**********************************************************
        #endregion
    }

}