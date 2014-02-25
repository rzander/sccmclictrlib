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



    }








}
