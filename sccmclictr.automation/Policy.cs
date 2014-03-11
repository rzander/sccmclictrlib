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
using System.IO;
using System.Xml;

namespace sccmclictr.automation.policy
{
    /// <summary>
    /// SCCM Requested Policy.
    /// </summary>
    public class requestedConfig : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="requestedConfig"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        /// <param name="oClient">A CCM Client object.</param>
        public requestedConfig(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Create a new ServiceWindows
        /// </summary>
        /// <param name="Schedules">ScheduleID</param>
        /// <param name="ServiceWindowType">1 = All Programs Service Window, 2 = Program Service Window, 3 = Reboot Required Service Window, 4 = Software Update Service Window, 5 = OSD Service Window, 6 = Corresponds to non-working hours.</param>
        /// <returns>Service Window ID (GUID)</returns>
        public string CreateServiceWindow(string Schedules, UInt32 ServiceWindowType)
        {
            string sPolicyID = System.Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");
            string sPSCommand = "$a = Set-WmiInstance -Class CCM_ServiceWindow -Namespace 'ROOT\\ccm\\Policy\\Machine\\RequestedConfig' -PutType 'CreateOnly' -argument @{PolicySource = 'LOCAL'; PolicyRuleID = 'NONE'; PolicyVersion = '1.0'; Schedules = '" + Schedules + "'; ServiceWindowType = " + ServiceWindowType.ToString() + "; ServiceWindowID = '" + sPolicyID + "'; PolicyID = '" + sPolicyID + "'; PolicyInstanceID = '"+sPolicyID+"'};$a.ServiceWindowID";
            List<PSObject> oResults = baseClient.GetObjectsFromPS(sPSCommand);
            foreach (PSObject oRes in oResults)
            {
                if (oRes != null)
                    if (oRes.ToString().Length == 36)
                        return oRes.ToString();
            }

            return null;
        }

        /// <summary>
        /// Deletes a service window.
        /// </summary>
        /// <param name="ServiceWindowID">A service window identifier.</param>
        public void DeleteServiceWindow(string ServiceWindowID)
        {
            string sResult = baseClient.GetStringFromPS("Get-WMIObject -Namespace 'ROOT\\ccm\\Policy\\Machine\\RequestedConfig' -Query 'SELECT * FROM CCM_ServiceWindow WHERE ServiceWindowID = \"" + ServiceWindowID + "\"' | Remove-WmiObject");
        }

        /// <summary>
        /// Gets a list of component client configuration.
        /// </summary>
        /// <value>A list of component client configuration.</value>
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

        /// <summary>
        /// Gets a list of software updates client configuration.
        /// </summary>
        /// <value>A list of software updates client configuration.</value>
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

        /// <summary>
        /// Gets a list of root ca certificates.
        /// </summary>
        /// <value>A list of root ca certificates.</value>
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
        /// Gets a list of source update client configuration.
        /// </summary>
        /// <value>A list of source update client configuration.</value>
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

        /// <summary>
        /// Gets a list of software center settings.
        /// </summary>
        /// <value>A list of software center settings.</value>
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

        /// <summary>
        /// Gets a list of software inventory client configuration.
        /// </summary>
        /// <value>A list of software inventory client configuration.</value>
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

        /// <summary>
        /// Gets a list of targeting settings.
        /// </summary>
        /// <value>A list of targeting settings.</value>
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

        /// <summary>
        /// Gets a list of multicast configuration.
        /// </summary>
        /// <value>A list of multicast configuration.</value>
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

        /// <summary>
        /// Gets a list of software distribution client configuration.
        /// </summary>
        /// <value>A list of software distribution client configuration.</value>
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

        /// <summary>
        /// Gets a list of configuration management client configuration.
        /// </summary>
        /// <value>A list of configuration management client configuration.</value>
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

        /// <summary>
        /// Gets a list of client agent configuration.
        /// </summary>
        /// <value>A list of client agent configuration.</value>
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

        /// <summary>
        /// Gets a list of system health client configuration.
        /// </summary>
        /// <value>A list of system health client configuration.</value>
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

        /// <summary>
        /// Gets a list of power management client configuration.
        /// </summary>
        /// <value>A list of power management client configuration.</value>
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

        /// <summary>
        /// Gets a list of software metering client configuration.
        /// </summary>
        /// <value>A list of software metering client configuration.</value>
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

        /// <summary>
        /// Gets a list of hardware inventory client configuration.
        /// </summary>
        /// <value>A list of hardware inventory client configuration.</value>
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

        /// <summary>
        /// Gets A list of remote tools policies.
        /// </summary>
        /// <value>A list of remote tools policies.</value>
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

        /// <summary>
        /// Gets a list of network access accounts.
        /// </summary>
        /// <value>A list of network access accounts.</value>
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

        /// <summary>
        /// Gets a list of application management client configurations.
        /// </summary>
        /// <value>A list of application management client configurations.</value>
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

        /// <summary>
        /// List of CCM_OutOfBandManagementClientConfig objects
        /// </summary>
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
        /// List of CCM_ServiceWindow Objects
        /// </summary>
        public List<CCM_ServiceWindow> ServiceWindow
        {
            get 
            {
                List<CCM_ServiceWindow> lCache = new List<CCM_ServiceWindow>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\RequestedConfig", "SELECT * FROM CCM_ServiceWindow", true);
                foreach (PSObject PSObj in oObj)
                {
                    CCM_ServiceWindow oSW= new CCM_ServiceWindow(PSObj, remoteRunspace, pSCode, baseClient);

                    oSW.remoteRunspace = remoteRunspace;
                    oSW.pSCode = pSCode;
                    lCache.Add(oSW);
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
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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



            /// <summary>
            /// Gets or sets the policy identifier.
            /// </summary>
            /// <value>Unique ID of the policy.</value>
            public String PolicyID { get; set; }

            /// <summary>
            /// Gets or sets the policy instance identifier.
            /// </summary>
            /// <value>The policy instance identifier.</value>
            public String PolicyInstanceID { get; set; }

            /// <summary>
            /// Gets or sets the policy precedence.
            /// </summary>
            /// <value>The policy precedence.</value>
            public UInt32? PolicyPrecedence { get; set; }

            /// <summary>
            /// Gets or sets the policy rule identifier.
            /// </summary>
            /// <value>Unique ID of the rule used to create the policy.</value>
            public String PolicyRuleID { get; set; }

            /// <summary>
            /// Gets or sets the policy source.
            /// </summary>
            /// <value>The policy source.</value>
            public String PolicySource { get; set; }

            /// <summary>
            /// Gets or sets the policy version.
            /// </summary>
            /// <value>The policy version.</value>
            public String PolicyVersion { get; set; }


            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_ComponentClientConfig : CCM_Policy
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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


            /// <summary>
            /// Gets or sets the name of the component.
            /// </summary>
            /// <value>The name of the client component.</value>
            public String ComponentName { get; set; }

            /// <summary>
            /// Gets or sets the enabled.
            /// </summary>
            /// <value>Shows weather the client component is enabled or not</value>
            public Boolean? Enabled { get; set; }


            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SoftwareUpdatesClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

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

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_RootCACertificates : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public String Reserved1 { get; set; }
            public String RootCACerts { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SourceUpdateClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? LocationTimeOut { get; set; }
            public UInt32? MaxRetryCount { get; set; }
            public UInt32? NetworkChangeDelay { get; set; }
            public Boolean? RemoteDPs { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? RetryTimeOut { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SoftwareCenterSettings : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public Boolean? AutoInstallRequiredSoftware { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            public Boolean? SuppressComputerActivityInPresentationMode { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SoftwareInventoryClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_TargetingSettings : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? AllowUserAffinity { get; set; }
            public UInt32? AllowUserAffinityAfterMinutes { get; set; }
            public UInt32? AutoApproveAffinity { get; set; }
            public UInt32? ConsoleMinutes { get; set; }
            public UInt32? IntervalDays { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_MulticastConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? RetryCount { get; set; }
            public UInt32? RetryDelay { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SoftwareDistributionClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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

            /// <summary>
            /// Gets or sets the advert reboot/logoff notification flag.
            /// </summary>
            /// <value>If TRUE a notification countdown timer will be shown telling user they will be logged off or the computer will be rebooted.</value>
            public Boolean? ADV_RebootLogoffNotification { get; set; }

            /// <summary>
            /// Gets or sets the duration of the advert reboot/logoff notification countdown.
            /// </summary>
            /// <value>The duration in seconds of the reboot/logoff notification countdown.</value>
            public UInt32? ADV_RebootLogoffNotificationCountdownDuration { get; set; }

            /// <summary>
            /// Gets or sets the advert reboot/logoff notification final window.
            /// </summary>
            /// <value>The duration in seconds of the final reboot/logoff notification countdown.</value>
            public UInt32? ADV_RebootLogoffNotificationFinalWindow { get; set; }

            /// <summary>
            /// Gets or sets the duration of the advert run notification countdown.
            /// </summary>
            /// <value>The duration in seconds of the advert run notification countdown.</value>
            public UInt32? ADV_RunNotificationCountdownDuration { get; set; }

            /// <summary>
            /// Gets or sets the duration that an advert appears in the whats new section.
            /// </summary>
            /// <value>The duration in days that a advert appears in the Whats new section.</value>
            public UInt32? ADV_WhatsNewDuration { get; set; }

            /// <summary>
            /// Gets or sets the cache content timeout.
            /// </summary>
            /// <value>The cache content timeout in seconds. After this period content can be deleted.</value>
            public UInt32? CacheContentTimeout { get; set; }

            /// <summary>
            /// Gets or sets the cache space failure retry count.
            /// </summary>
            /// <value>The number of times to retry caching content if it fails due to space restrictions.</value>
            public UInt32? CacheSpaceFailureRetryCount { get; set; }

            /// <summary>
            /// Gets or sets the cache space failure retry interval.
            /// </summary>
            /// <value>The time in seconds to wait before retrying to cache content that previous failed.</value>
            public UInt32? CacheSpaceFailureRetryInterval { get; set; }

            /// <summary>
            /// Gets or sets the minimum duration the content should be available in the cache.
            /// </summary>
            /// <value>The minimum duration in seconds that the content should be available in the cache.</value>
            public UInt32? CacheTombstoneContentMinDuration { get; set; }

            /// <summary>
            /// Gets or sets the content location timeout interval.
            /// </summary>
            /// <value>The maximum time in seconds that the client should search for content locations.</value>
            public UInt32? ContentLocationTimeoutInterval { get; set; }

            /// <summary>
            /// Gets or sets the content location timeout retry count.
            /// </summary>
            /// <value>The maximum number of times that the client should search for content locations.</value>
            public UInt32? ContentLocationTimeoutRetryCount { get; set; }

            /// <summary>
            /// Gets or sets the default duration of the program.
            /// </summary>
            /// <value>The default duration that the program can run for.</value>
            public UInt32? DefaultMaxDuration { get; set; }

            /// <summary>
            /// Gets or sets the display new program notification.
            /// </summary>
            /// <value>If TRUE notify the user that a new program is available.</value>
            public Boolean? DisplayNewProgramNotification { get; set; }

            /// <summary>
            /// Gets or sets the execution failure retry count.
            /// </summary>
            /// <value>The number of times to retry running the program if it previously failed.</value>
            public UInt32? ExecutionFailureRetryCount { get; set; }

            /// <summary>
            /// Gets or sets the execution failure retry error codes.
            /// </summary>
            /// <value>The error codes that will trigger a retry. refer to: http://msdn.microsoft.com/en-us/library/cc143632.aspx for the list of error codes</value>
            public UInt32?[] ExecutionFailureRetryErrorCodes { get; set; }

            /// <summary>
            /// Gets or sets the execution failure retry interval.
            /// </summary>
            /// <value>The execution failure retry interval.</value>
            public UInt32? ExecutionFailureRetryInterval { get; set; }

            /// <summary>
            /// Gets or sets the lock settings.
            /// </summary>
            /// <value>If TRUE site settings cannot be overridden.</value>
            public Boolean? LockSettings { get; set; }

            /// <summary>
            /// Gets or sets the logoff return codes.
            /// </summary>
            /// <value>Array of return codes that if returned by a program signals that a logoff is required.</value>
            public UInt32?[] LogoffReturnCodes { get; set; }

            /// <summary>
            /// Gets or sets the network access account password.
            /// </summary>
            /// <value>The network access account password.</value>
            public String NetworkAccessPassword { get; set; }

            /// <summary>
            /// Gets or sets the network access account username.
            /// </summary>
            /// <value>The network access account username.</value>
            public String NetworkAccessUsername { get; set; }

            /// <summary>
            /// Gets or sets the network failure retry count.
            /// </summary>
            /// <value>The number of times to try connecting to the network.</value>
            public UInt32? NetworkFailureRetryCount { get; set; }

            /// <summary>
            /// Gets or sets the network failure retry interval.
            /// </summary>
            /// <value>The time in seconds between trying to connect to the network.</value>
            public UInt32? NetworkFailureRetryInterval { get; set; }

            /// <summary>
            /// Gets or sets the new program notification UI interface.
            /// </summary>
            /// <value>The new program notification UI interface. Either ARP or RAP. This is the interface that is displayed when the notification ballon is clicked.</value>
            public String NewProgramNotificationUI { get; set; }

            /// <summary>
            /// Gets or sets the program run notification.
            /// </summary>
            /// <value>If TRUE signifys that the client will display a countdown notification.</value>
            public Boolean? PRG_PRF_RunNotification { get; set; }

            /// <summary>
            /// Gets or sets the reboot return codes.
            /// </summary>
            /// <value>An array of return codes that if a program returns signifies that a reboot is required.</value>
            public UInt32?[] RebootReturnCodes { get; set; }

            /// <summary>
            /// Gets or sets reserved.
            /// </summary>
            /// <value>Reserved.</value>
            public String Reserved { get; set; }

            /// <summary>
            /// Gets or sets reserved1.
            /// </summary>
            /// <value>Reserved1.</value>
            public String Reserved1 { get; set; }

            /// <summary>
            /// Gets or sets reserved2.
            /// </summary>
            /// <value>Reserved2.</value>
            public String Reserved2 { get; set; }

            /// <summary>
            /// Gets or sets reserved3.
            /// </summary>
            /// <value>Reserved3.</value>
            public String Reserved3 { get; set; }

            /// <summary>
            /// Gets or sets the site settings key.
            /// </summary>
            /// <value>The site settings key.</value>
            public UInt32? SiteSettingsKey { get; set; }

            /// <summary>
            /// Gets or sets the success return codes.
            /// </summary>
            /// <value>An array of return codes that if a program returns signifies that a the program installed successfully.</value>
            public UInt32?[] SuccessReturnCodes { get; set; }

            /// <summary>
            /// Gets or sets the UI content location timeout interval.
            /// </summary>
            /// <value>The UI content location timeout interval.</value>
            public UInt32? UIContentLocationTimeoutInterval { get; set; }

            /// <summary>
            /// Gets or sets the user preemption countdown.
            /// </summary>
            /// <value>The user preemption countdown.</value>
            public UInt32? UserPreemptionCountdown { get; set; }

            /// <summary>
            /// Gets or sets the user preemption timeout.
            /// </summary>
            /// <value>The user preemption timeout.</value>
            public UInt32? UserPreemptionTimeout { get; set; }

            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_ConfigurationManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? PerProviderTimeOut { get; set; }
            public UInt32? PerScanTimeout { get; set; }
            public UInt32? PerScanTTL { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_ClientAgentConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

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

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SystemHealthClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

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

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_PowerManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public Boolean? AllowUserToOptOutFromPowerPlan { get; set; }
            public Boolean? EnableUserIdleMonitoring { get; set; }
            public UInt32? NumberOfDaysToKeep { get; set; }
            public UInt32? NumberOfMonthsToKeep { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_SoftwareMeteringClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? MaximumUsageInstancesPerReport { get; set; }
            public UInt32? ReportTimeout { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_HardwareInventoryClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_RemoteTools_Policy : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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

            /// <summary>
            /// Gets or sets the network access account password.
            /// </summary>
            /// <value>The network access account password.</value>
            public String NetworkAccessPassword { get; set; }

            /// <summary>
            /// Gets or sets the network access account username.
            /// </summary>
            /// <value>The network access account username.</value>
            public String NetworkAccessUsername { get; set; }

            /// <summary>
            /// Gets or sets reserved1.
            /// </summary>
            /// <value>Reserved1.</value>
            public String Reserved1 { get; set; }

            /// <summary>
            /// Gets or sets reserved2.
            /// </summary>
            /// <value>Reserved2.</value>
            public String Reserved2 { get; set; }

            /// <summary>
            /// Gets or sets reserved3.
            /// </summary>
            /// <value>Reserved3.</value>
            public String Reserved3 { get; set; }

            /// <summary>
            /// Gets or sets the site settings key.
            /// </summary>
            /// <value>The site settings key.</value>
            public UInt32? SiteSettingsKey { get; set; }

            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_ApplicationManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? ContentDownloadTimeOut { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_OutOfBandManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
            public CCM_OutOfBandManagementClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\RequestedConfig
        /// </summary>
        public class CCM_ServiceWindow : CCM_Policy
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
            public CCM_ServiceWindow(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.Schedules = WMIObject.Properties["Schedules"].Value as String;
                this.ServiceWindowID = WMIObject.Properties["ServiceWindowID"].Value as String;
                this.ServiceWindowType = WMIObject.Properties["ServiceWindowType"].Value as UInt32?;
            }
            
            #region Properties
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public String Schedules { get; set; }
            public String ServiceWindowID { get; set; }
            public UInt32? ServiceWindowType { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments

            /// <summary>
            /// Decode ScheduleID to Object of type: SMS_ST_NonRecurring, SMS_ST_RecurInterval, SMS_ST_RecurWeekly, SMS_ST_RecurMonthlyByWeekday or SMS_ST_RecurMonthlyByDate
            /// </summary>
            public object DecodedSchedule
            {
                get 
                {
                    string sSchedule = Schedules;
                    string[] aSchedules = schedule.ScheduleDecoding.GetScheduleIDs(Schedules);
                    if (aSchedules.Length <=1)
                    {
                        if(aSchedules.Length == 1)
                            return schedule.ScheduleDecoding.DecodeScheduleID(aSchedules[0]);
                        else
                            return null;
                    }
                    else
                    {
                        List<object> oResult = new List<object>();
                        foreach (string s in aSchedules)
                        {
                            oResult.Add(schedule.ScheduleDecoding.DecodeScheduleID(s));
                        }

                        return oResult;
                    }
                }
            }

            #endregion

        }
    }

    /// <summary>
    /// Class actualConfig.
    /// </summary>
    public class actualConfig : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="actualConfig"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        /// <param name="oClient">A CCM Client object.</param>
        public actualConfig(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Creates the service window.
        /// </summary>
        /// <param name="Schedules">The schedules.</param>
        /// <param name="ServiceWindowType">Type of the service window.</param>
        /// <returns>System.String.</returns>
        public string CreateServiceWindow(string Schedules, UInt32 ServiceWindowType)
        {
            List<PSObject> oResults = baseClient.GetObjectsFromPS(string.Format("$a = Set-WmiInstance -Class CCM_ServiceWindow -Namespace 'ROOT\\ccm\\Policy\\Machine\\ActualConfig' -PutType 'CreateOnly';$a.ServiceWindowType = {0};$a.Schedules = '{1}';$a.Put() | Out-Null;$a.ServiceWindowID", ServiceWindowType.ToString(), Schedules));
            foreach (PSObject oRes in oResults)
            {
                if (oRes != null)
                    if(oRes.ToString().Length == 38)
                        return oRes.ToString();
            }

            return null;
        }


        /// <summary>
        /// Gets a list of component client configurations.
        /// </summary>
        /// <value>A list of component client configurations.</value>
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

        /// <summary>
        /// Gets a list of software updates client configurations.
        /// </summary>
        /// <value>A list of software updates client configuration.</value>
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

        /// <summary>
        /// Gets a list of root ca certificates.
        /// </summary>
        /// <value>A list of root ca certificates.</value>
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

        /// <summary>
        /// Gets a list of source update client configuration.
        /// </summary>
        /// <value>A list of source update client configuration.</value>
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

        /// <summary>
        /// Gets a list of software center settings.
        /// </summary>
        /// <value>A list of software center settings.</value>
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

        /// <summary>
        /// Gets A list of software inventory client configurations.
        /// </summary>
        /// <value>A list of software inventory client configurations.</value>
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

        /// <summary>
        /// Gets a list of targeting settings.
        /// </summary>
        /// <value>A list of targeting settings.</value>
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

        /// <summary>
        /// Gets a list of multicast configurations.
        /// </summary>
        /// <value>A list of multicast configurations.</value>
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

        /// <summary>
        /// Gets a list of software distribution client configuration.
        /// </summary>
        /// <value>A list of software distribution client configuration.</value>
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

        /// <summary>
        /// Gets a list of configuration management client configurations.
        /// </summary>
        /// <value>A list of configuration management client configurations.</value>
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

        /// <summary>
        /// Gets a list of client agent configurations.
        /// </summary>
        /// <value>A list of client agent configurations.</value>
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

        /// <summary>
        /// Gets a list of system health client configurations.
        /// </summary>
        /// <value>A list of system health client configurations.</value>
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

        /// <summary>
        /// Gets a list of power management client configurations.
        /// </summary>
        /// <value>A list of power management client configurations.</value>
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

        /// <summary>
        /// Gets a list of software metering client configurations.
        /// </summary>
        /// <value>A list of software metering client configurations.</value>
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

        /// <summary>
        /// Gets a list of hardware inventory client configurations.
        /// </summary>
        /// <value>A list of hardware inventory client configurations.</value>
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

        /// <summary>
        /// Gets a list of remote tools policies.
        /// </summary>
        /// <value>A list of remote tools policies.</value>
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

        /// <summary>
        /// Gets a list of network access accounts.
        /// </summary>
        /// <value>A list of network access accounts.</value>
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

        /// <summary>
        /// Gets a list of application management client configurations.
        /// </summary>
        /// <value>A list of application management client configurations.</value>
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

        /// <summary>
        /// Gets a list of out of band management client configurations.
        /// </summary>
        /// <value>A list of out of band management client configurations.</value>
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
        /// Gets a list of service windows.
        /// </summary>
        /// <value>A list of service windows.</value>
        public List<CCM_ServiceWindow> ServiceWindow
        {
            get
            {
                List<CCM_ServiceWindow> lCache = new List<CCM_ServiceWindow>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * FROM CCM_ServiceWindow", true);
                foreach (PSObject PSObj in oObj)
                {
                    CCM_ServiceWindow oSW = new CCM_ServiceWindow(PSObj, remoteRunspace, pSCode, baseClient);

                    oSW.remoteRunspace = remoteRunspace;
                    oSW.pSCode = pSCode;
                    lCache.Add(oSW);
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
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            /*internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode; */
            public String ComponentName { get; set; }
            public Boolean? Enabled { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SoftwareUpdatesClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

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

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_RootCACertificates : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public String Reserved1 { get; set; }
            public String RootCACerts { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SourceUpdateClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? LocationTimeOut { get; set; }
            public UInt32? MaxRetryCount { get; set; }
            public UInt32? NetworkChangeDelay { get; set; }
            public Boolean? RemoteDPs { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? RetryTimeOut { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SoftwareCenterSettings : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public Boolean? AutoInstallRequiredSoftware { get; set; }
            public UInt32? SiteSettingsKey { get; set; }
            public Boolean? SuppressComputerActivityInPresentationMode { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SoftwareInventoryClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_TargetingSettings : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? AllowUserAffinity { get; set; }
            public UInt32? AllowUserAffinityAfterMinutes { get; set; }
            public UInt32? AutoApproveAffinity { get; set; }
            public UInt32? ConsoleMinutes { get; set; }
            public UInt32? IntervalDays { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_MulticastConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? RetryCount { get; set; }
            public UInt32? RetryDelay { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SoftwareDistributionClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

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

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_ConfigurationManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? PerProviderTimeOut { get; set; }
            public UInt32? PerScanTimeout { get; set; }
            public UInt32? PerScanTTL { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_ClientAgentConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

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

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SystemHealthClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

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

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_PowerManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public Boolean? AllowUserToOptOutFromPowerPlan { get; set; }
            public Boolean? EnableUserIdleMonitoring { get; set; }
            public UInt32? NumberOfDaysToKeep { get; set; }
            public UInt32? NumberOfMonthsToKeep { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_SoftwareMeteringClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? MaximumUsageInstancesPerReport { get; set; }
            public UInt32? ReportTimeout { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_HardwareInventoryClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_RemoteTools_Policy : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public String NetworkAccessPassword { get; set; }
            public String NetworkAccessUsername { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_ApplicationManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
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
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? ContentDownloadTimeOut { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_OutOfBandManagementClientConfig : CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Policy" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
            public CCM_OutOfBandManagementClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.SiteSettingsKey = WMIObject.Properties["SiteSettingsKey"].Value as UInt32?;
            }

            #region Properties
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? SiteSettingsKey { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_ServiceWindow : CCM_Policy
        {
            internal ccm baseClient;
            //Constructor
            internal  CCM_ServiceWindow(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
                : base(WMIObject, RemoteRunspace, PSCode, oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;
                baseClient = oClient;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.Schedules = WMIObject.Properties["Schedules"].Value as String;
                this.ServiceWindowID = WMIObject.Properties["ServiceWindowID"].Value as String;
                this.ServiceWindowType = WMIObject.Properties["ServiceWindowType"].Value as UInt32?;
            }



            #region Properties
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public String Schedules { get; set; }
            public String ServiceWindowID { get; set; }
            public UInt32? ServiceWindowType { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments

            /// <summary>
            /// Decode ScheduleID to Object of type: SMS_ST_NonRecurring, SMS_ST_RecurInterval, SMS_ST_RecurWeekly, SMS_ST_RecurMonthlyByWeekday or SMS_ST_RecurMonthlyByDate
            /// </summary>
            public object DecodedSchedule
            {
                get
                {
                    string sSchedule = Schedules;
                    string[] aSchedules = schedule.ScheduleDecoding.GetScheduleIDs(Schedules);
                    if (aSchedules.Length <= 1)
                    {
                        if (aSchedules.Length == 1)
                            return schedule.ScheduleDecoding.DecodeScheduleID(aSchedules[0]);
                        else
                            return null;
                    }
                    else
                    {
                        List<object> oResult = new List<object>();
                        foreach (string s in aSchedules)
                        {
                            oResult.Add(schedule.ScheduleDecoding.DecodeScheduleID(s));
                        }

                        return oResult; 
                    }
                }
            }

            #endregion

        }
    }

    /// <summary>
    /// Class localpolicy.
    /// </summary>
    public class localpolicy
    {
        /// <summary>
        /// Download policy from URL (or File)
        /// </summary>
        /// <param name="URL">e.g. http://win-29hctu7qses.corp.lab/SMS_MP/.sms_pol?{ce839d51-8469-42b2-ae09-c5a8faaa1ef7}.7_00</param>
        /// <returns>XML Body</returns>
        public static string DownloadPolicyFromURL(string URL)
        {
            //http://win-29hctu7qses.corp.lab/SMS_MP/.sms_pol?{ce839d51-8469-42b2-ae09-c5a8faaa1ef7}.7_00

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(URL);

            //Check if it's a compressed policy
            try
            {
                XmlNode xNode = xDoc.SelectSingleNode("PolicyXML");
                if (string.Compare(xNode.Attributes["Compression"].Value, "zlib", true) == 0)
                {
                    return DecompressPolicy(xNode.InnerText);
                }
            }
            catch { }

            return xDoc.InnerXml;
        }

        /// <summary>
        /// Decompress the hexstring of compressed policies...
        /// </summary>
        /// <param name="PolicyHexData">a string like: 789CDD96CB6EDA501086675DA9EF80B2A7...</param>
        /// <returns>the decompressed data (normaly XML) as string.</returns>
        public static string DecompressPolicy(string PolicyHexData)
        {
            //string zLib = "789CDD96CB6EDA501086675DA9EF80B2A7401C30A6AE239B8B6AA949A380D2EE9063885A89400449D5AAEAABB7F96670C038A052299BA2239FCBDCFE99FF78307F7EFB722ADFE5562652926F3296B92CE4ABCC642AEFE4486AF246AAACA712C86B7925BE5CA09B6091CA0F3CF2A701CF1D11D4EF4C12645FD04C911C61D9B7F318A444AE76E02892A2940A238F124BC73C34DECCF06219C950DA722C27E2495342D648CAE22273885CE65C47AB32D52E77BA76787476B10BA5875D552AECEE184BBC44EEB32C8758D588EFC835AB0B66195F079B86213490388670CD5E651E92136CEBCC89DCE0A3A7110857E4AC737F8F7A9F73F5779F3EFB07BCD2EC36B67B042BE9E6AD5EE23BC1B3B455FAC4FFCF9564782077F18BAA820DA6367909999F3228EDD4E4BBE0137D1083FE99F5C32ABA6F5DB1C023614D8DE9945809B285F9B51967C6635CB8FF98E786DD3A531FB479C6FE9C98DA955362DDAE3A71469D63E29F6752EDC6FB5C964D8BE6D3FD09560F4883FFF8467D30F295E8B9C8D0BEDC6D725FE44DF37DCE5C8DF1923974D0A5C4D1F314FDFA0D38E45BAC188F15DEDE90F73606B74B350369216DB17B6FB9D589D142A739360CB14744CF6AD22A22CE2E67D72A88B06992BDD6EF60D363D69CAAE8AB565795881E7154A6D5B6B174410E796A567B883C329B0E3AC58CE4ED01F07CC1AFF947E62EEB802A94ED16318B5CBBC6D09209ADBF678C36B28C1D74C7F8AA54990FADDA1A16CAA06BFC6A067A174B3E551F5974CF18732D7A97E119678AAC95768D29EF453BEB9275613AFDDF33DAB3BFEBFF90836A8ABFF5C52F4C65E79724FF7DAE6CFD16AF51F2DA401E01942A69CD"; 
            string sResult = "";
            try
            {
                //remove the first two bytes ("789C")
                byte[] polBytes = _stringToByteArray(PolicyHexData.Substring(4));

                System.IO.Compression.DeflateStream df = new System.IO.Compression.DeflateStream(new MemoryStream(polBytes), System.IO.Compression.CompressionMode.Decompress);

                
                using (StreamReader reader = new StreamReader(df))
                {
                    sResult = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return sResult; 
        }

        //copied from: http://stackoverflow.com/questions/321370/convert-hex-string-to-byte-array
        internal static byte[] _stringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((_getHexVal(hex[i << 1]) << 4) + (_getHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        //copied from: http://stackoverflow.com/questions/321370/convert-hex-string-to-byte-array
        internal static int _getHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        /// <summary>
        /// Format xml string into a formated xml structure
        /// </summary>
        /// <param name="xmlstring">xml string</param>
        /// <returns>formated xml string</returns>
        public static string FormatXML(string xmlstring)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xmlstring);
            StringBuilder sb = new StringBuilder();
            System.IO.TextWriter tr = new System.IO.StringWriter(sb);

            System.Xml.XmlTextWriter wr = new XmlTextWriter(tr);

            wr.Formatting = Formatting.Indented;

            xDoc.Save(wr);

            wr.Close();

            return sb.ToString();
        }
    }
}
