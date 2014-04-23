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
    /// Inventory Class
    /// </summary>
    public class inventory : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="inventory"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        /// <param name="oClient">A CCM Client object.</param>
        public inventory(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Show all Installed Software (like AddRemove Programs)
        /// </summary>
        public List<AI_InstalledSoftwareCache> InstalledSoftware
        {
            get
            {
                List<AI_InstalledSoftwareCache> lCache = new List<AI_InstalledSoftwareCache>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\invagt", "SELECT * FROM AI_InstalledSoftwareCache", true);
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    AI_InstalledSoftwareCache oCIEx = new AI_InstalledSoftwareCache(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                if (lCache.Count == 0)
                {
                    List<PSObject> oObj2 = GetObjects(@"root\CIMV2\sms", "SELECT * FROM SMS_InstalledSoftware", true);
                    foreach (PSObject PSObj in oObj2)
                    {
                        //Get AppDTs sub Objects
                        AI_InstalledSoftwareCache oCIEx = new AI_InstalledSoftwareCache(PSObj, remoteRunspace, pSCode);

                        oCIEx.remoteRunspace = remoteRunspace;
                        oCIEx.pSCode = pSCode;
                        lCache.Add(oCIEx);
                    }
                }
                return lCache; 
            } 
        }

        /// <summary>
        /// Status of Inventory Actions
        /// </summary>
        public List<InventoryActionStatus> InventoryActionStatusList
        {
            get
            {
                List<InventoryActionStatus> lCache = new List<InventoryActionStatus>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\invagt", "SELECT * FROM InventoryActionStatus");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    InventoryActionStatus oCIEx = new InventoryActionStatus(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }
        }

        /// <summary>
        /// Get System Power Management Capabilities
        /// </summary>
        /// <returns></returns>
        public CCM_PwrMgmtSystemPowerCapabilities PwrMgmtSystemPowerCapabilities()
        {
            foreach (PSObject PSObj in GetObjects(@"ROOT\ccm\PowerManagementAgent", "SELECT * FROM CCM_PwrMgmtSystemPowerCapabilities", false, new TimeSpan(0, 1, 0)))
            {
                CCM_PwrMgmtSystemPowerCapabilities oPWR = new CCM_PwrMgmtSystemPowerCapabilities(PSObj, remoteRunspace, pSCode);
                return oPWR;
            }
            return null;
        }

        /// <summary>
        /// List of Daily PowerMgmt data
        /// </summary>
        public List<CCM_PwrMgmtActualDay> PwrMgmtActualDay
        {
            get 
            {
                List<CCM_PwrMgmtActualDay> lCache = new List<CCM_PwrMgmtActualDay>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\PowerManagementAgent", "SELECT * FROM CCM_PwrMgmtActualDay", false, new TimeSpan(0,1,0));
                foreach (PSObject PSObj in oObj)
                {
                    CCM_PwrMgmtActualDay oDay = new CCM_PwrMgmtActualDay(PSObj, remoteRunspace, pSCode);
                    lCache.Add(oDay);
                }

                return lCache;
            }
        }

        /// <summary>
        /// Monthly PwrMgmt data
        /// </summary>
        public List<CCM_PwrMgmtMonth> PwrMgmtMonth
        {
            get
            {
                List<CCM_PwrMgmtMonth> lCache = new List<CCM_PwrMgmtMonth>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\PowerManagementAgent", "SELECT * FROM CCM_PwrMgmtMonth", false, new TimeSpan(0, 5, 0));
                foreach (PSObject PSObj in oObj)
                {
                    CCM_PwrMgmtMonth oMonth = new CCM_PwrMgmtMonth(PSObj, remoteRunspace, pSCode);
                    lCache.Add(oMonth);
                }

                return lCache;
            }
        }

        /// <summary>
        /// Current Power Settings on the System
        /// </summary>
        /// <param name="Reload"></param>
        /// <returns></returns>
        public List<SMS_PowerSettings> PowerSettings(bool Reload)
        {

                List<SMS_PowerSettings> lCache = new List<SMS_PowerSettings>();
                List<PSObject> oObj = GetObjects(@"ROOT\cimv2\sms", "SELECT * FROM SMS_PowerSettings", Reload);
                foreach (PSObject PSObj in oObj)
                {
                    SMS_PowerSettings oPWR = new SMS_PowerSettings(PSObj, remoteRunspace, pSCode);
                    lCache.Add(oPWR);
                }

                return lCache;
            
        }

        /// <summary>
        /// Return OS Architecture (x64 or x86)
        /// </summary>
        public string OSArchitecture
        {
            get
            {
                TimeSpan toldCacheTime = base.cacheTime;
                
                //Cache for 15minutes
                base.cacheTime = new TimeSpan(0, 15, 0);

                string sAddressWith = base.GetStringFromPS("(Get-WmiObject Win32_Processor | where {$_.DeviceID -eq 'CPU0'}).AddressWidth");
                base.cacheTime = toldCacheTime;

                if(string.Compare("64", sAddressWith, true) == 0)
                    return "x64";
                else
                    return "x86";
            }
        }

        /// <summary>
        /// Return True if OS is x64 Architecture
        /// </summary>
        public Boolean isx64OS
        {
            get
            {
                if (OSArchitecture == "x64")
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Source:ROOT\ccm\invagt
        /// </summary>
        public class AI_InstalledSoftwareCache
        {
            internal baseInit oNewBase;
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="AI_InstalledSoftwareCache"/> class.
            /// </summary>
            /// <param name="WMIObject">A WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public AI_InstalledSoftwareCache(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;
                oNewBase = new baseInit(remoteRunspace, pSCode);

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ARPDisplayName = WMIObject.Properties["ARPDisplayName"].Value as String;
                this.ChannelCode = WMIObject.Properties["ChannelCode"].Value as String;
                this.CM_DSLID = WMIObject.Properties["CM_DSLID"].Value as String;
                this.EvidenceSource = WMIObject.Properties["EvidenceSource"].Value as String;
                this.InstallDate = WMIObject.Properties["InstallDate"].Value as String;
                this.InstallDirectoryValidation = WMIObject.Properties["InstallDirectoryValidation"].Value as String;
                this.InstalledLocation = WMIObject.Properties["InstalledLocation"].Value as String;
                this.InstallSource = WMIObject.Properties["InstallSource"].Value as String;
                this.InstallType = WMIObject.Properties["InstallType"].Value as String;
                this.Language = WMIObject.Properties["Language"].Value as String;
                this.LocalPackage = WMIObject.Properties["LocalPackage"].Value as String;
                this.ProductID = WMIObject.Properties["ProductID"].Value as String;
                this.ProductName = WMIObject.Properties["ProductName"].Value as String;
                this.ProductVersion = WMIObject.Properties["ProductVersion"].Value as String;
                this.Publisher = WMIObject.Properties["Publisher"].Value as String;
                this.RegisteredUser = WMIObject.Properties["RegisteredUser"].Value as String;
                this.ServicePack = WMIObject.Properties["ServicePack"].Value as String;
                this.SoftwareCode = WMIObject.Properties["SoftwareCode"].Value as String;
                this.SoftwarePropertiesHash = WMIObject.Properties["SoftwarePropertiesHash"].Value as String;
                this.SoftwarePropertiesHashEx = WMIObject.Properties["SoftwarePropertiesHashEx"].Value as String;
                this.UninstallString = WMIObject.Properties["UninstallString"].Value as String;
                this.UpgradeCode = WMIObject.Properties["UpgradeCode"].Value as String;
                this.VersionMajor = WMIObject.Properties["VersionMajor"].Value as String;
                this.VersionMinor = WMIObject.Properties["VersionMinor"].Value as String;
            }

            #region Properties
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String ARPDisplayName { get; set; }
            public String ChannelCode { get; set; }
            public String CM_DSLID { get; set; }
            public String EvidenceSource { get; set; }
            public String InstallDate { get; set; }
            public String InstallDirectoryValidation { get; set; }
            public String InstalledLocation { get; set; }
            public String InstallSource { get; set; }
            public String InstallType { get; set; }
            public String Language { get; set; }
            public String LocalPackage { get; set; }
            public String ProductID { get; set; }
            public String ProductName { get; set; }
            public String ProductVersion { get; set; }
            public String Publisher { get; set; }
            public String RegisteredUser { get; set; }
            public String ServicePack { get; set; }
            public String SoftwareCode { get; set; }
            public String SoftwarePropertiesHash { get; set; }
            public String SoftwarePropertiesHashEx { get; set; }
            public String UninstallString { get; set; }
            public String UpgradeCode { get; set; }
            public String VersionMajor { get; set; }
            public String VersionMinor { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

            #region Methods
            /// <summary>
            /// Uninstalls this instance.
            /// </summary>
            /// <returns>System.String.</returns>
            public string Uninstall()
            {
                if (SoftwareCode.StartsWith("{"))
                {
                    return oNewBase.GetStringFromPS("Invoke-Expression(\"msiexec.exe /x '" + SoftwareCode + "' REBOOT=ReallySuppress /q\")");
                }

                return null;
            }

            /// <summary>
            /// Repairs this instance.
            /// </summary>
            /// <returns>System.String.</returns>
            public string Repair()
            {
                if (SoftwareCode.StartsWith("{"))
                {
                    return oNewBase.GetStringFromPS("Invoke-Expression(\"msiexec.exe /fpecmsu '" + SoftwareCode + "' REBOOT=ReallySuppress REINSTALL=ALL /q\")");
                }

                return null;
            }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\invagt
        /// </summary>
        public class InventoryActionStatus
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="InventoryActionStatus"/> class.
            /// </summary>
            /// <param name="WMIObject">A WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public InventoryActionStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.InventoryActionID = WMIObject.Properties["InventoryActionID"].Value as String;
                string sLastCycleStartedDate = WMIObject.Properties["LastCycleStartedDate"].Value as string;
                if (string.IsNullOrEmpty(sLastCycleStartedDate))
                    this.LastCycleStartedDate = null;
                else
                    this.LastCycleStartedDate = ManagementDateTimeConverter.ToDateTime(sLastCycleStartedDate) as DateTime?;
                this.LastMajorReportVersion = WMIObject.Properties["LastMajorReportVersion"].Value as UInt32?;
                this.LastMinorReportVersion = WMIObject.Properties["LastMinorReportVersion"].Value as UInt32?;
                string sLastReportDate = WMIObject.Properties["LastReportDate"].Value as string;
                if (string.IsNullOrEmpty(sLastReportDate))
                    this.LastReportDate = null;
                else
                    this.LastReportDate = ManagementDateTimeConverter.ToDateTime(sLastReportDate) as DateTime?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;

            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public String InventoryActionID { get; set; }
            public DateTime? LastCycleStartedDate { get; set; }
            public UInt32? LastMajorReportVersion { get; set; }
            public UInt32? LastMinorReportVersion { get; set; }
            public DateTime? LastReportDate { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }

        #region PowerMgmt

        /// <summary>
        /// Source:ROOT\ccm\PowerManagementAgent
        /// </summary>
        public class CCM_PwrMgmtActualDay
        {
            //Constructor
            public CCM_PwrMgmtActualDay(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                string sDate = WMIObject.Properties["Date"].Value as string;
                if (string.IsNullOrEmpty(sDate))
                    this.Date = null;
                else
                    this.Date = ManagementDateTimeConverter.ToDateTime(sDate) as DateTime?;
                this.hr0_1 = WMIObject.Properties["hr0_1"].Value as UInt32?;
                this.hr10_11 = WMIObject.Properties["hr10_11"].Value as UInt32?;
                this.hr11_12 = WMIObject.Properties["hr11_12"].Value as UInt32?;
                this.hr12_13 = WMIObject.Properties["hr12_13"].Value as UInt32?;
                this.hr13_14 = WMIObject.Properties["hr13_14"].Value as UInt32?;
                this.hr14_15 = WMIObject.Properties["hr14_15"].Value as UInt32?;
                this.hr15_16 = WMIObject.Properties["hr15_16"].Value as UInt32?;
                this.hr16_17 = WMIObject.Properties["hr16_17"].Value as UInt32?;
                this.hr17_18 = WMIObject.Properties["hr17_18"].Value as UInt32?;
                this.hr18_19 = WMIObject.Properties["hr18_19"].Value as UInt32?;
                this.hr19_20 = WMIObject.Properties["hr19_20"].Value as UInt32?;
                this.hr1_2 = WMIObject.Properties["hr1_2"].Value as UInt32?;
                this.hr20_21 = WMIObject.Properties["hr20_21"].Value as UInt32?;
                this.hr21_22 = WMIObject.Properties["hr21_22"].Value as UInt32?;
                this.hr22_23 = WMIObject.Properties["hr22_23"].Value as UInt32?;
                this.hr23_0 = WMIObject.Properties["hr23_0"].Value as UInt32?;
                this.hr2_3 = WMIObject.Properties["hr2_3"].Value as UInt32?;
                this.hr3_4 = WMIObject.Properties["hr3_4"].Value as UInt32?;
                this.hr4_5 = WMIObject.Properties["hr4_5"].Value as UInt32?;
                this.hr5_6 = WMIObject.Properties["hr5_6"].Value as UInt32?;
                this.hr6_7 = WMIObject.Properties["hr6_7"].Value as UInt32?;
                this.hr7_8 = WMIObject.Properties["hr7_8"].Value as UInt32?;
                this.hr8_9 = WMIObject.Properties["hr8_9"].Value as UInt32?;
                this.hr9_10 = WMIObject.Properties["hr9_10"].Value as UInt32?;
                this.minutesTotal = WMIObject.Properties["minutesTotal"].Value as UInt32?;
                this.TypeOfEvent = WMIObject.Properties["TypeOfEvent"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public DateTime? Date { get; set; }
            public UInt32? hr0_1 { get; set; }
            public UInt32? hr10_11 { get; set; }
            public UInt32? hr11_12 { get; set; }
            public UInt32? hr12_13 { get; set; }
            public UInt32? hr13_14 { get; set; }
            public UInt32? hr14_15 { get; set; }
            public UInt32? hr15_16 { get; set; }
            public UInt32? hr16_17 { get; set; }
            public UInt32? hr17_18 { get; set; }
            public UInt32? hr18_19 { get; set; }
            public UInt32? hr19_20 { get; set; }
            public UInt32? hr1_2 { get; set; }
            public UInt32? hr20_21 { get; set; }
            public UInt32? hr21_22 { get; set; }
            public UInt32? hr22_23 { get; set; }
            public UInt32? hr23_0 { get; set; }
            public UInt32? hr2_3 { get; set; }
            public UInt32? hr3_4 { get; set; }
            public UInt32? hr4_5 { get; set; }
            public UInt32? hr5_6 { get; set; }
            public UInt32? hr6_7 { get; set; }
            public UInt32? hr7_8 { get; set; }
            public UInt32? hr8_9 { get; set; }
            public UInt32? hr9_10 { get; set; }
            public UInt32? minutesTotal { get; set; }
            public String TypeOfEvent { get; set; }


            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\PowerManagementAgent
        /// </summary>
        public class CCM_PwrMgmtInternalAgentState
        {
            //Constructor
            public CCM_PwrMgmtInternalAgentState(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                string sLKGTime = WMIObject.Properties["LKGTime"].Value as string;
                if (string.IsNullOrEmpty(sLKGTime))
                    this.LKGTime = null;
                else
                    this.LKGTime = ManagementDateTimeConverter.ToDateTime(sLKGTime) as DateTime?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public DateTime? LKGTime { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\PowerManagementAgent
        /// </summary>
        public class CCM_PwrMgmtInternalEventStateCookie
        {
            //Constructor
            public CCM_PwrMgmtInternalEventStateCookie(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ClientID = WMIObject.Properties["ClientID"].Value as String;
                this.ComputerActiveState = WMIObject.Properties["ComputerActiveState"].Value as UInt32?;
                this.ComputerOnState = WMIObject.Properties["ComputerOnState"].Value as UInt32?;
                this.ComputerShutdownState = WMIObject.Properties["ComputerShutdownState"].Value as UInt32?;
                this.ComputerSleepState = WMIObject.Properties["ComputerSleepState"].Value as UInt32?;
                string sLastRecordedDate = WMIObject.Properties["LastRecordedDate"].Value as string;
                if (string.IsNullOrEmpty(sLastRecordedDate))
                    this.LastRecordedDate = null;
                else
                    this.LastRecordedDate = ManagementDateTimeConverter.ToDateTime(sLastRecordedDate) as DateTime?;
                this.MonitorOnState = WMIObject.Properties["MonitorOnState"].Value as UInt32?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String ClientID { get; set; }
            public UInt32? ComputerActiveState { get; set; }
            public UInt32? ComputerOnState { get; set; }
            public UInt32? ComputerShutdownState { get; set; }
            public UInt32? ComputerSleepState { get; set; }
            public DateTime? LastRecordedDate { get; set; }
            public UInt32? MonitorOnState { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\PowerManagementAgent
        /// </summary>
        public class CCM_PwrMgmtInternalRawEvent
        {
            //Constructor
            public CCM_PwrMgmtInternalRawEvent(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.eventID = WMIObject.Properties["eventID"].Value as UInt32?;
                this.GUID = WMIObject.Properties["GUID"].Value as String;
                string stime = WMIObject.Properties["time"].Value as string;
                if (string.IsNullOrEmpty(stime))
                    this.time = null;
                else
                    this.time = ManagementDateTimeConverter.ToDateTime(stime) as DateTime?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public UInt32? eventID { get; set; }
            public String GUID { get; set; }
            public DateTime? time { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\PowerManagementAgent
        /// </summary>
        public class CCM_PwrMgmtLastSuspendError
        {
            //Constructor
            public CCM_PwrMgmtLastSuspendError(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AdditionalCode = WMIObject.Properties["AdditionalCode"].Value as UInt32?;
                this.AdditionalInfo = WMIObject.Properties["AdditionalInfo"].Value as String;
                this.Requester = WMIObject.Properties["Requester"].Value as String;
                this.RequesterInfo = WMIObject.Properties["RequesterInfo"].Value as String;
                this.RequesterType = WMIObject.Properties["RequesterType"].Value as String;
                this.RequestType = WMIObject.Properties["RequestType"].Value as String;
                string sTime = WMIObject.Properties["Time"].Value as string;
                if (string.IsNullOrEmpty(sTime))
                    this.Time = null;
                else
                    this.Time = ManagementDateTimeConverter.ToDateTime(sTime) as DateTime?;
                this.UnknownRequester = WMIObject.Properties["UnknownRequester"].Value as Boolean?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public UInt32? AdditionalCode { get; set; }
            public String AdditionalInfo { get; set; }
            public String Requester { get; set; }
            public String RequesterInfo { get; set; }
            public String RequesterType { get; set; }
            public String RequestType { get; set; }
            public DateTime? Time { get; set; }
            public Boolean? UnknownRequester { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\PowerManagementAgent
        /// </summary>
        public class CCM_PwrMgmtMonth
        {
            //Constructor
            public CCM_PwrMgmtMonth(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.minutesComputerActive = WMIObject.Properties["minutesComputerActive"].Value as UInt32?;
                this.minutesComputerOn = WMIObject.Properties["minutesComputerOn"].Value as UInt32?;
                this.minutesComputerShutdown = WMIObject.Properties["minutesComputerShutdown"].Value as UInt32?;
                this.minutesComputerSleep = WMIObject.Properties["minutesComputerSleep"].Value as UInt32?;
                this.minutesMonitorOn = WMIObject.Properties["minutesMonitorOn"].Value as UInt32?;
                this.minutesTotal = WMIObject.Properties["minutesTotal"].Value as UInt32?;
                string sMonthStart = WMIObject.Properties["MonthStart"].Value as string;
                if (string.IsNullOrEmpty(sMonthStart))
                    this.MonthStart = null;
                else
                    this.MonthStart = ManagementDateTimeConverter.ToDateTime(sMonthStart) as DateTime?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public UInt32? minutesComputerActive { get; set; }
            public UInt32? minutesComputerOn { get; set; }
            public UInt32? minutesComputerShutdown { get; set; }
            public UInt32? minutesComputerSleep { get; set; }
            public UInt32? minutesMonitorOn { get; set; }
            public UInt32? minutesTotal { get; set; }
            public DateTime? MonthStart { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\PowerManagementAgent
        /// </summary>
        public class CCM_PwrMgmtSystemPowerCapabilities
        {
            //Constructor
            public CCM_PwrMgmtSystemPowerCapabilities(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ApmPresent = WMIObject.Properties["ApmPresent"].Value as Boolean?;
                this.BatteriesAreShortTerm = WMIObject.Properties["BatteriesAreShortTerm"].Value as Boolean?;
                this.FullWake = WMIObject.Properties["FullWake"].Value as Boolean?;
                this.LidPresent = WMIObject.Properties["LidPresent"].Value as Boolean?;
                this.MinDeviceWakeState = WMIObject.Properties["MinDeviceWakeState"].Value as String;
                this.PreferredPMProfile = WMIObject.Properties["PreferredPMProfile"].Value as UInt32?;
                this.ProcessorThrottle = WMIObject.Properties["ProcessorThrottle"].Value as Boolean?;
                this.RtcWake = WMIObject.Properties["RtcWake"].Value as String;
                this.SystemBatteriesPresent = WMIObject.Properties["SystemBatteriesPresent"].Value as Boolean?;
                this.SystemS1 = WMIObject.Properties["SystemS1"].Value as Boolean?;
                this.SystemS2 = WMIObject.Properties["SystemS2"].Value as Boolean?;
                this.SystemS3 = WMIObject.Properties["SystemS3"].Value as Boolean?;
                this.SystemS4 = WMIObject.Properties["SystemS4"].Value as Boolean?;
                this.SystemS5 = WMIObject.Properties["SystemS5"].Value as Boolean?;
                this.UpsPresent = WMIObject.Properties["UpsPresent"].Value as Boolean?;
                this.VideoDimPresent = WMIObject.Properties["VideoDimPresent"].Value as Boolean?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public Boolean? ApmPresent { get; set; }
            public Boolean? BatteriesAreShortTerm { get; set; }
            public Boolean? FullWake { get; set; }
            public Boolean? LidPresent { get; set; }
            public String MinDeviceWakeState { get; set; }
            public UInt32? PreferredPMProfile { get; set; }
            public Boolean? ProcessorThrottle { get; set; }
            public String RtcWake { get; set; }
            public Boolean? SystemBatteriesPresent { get; set; }
            public Boolean? SystemS1 { get; set; }
            public Boolean? SystemS2 { get; set; }
            public Boolean? SystemS3 { get; set; }
            public Boolean? SystemS4 { get; set; }
            public Boolean? SystemS5 { get; set; }
            public Boolean? UpsPresent { get; set; }
            public Boolean? VideoDimPresent { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\PowerManagementAgent
        /// </summary>
        public class CCM_PwrMgmtUserClientSettings
        {
            //Constructor
            public CCM_PwrMgmtUserClientSettings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.IsClientOptOut = WMIObject.Properties["IsClientOptOut"].Value as Boolean?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public Boolean? IsClientOptOut { get; set; }
            #endregion

        }


        /// <summary>
        /// Source:ROOT\cimv2\sms
        /// </summary>
        public class SMS_PowerSettings
        {
            //Constructor
            public SMS_PowerSettings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ACSettingIndex = WMIObject.Properties["ACSettingIndex"].Value as String;
                this.ACValue = WMIObject.Properties["ACValue"].Value as String;
                this.DCSettingIndex = WMIObject.Properties["DCSettingIndex"].Value as String;
                this.DCValue = WMIObject.Properties["DCValue"].Value as String;
                this.GUID = WMIObject.Properties["GUID"].Value as String;
                this.Name = WMIObject.Properties["Name"].Value as String;
                this.UnitSpecifier = WMIObject.Properties["UnitSpecifier"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String ACSettingIndex { get; set; }
            public String ACValue { get; set; }
            public String DCSettingIndex { get; set; }
            public String DCValue { get; set; }
            public String GUID { get; set; }
            public String Name { get; set; }
            public String UnitSpecifier { get; set; }

            public string ACDisplayvalue { 
                get
                {
                    if (Name == ACValue)
                        return ACSettingIndex + " " + UnitSpecifier;
                    else
                        return ACValue;
                } 
            }

            public string DCDisplayvalue
            {
                get 
                {
                    if (Name == DCValue)
                        return DCSettingIndex + " " + UnitSpecifier;
                    else
                        return DCValue;
                }
            }
            #endregion

        }

        #endregion

    }








}
