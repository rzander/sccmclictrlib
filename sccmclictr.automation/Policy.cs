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
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Web;

namespace sccmclictr.automation.policy
{
    /// <summary>
    /// SCCM Requested Policy
    /// </summary>
    public class requestedConfig : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public requestedConfig(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }


        public List<CCM_ComponentClientConfig> ComponentClientConfig
        {
            get
            {
                List<CCM_ComponentClientConfig> lCache = new List<CCM_ComponentClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_ComponentClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_ComponentClientConfig oCIEx = new CCM_ComponentClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache; 
            } 
        }

        public List<CCM_SoftwareUpdatesClientConfig> SoftwareUpdatesClientConfig
        {
            get
            {
                List<CCM_SoftwareUpdatesClientConfig> lCache = new List<CCM_SoftwareUpdatesClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_SoftwareUpdatesClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_SoftwareUpdatesClientConfig oCIEx = new CCM_SoftwareUpdatesClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_RootCACertificates> RootCACertificates
        {
            get
            {
                List<CCM_RootCACertificates> lCache = new List<CCM_RootCACertificates>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_RootCACertificates");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_RootCACertificates oCIEx = new CCM_RootCACertificates(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SourceUpdateClientConfig> SourceUpdateClientConfig
        {
            get
            {
                List<CCM_SourceUpdateClientConfig> lCache = new List<CCM_SourceUpdateClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_SourceUpdateClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_SourceUpdateClientConfig oCIEx = new CCM_SourceUpdateClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SoftwareCenterSettings> SoftwareCenterSettings
        {
            get
            {
                List<CCM_SoftwareCenterSettings> lCache = new List<CCM_SoftwareCenterSettings>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_SoftwareCenterSettings");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_SoftwareCenterSettings oCIEx = new CCM_SoftwareCenterSettings(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SoftwareInventoryClientConfig> SoftwareInventoryClientConfig
        {
            get
            {
                List<CCM_SoftwareInventoryClientConfig> lCache = new List<CCM_SoftwareInventoryClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_SoftwareInventoryClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_SoftwareInventoryClientConfig oCIEx = new CCM_SoftwareInventoryClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_TargetingSettings> TargetingSettings
        {
            get
            {
                List<CCM_TargetingSettings> lCache = new List<CCM_TargetingSettings>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_TargetingSettings");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_TargetingSettings oCIEx = new CCM_TargetingSettings(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_MulticastConfig> MulticastConfig
        {
            get
            {
                List<CCM_MulticastConfig> lCache = new List<CCM_MulticastConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_MulticastConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_MulticastConfig oCIEx = new CCM_MulticastConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SoftwareDistributionClientConfig> SoftwareDistributionClientConfig
        {
            get
            {
                List<CCM_SoftwareDistributionClientConfig> lCache = new List<CCM_SoftwareDistributionClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_SoftwareDistributionClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_SoftwareDistributionClientConfig oCIEx = new CCM_SoftwareDistributionClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_ConfigurationManagementClientConfig> ConfigurationManagementClientConfig
        {
            get
            {
                List<CCM_ConfigurationManagementClientConfig> lCache = new List<CCM_ConfigurationManagementClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_ConfigurationManagementClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_ConfigurationManagementClientConfig oCIEx = new CCM_ConfigurationManagementClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_ClientAgentConfig> ClientAgentConfig
        {
            get
            {
                List<CCM_ClientAgentConfig> lCache = new List<CCM_ClientAgentConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_ClientAgentConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_ClientAgentConfig oCIEx = new CCM_ClientAgentConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SystemHealthClientConfig> SystemHealthClientConfig
        {
            get
            {
                List<CCM_SystemHealthClientConfig> lCache = new List<CCM_SystemHealthClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_SystemHealthClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_SystemHealthClientConfig oCIEx = new CCM_SystemHealthClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_PowerManagementClientConfig> PowerManagementClientConfig
        {
            get
            {
                List<CCM_PowerManagementClientConfig> lCache = new List<CCM_PowerManagementClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_PowerManagementClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_PowerManagementClientConfig oCIEx = new CCM_PowerManagementClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SoftwareMeteringClientConfig> SoftwareMeteringClientConfig
        {
            get
            {
                List<CCM_SoftwareMeteringClientConfig> lCache = new List<CCM_SoftwareMeteringClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_SoftwareMeteringClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_SoftwareMeteringClientConfig oCIEx = new CCM_SoftwareMeteringClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_HardwareInventoryClientConfig> HardwareInventoryClientConfig
        {
            get
            {
                List<CCM_HardwareInventoryClientConfig> lCache = new List<CCM_HardwareInventoryClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_HardwareInventoryClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_HardwareInventoryClientConfig oCIEx = new CCM_HardwareInventoryClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_RemoteTools_Policy> RemoteTools_Policy
        {
            get
            {
                List<CCM_RemoteTools_Policy> lCache = new List<CCM_RemoteTools_Policy>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_RemoteTools_Policy");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_RemoteTools_Policy oCIEx = new CCM_RemoteTools_Policy(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_NetworkAccessAccount> NetworkAccessAccount
        {
            get
            {
                List<CCM_NetworkAccessAccount> lCache = new List<CCM_NetworkAccessAccount>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_NetworkAccessAccount");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_NetworkAccessAccount oCIEx = new CCM_NetworkAccessAccount(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_ApplicationManagementClientConfig> ApplicationManagementClientConfig
        {
            get
            {
                List<CCM_ApplicationManagementClientConfig> lCache = new List<CCM_ApplicationManagementClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_ApplicationManagementClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_ApplicationManagementClientConfig oCIEx = new CCM_ApplicationManagementClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_OutOfBandManagementClientConfig> OutOfBandManagementClientConfig
        {
            get
            {
                List<CCM_OutOfBandManagementClientConfig> lCache = new List<CCM_OutOfBandManagementClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * CCM_OutOfBandManagementClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_OutOfBandManagementClientConfig oCIEx = new CCM_OutOfBandManagementClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_Policy
        {
            //Constructor
            public CCM_Policy(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.PolicyID = WMIObject.Properties["PolicyID"].Value as String;
                this.PolicyInstanceID = WMIObject.Properties["PolicyInstanceID"].Value as String;
                this.PolicyPrecedence = WMIObject.Properties["PolicyPrecedence"].Value as UInt32?;
                this.PolicyRuleID = WMIObject.Properties["PolicyRuleID"].Value as String;
                this.PolicySource = WMIObject.Properties["PolicySource"].Value as String;
                this.PolicyVersion = WMIObject.Properties["PolicyVersion"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String PolicyID { get; set; }
            public String PolicyInstanceID { get; set; }
            public UInt32? PolicyPrecedence { get; set; }
            public String PolicyRuleID { get; set; }
            public String PolicySource { get; set; }
            public String PolicyVersion { get; set; }
            #endregion
            
        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_ComponentClientConfig : CCM_Policy
        {
            //Constructor
            public CCM_ComponentClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;
                
                /*this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject; */
                this.ComponentName = WMIObject.Properties["ComponentName"].Value as String;
                this.Enabled = WMIObject.Properties["Enabled"].Value as Boolean?;
            }

            #region Properties

            /*internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode; */
            public String ComponentName { get; set; }
            public Boolean? Enabled { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SoftwareUpdatesClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SoftwareUpdatesClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.AssignmentBatchingTimeout = WMIObject.Properties["AssignmentBatchingTimeout"].Value as UInt32?;
                this.BrandingSubTitle = WMIObject.Properties["BrandingSubTitle"].Value as String;
                this.BrandingTitle = WMIObject.Properties["BrandingTitle"].Value as String;
                this.ContentDownloadTimeout = WMIObject.Properties["ContentDownloadTimeout"].Value as UInt32?;
                this.ContentLocationTimeout = WMIObject.Properties["ContentLocationTimeout"].Value as UInt32?;
                this.DayReminderInterval = WMIObject.Properties["DayReminderInterval"].Value as UInt32?;
                this.HourReminderInterval = WMIObject.Properties["HourReminderInterval"].Value as UInt32?;
                this.MaxScanRetryCount = WMIObject.Properties["MaxScanRetryCount"].Value as UInt32?;
                this.PerDPInactivityTimeout = WMIObject.Properties["PerDPInactivityTimeout"].Value as UInt32?;
                this.ReminderInterval = WMIObject.Properties["ReminderInterval"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.ScanRetryDelay = WMIObject.Properties["ScanRetryDelay"].Value as UInt32?;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
                this.TotalInactivityTimeout = WMIObject.Properties["TotalInactivityTimeout"].Value as UInt32?;
                this.UserJobPerDPInactivityTimeout = WMIObject.Properties["UserJobPerDPInactivityTimeout"].Value as UInt32?;
                this.UserJobTotalInactivityTimeout = WMIObject.Properties["UserJobTotalInactivityTimeout"].Value as UInt32?;
                this.WSUSLocationTimeout = WMIObject.Properties["WSUSLocationTimeout"].Value as UInt32?;
            }

            #region Properties

            public UInt32? AssignmentBatchingTimeout { get; set; }
            public String BrandingSubTitle { get; set; }
            public String BrandingTitle { get; set; }
            public UInt32? ContentDownloadTimeout { get; set; }
            public UInt32? ContentLocationTimeout { get; set; }
            public UInt32? DayReminderInterval { get; set; }
            public UInt32? HourReminderInterval { get; set; }
            public UInt32? MaxScanRetryCount { get; set; }
            public UInt32? PerDPInactivityTimeout { get; set; }
            public UInt32? ReminderInterval { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? ScanRetryDelay { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            public UInt32? TotalInactivityTimeout { get; set; }
            public UInt32? UserJobPerDPInactivityTimeout { get; set; }
            public UInt32? UserJobTotalInactivityTimeout { get; set; }
            public UInt32? WSUSLocationTimeout { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_RootCACertificates : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_RootCACertificates(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.RootCACerts = WMIObject.Properties["RootCACerts"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public String Reserved1 { get; set; }
            public String RootCACerts { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SourceUpdateClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SourceUpdateClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.LocationTimeOut = WMIObject.Properties["LocationTimeOut"].Value as UInt32?;
                this.MaxRetryCount = WMIObject.Properties["MaxRetryCount"].Value as UInt32?;
                this.NetworkChangeDelay = WMIObject.Properties["NetworkChangeDelay"].Value as UInt32?;
                this.RemoteDPs = WMIObject.Properties["RemoteDPs"].Value as Boolean?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.RetryTimeOut = WMIObject.Properties["RetryTimeOut"].Value as UInt32?;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? LocationTimeOut { get; set; }
            public UInt32? MaxRetryCount { get; set; }
            public UInt32? NetworkChangeDelay { get; set; }
            public Boolean? RemoteDPs { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? RetryTimeOut { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SoftwareCenterSettings : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SoftwareCenterSettings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.AutoInstallRequiredSoftware = WMIObject.Properties["AutoInstallRequiredSoftware"].Value as Boolean?;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
                this.SuppressComputerActivityInPresentationMode = WMIObject.Properties["SuppressComputerActivityInPresentationMode"].Value as Boolean?;
            }

            #region Properties

            public Boolean? AutoInstallRequiredSoftware { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            public Boolean? SuppressComputerActivityInPresentationMode { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SoftwareInventoryClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SoftwareInventoryClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_TargetingSettings : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_TargetingSettings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.AllowUserAffinity = WMIObject.Properties["AllowUserAffinity"].Value as UInt32?;
                this.AllowUserAffinityAfterMinutes = WMIObject.Properties["AllowUserAffinityAfterMinutes"].Value as UInt32?;
                this.AutoApproveAffinity = WMIObject.Properties["AutoApproveAffinity"].Value as UInt32?;
                this.ConsoleMinutes = WMIObject.Properties["ConsoleMinutes"].Value as UInt32?;
                this.IntervalDays = WMIObject.Properties["IntervalDays"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? AllowUserAffinity { get; set; }
            public UInt32? AllowUserAffinityAfterMinutes { get; set; }
            public UInt32? AutoApproveAffinity { get; set; }
            public UInt32? ConsoleMinutes { get; set; }
            public UInt32? IntervalDays { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_MulticastConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_MulticastConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.RetryCount = WMIObject.Properties["RetryCount"].Value as UInt32?;
                this.RetryDelay = WMIObject.Properties["RetryDelay"].Value as UInt32?;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? RetryCount { get; set; }
            public UInt32? RetryDelay { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SoftwareDistributionClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SoftwareDistributionClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.ADV_RebootLogoffNotification = WMIObject.Properties["ADV_RebootLogoffNotification"].Value as Boolean?;
                this.ADV_RebootLogoffNotificationCountdownDuration = WMIObject.Properties["ADV_RebootLogoffNotificationCountdownDuration"].Value as UInt32?;
                this.ADV_RebootLogoffNotificationFinalWindow = WMIObject.Properties["ADV_RebootLogoffNotificationFinalWindow"].Value as UInt32?;
                this.ADV_RunNotificationCountdownDuration = WMIObject.Properties["ADV_RunNotificationCountdownDuration"].Value as UInt32?;
                this.ADV_WhatsNewDuration = WMIObject.Properties["ADV_WhatsNewDuration"].Value as UInt32?;
                this.CacheContentTimeout = WMIObject.Properties["CacheContentTimeout"].Value as UInt32?;
                this.CacheSpaceFailureRetryCount = WMIObject.Properties["CacheSpaceFailureRetryCount"].Value as UInt32?;
                this.CacheSpaceFailureRetryInterval = WMIObject.Properties["CacheSpaceFailureRetryInterval"].Value as UInt32?;
                this.CacheTombstoneContentMinDuration = WMIObject.Properties["CacheTombstoneContentMinDuration"].Value as UInt32?;
                this.ContentLocationTimeoutInterval = WMIObject.Properties["ContentLocationTimeoutInterval"].Value as UInt32?;
                this.ContentLocationTimeoutRetryCount = WMIObject.Properties["ContentLocationTimeoutRetryCount"].Value as UInt32?;
                this.DefaultMaxDuration = WMIObject.Properties["DefaultMaxDuration"].Value as UInt32?;
                this.DisplayNewProgramNotification = WMIObject.Properties["DisplayNewProgramNotification"].Value as Boolean?;
                this.ExecutionFailureRetryCount = WMIObject.Properties["ExecutionFailureRetryCount"].Value as UInt32?;
                this.ExecutionFailureRetryErrorCodes = WMIObject.Properties["ExecutionFailureRetryErrorCodes"].Value as UInt32?[];
                this.ExecutionFailureRetryInterval = WMIObject.Properties["ExecutionFailureRetryInterval"].Value as UInt32?;
                this.LockSettings = WMIObject.Properties["LockSettings"].Value as Boolean?;
                this.LogoffReturnCodes = WMIObject.Properties["LogoffReturnCodes"].Value as UInt32?[];
                this.NetworkAccessPassword = WMIObject.Properties["NetworkAccessPassword"].Value as String;
                this.NetworkAccessUsername = WMIObject.Properties["NetworkAccessUsername"].Value as String;
                this.NetworkFailureRetryCount = WMIObject.Properties["NetworkFailureRetryCount"].Value as UInt32?;
                this.NetworkFailureRetryInterval = WMIObject.Properties["NetworkFailureRetryInterval"].Value as UInt32?;
                this.NewProgramNotificationUI = WMIObject.Properties["NewProgramNotificationUI"].Value as String;
                this.PRG_PRF_RunNotification = WMIObject.Properties["PRG_PRF_RunNotification"].Value as Boolean?;
                this.RebootReturnCodes = WMIObject.Properties["RebootReturnCodes"].Value as UInt32?[];
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
                this.SuccessReturnCodes = WMIObject.Properties["SuccessReturnCodes"].Value as UInt32?[];
                this.UIContentLocationTimeoutInterval = WMIObject.Properties["UIContentLocationTimeoutInterval"].Value as UInt32?;
                this.UserPreemptionCountdown = WMIObject.Properties["UserPreemptionCountdown"].Value as UInt32?;
                this.UserPreemptionTimeout = WMIObject.Properties["UserPreemptionTimeout"].Value as UInt32?;
            }

            #region Properties

            public Boolean? ADV_RebootLogoffNotification { get; set; }
            public UInt32? ADV_RebootLogoffNotificationCountdownDuration { get; set; }
            public UInt32? ADV_RebootLogoffNotificationFinalWindow { get; set; }
            public UInt32? ADV_RunNotificationCountdownDuration { get; set; }
            public UInt32? ADV_WhatsNewDuration { get; set; }
            public UInt32? CacheContentTimeout { get; set; }
            public UInt32? CacheSpaceFailureRetryCount { get; set; }
            public UInt32? CacheSpaceFailureRetryInterval { get; set; }
            public UInt32? CacheTombstoneContentMinDuration { get; set; }
            public UInt32? ContentLocationTimeoutInterval { get; set; }
            public UInt32? ContentLocationTimeoutRetryCount { get; set; }
            public UInt32? DefaultMaxDuration { get; set; }
            public Boolean? DisplayNewProgramNotification { get; set; }
            public UInt32? ExecutionFailureRetryCount { get; set; }
            public UInt32?[] ExecutionFailureRetryErrorCodes { get; set; }
            public UInt32? ExecutionFailureRetryInterval { get; set; }
            public Boolean? LockSettings { get; set; }
            public UInt32?[] LogoffReturnCodes { get; set; }
            public String NetworkAccessPassword { get; set; }
            public String NetworkAccessUsername { get; set; }
            public UInt32? NetworkFailureRetryCount { get; set; }
            public UInt32? NetworkFailureRetryInterval { get; set; }
            public String NewProgramNotificationUI { get; set; }
            public Boolean? PRG_PRF_RunNotification { get; set; }
            public UInt32?[] RebootReturnCodes { get; set; }
            public String Reserved { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            public UInt32?[] SuccessReturnCodes { get; set; }
            public UInt32? UIContentLocationTimeoutInterval { get; set; }
            public UInt32? UserPreemptionCountdown { get; set; }
            public UInt32? UserPreemptionTimeout { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_ConfigurationManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_ConfigurationManagementClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.PerProviderTimeOut = WMIObject.Properties["PerProviderTimeOut"].Value as UInt32?;
                this.PerScanTimeout = WMIObject.Properties["PerScanTimeout"].Value as UInt32?;
                this.PerScanTTL = WMIObject.Properties["PerScanTTL"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? PerProviderTimeOut { get; set; }
            public UInt32? PerScanTimeout { get; set; }
            public UInt32? PerScanTTL { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_ClientAgentConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_ClientAgentConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.BrandingTitle = WMIObject.Properties["BrandingTitle"].Value as String;
                this.DayReminderInterval = WMIObject.Properties["DayReminderInterval"].Value as UInt32?;
                this.DisplayNewProgramNotification = WMIObject.Properties["DisplayNewProgramNotification"].Value as Boolean?;
                this.EnableThirdPartyOrchestration = WMIObject.Properties["EnableThirdPartyOrchestration"].Value as UInt32?;
                this.HourReminderInterval = WMIObject.Properties["HourReminderInterval"].Value as UInt32?;
                this.InstallRestriction = WMIObject.Properties["InstallRestriction"].Value as UInt32?;
                this.OSDBrandingSubTitle = WMIObject.Properties["OSDBrandingSubTitle"].Value as String;
                this.PowerShellExecutionPolicy = WMIObject.Properties["PowerShellExecutionPolicy"].Value as UInt32?;
                this.ReminderInterval = WMIObject.Properties["ReminderInterval"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
                this.SUMBrandingSubTitle = WMIObject.Properties["SUMBrandingSubTitle"].Value as String;
                this.SuspendBitLocker = WMIObject.Properties["SuspendBitLocker"].Value as UInt32?;
                this.SWDBrandingSubTitle = WMIObject.Properties["SWDBrandingSubTitle"].Value as String;
                this.SystemRestartTurnaroundTime = WMIObject.Properties["SystemRestartTurnaroundTime"].Value as UInt32?;
            }

            #region Properties

            public String BrandingTitle { get; set; }
            public UInt32? DayReminderInterval { get; set; }
            public Boolean? DisplayNewProgramNotification { get; set; }
            public UInt32? EnableThirdPartyOrchestration { get; set; }
            public UInt32? HourReminderInterval { get; set; }
            public UInt32? InstallRestriction { get; set; }
            public String OSDBrandingSubTitle { get; set; }
            public UInt32? PowerShellExecutionPolicy { get; set; }
            public UInt32? ReminderInterval { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            public String SUMBrandingSubTitle { get; set; }
            public UInt32? SuspendBitLocker { get; set; }
            public String SWDBrandingSubTitle { get; set; }
            public UInt32? SystemRestartTurnaroundTime { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SystemHealthClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SystemHealthClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.CumulativeDownloadTimeout = WMIObject.Properties["CumulativeDownloadTimeout"].Value as UInt32?;
                this.CumulativeInactivityTimeout = WMIObject.Properties["CumulativeInactivityTimeout"].Value as UInt32?;
                this.DPLocality = WMIObject.Properties["DPLocality"].Value as UInt32?;
                this.EffectiveTimeinUTC = WMIObject.Properties["EffectiveTimeinUTC"].Value as UInt32?;
                this.ForceScan = WMIObject.Properties["ForceScan"].Value as Boolean?;
                this.LocationsTimeout = WMIObject.Properties["LocationsTimeout"].Value as UInt32?;
                this.PerDPInactivityTimeout = WMIObject.Properties["PerDPInactivityTimeout"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.Reserved4 = WMIObject.Properties["Reserved4"].Value as UInt32?;
                this.SiteCode = WMIObject.Properties["SiteCode"].Value as String;
                this.SiteID = WMIObject.Properties["SiteID"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? CumulativeDownloadTimeout { get; set; }
            public UInt32? CumulativeInactivityTimeout { get; set; }
            public UInt32? DPLocality { get; set; }
            public UInt32? EffectiveTimeinUTC { get; set; }
            public Boolean? ForceScan { get; set; }
            public UInt32? LocationsTimeout { get; set; }
            public UInt32? PerDPInactivityTimeout { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? Reserved4 { get; set; }
            public String SiteCode { get; set; }
            public String SiteID { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_PowerManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_PowerManagementClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.AllowUserToOptOutFromPowerPlan = WMIObject.Properties["AllowUserToOptOutFromPowerPlan"].Value as Boolean?;
                this.EnableUserIdleMonitoring = WMIObject.Properties["EnableUserIdleMonitoring"].Value as Boolean?;
                this.NumberOfDaysToKeep = WMIObject.Properties["NumberOfDaysToKeep"].Value as UInt32?;
                this.NumberOfMonthsToKeep = WMIObject.Properties["NumberOfMonthsToKeep"].Value as UInt32?;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public Boolean? AllowUserToOptOutFromPowerPlan { get; set; }
            public Boolean? EnableUserIdleMonitoring { get; set; }
            public UInt32? NumberOfDaysToKeep { get; set; }
            public UInt32? NumberOfMonthsToKeep { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SoftwareMeteringClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SoftwareMeteringClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.MaximumUsageInstancesPerReport = WMIObject.Properties["MaximumUsageInstancesPerReport"].Value as UInt32?;
                this.ReportTimeout = WMIObject.Properties["ReportTimeout"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? MaximumUsageInstancesPerReport { get; set; }
            public UInt32? ReportTimeout { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_HardwareInventoryClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_HardwareInventoryClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;

                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_RemoteTools_Policy : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_RemoteTools_Policy(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

            }

            #region Properties

            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_NetworkAccessAccount : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_NetworkAccessAccount(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.NetworkAccessPassword = WMIObject.Properties["NetworkAccessPassword"].Value as String;
                this.NetworkAccessUsername = WMIObject.Properties["NetworkAccessUsername"].Value as String;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public String NetworkAccessPassword { get; set; }
            public String NetworkAccessUsername { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_ApplicationManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_ApplicationManagementClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.ContentDownloadTimeOut = WMIObject.Properties["ContentDownloadTimeOut"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? ContentDownloadTimeOut { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_OutOfBandManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_OutOfBandManagementClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }
    }

    public class actualConfig : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public actualConfig(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }


        public List<CCM_ComponentClientConfig> ComponentClientConfig
        {
            get
            {
                List<CCM_ComponentClientConfig> lCache = new List<CCM_ComponentClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * FROM CCM_ComponentClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_ComponentClientConfig oCIEx = new CCM_ComponentClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SoftwareUpdatesClientConfig> SoftwareUpdatesClientConfig
        {
            get
            {
                List<CCM_SoftwareUpdatesClientConfig> lCache = new List<CCM_SoftwareUpdatesClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * FROM CCM_SoftwareUpdatesClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_SoftwareUpdatesClientConfig oCIEx = new CCM_SoftwareUpdatesClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_RootCACertificates> RootCACertificates
        {
            get
            {
                List<CCM_RootCACertificates> lCache = new List<CCM_RootCACertificates>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * FROM CCM_RootCACertificates");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_RootCACertificates oCIEx = new CCM_RootCACertificates(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SourceUpdateClientConfig> SourceUpdateClientConfig
        {
            get
            {
                List<CCM_SourceUpdateClientConfig> lCache = new List<CCM_SourceUpdateClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * FROM CCM_SourceUpdateClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_SourceUpdateClientConfig oCIEx = new CCM_SourceUpdateClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SoftwareCenterSettings> SoftwareCenterSettings
        {
            get
            {
                List<CCM_SoftwareCenterSettings> lCache = new List<CCM_SoftwareCenterSettings>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_SoftwareCenterSettings");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_SoftwareCenterSettings oCIEx = new CCM_SoftwareCenterSettings(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SoftwareInventoryClientConfig> SoftwareInventoryClientConfig
        {
            get
            {
                List<CCM_SoftwareInventoryClientConfig> lCache = new List<CCM_SoftwareInventoryClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_SoftwareInventoryClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_SoftwareInventoryClientConfig oCIEx = new CCM_SoftwareInventoryClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_TargetingSettings> TargetingSettings
        {
            get
            {
                List<CCM_TargetingSettings> lCache = new List<CCM_TargetingSettings>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_TargetingSettings");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_TargetingSettings oCIEx = new CCM_TargetingSettings(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_MulticastConfig> MulticastConfig
        {
            get
            {
                List<CCM_MulticastConfig> lCache = new List<CCM_MulticastConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_MulticastConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_MulticastConfig oCIEx = new CCM_MulticastConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SoftwareDistributionClientConfig> SoftwareDistributionClientConfig
        {
            get
            {
                List<CCM_SoftwareDistributionClientConfig> lCache = new List<CCM_SoftwareDistributionClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_SoftwareDistributionClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_SoftwareDistributionClientConfig oCIEx = new CCM_SoftwareDistributionClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_ConfigurationManagementClientConfig> ConfigurationManagementClientConfig
        {
            get
            {
                List<CCM_ConfigurationManagementClientConfig> lCache = new List<CCM_ConfigurationManagementClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_ConfigurationManagementClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_ConfigurationManagementClientConfig oCIEx = new CCM_ConfigurationManagementClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_ClientAgentConfig> ClientAgentConfig
        {
            get
            {
                List<CCM_ClientAgentConfig> lCache = new List<CCM_ClientAgentConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_ClientAgentConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_ClientAgentConfig oCIEx = new CCM_ClientAgentConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SystemHealthClientConfig> SystemHealthClientConfig
        {
            get
            {
                List<CCM_SystemHealthClientConfig> lCache = new List<CCM_SystemHealthClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_SystemHealthClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_SystemHealthClientConfig oCIEx = new CCM_SystemHealthClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_PowerManagementClientConfig> PowerManagementClientConfig
        {
            get
            {
                List<CCM_PowerManagementClientConfig> lCache = new List<CCM_PowerManagementClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_PowerManagementClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_PowerManagementClientConfig oCIEx = new CCM_PowerManagementClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_SoftwareMeteringClientConfig> SoftwareMeteringClientConfig
        {
            get
            {
                List<CCM_SoftwareMeteringClientConfig> lCache = new List<CCM_SoftwareMeteringClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_SoftwareMeteringClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_SoftwareMeteringClientConfig oCIEx = new CCM_SoftwareMeteringClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_HardwareInventoryClientConfig> HardwareInventoryClientConfig
        {
            get
            {
                List<CCM_HardwareInventoryClientConfig> lCache = new List<CCM_HardwareInventoryClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_HardwareInventoryClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_HardwareInventoryClientConfig oCIEx = new CCM_HardwareInventoryClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_RemoteTools_Policy> RemoteTools_Policy
        {
            get
            {
                List<CCM_RemoteTools_Policy> lCache = new List<CCM_RemoteTools_Policy>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_RemoteTools_Policy");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_RemoteTools_Policy oCIEx = new CCM_RemoteTools_Policy(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_NetworkAccessAccount> NetworkAccessAccount
        {
            get
            {
                List<CCM_NetworkAccessAccount> lCache = new List<CCM_NetworkAccessAccount>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_NetworkAccessAccount");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_NetworkAccessAccount oCIEx = new CCM_NetworkAccessAccount(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_ApplicationManagementClientConfig> ApplicationManagementClientConfig
        {
            get
            {
                List<CCM_ApplicationManagementClientConfig> lCache = new List<CCM_ApplicationManagementClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_ApplicationManagementClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_ApplicationManagementClientConfig oCIEx = new CCM_ApplicationManagementClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        public List<CCM_OutOfBandManagementClientConfig> OutOfBandManagementClientConfig
        {
            get
            {
                List<CCM_OutOfBandManagementClientConfig> lCache = new List<CCM_OutOfBandManagementClientConfig>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * CCM_OutOfBandManagementClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_OutOfBandManagementClientConfig oCIEx = new CCM_OutOfBandManagementClientConfig(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_Policy
        {
            //Constructor
            public CCM_Policy(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_ComponentClientConfig : CCM_Policy
        {
            //Constructor
            public CCM_ComponentClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                /*this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject; */
                this.ComponentName = WMIObject.Properties["ComponentName"].Value as String;
                this.Enabled = WMIObject.Properties["Enabled"].Value as Boolean?;
            }

            #region Properties

            /*internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode; */
            public String ComponentName { get; set; }
            public Boolean? Enabled { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SoftwareUpdatesClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SoftwareUpdatesClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.AssignmentBatchingTimeout = WMIObject.Properties["AssignmentBatchingTimeout"].Value as UInt32?;
                this.BrandingSubTitle = WMIObject.Properties["BrandingSubTitle"].Value as String;
                this.BrandingTitle = WMIObject.Properties["BrandingTitle"].Value as String;
                this.ContentDownloadTimeout = WMIObject.Properties["ContentDownloadTimeout"].Value as UInt32?;
                this.ContentLocationTimeout = WMIObject.Properties["ContentLocationTimeout"].Value as UInt32?;
                this.DayReminderInterval = WMIObject.Properties["DayReminderInterval"].Value as UInt32?;
                this.HourReminderInterval = WMIObject.Properties["HourReminderInterval"].Value as UInt32?;
                this.MaxScanRetryCount = WMIObject.Properties["MaxScanRetryCount"].Value as UInt32?;
                this.PerDPInactivityTimeout = WMIObject.Properties["PerDPInactivityTimeout"].Value as UInt32?;
                this.ReminderInterval = WMIObject.Properties["ReminderInterval"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.ScanRetryDelay = WMIObject.Properties["ScanRetryDelay"].Value as UInt32?;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
                this.TotalInactivityTimeout = WMIObject.Properties["TotalInactivityTimeout"].Value as UInt32?;
                this.UserJobPerDPInactivityTimeout = WMIObject.Properties["UserJobPerDPInactivityTimeout"].Value as UInt32?;
                this.UserJobTotalInactivityTimeout = WMIObject.Properties["UserJobTotalInactivityTimeout"].Value as UInt32?;
                this.WSUSLocationTimeout = WMIObject.Properties["WSUSLocationTimeout"].Value as UInt32?;
            }

            #region Properties

            public UInt32? AssignmentBatchingTimeout { get; set; }
            public String BrandingSubTitle { get; set; }
            public String BrandingTitle { get; set; }
            public UInt32? ContentDownloadTimeout { get; set; }
            public UInt32? ContentLocationTimeout { get; set; }
            public UInt32? DayReminderInterval { get; set; }
            public UInt32? HourReminderInterval { get; set; }
            public UInt32? MaxScanRetryCount { get; set; }
            public UInt32? PerDPInactivityTimeout { get; set; }
            public UInt32? ReminderInterval { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? ScanRetryDelay { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            public UInt32? TotalInactivityTimeout { get; set; }
            public UInt32? UserJobPerDPInactivityTimeout { get; set; }
            public UInt32? UserJobTotalInactivityTimeout { get; set; }
            public UInt32? WSUSLocationTimeout { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_RootCACertificates : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_RootCACertificates(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.RootCACerts = WMIObject.Properties["RootCACerts"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public String Reserved1 { get; set; }
            public String RootCACerts { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SourceUpdateClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SourceUpdateClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.LocationTimeOut = WMIObject.Properties["LocationTimeOut"].Value as UInt32?;
                this.MaxRetryCount = WMIObject.Properties["MaxRetryCount"].Value as UInt32?;
                this.NetworkChangeDelay = WMIObject.Properties["NetworkChangeDelay"].Value as UInt32?;
                this.RemoteDPs = WMIObject.Properties["RemoteDPs"].Value as Boolean?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.RetryTimeOut = WMIObject.Properties["RetryTimeOut"].Value as UInt32?;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? LocationTimeOut { get; set; }
            public UInt32? MaxRetryCount { get; set; }
            public UInt32? NetworkChangeDelay { get; set; }
            public Boolean? RemoteDPs { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? RetryTimeOut { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SoftwareCenterSettings : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SoftwareCenterSettings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.AutoInstallRequiredSoftware = WMIObject.Properties["AutoInstallRequiredSoftware"].Value as Boolean?;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
                this.SuppressComputerActivityInPresentationMode = WMIObject.Properties["SuppressComputerActivityInPresentationMode"].Value as Boolean?;
            }

            #region Properties

            public Boolean? AutoInstallRequiredSoftware { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            public Boolean? SuppressComputerActivityInPresentationMode { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SoftwareInventoryClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SoftwareInventoryClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_TargetingSettings : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_TargetingSettings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.AllowUserAffinity = WMIObject.Properties["AllowUserAffinity"].Value as UInt32?;
                this.AllowUserAffinityAfterMinutes = WMIObject.Properties["AllowUserAffinityAfterMinutes"].Value as UInt32?;
                this.AutoApproveAffinity = WMIObject.Properties["AutoApproveAffinity"].Value as UInt32?;
                this.ConsoleMinutes = WMIObject.Properties["ConsoleMinutes"].Value as UInt32?;
                this.IntervalDays = WMIObject.Properties["IntervalDays"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? AllowUserAffinity { get; set; }
            public UInt32? AllowUserAffinityAfterMinutes { get; set; }
            public UInt32? AutoApproveAffinity { get; set; }
            public UInt32? ConsoleMinutes { get; set; }
            public UInt32? IntervalDays { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_MulticastConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_MulticastConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.RetryCount = WMIObject.Properties["RetryCount"].Value as UInt32?;
                this.RetryDelay = WMIObject.Properties["RetryDelay"].Value as UInt32?;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? RetryCount { get; set; }
            public UInt32? RetryDelay { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SoftwareDistributionClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SoftwareDistributionClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.ADV_RebootLogoffNotification = WMIObject.Properties["ADV_RebootLogoffNotification"].Value as Boolean?;
                this.ADV_RebootLogoffNotificationCountdownDuration = WMIObject.Properties["ADV_RebootLogoffNotificationCountdownDuration"].Value as UInt32?;
                this.ADV_RebootLogoffNotificationFinalWindow = WMIObject.Properties["ADV_RebootLogoffNotificationFinalWindow"].Value as UInt32?;
                this.ADV_RunNotificationCountdownDuration = WMIObject.Properties["ADV_RunNotificationCountdownDuration"].Value as UInt32?;
                this.ADV_WhatsNewDuration = WMIObject.Properties["ADV_WhatsNewDuration"].Value as UInt32?;
                this.CacheContentTimeout = WMIObject.Properties["CacheContentTimeout"].Value as UInt32?;
                this.CacheSpaceFailureRetryCount = WMIObject.Properties["CacheSpaceFailureRetryCount"].Value as UInt32?;
                this.CacheSpaceFailureRetryInterval = WMIObject.Properties["CacheSpaceFailureRetryInterval"].Value as UInt32?;
                this.CacheTombstoneContentMinDuration = WMIObject.Properties["CacheTombstoneContentMinDuration"].Value as UInt32?;
                this.ContentLocationTimeoutInterval = WMIObject.Properties["ContentLocationTimeoutInterval"].Value as UInt32?;
                this.ContentLocationTimeoutRetryCount = WMIObject.Properties["ContentLocationTimeoutRetryCount"].Value as UInt32?;
                this.DefaultMaxDuration = WMIObject.Properties["DefaultMaxDuration"].Value as UInt32?;
                this.DisplayNewProgramNotification = WMIObject.Properties["DisplayNewProgramNotification"].Value as Boolean?;
                this.ExecutionFailureRetryCount = WMIObject.Properties["ExecutionFailureRetryCount"].Value as UInt32?;
                this.ExecutionFailureRetryErrorCodes = WMIObject.Properties["ExecutionFailureRetryErrorCodes"].Value as UInt32?[];
                this.ExecutionFailureRetryInterval = WMIObject.Properties["ExecutionFailureRetryInterval"].Value as UInt32?;
                this.LockSettings = WMIObject.Properties["LockSettings"].Value as Boolean?;
                this.LogoffReturnCodes = WMIObject.Properties["LogoffReturnCodes"].Value as UInt32?[];
                this.NetworkAccessPassword = WMIObject.Properties["NetworkAccessPassword"].Value as String;
                this.NetworkAccessUsername = WMIObject.Properties["NetworkAccessUsername"].Value as String;
                this.NetworkFailureRetryCount = WMIObject.Properties["NetworkFailureRetryCount"].Value as UInt32?;
                this.NetworkFailureRetryInterval = WMIObject.Properties["NetworkFailureRetryInterval"].Value as UInt32?;
                this.NewProgramNotificationUI = WMIObject.Properties["NewProgramNotificationUI"].Value as String;
                this.PRG_PRF_RunNotification = WMIObject.Properties["PRG_PRF_RunNotification"].Value as Boolean?;
                this.RebootReturnCodes = WMIObject.Properties["RebootReturnCodes"].Value as UInt32?[];
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
                this.SuccessReturnCodes = WMIObject.Properties["SuccessReturnCodes"].Value as UInt32?[];
                this.UIContentLocationTimeoutInterval = WMIObject.Properties["UIContentLocationTimeoutInterval"].Value as UInt32?;
                this.UserPreemptionCountdown = WMIObject.Properties["UserPreemptionCountdown"].Value as UInt32?;
                this.UserPreemptionTimeout = WMIObject.Properties["UserPreemptionTimeout"].Value as UInt32?;
            }

            #region Properties

            public Boolean? ADV_RebootLogoffNotification { get; set; }
            public UInt32? ADV_RebootLogoffNotificationCountdownDuration { get; set; }
            public UInt32? ADV_RebootLogoffNotificationFinalWindow { get; set; }
            public UInt32? ADV_RunNotificationCountdownDuration { get; set; }
            public UInt32? ADV_WhatsNewDuration { get; set; }
            public UInt32? CacheContentTimeout { get; set; }
            public UInt32? CacheSpaceFailureRetryCount { get; set; }
            public UInt32? CacheSpaceFailureRetryInterval { get; set; }
            public UInt32? CacheTombstoneContentMinDuration { get; set; }
            public UInt32? ContentLocationTimeoutInterval { get; set; }
            public UInt32? ContentLocationTimeoutRetryCount { get; set; }
            public UInt32? DefaultMaxDuration { get; set; }
            public Boolean? DisplayNewProgramNotification { get; set; }
            public UInt32? ExecutionFailureRetryCount { get; set; }
            public UInt32?[] ExecutionFailureRetryErrorCodes { get; set; }
            public UInt32? ExecutionFailureRetryInterval { get; set; }
            public Boolean? LockSettings { get; set; }
            public UInt32?[] LogoffReturnCodes { get; set; }
            public String NetworkAccessPassword { get; set; }
            public String NetworkAccessUsername { get; set; }
            public UInt32? NetworkFailureRetryCount { get; set; }
            public UInt32? NetworkFailureRetryInterval { get; set; }
            public String NewProgramNotificationUI { get; set; }
            public Boolean? PRG_PRF_RunNotification { get; set; }
            public UInt32?[] RebootReturnCodes { get; set; }
            public String Reserved { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            public UInt32?[] SuccessReturnCodes { get; set; }
            public UInt32? UIContentLocationTimeoutInterval { get; set; }
            public UInt32? UserPreemptionCountdown { get; set; }
            public UInt32? UserPreemptionTimeout { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_ConfigurationManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_ConfigurationManagementClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.PerProviderTimeOut = WMIObject.Properties["PerProviderTimeOut"].Value as UInt32?;
                this.PerScanTimeout = WMIObject.Properties["PerScanTimeout"].Value as UInt32?;
                this.PerScanTTL = WMIObject.Properties["PerScanTTL"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? PerProviderTimeOut { get; set; }
            public UInt32? PerScanTimeout { get; set; }
            public UInt32? PerScanTTL { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_ClientAgentConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_ClientAgentConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.BrandingTitle = WMIObject.Properties["BrandingTitle"].Value as String;
                this.DayReminderInterval = WMIObject.Properties["DayReminderInterval"].Value as UInt32?;
                this.DisplayNewProgramNotification = WMIObject.Properties["DisplayNewProgramNotification"].Value as Boolean?;
                this.EnableThirdPartyOrchestration = WMIObject.Properties["EnableThirdPartyOrchestration"].Value as UInt32?;
                this.HourReminderInterval = WMIObject.Properties["HourReminderInterval"].Value as UInt32?;
                this.InstallRestriction = WMIObject.Properties["InstallRestriction"].Value as UInt32?;
                this.OSDBrandingSubTitle = WMIObject.Properties["OSDBrandingSubTitle"].Value as String;
                this.PowerShellExecutionPolicy = WMIObject.Properties["PowerShellExecutionPolicy"].Value as UInt32?;
                this.ReminderInterval = WMIObject.Properties["ReminderInterval"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
                this.SUMBrandingSubTitle = WMIObject.Properties["SUMBrandingSubTitle"].Value as String;
                this.SuspendBitLocker = WMIObject.Properties["SuspendBitLocker"].Value as UInt32?;
                this.SWDBrandingSubTitle = WMIObject.Properties["SWDBrandingSubTitle"].Value as String;
                this.SystemRestartTurnaroundTime = WMIObject.Properties["SystemRestartTurnaroundTime"].Value as UInt32?;
            }

            #region Properties

            public String BrandingTitle { get; set; }
            public UInt32? DayReminderInterval { get; set; }
            public Boolean? DisplayNewProgramNotification { get; set; }
            public UInt32? EnableThirdPartyOrchestration { get; set; }
            public UInt32? HourReminderInterval { get; set; }
            public UInt32? InstallRestriction { get; set; }
            public String OSDBrandingSubTitle { get; set; }
            public UInt32? PowerShellExecutionPolicy { get; set; }
            public UInt32? ReminderInterval { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            public String SUMBrandingSubTitle { get; set; }
            public UInt32? SuspendBitLocker { get; set; }
            public String SWDBrandingSubTitle { get; set; }
            public UInt32? SystemRestartTurnaroundTime { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SystemHealthClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SystemHealthClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.CumulativeDownloadTimeout = WMIObject.Properties["CumulativeDownloadTimeout"].Value as UInt32?;
                this.CumulativeInactivityTimeout = WMIObject.Properties["CumulativeInactivityTimeout"].Value as UInt32?;
                this.DPLocality = WMIObject.Properties["DPLocality"].Value as UInt32?;
                this.EffectiveTimeinUTC = WMIObject.Properties["EffectiveTimeinUTC"].Value as UInt32?;
                this.ForceScan = WMIObject.Properties["ForceScan"].Value as Boolean?;
                this.LocationsTimeout = WMIObject.Properties["LocationsTimeout"].Value as UInt32?;
                this.PerDPInactivityTimeout = WMIObject.Properties["PerDPInactivityTimeout"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.Reserved4 = WMIObject.Properties["Reserved4"].Value as UInt32?;
                this.SiteCode = WMIObject.Properties["SiteCode"].Value as String;
                this.SiteID = WMIObject.Properties["SiteID"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? CumulativeDownloadTimeout { get; set; }
            public UInt32? CumulativeInactivityTimeout { get; set; }
            public UInt32? DPLocality { get; set; }
            public UInt32? EffectiveTimeinUTC { get; set; }
            public Boolean? ForceScan { get; set; }
            public UInt32? LocationsTimeout { get; set; }
            public UInt32? PerDPInactivityTimeout { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? Reserved4 { get; set; }
            public String SiteCode { get; set; }
            public String SiteID { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_PowerManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_PowerManagementClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.AllowUserToOptOutFromPowerPlan = WMIObject.Properties["AllowUserToOptOutFromPowerPlan"].Value as Boolean?;
                this.EnableUserIdleMonitoring = WMIObject.Properties["EnableUserIdleMonitoring"].Value as Boolean?;
                this.NumberOfDaysToKeep = WMIObject.Properties["NumberOfDaysToKeep"].Value as UInt32?;
                this.NumberOfMonthsToKeep = WMIObject.Properties["NumberOfMonthsToKeep"].Value as UInt32?;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public Boolean? AllowUserToOptOutFromPowerPlan { get; set; }
            public Boolean? EnableUserIdleMonitoring { get; set; }
            public UInt32? NumberOfDaysToKeep { get; set; }
            public UInt32? NumberOfMonthsToKeep { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SoftwareMeteringClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_SoftwareMeteringClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.MaximumUsageInstancesPerReport = WMIObject.Properties["MaximumUsageInstancesPerReport"].Value as UInt32?;
                this.ReportTimeout = WMIObject.Properties["ReportTimeout"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? MaximumUsageInstancesPerReport { get; set; }
            public UInt32? ReportTimeout { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_HardwareInventoryClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_HardwareInventoryClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;

                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_RemoteTools_Policy : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_RemoteTools_Policy(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

            }

            #region Properties

            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_NetworkAccessAccount : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_NetworkAccessAccount(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.NetworkAccessPassword = WMIObject.Properties["NetworkAccessPassword"].Value as String;
                this.NetworkAccessUsername = WMIObject.Properties["NetworkAccessUsername"].Value as String;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public String NetworkAccessPassword { get; set; }
            public String NetworkAccessUsername { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_ApplicationManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_ApplicationManagementClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.ContentDownloadTimeOut = WMIObject.Properties["ContentDownloadTimeOut"].Value as UInt32?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? ContentDownloadTimeOut { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_OutOfBandManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            public CCM_OutOfBandManagementClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties

            public UInt32? SiteSettingsKey { get; set; }
            #endregion

        }
    }


}
