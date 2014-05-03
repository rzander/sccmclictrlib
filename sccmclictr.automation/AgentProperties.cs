//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

#define CM2012
#define CM2007

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sccmclictr.automation;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Web;


namespace sccmclictr.automation.functions
{

    /// <summary>
    /// Class agentproperties.
    /// </summary>
    public class agentproperties : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;


        /// <summary>
        /// Initializes a new instance of the <see cref="agentproperties"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The ps code.</param>
        /// <param name="oClient">The CCM client object.</param>
        public agentproperties(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        

#if CM2012

        /// <summary>
        /// Return the ActiveDirectory Site-Name (if exist).
        /// </summary>
        public string ADSiteName
        {
            get
            {
                return base.GetStringFromPS("$a=New-Object -comObject 'CPAPPLET.CPAppletMgr';($a.GetClientProperties() | Where-Object { $_.Name -eq 'ADSiteName' }).Value");
            }
        }

        /// <summary>
        /// Return the Agent CommunicationMode (if exist).
        /// </summary>
        public string CommunicationMode
        {
            get
            {
                return base.GetStringFromPS("$a=New-Object -comObject 'CPAPPLET.CPAppletMgr';($a.GetClientProperties() | Where-Object { $_.Name -eq 'CommunicationMode' }).Value");
            }
        }

        /// <summary>
        /// Return the Agent CertKeyType (if exist).
        /// </summary>
        public string CertKeyType
        {
            get
            {
                return base.GetStringFromPS("$a=New-Object -comObject 'CPAPPLET.CPAppletMgr';($a.GetClientProperties() | Where-Object { $_.Name -eq 'CertKeyType' }).Value");
            }
        }

        #region CCM_ClientAgentSettings
        /// <summary>
        /// Gets the branding title.
        /// </summary>
        /// <value>The branding title.</value>
        public String BrandingTitle
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "BrandingTitle");
            }
        }

        /// <summary>
        /// Gets the day reminder interval.
        /// </summary>
        /// <value>The day reminder interval.</value>
        public UInt32 DayReminderInterval
        {
            get
            {
                return UInt32.Parse(base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "DayReminderInterval"));
            }
        }

        /// <summary>
        /// Gets the display new program notification.
        /// </summary>
        /// <value>True if the display new program notification is enabled, otherewise false</value>
        public Boolean DisplayNewProgramNotification
        {
            get
            {
                return Boolean.Parse(base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "DisplayNewProgramNotification"));
            }
        }

        /// <summary>
        /// Gets the enable third party orchestration.
        /// </summary>
        /// <value>The enable third party orchestration.</value>
        public UInt32 EnableThirdPartyOrchestration
        {
            get
            {
                return UInt32.Parse(base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "EnableThirdPartyOrchestration"));
            }
        }

        /// <summary>
        /// Gets the hour reminder interval.
        /// </summary>
        /// <value>The hour reminder interval.</value>
        public UInt32 HourReminderInterval
        {
            get
            {
                return UInt32.Parse(base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "HourReminderInterval"));
            }
        }

        /// <summary>
        /// Gets the install restriction.
        /// </summary>
        /// <value>The install restriction.</value>
        public UInt32 InstallRestriction
        {
            get
            {
                return UInt32.Parse(base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "InstallRestriction"));
            }
        }

        /// <summary>
        /// Gets the osd branding subtitle.
        /// </summary>
        /// <value>The osd branding subtitle.</value>
        public String OSDBrandingSubtitle
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "OSDBrandingSubtitle");
            }
        }

        /// <summary>
        /// Gets the reminder interval.
        /// </summary>
        /// <value>The reminder interval.</value>
        public UInt32 ReminderInterval
        {
            get
            {
                return UInt32.Parse(base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "ReminderInterval"));
            }
        }

        /// <summary>
        /// Gets the sum branding subtitle.
        /// </summary>
        /// <value>The sum branding subtitle.</value>
        public String SUMBrandingSubtitle
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "SUMBrandingSubtitle");
            }
        }

        /// <summary>
        /// Gets the suspend bit locker.
        /// </summary>
        /// <value>The suspend bit locker.</value>
        public UInt32 SuspendBitLocker
        {
            get
            {
                return UInt32.Parse(base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "SuspendBitLocker"));
            }
        }

        /// <summary>
        /// Gets the SWD branding subtitle.
        /// </summary>
        /// <value>The SWD branding subtitle.</value>
        public String SWDBrandingSubtitle
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "SWDBarndingSubtitle");
            }
        }

        /// <summary>
        /// Gets the system restart turnaround time.
        /// </summary>
        /// <value>The system restart turnaround time.</value>
        public UInt32 SystemRestartTurnaroundTime
        {
            get
            {
                return UInt32.Parse(base.GetProperty(@"ROOT\ccm\ClientSDK:CCM_ClientAgentSettings=@", "SystemRestartTurnaroundTime"));
            }
        }
        #endregion

        #region CCM_ClientInternalUtilities

        /// <summary>
        /// Gets the are multi users logged on.
        /// </summary>
        /// <value>True if there are multiple users logged on, otherwise false.</value>
        public Boolean AreMultiUsersLoggedOn
        {
            get
            {
                return Boolean.Parse(GetStringFromMethod(@"ROOT\ccm\ClientSDK:CCM_ClientInternalUtilities=@", "AreMultiUsersLoggedOn", "MultiUsersLoggedOn"));
            }
        }

        /// <summary>
        /// Notifies the presentation mode changed.
        /// </summary>
        /// <returns>UInt32.</returns>
        public UInt32 NotifyPresentationModeChanged()
        {
            return UInt32.Parse(GetStringFromMethod(@"ROOT\ccm\ClientSDK:CCM_ClientInternalUtilities=@", "NotifyPresentationModeChanged", "ReturnValue"));
        }

        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="ActionType">Type of the action.</param>
        /// <param name="ClassName">Name of the class.</param>
        /// <param name="MessageLevel">The message level.</param>
        /// <param name="SessionID">The session identifier.</param>
        /// <param name="TargetInstancePath">The target instance path.</param>
        /// <param name="UserSID">The user sid.</param>
        /// <param name="Value">The value.</param>
        /// <param name="Verbosity">The verbosity.</param>
        /// <returns>UInt32.</returns>
        public UInt32 RaiseEvent(UInt32 ActionType, String ClassName, UInt32 MessageLevel, UInt32 SessionID, String TargetInstancePath, String UserSID, String Value, UInt32 Verbosity)
        {
            return UInt32.Parse(GetStringFromMethod(@"ROOT\ccm\ClientSDK:CCM_ClientInternalUtilities=@", string.Format("RaiseEvent({0},{1},{2},{3},{4},{5},{6},{7}", new object[] { ClassName, TargetInstancePath, ActionType, UserSID, SessionID, MessageLevel, Value, Verbosity  }), "ReturnValue"));
        }

        #endregion

        #region CCM_ClientUXSettings
        /// <summary>
        /// Gets the business hours.
        /// </summary>
        /// <returns>PSObject.</returns>
        public PSObject GetBusinessHours()
        {
            PSObject oResult = CallClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUXSettings", "GetBusinessHours", "");
            return oResult;
        }

        /// <summary>
        /// Gets the business hours.
        /// </summary>
        /// <param name="EndTime">The end time.</param>
        /// <param name="StartTime">The start time.</param>
        /// <param name="WorkingDays">The working days.</param>
        /// <returns>PSObject.</returns>
        public PSObject GetBusinessHours(out UInt32 EndTime, out UInt32 StartTime, out UInt32 WorkingDays)
        {
            PSObject oResult = CallClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUXSettings", "GetBusinessHours", "");
            EndTime = UInt32.Parse(oResult.Properties["EndTime"].Value.ToString());
            StartTime = UInt32.Parse(oResult.Properties["StartTime"].Value.ToString());
            WorkingDays = UInt32.Parse(oResult.Properties["WorkingDays"].Value.ToString());
            return oResult;
        }

        /// <summary>
        /// Sets the business hours.
        /// </summary>
        /// <param name="EndTime">The end time.</param>
        /// <param name="StartTime">The start time.</param>
        /// <param name="WorkingDays">The working days.</param>
        /// <returns>0 = Success, non zero for failure</returns>
        public UInt32 SetBusinessHours(UInt32 EndTime, UInt32 StartTime, UInt32 WorkingDays)
        {
            PSObject oResult = CallClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUXSettings", "GetBusinessHours", string.Format("{0}, {1}, {2}", new object[] { WorkingDays, StartTime, EndTime }));
            return UInt32.Parse(oResult.Properties["ReturnValue"].Value.ToString());
        }

        /// <summary>
        /// Sets the automatic install required software to non business hours.
        /// </summary>
        /// <param name="AutomaticallyInstallSoftware">True to enable this otherwise false</param>
        /// <returns>0 = Success, non zero for failure</returns>
        public UInt32 SetAutoInstallRequiredSoftwaretoNonBusinessHours(Boolean AutomaticallyInstallSoftware)
        {
            return UInt32.Parse(GetStringFromClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUXSettings", "SetAutoInstallRequiredSoftwaretoNonBusinessHours(" + AutomaticallyInstallSoftware.ToString() + ")", "ReturnValue"));
        }

        /// <summary>
        /// Gets the automatic install required software to non business hours setting.
        /// </summary>
        /// <param name="AutomaticallyInstallSoftware">Gets the settings.</param>
        /// <returns>0 = Success, non zero for failure</returns>
        public UInt32 GetAutoInstallRequiredSoftwaretoNonBusinessHours(out Boolean AutomaticallyInstallSoftware)
        {
            PSObject oResult = CallClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUXSettings", "GetAutoInstallRequiredSoftwaretoNonBusinessHours", "");
            AutomaticallyInstallSoftware = Boolean.Parse(oResult.Properties["AutomaticallyInstallSoftware"].Value.ToString());
            return UInt32.Parse(oResult.Properties["ReturnValue"].Value.ToString());
        }

        /// <summary>
        /// Sets the suppress computer activity in presentation mode.
        /// </summary>
        /// <param name="SuppressComputerActivityInPresentationMode">The suppress computer activity in presentation mode.</param>
        /// <returns>0 = Success, non zero for failure</returns>
        public UInt32 SetSuppressComputerActivityInPresentationMode(Boolean SuppressComputerActivityInPresentationMode)
        {
            return UInt32.Parse(GetStringFromClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUXSettings", "SetSuppressComputerActivityInPresentationMode(" + SuppressComputerActivityInPresentationMode.ToString() + ")", "ReturnValue"));
        }

        /// <summary>
        /// Gets the suppress computer activity in presentation mode.
        /// </summary>
        /// <param name="SuppressComputerActivityInPresentationMode">Gets the setting. True  means suppress computer activity in presentation mode. </param>
        /// <returns>0 = Success, non zero for failure</returns>
        public UInt32 GetSuppressComputerActivityInPresentationMode(out Boolean SuppressComputerActivityInPresentationMode)
        {
            PSObject oResult = CallClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUXSettings", "GetSuppressComputerActivityInPresentationMode", "");
            SuppressComputerActivityInPresentationMode = Boolean.Parse(oResult.Properties["SuppressComputerActivityInPresentationMode"].Value.ToString());
            return UInt32.Parse(oResult.Properties["ReturnValue"].Value.ToString());
        }
        #endregion

        /// <summary>
        /// Restart Computer from CM12 CCM_ClientUtilities function
        /// </summary>
        /// <returns>Return Code</returns>
        public UInt32 RestartComputer()
        {
            try
            {
                return UInt32.Parse(GetStringFromClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUtilities", "RestartComputer()", "ReturnValue"));
            }
            catch (Exception ex)
            {
                Trace.TraceError("RestartComputer: " + ex.Message);
            }

            return 1;
        }

        /// <summary>
        /// Determine pending reboots (from ConfigMgr. !)
        /// </summary>
        /// <returns></returns>
        public PSObject DetermineIfRebootPending()
        {
            PSObject oResult = CallClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUtilities", "DetermineIfRebootPending", "");
            return oResult;
        }

        /// <summary>
        /// Determine pending reboots (from ConfigMgr. !)
        /// </summary>
        /// <param name="DisableHideTime"></param>
        /// <param name="InGracePeriod"></param>
        /// <param name="IsHardRebootPending"></param>
        /// <param name="RebootDeadline"></param>
        /// <param name="RebootPending"></param>
        /// <returns></returns>
        public PSObject DetermineIfRebootPending(out DateTime DisableHideTime, out Boolean InGracePeriod, out Boolean IsHardRebootPending, out DateTime RebootDeadline, out Boolean RebootPending)
        {
            PSObject oResult = CallClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUtilities", "DetermineIfRebootPending", "");
            DisableHideTime = DateTime.Parse(oResult.Properties["DisableHideTime"].Value.ToString());
            InGracePeriod = Boolean.Parse(oResult.Properties["InGracePeriod"].Value.ToString());
            IsHardRebootPending = Boolean.Parse(oResult.Properties["IsHardRebootPending"].Value.ToString());
            RebootDeadline = DateTime.Parse(oResult.Properties["RebootDeadline"].Value.ToString());
            RebootPending = Boolean.Parse(oResult.Properties["RebootPending"].Value.ToString());
            return oResult;
        }

        /// <summary>
        /// Check if ConfigMgr. requires a reboot;
        /// </summary>
        /// <returns></returns>
        public Boolean RebootPending()
        {
            PSObject oResult = CallClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUtilities", "DetermineIfRebootPending", "");
            //DisableHideTime = DateTime.Parse(oResult.Properties["DisableHideTime"].Value.ToString());
            //Boolean InGracePeriod = Boolean.Parse(oResult.Properties["InGracePeriod"].Value.ToString());
            Boolean IsHardRebootPending = Boolean.Parse(oResult.Properties["IsHardRebootPending"].Value.ToString());
            //RebootDeadline = DateTime.Parse(oResult.Properties["RebootDeadline"].Value.ToString());
            Boolean RebootPending = Boolean.Parse(oResult.Properties["RebootPending"].Value.ToString());

            if (IsHardRebootPending | RebootPending)
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// t.b.d
        /// </summary>
        /// <param name="Feature"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public PSObject GetUserCapability(UInt32 Feature, out UInt32 Value)
        {
            PSObject oResult = CallClassMethod(@"ROOT\ccm\ClientSDK:CCM_ClientUtilities", "DetermineIfRebootPending", Feature.ToString());
            Value = UInt32.Parse(oResult.Properties["Value"].Value.ToString());
            return oResult;
        }

#endif

        /// <summary>
        /// Get/Set the option if an Administrator can Override Agent Settings from the ControlPanel Applet
        /// </summary>
        public Boolean AllowLocalAdminOverride
        {
            get
            {
                return bool.Parse(base.GetProperty(@"ROOT\ccm:SMS_Client=@", "AllowLocalAdminOverride"));
            }
            set
            {
                base.SetProperty(@"ROOT\ccm:SMS_Client=@", "AllowLocalAdminOverride", "$" + value.ToString());
            }
        }

        /// <summary>
        /// Return the SCCM Agent GUID
        /// </summary>
        public string ClientId
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm:CCM_Client=@", "ClientId");
            }
        }

        /// <summary>
        /// Return the previous SCCM Agent GUID
        /// </summary>
        public string PreviousClientId
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm:CCM_Client=@", "PreviousClientId");
            }
        }

        /// <summary>
        /// Return the full SCCM Agent ClientVersion
        /// </summary>
        public string ClientVersion
        {
            get
            {
                TimeSpan toldCacheTime = base.cacheTime;
                base.cacheTime = new TimeSpan(0, 5, 0);
                string sResult = base.GetProperty(@"ROOT\ccm:SMS_Client=@", "ClientVersion");
                base.cacheTime = toldCacheTime;

                return sResult;
            }
        }

        /// <summary>
        /// Return the SCCM Agent GUID creation/change date as string
        /// </summary>
        public string ClientIdChangeDate
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm:CCM_Client=@", "ClientIdChangeDate");
            }
        }

        /// <summary>
        /// Return the SCCM Client Type. 0=Desktop;1=Remote
        /// </summary>
        public UInt32 ClientType
        {
            get
            {
                return UInt32.Parse(base.GetProperty(@"ROOT\ccm:SMS_Client=@", "ClientType"));
            }
        }

        /// <summary>
        /// Enable Site Code Auto Assignment on next Agent Restart
        /// </summary>
        public Boolean EnableAutoAssignment
        {
            get
            {
                return bool.Parse(base.GetProperty(@"ROOT\ccm:SMS_Client=@", "EnableAutoAssignment"));
            }
            set
            {
                base.SetProperty(@"ROOT\ccm:SMS_Client=@", "EnableAutoAssignment", "$" + value.ToString());
            }
        }

        /// <summary>
        /// Return Days Since last reboot
        /// </summary>
        public int DaysSinceLastReboot
        {
            get
            {
                return int.Parse(base.GetStringFromPS("$wmi = Get-WmiObject -Class Win32_OperatingSystem;$a = New-TimeSpan $wmi.ConvertToDateTime($wmi.LastBootUpTime) $(Get-Date);$a.Days"));
            }
        }

        /// <summary>
        /// DateTime of last reboot
        /// </summary>
        public DateTime LastReboot
        {
            get
            {
                return DateTime.ParseExact(base.GetStringFromPS("$wmi = Get-WmiObject -Class Win32_OperatingSystem;$a = $wmi.ConvertToDateTime($wmi.LastBootUpTime);$a.ToString(\"yyyy-MM-dd HH:mm\")"),"yyyy-MM-dd HH:mm", null);
            }
        }

        /// <summary>
        /// Get TimeSpan of LastReboot
        /// </summary>
        public TimeSpan LastRebootTimeSpan
        {
            get
            {
                return TimeSpan.FromSeconds(double.Parse(base.GetStringFromPS("$wmi = Get-WmiObject -Class Win32_OperatingSystem;$a = New-TimeSpan $wmi.ConvertToDateTime($wmi.LastBootUpTime) $(Get-Date);$a.TotalSeconds").ToString()));
            }
        }

        /// <summary>
        /// Assigned SCCM Agent Site Code
        /// </summary>
        public string AssignedSite
        {
            get
            {
                //Backup original Cache timout value
                TimeSpan oldTime = base.cacheTime;

                //Set new CacheTimeout for the AssignedSite Code to 1hour
                base.cacheTime = new TimeSpan(1, 0, 0);
                string sSiteCode = base.GetStringFromClassMethod(@"ROOT\ccm:SMS_Client", "GetAssignedSite()", "sSiteCode");

                //Reset to original Cache timeout value
                base.cacheTime = oldTime;

                return sSiteCode;
            }
            set
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "SetAssignedSite", "\"" + value + "\"");
                
                //Remove Site Code from Cache
                string sHash = CreateHash(@"ROOT\ccm:SMS_Client" + "GetAssignedSite()" + ".sSiteCode");
                base.Cache.Remove(sHash);
            }
        }

        /// <summary>
        /// Get the assigned Management Point
        /// </summary>
        public string ManagementPoint
        {
            get
            {
                //Backup original Cache timout value
                TimeSpan oldTime = base.cacheTime;

                //Set new CacheTimeout for the ManagementPointe to 1hour
                base.cacheTime = new TimeSpan(1, 0, 0);
                
                string sMP = base.GetProperty(@"ROOT\ccm:SMS_Authority.Name='SMS:" + AssignedSite + "'", "CurrentManagementPoint");
                
                //Reset to original Cache timeout value
                base.cacheTime = oldTime;

                return sMP;

            }
        }

        /// <summary>
        /// Configure Internet Management Point
        /// </summary>
        public string ManagementPointInternet
        {
            get
            {
                if (baseClient.Inventory.isx64OS & !baseClient.AgentProperties.isSCCM2012)
                {
                    return base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Client\\Internet Facing\")).$(\"Internet MP Hostname\")");
                }
                else
                {
                    return base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Client\\Internet Facing\")).$(\"Internet MP Hostname\")");
                }
            }
            set
            {
                if (baseClient.Inventory.isx64OS)
                {
                    base.GetStringFromPS("Set-ItemProperty -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Client\\Internet Facing\" -name \"Internet MP Hostname\" -value \"" + value + "\"");

                    //Remove Internet MP from Cache
                    string sHash = CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Client\\Internet Facing\")).$(\"Internet MP Hostname\")");
                    base.Cache.Remove(sHash);
                }
                else
                {
                    base.GetStringFromPS("Set-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Client\\Internet Facing\" -name \"Internet MP Hostname\" -value \"" + value + "\"");

                    //Remove Internet MP from Cache
                    string sHash = CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Client\\Internet Facing\")).$(\"Internet MP Hostname\")");
                    base.Cache.Remove(sHash);
                }
            }
        }

        /// <summary>
        /// Get the assigned Proxy Management Point
        /// </summary>
        public string ManagementPointProxy
        {
            get
            {
                //Backup original Cache timout value
                TimeSpan oldTime = base.cacheTime;

                //Set new CacheTimeout for the ManagementPoint to 1 Minute
                base.cacheTime = new TimeSpan(0, 1, 0);

                string sMP = "";
                
                try
                {
                    foreach (PSObject oMP in base.GetObjects(@"ROOT\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'"))
                    {
                        sMP = oMP.Properties["Name"].Value.ToString();
                    }
                }
                catch { }

                //Reset to original Cache timeout value
                base.cacheTime = oldTime;

                return sMP;

            }
        }

        /// <summary>
        /// determine if SCCM Agent is from SCCM2012(TRUE) otherwise it's SCCM2007(FALSE)
        /// </summary>
        public Boolean isSCCM2012
        {
            get
            {
                if (ClientVersion.StartsWith("5.", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the local Path of the SCCM Agent (e.g. C:\Windows\CCM )
        /// </summary>
        public string LocalSCCMAgentPath
        {
            get
            {
                if (baseClient.Inventory.isx64OS & !baseClient.AgentProperties.isSCCM2012)
                {
                    return base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Client\\Configuration\\Client Properties\")).$(\"Local SMS Path\")");
                }
                else
                {
                    return base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Client\\Configuration\\Client Properties\")).$(\"Local SMS Path\")");
                }
            }
        }

        /// <summary>
        /// Get the local Path of the SCCM Agent Log Files (e.g. C:\windows\ccm\logs)
        /// </summary>
        public string LocalSCCMAgentLogPath
        {
            get
            {
                return System.IO.Path.Combine(LocalSCCMAgentPath, "Logs");
            }
        }

        /// <summary>
        /// Get all Log Files from the SCCM Agent Log Folder
        /// </summary>
        public List<string> LocalSCCMAgentLogFiles
        {
            get
            {
                List<string> lResult = new List<string>();
                try
                {
                    List<PSObject> oRes = base.GetObjectsFromPS(string.Format("(get-item {0}\\*.log).Name", LocalSCCMAgentLogPath), false, new TimeSpan(0, 5, 0));
                    foreach (PSObject oFile in oRes)
                    {
                        lResult.Add(oFile.ToString());
                    }
                }
                catch { }

                return lResult;
            }
        }

        /// <summary>
        /// Get or Set the Server Locator Point (in CM12 it's the Management Point)
        /// </summary>
        public string ServerLocatorPoint
        {
            get
            {
                if (baseClient.Inventory.isx64OS & !baseClient.AgentProperties.isSCCM2012)
                {
                    return base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"SMSSLP\")");
                }
                else
                {
                    return base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"SMSSLP\")");
                }
            }

            set
            {
                if (baseClient.Inventory.isx64OS & !baseClient.AgentProperties.isSCCM2012)
                {
                    base.GetStringFromPS(string.Format("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\" -name \"SMSSLP\" -PropertyType String -Force -value {0}", value));
                    //Remove SLP from Cache
                    string sHash = CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"SMSSLP\")");
                    base.Cache.Remove(sHash);
                }
                else
                {
                    base.GetStringFromPS(string.Format("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\" -name \"SMSSLP\" -PropertyType String -Force -value {0}", value));
                    //Remove SLP from Cache
                    string sHash = CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"SMSSLP\")");
                    base.Cache.Remove(sHash);
                }
            }
        }

        /// <summary>
        /// Get or Set the DNS Suffix
        /// </summary>
        public string DNSSuffix
        {
            get
            {
                if (baseClient.Inventory.isx64OS & !baseClient.AgentProperties.isSCCM2012)
                {
                    return base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\\LocationServices\")).$(\"DnsSuffix\")");
                }
                else
                {
                    return base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\\LocationServices\")).$(\"DnsSuffix\")");
                }
            }

            set
            {
                if (baseClient.Inventory.isx64OS & !baseClient.AgentProperties.isSCCM2012)
                {
                    base.GetStringFromPS(string.Format("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\\LocationServices\" -name \"DnsSuffix\" -PropertyType String -Force -value {0}", value));
                    //Remove DNS from Cache
                    string sHash = CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\\LocationServices\")).$(\"DnsSuffix\")");
                    base.Cache.Remove(sHash);
                }
                else
                {
                    base.GetStringFromPS(string.Format("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\\LocationServices\" -name \"DnsSuffix\" -PropertyType String -Force -value {0}", value));
                    //Remove DNS from Cache
                    string sHash = CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\\LocationServices\")).$(\"DnsSuffix\")");
                    base.Cache.Remove(sHash);
                }
            }
        }

        /// <summary>
        /// Get or Set the HTTP Port from the Agent.
        /// </summary>
        public int? HTTPPort
        {
            get
            {
                if (baseClient.Inventory.isx64OS & !baseClient.AgentProperties.isSCCM2012)
                {
                    string sPort = base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"HttpPort\")");
                    if(!string.IsNullOrEmpty(sPort))
                        return int.Parse(sPort);
                    else
                        return null; 
                }
                else
                {
                    //return base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\\LocationServices\")).$(\"DnsSuffix\")");
                    string sPort = base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"HttpPort\")");
                    if (!string.IsNullOrEmpty(sPort))
                        return int.Parse(sPort);
                    else
                        return null; 
                }
            }

            set
            {
                if (baseClient.Inventory.isx64OS & !baseClient.AgentProperties.isSCCM2012)
                {
                    base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"HttpPort\")");

                    //Remove HTTP Port from Cache
                    string sHash = CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"HttpPort\")");
                    base.Cache.Remove(sHash);
                }
                else
                {
                    base.GetStringFromPS(string.Format("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\" -name \"HttpPort\" -Type DWORD -force -value {0}", value.ToString()));
                    
                    //Remove HTTP Port from Cache
                    string sHash = CreateHash(string.Format("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\" -name \"HttpPort\" -Type DWORD -force -value {0}", value.ToString()));
                    base.Cache.Remove(sHash);
                }
            } 
        }

        /// <summary>
        /// Get or Set the HTTPS Port from the Agent.
        /// </summary>
        public int? HTTPSPort
        {
            get
            {
                if (baseClient.Inventory.isx64OS & !baseClient.AgentProperties.isSCCM2012)
                {
                    string sPort = base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"HttpsPort\")");
                    if (!string.IsNullOrEmpty(sPort))
                        return int.Parse(sPort);
                    else
                        return null;
                }
                else
                {
                    string sPort = base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"HttpsPort\")");
                    if (!string.IsNullOrEmpty(sPort))
                        return int.Parse(sPort);
                    else
                        return null;
                }
            }

            set
            {
                if (baseClient.Inventory.isx64OS & !baseClient.AgentProperties.isSCCM2012)
                {
                    base.GetStringFromPS(string.Format("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\" -name \"HttpsPort\" -Type DWORD -force -value {0}", value.ToString()));

                    //Remove HTTPS Port from Cache
                    string sHash = CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\CCM\")).$(\"HttpsPort\")");
                    base.Cache.Remove(sHash);
                }
                else
                {
                    base.GetStringFromPS(string.Format("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\" -name \"HttpsPort\" -Type DWORD -force -value {0}", value.ToString()));
                    
                    //Remove HTTPS Port from Cache
                    string sHash = CreateHash("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"HttpsPort\")");
                    base.Cache.Remove(sHash);
                }
            }
        }

        /// <summary>
        /// Determine if pending FileRenameOperations exists...
        /// </summary>
        public bool FileRenameOperationsPending
        {
            get
            {
                List<PSObject> lResult = base.GetObjectsFromPS("(Get-ItemProperty(\"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\")).$(\"PendingFileRenameOperations\")");
                if (lResult.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Determin if component chnage requires a reboot
        /// </summary>
        public bool ComponentServicingRebootPending
        {
            get
            {
                List<PSObject> lResult = base.GetObjectsFromPS("if(test-path \"HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\RebootPending\"){ $true }");
                if (lResult.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the MSI Product Code of the SCCM/CM12 Agent
        /// </summary>
        public string ProductCode
        {
            get
            {
                string sProductCode = base.GetStringFromPS("(Get-WmiObject -Class CCM_InstalledProduct -Namespace \"root\\ccm\").ProductCode");
                return sProductCode;
            }
        }

        /// <summary>
        /// Delete root\ccm Namespace 
        /// Query from rchiav (https://sccmclictrlib.codeplex.com/discussions/349818)
        /// </summary>
        /// <returns></returns>
        public string DeleteCCMNamespace()
        {
                string sProductCode = base.GetStringFromPS("gwmi -query \"SELECT * FROM __Namespace WHERE Name='CCM'\" -Namespace \"root\" | Remove-WmiObject");
                return sProductCode;      
        }

        /// <summary>
        /// Get the SID's of all logged on Users
        /// </summary>
        /// <returns></returns>
        public List<string> GetLoggedOnUserSIDs()
        {
            List<string> oResult = new List<string>();
            if (ClientVersion.StartsWith("5.00.78"))
            {
                try
                {
                    string sSidQuery = "$username = (get-wmiobject -query \"SELECT Username FROM Win32_ComputerSystem\" -namespace \"root\\cimv2\").Username;$user = New-Object System.Security.Principal.NTAccount($username.split('\\')[0],$username.split('\\')[1]);$sid = $user.Translate([System.Security.Principal.SecurityIdentifier]);$sid.Value";
                    oResult.Add(base.GetStringFromPS(sSidQuery));
                }
                catch { }
            }
            else
            {
                //Does only work with CM12 R2 or later...
                foreach (PSObject oUsr in base.GetObjectsFromPS("get-wmiobject -query \"SELECT UserSID FROM CCM_UserLogonEvents WHERE LogoffTime = NULL\" -namespace \"ROOT\\ccm\""))
                {
                    try
                    {
                        oResult.Add(oUsr.Properties["UserSID"].Value.ToString());
                    }
                    catch { }
                }

            }
            return oResult;
        }
    }
}
