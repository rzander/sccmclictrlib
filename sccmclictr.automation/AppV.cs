//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

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
    /// App-V 5.x Classes
    /// Thanks to Mattias Benninge to provide the details
    /// </summary>
    public class appv5 : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public appv5(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        public List<AppvClientApplication> Appv5ClientApplications
        {
            get
            {
                List<AppvClientApplication> lApps = new List<AppvClientApplication>();
                List<PSObject> oObj = GetObjects(@"ROOT\Appv", "SELECT * FROM AppvClientApplication");
                foreach (PSObject PSObj in oObj)
                {
                    AppvClientApplication oCIEx = new AppvClientApplication(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lApps.Add(oCIEx);
                }
                return lApps;
            }
        }

        public List<AppvClientAsset> Appv5AppvClientAssets
        {
            get
            {
                List<AppvClientAsset> lApps = new List<AppvClientAsset>();
                List<PSObject> oObj = GetObjects(@"ROOT\Appv", "SELECT * FROM AppvClientAsset");
                foreach (PSObject PSObj in oObj)
                {
                    AppvClientAsset oCIEx = new AppvClientAsset(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lApps.Add(oCIEx);
                }
                return lApps;
            }
        }

        public List<AppvClientConnectionGroup> Appv5AppvClientConnectionGroups
        {
            get
            {
                List<AppvClientConnectionGroup> lApps = new List<AppvClientConnectionGroup>();
                List<PSObject> oObj = GetObjects(@"ROOT\Appv", "SELECT * FROM AppvClientConnectionGroup");
                foreach (PSObject PSObj in oObj)
                {
                    AppvClientConnectionGroup oCIEx = new AppvClientConnectionGroup(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lApps.Add(oCIEx);
                }
                return lApps;
            }
        }

        public List<AppvClientPackage> Appv5AppvClientPackages
        {
            get
            {
                List<AppvClientPackage> lApps = new List<AppvClientPackage>();
                List<PSObject> oObj = GetObjects(@"ROOT\Appv", "SELECT * FROM AppvClientPackage");
                foreach (PSObject PSObj in oObj)
                {
                    AppvClientPackage oCIEx = new AppvClientPackage(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lApps.Add(oCIEx);
                }
                return lApps;
            }
        }

        public List<AppvPublishingServer> Appv5AppvPublishingServers
        {
            get
            {
                List<AppvPublishingServer> lApps = new List<AppvPublishingServer>();
                List<PSObject> oObj = GetObjects(@"ROOT\Appv", "SELECT * FROM AppvPublishingServer");
                foreach (PSObject PSObj in oObj)
                {
                    AppvPublishingServer oCIEx = new AppvPublishingServer(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lApps.Add(oCIEx);
                }
                return lApps;
            }
        }

        /// <summary>
        /// Source:ROOT\Appv
        /// </summary>
        public class AppvClientApplication
        {
            //Constructor
            public AppvClientApplication(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ApplicationId = WMIObject.Properties["ApplicationId"].Value as String;
                this.EnabledForUser = WMIObject.Properties["EnabledForUser"].Value as Boolean?;
                this.EnabledGlobally = WMIObject.Properties["EnabledGlobally"].Value as Boolean?;
                this.Name = WMIObject.Properties["Name"].Value as String;
                this.PackageId = WMIObject.Properties["PackageId"].Value as String;
                this.PackageVersionId = WMIObject.Properties["PackageVersionId"].Value as String;
                this.TargetPath = WMIObject.Properties["TargetPath"].Value as String;
                this.Version = WMIObject.Properties["Version"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String ApplicationId { get; set; }
            public Boolean? EnabledForUser { get; set; }
            public Boolean? EnabledGlobally { get; set; }
            public String Name { get; set; }
            public String PackageId { get; set; }
            public String PackageVersionId { get; set; }
            public String TargetPath { get; set; }
            public String Version { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\Appv
        /// </summary>
        public class AppvClientAsset
        {
            //Constructor
            public AppvClientAsset(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ChannelCode = WMIObject.Properties["ChannelCode"].Value as String;
                this.CM_DSLID = WMIObject.Properties["CM_DSLID"].Value as String;
                this.InstallDate = WMIObject.Properties["InstallDate"].Value as String;
                this.InstalledLocation = WMIObject.Properties["InstalledLocation"].Value as String;
                this.Language = WMIObject.Properties["Language"].Value as String;
                this.OsComponent = WMIObject.Properties["OsComponent"].Value as String;
                this.PackageId = WMIObject.Properties["PackageId"].Value as String;
                this.PackageVersionId = WMIObject.Properties["PackageVersionId"].Value as String;
                this.ProductID = WMIObject.Properties["ProductID"].Value as String;
                this.ProductName = WMIObject.Properties["ProductName"].Value as String;
                this.ProductVersion = WMIObject.Properties["ProductVersion"].Value as String;
                this.Publisher = WMIObject.Properties["Publisher"].Value as String;
                this.RegisteredUser = WMIObject.Properties["RegisteredUser"].Value as String;
                this.ServicePack = WMIObject.Properties["ServicePack"].Value as String;
                this.SoftwareCode = WMIObject.Properties["SoftwareCode"].Value as String;
                this.UpgradeCode = WMIObject.Properties["UpgradeCode"].Value as String;
                this.VersionMajor = WMIObject.Properties["VersionMajor"].Value as String;
                this.VersionMinor = WMIObject.Properties["VersionMinor"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String ChannelCode { get; set; }
            public String CM_DSLID { get; set; }
            public String InstallDate { get; set; }
            public String InstalledLocation { get; set; }
            public String Language { get; set; }
            public String OsComponent { get; set; }
            public String PackageId { get; set; }
            public String PackageVersionId { get; set; }
            public String ProductID { get; set; }
            public String ProductName { get; set; }
            public String ProductVersion { get; set; }
            public String Publisher { get; set; }
            public String RegisteredUser { get; set; }
            public String ServicePack { get; set; }
            public String SoftwareCode { get; set; }
            public String UpgradeCode { get; set; }
            public String VersionMajor { get; set; }
            public String VersionMinor { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\Appv
        /// </summary>
        public class AppvClientConnectionGroup
        {
            //Constructor
            public AppvClientConnectionGroup(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.CustomData = WMIObject.Properties["CustomData"].Value as String;
                this.GlobalPending = WMIObject.Properties["GlobalPending"].Value as Boolean?;
                this.GroupId = WMIObject.Properties["GroupId"].Value as String;
                this.InUse = WMIObject.Properties["InUse"].Value as Boolean?;
                this.IsEnabledGlobally = WMIObject.Properties["IsEnabledGlobally"].Value as Boolean?;
                this.IsEnabledToUser = WMIObject.Properties["IsEnabledToUser"].Value as Boolean?;
                this.Name = WMIObject.Properties["Name"].Value as String;
                this.Packages = WMIObject.Properties["Packages"].Value as String[];
                this.PercentLoaded = WMIObject.Properties["PercentLoaded"].Value as UInt16?;
                this.Priority = WMIObject.Properties["Priority"].Value as UInt32?;
                this.UserPending = WMIObject.Properties["UserPending"].Value as Boolean?;
                this.VersionId = WMIObject.Properties["VersionId"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String CustomData { get; set; }
            public Boolean? GlobalPending { get; set; }
            public String GroupId { get; set; }
            public Boolean? InUse { get; set; }
            public Boolean? IsEnabledGlobally { get; set; }
            public Boolean? IsEnabledToUser { get; set; }
            public String Name { get; set; }
            public String[] Packages { get; set; }
            public UInt16? PercentLoaded { get; set; }
            public UInt32? Priority { get; set; }
            public Boolean? UserPending { get; set; }
            public String VersionId { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\Appv
        /// </summary>
        public class AppvClientPackage
        {
            //Constructor
            public AppvClientPackage(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.Assets = WMIObject.Properties["Assets"].Value as String[];
                this.DeploymentMachineData = WMIObject.Properties["DeploymentMachineData"].Value as String;
                this.DeploymentUserData = WMIObject.Properties["DeploymentUserData"].Value as String;
                this.GlobalPending = WMIObject.Properties["GlobalPending"].Value as Boolean?;
                this.HasAssetIntelligence = WMIObject.Properties["HasAssetIntelligence"].Value as Boolean?;
                this.InUse = WMIObject.Properties["InUse"].Value as Boolean?;
                this.IsPublishedGlobally = WMIObject.Properties["IsPublishedGlobally"].Value as Boolean?;
                this.IsPublishedToUser = WMIObject.Properties["IsPublishedToUser"].Value as Boolean?;
                this.Name = WMIObject.Properties["Name"].Value as String;
                this.PackageId = WMIObject.Properties["PackageId"].Value as String;
                this.PackageSize = WMIObject.Properties["PackageSize"].Value as UInt64?;
                this.Path = WMIObject.Properties["Path"].Value as String;
                this.PercentLoaded = WMIObject.Properties["PercentLoaded"].Value as UInt16?;
                this.UserConfigurationData = WMIObject.Properties["UserConfigurationData"].Value as String;
                this.UserPending = WMIObject.Properties["UserPending"].Value as Boolean?;
                this.Version = WMIObject.Properties["Version"].Value as String;
                this.VersionId = WMIObject.Properties["VersionId"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String[] Assets { get; set; }
            public String DeploymentMachineData { get; set; }
            public String DeploymentUserData { get; set; }
            public Boolean? GlobalPending { get; set; }
            public Boolean? HasAssetIntelligence { get; set; }
            public Boolean? InUse { get; set; }
            public Boolean? IsPublishedGlobally { get; set; }
            public Boolean? IsPublishedToUser { get; set; }
            public String Name { get; set; }
            public String PackageId { get; set; }
            public UInt64? PackageSize { get; set; }
            public String Path { get; set; }
            public UInt16? PercentLoaded { get; set; }
            public String UserConfigurationData { get; set; }
            public Boolean? UserPending { get; set; }
            public String Version { get; set; }
            public String VersionId { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\Appv
        /// </summary>
        public class AppvPublishingServer
        {
            //Constructor
            public AppvPublishingServer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.GlobalRefreshEnabled = WMIObject.Properties["GlobalRefreshEnabled"].Value as Boolean?;
                this.GlobalRefreshInterval = WMIObject.Properties["GlobalRefreshInterval"].Value as UInt32?;
                this.GlobalRefreshIntervalUnit = WMIObject.Properties["GlobalRefreshIntervalUnit"].Value as String;
                this.GlobalRefreshOnLogon = WMIObject.Properties["GlobalRefreshOnLogon"].Value as Boolean?;
                this.ID = WMIObject.Properties["ID"].Value as UInt32?;
                this.SetByGroupPolicy = WMIObject.Properties["SetByGroupPolicy"].Value as Boolean?;
                this.Url = WMIObject.Properties["Url"].Value as String;
                this.UserRefreshEnabled = WMIObject.Properties["UserRefreshEnabled"].Value as Boolean?;
                this.UserRefreshInterval = WMIObject.Properties["UserRefreshInterval"].Value as UInt32?;
                this.UserRefreshIntervalUnit = WMIObject.Properties["UserRefreshIntervalUnit"].Value as String;
                this.UserRefreshOnLogon = WMIObject.Properties["UserRefreshOnLogon"].Value as Boolean?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public Boolean? GlobalRefreshEnabled { get; set; }
            public UInt32? GlobalRefreshInterval { get; set; }
            public String GlobalRefreshIntervalUnit { get; set; }
            public Boolean? GlobalRefreshOnLogon { get; set; }
            public UInt32? ID { get; set; }
            public Boolean? SetByGroupPolicy { get; set; }
            public String Url { get; set; }
            public Boolean? UserRefreshEnabled { get; set; }
            public UInt32? UserRefreshInterval { get; set; }
            public String UserRefreshIntervalUnit { get; set; }
            public Boolean? UserRefreshOnLogon { get; set; }
            #endregion

        }
    }

        /// <summary>
    /// App-V 4.x Classes
    /// Thanks to Mattias Benninge to provide the details
    /// </summary>
    public class appv4 : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public appv4(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        public List<Application> Appv4Applications
        {
            get
            {
                List<Application> lApps = new List<Application>();
                List<PSObject> oObj = GetObjects(@"ROOT\microsoft\appvirt\client", "SELECT * FROM Application");
                foreach (PSObject PSObj in oObj)
                {
                    Application oCIEx = new Application(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lApps.Add(oCIEx);
                }
                return lApps;
            }
        }

        public List<Package> Appv4Packages
        {
            get
            {
                List<Package> lPkg = new List<Package>();
                List<PSObject> oObj = GetObjects(@"ROOT\microsoft\appvirt\client", "SELECT * FROM Package");
                foreach (PSObject PSObj in oObj)
                {
                    Package oCIEx = new Package(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lPkg.Add(oCIEx);
                }
                return lPkg;
            }
        }

        /// <summary>
        /// Source:ROOT\microsoft\appvirt\client
        /// </summary>
        public class Application
        {
            //Constructor
            public Application(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.CachedOsdPath = WMIObject.Properties["CachedOsdPath"].Value as String;
                this.GlobalRunningCount = WMIObject.Properties["GlobalRunningCount"].Value as UInt32?;
                string sLastLaunchOnSystem = WMIObject.Properties["LastLaunchOnSystem"].Value as string;
                if (string.IsNullOrEmpty(sLastLaunchOnSystem))
                    this.LastLaunchOnSystem = null;
                else
                    this.LastLaunchOnSystem = ManagementDateTimeConverter.ToDateTime(sLastLaunchOnSystem) as DateTime?;
                this.Loading = WMIObject.Properties["Loading"].Value as Boolean?;
                this.Name = WMIObject.Properties["Name"].Value as String;
                this.OriginalOsdPath = WMIObject.Properties["OriginalOsdPath"].Value as String;
                this.PackageGUID = WMIObject.Properties["PackageGUID"].Value as String;
                this.Version = WMIObject.Properties["Version"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String CachedOsdPath { get; set; }
            public UInt32? GlobalRunningCount { get; set; }
            public DateTime? LastLaunchOnSystem { get; set; }
            public Boolean? Loading { get; set; }
            public String Name { get; set; }
            public String OriginalOsdPath { get; set; }
            public String PackageGUID { get; set; }
            public String Version { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\microsoft\appvirt\client
        /// </summary>
        public class Package
        {
            //Constructor
            public Package(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.CachedLaunchSize = WMIObject.Properties["CachedLaunchSize"].Value as UInt64?;
                this.CachedPercentage = WMIObject.Properties["CachedPercentage"].Value as UInt16?;
                this.CachedSize = WMIObject.Properties["CachedSize"].Value as UInt64?;
                this.InUse = WMIObject.Properties["InUse"].Value as Boolean?;
                this.LaunchSize = WMIObject.Properties["LaunchSize"].Value as UInt64?;
                this.Locked = WMIObject.Properties["Locked"].Value as Boolean?;
                this.Name = WMIObject.Properties["Name"].Value as String;
                this.PackageGUID = WMIObject.Properties["PackageGUID"].Value as String;
                this.SftPath = WMIObject.Properties["SftPath"].Value as String;
                this.TotalSize = WMIObject.Properties["TotalSize"].Value as UInt64?;
                this.Version = WMIObject.Properties["Version"].Value as String;
                this.VersionGUID = WMIObject.Properties["VersionGUID"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public UInt64? CachedLaunchSize { get; set; }
            public UInt16? CachedPercentage { get; set; }
            public UInt64? CachedSize { get; set; }
            public Boolean? InUse { get; set; }
            public UInt64? LaunchSize { get; set; }
            public Boolean? Locked { get; set; }
            public String Name { get; set; }
            public String PackageGUID { get; set; }
            public String SftPath { get; set; }
            public UInt64? TotalSize { get; set; }
            public String Version { get; set; }
            public String VersionGUID { get; set; }
            #endregion

        }
    }
}
