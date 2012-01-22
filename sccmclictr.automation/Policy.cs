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

namespace sccmclictr.automation.functions
{
    /// <summary>
    /// SCCM Requested Policy
    /// </summary>
    public class requestedPolicy : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public requestedPolicy(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
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
    }


}
