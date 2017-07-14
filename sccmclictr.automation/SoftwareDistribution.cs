//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2017 by Roger Zander

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

using System.Drawing;
using System.Xml;
using System.Net;
using System.IO;
using System.Reflection;

namespace sccmclictr.automation.functions
{
#if CM2012

    /// <summary>
    /// Template for an empty Class
    /// </summary>
    public class softwaredistribution : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="softwaredistribution"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        /// <param name="oClient">A CCM Client object.</param>
        public softwaredistribution(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Get a list of Applications (SELECT * FROM CCM_Application)
        /// </summary>
        public List<CCM_Application> Applications
        {
            get
            {
                return Applications_(false);
            }
        }

        /// <summary>
        /// Get a list of Applications (SELECT * FROM CCM_Application)
        /// </summary>
        /// <param name="bReload">enforce reload</param>
        /// <returns>List of CCM_Application</returns>
        public List<CCM_Application> Applications_(Boolean bReload)
        {
            return Applications_(bReload, new TimeSpan(0, 3, 0));
        }

        /// <summary>
        /// Get a list of Applications (SELECT * FROM CCM_Application)
        /// </summary>
        /// <param name="bReload">enforce reload</param>
        /// <param name="CacheTime">TTL for Cached items</param>
        /// <returns></returns>
        public List<CCM_Application> Applications_(Boolean bReload, TimeSpan CacheTime)
        {

            List<CCM_Application> lApps = new List<CCM_Application>();
            List<PSObject> oObj = new List<PSObject>();

            oObj = GetObjects(@"ROOT\ccm\ClientSDK", "SELECT * FROM CCM_Application", bReload, CacheTime);

            foreach (PSObject PSObj in oObj)
            {
                try
                {
                    //Get AppDTs sub Objects
                    CCM_Application oApp = new CCM_Application(PSObj, remoteRunspace, pSCode);

                    oApp.remoteRunspace = remoteRunspace;
                    oApp.pSCode = pSCode;
                    lApps.Add(oApp);
                }
                catch { }
            }
            return lApps;

        }

        /// <summary>
        /// Get a list of Programs
        /// </summary>
        public List<CCM_Program> Programs
        {
            get
            {
                List<CCM_Program> lApps = new List<CCM_Program>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\ClientSDK", "SELECT * FROM CCM_Program");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_Program oApp = new CCM_Program(PSObj, remoteRunspace, pSCode);

                    oApp.remoteRunspace = remoteRunspace;
                    oApp.pSCode = pSCode;
                    lApps.Add(oApp);
                }
                return lApps;
            }
        }

        /// <summary>
        /// List of the System Execution History (only Machine based !)
        /// </summary>
        public List<REG_ExecutionHistory> ExecutionHistory
        {
            get { return ExecutionHistory_(false); }
        }
        /// <summary>
        /// List of the System Execution History (only Machine based !)
        /// </summary>
        public List<REG_ExecutionHistory> ExecutionHistory_(Boolean bReload)
        {

            List<REG_ExecutionHistory> lExec = new List<REG_ExecutionHistory>();
            List<PSObject> oObj = new List<PSObject>();
            Boolean bisSCCM2012 = baseClient.AgentProperties.isSCCM2012;
            Boolean bisx64OS = true;

            //Only Get Architecture if SCCM < 2012
            if (!bisSCCM2012)
                bisx64OS = baseClient.Inventory.isx64OS;

            if (bisSCCM2012)
                oObj = GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\" -Recurse | % { get-itemproperty -path  $_.PsPath }", bReload, new TimeSpan(0, 0, 10));
            if (!bisSCCM2012 & bisx64OS)
                oObj = GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\" -Recurse | % { get-itemproperty -path  $_.PsPath }", bReload, new TimeSpan(0, 0, 10));
            if (!bisSCCM2012 & !bisx64OS)
                oObj = GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\" -Recurse | % { get-itemproperty -path  $_.PsPath }", bReload, new TimeSpan(0, 0, 10));

            foreach (PSObject PSObj in oObj)
            {
                //Get AppDTs sub Objects
                REG_ExecutionHistory oReg = new REG_ExecutionHistory(PSObj, remoteRunspace, pSCode);

                oReg.remoteRunspace = remoteRunspace;
                oReg.pSCode = pSCode;
                lExec.Add(oReg);
            }

            return lExec;
        }

        /// <summary>
        /// List of Package-Deployments (old Package-Model)
        /// </summary>
        public List<CCM_SoftwareDistribution> Advertisements
        {
            get
            {
                return Advertisements_(false);
            }
        }
        /// <summary>
        /// List of Package-Deployments (old Package-Model)
        /// </summary>
        public List<CCM_SoftwareDistribution> Advertisements_(Boolean bReload)
        {
            List<CCM_SoftwareDistribution> lApps = new List<CCM_SoftwareDistribution>();
            List<PSObject> oObj = GetObjects(@"root\ccm\policy\machine\actualconfig", "SELECT * FROM CCM_SoftwareDistribution", bReload);
            foreach (PSObject PSObj in oObj)
            {
                //Get AppDTs sub Objects
                CCM_SoftwareDistribution oApp = new CCM_SoftwareDistribution(PSObj, remoteRunspace, pSCode);

                oApp.remoteRunspace = remoteRunspace;
                oApp.pSCode = pSCode;
                lApps.Add(oApp);
            }
            return lApps;
        }

        /// <summary>
        /// List of Applications, Updates and Acvertisements
        /// </summary>
        public List<SoftwareStatus> SoftwareSummary
        {
            get
            {
                return SoftwareSummary_(false);
            }
        }

        /// <summary>
        /// List of Applications, Updates and Acvertisements
        /// </summary>
        /// <param name="bReload">enforce a reload, otherwise it will use the data from cache</param>
        /// <returns></returns>
        public List<SoftwareStatus> SoftwareSummary_(Boolean bReload)
        {

            List<SoftwareStatus> lSW = new List<SoftwareStatus>();
            List<PSObject> oObj = new List<PSObject>();

            oObj = GetObjects(@"ROOT\ccm\ClientSDK", "SELECT * FROM CCM_SoftwareBase", bReload);

            foreach (PSObject PSObj in oObj)
            {
                try
                {
                    //Get AppDTs sub Objects
                    SoftwareStatus oApp = new SoftwareStatus(PSObj, remoteRunspace, pSCode);
                    if (!string.IsNullOrEmpty(oApp.Type))
                    {
                        oApp.remoteRunspace = remoteRunspace;
                        oApp.pSCode = pSCode;
                        lSW.Add(oApp);
                    }
                }
                catch { }
            }
            return lSW;

        }

        /// <summary>
        /// Get Application from the Catalog
        /// </summary>
        /// <param name="AppCatalogURL">e.g. http://CatalogServer/CMApplicationCatalog</param>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        public List<AppDetailView> ApplicationCatalog(string AppCatalogURL, string searchFilter = "")
        {
            List<AppDetailView> lResult = new List<AppDetailView>();
            try
            {
                string SOAPResult = "";
                if (string.IsNullOrEmpty(AppCatalogURL))
                    AppCatalogURL = baseClient.AgentProperties.PortalURL;
                string OSArchitecture = baseClient.Inventory.OSArchitecture;
                string OSversionFull = baseClient.Inventory.OSVersion;
                string OSVersion = OSversionFull.Substring(0, OSversionFull.LastIndexOf('.'));

                if (!string.IsNullOrEmpty(AppCatalogURL))
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(AppCatalogURL + "/applicationviewservice.asmx");
                    webRequest.Headers.Add("SOAPAction", "http://schemas.microsoft.com/5.0.0.0/ConfigurationManager/SoftwareCatalog/Website/GetFilteredApplications");
                    webRequest.Headers.Add("Accept-Language", "de-DE");
                    webRequest.Headers.Add("request-source", "softwarecenter");
                    webRequest.Headers.Add("api-version", "4.0");
                    webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                    webRequest.Accept = "text/xml";
                    webRequest.Method = "POST";
                    webRequest.UseDefaultCredentials = true;

                    string sQuery = "<queryString/>";
                    if (!string.IsNullOrEmpty(searchFilter))
                        sQuery = "<queryString>" + searchFilter + "</queryString>";

                    string sXMLEnvelope = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">" +
                        "<s:Body>" +
                        @"<GetFilteredApplications xmlns=""http://schemas.microsoft.com/5.0.0.0/ConfigurationManager/SoftwareCatalog/Website"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">" +
                        "<sortBy>Name</sortBy>" +
                        "<filterByProperty>IsFeatured</filterByProperty >" +
                        sQuery +
                        "<maximumRows>1000</maximumRows>" +
                        "<startRowIndex>0</startRowIndex>" +
                        "<sortAscending>true</sortAscending>" +
                        "<classicNameFields>PackageProgramName</classicNameFields>" +
                        "<useSecondarySort>true</useSecondarySort>" +
                        "<fillInIcon>false</fillInIcon>" +
                        "<platform>" +
                        "<OSVersion>" + OSVersion + "</OSVersion>" +
                        "<OSArchitecture>" + OSArchitecture + "</OSArchitecture>" +
                        "<OSProductType>1</OSProductType>" +
                        "<SMSID/>" +
                        "<SspVersion>SWCenter:4.0.0.0</SspVersion>" +
                        "<IsClassicAppSupported>true</IsClassicAppSupported>" +
                        "</platform>" +
                        "</GetFilteredApplications>" +
                        "</s:Body>" +
                        "</s:Envelope> ";

                    XmlDocument soapEnvelopeXml = new XmlDocument();
                    soapEnvelopeXml.LoadXml(sXMLEnvelope);

                    //Console.WriteLine(soapEnvelopeXml.InnerXml);

                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        soapEnvelopeXml.Save(stream);
                    }

                    using (WebResponse response = webRequest.GetResponse())
                    {
                        using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        {
                            SOAPResult = rd.ReadToEnd();
                        }
                    }
                }
                else
                    return lResult;

                if (!string.IsNullOrEmpty(SOAPResult))
                {
                    XmlDocument xResult = new XmlDocument();
                    xResult.LoadXml(SOAPResult);
                    XmlNamespaceManager manager = new XmlNamespaceManager(xResult.NameTable);
                    manager.AddNamespace("ns", "http://schemas.microsoft.com/5.0.0.0/ConfigurationManager/SoftwareCatalog/Website");

                    foreach (XmlNode xNode in xResult.SelectNodes("//ns:GetFilteredApplicationsResponse/ns:GetFilteredApplicationsResult/ns:AppDetailView", manager))
                    {
                        AppDetailView oApp = new AppDetailView();

                        FieldInfo[] properties = oApp.GetType().GetFields();

                        foreach (FieldInfo p in properties)
                        {
                            try
                            {
                                p.SetValue(oApp, xNode[p.Name].InnerText);
                            }
                            catch { }
                        }

                        lResult.Add(oApp);
                    }

                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return lResult;

        }

        /// <summary>
        /// Get ApplicationCIAssignment's from ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        /// <returns></returns>
        public List<CCM_ApplicationCIAssignment> ApplicationCIAssignment()
        {
            return ApplicationCIAssignment(false, new TimeSpan(0, 0, 30));
        }

        /// <summary>
        /// Get ApplicationCIAssignment's from ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        /// <param name="bReload">true = do not use cached Version</param>
        /// <returns></returns>
        public List<CCM_ApplicationCIAssignment> ApplicationCIAssignment(Boolean bReload)
        {
            return ApplicationCIAssignment(bReload, new TimeSpan(0, 0, 30));
        }

        /// <summary>
        /// Get ApplicationCIAssignment's from ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        /// <param name="bReload">true = do not use cached Version</param>
        /// <param name="CacheTime">TTL for caching</param>
        /// <returns></returns>
        public List<CCM_ApplicationCIAssignment> ApplicationCIAssignment(Boolean bReload, TimeSpan CacheTime)
        {
            List<CCM_ApplicationCIAssignment> lResult = new List<CCM_ApplicationCIAssignment>();

            List<PSObject> oObj = new List<PSObject>();

            oObj = GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * FROM CCM_ApplicationCIAssignment", bReload, CacheTime);
            foreach (PSObject PSObj in oObj)
            {
                try
                {
                    //Get CCM_ApplicationCIAssignment
                    CCM_ApplicationCIAssignment oApp = new CCM_ApplicationCIAssignment(PSObj, remoteRunspace, pSCode);

                    oApp.remoteRunspace = remoteRunspace;
                    oApp.pSCode = pSCode;
                    lResult.Add(oApp);
                }
                catch { }
            }

            return lResult;
        }

        /// <summary>
        /// ROOT\ccm\ClientSDK:CCM_SoftwareBase
        /// </summary>
        public class CCM_SoftwareBase
        {
            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;

#pragma warning disable 1591 // Disable warnings about missing XML comments

            public UInt32? ContentSize { get; set; }
            public DateTime? Deadline { get; set; }
            public String Description { get; set; }
            public UInt32? ErrorCode { get; set; }
            public UInt32? EstimatedInstallTime { get; set; }
            public UInt32? EvaluationState { get; set; }
            public String FullName { get; set; }
            public String Name { get; set; }
            public DateTime? NextUserScheduledTime { get; set; }
            public UInt32? PercentComplete { get; set; }
            public String Publisher { get; set; }
            public UInt32? Type { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            #endregion

            #region Methods

            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_SoftwareBase"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            public CCM_SoftwareBase(PSObject WMIObject)
            {
                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;

                this.ContentSize = WMIObject.Properties["ContentSize"].Value as uint?;

                string sDeadline = WMIObject.Properties["Deadline"].Value as string;
                if (string.IsNullOrEmpty(sDeadline))
                    this.Deadline = null;
                else
                {
                    this.Deadline = ManagementDateTimeConverter.ToDateTime(sDeadline) as DateTime?;
                    this.Deadline = ((DateTime)this.Deadline).ToUniversalTime();
                }

                this.Description = WMIObject.Properties["Description"].Value as string;
                this.ErrorCode = WMIObject.Properties["ErrorCode"].Value as uint?;
                this.EstimatedInstallTime = WMIObject.Properties["EstimatedInstallTime"].Value as uint?;
                this.EvaluationState = WMIObject.Properties["EvaluationState"].Value as uint?;
                this.FullName = WMIObject.Properties["FullName"].Value as string;
                this.Name = WMIObject.Properties["Name"].Value as string;
                string sNextUserScheduledTime = WMIObject.Properties["NextUserScheduledTime"].Value as string;
                if (string.IsNullOrEmpty(sNextUserScheduledTime))
                    this.NextUserScheduledTime = null;
                else
                    this.NextUserScheduledTime = ManagementDateTimeConverter.ToDateTime(sNextUserScheduledTime) as DateTime?;
                this.PercentComplete = WMIObject.Properties["PercentComplete"].Value as uint?;
                this.Publisher = WMIObject.Properties["Publisher"].Value as string;
                this.Type = WMIObject.Properties["Type"].Value as uint?;
            }
        }

        /// <summary>
        /// Class CCM_AppDeploymentType.
        /// </summary>
        public class CCM_AppDeploymentType : CCM_SoftwareBase
        {
            #region Properties

            /// <summary>
            /// Gets or sets the allowed actions.
            /// </summary>
            /// <value>The allowed actions.</value>
            public String[] AllowedActions { get; set; }

            /// <summary>
            /// Gets or sets the state of the applicability.
            /// </summary>
            /// <value>The state of the applicability.</value>
            public String ApplicabilityState { get; set; }

            /// <summary>
            /// Gets or sets the dependencies.
            /// </summary>
            /// <value>The dependencies.</value>
            public CCM_AppDeploymentType[] Dependencies { get; set; }
            /*{
                get
                {
                    //Get Dependencies sub Objects
                    List<CCM_AppDeploymentType> lTypes = new List<CCM_AppDeploymentType>();

                    baseInit oNewBase = new baseInit(remoteRunspace, pSCode);

                    List<PSObject> lPSAppDts = oNewBase.GetProperties(@"ROOT\ccm\clientsdk:" + this.__RELPATH, "Dependencies");
                    List<CCM_AppDeploymentType> lAppDTs = new List<CCM_AppDeploymentType>();
                    foreach (PSObject pAppDT in lPSAppDts)
                    {
                        lAppDTs.Add(new CCM_AppDeploymentType(pAppDT));
                    }
                    return lAppDTs.ToArray();
                }
                set
                {
                    Dependencies = value;
                }
            } */

            /// <summary>
            /// Gets or sets the deployment report.
            /// </summary>
            /// <value>The deployment report.</value>
            public String DeploymentReport { get; set; }


            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>The identifier.</value>
            public String Id { get; set; }

            /// <summary>
            /// Gets or sets the state of the install.
            /// </summary>
            /// <value>The state of the install.</value>
            public String InstallState { get; set; }

            /// <summary>
            /// Gets or sets the last eval time.
            /// </summary>
            /// <value>The last eval time.</value>
            public DateTime? LastEvalTime { get; set; }

            /// <summary>
            /// Gets or sets the post install action.
            /// </summary>
            /// <value>The post install action.</value>
            public String PostInstallAction { get; set; }

            /// <summary>
            /// Gets or sets the resolved state.
            /// </summary>
            /// <value>The resolved state.</value>
            public String ResolvedState { get; set; }

            /// <summary>
            /// Gets or sets the number of retries remaining.
            /// </summary>
            /// <value>The number of retries remaining.</value>
            public UInt32? RetriesRemaining { get; set; }

            /// <summary>
            /// Gets or sets the revision.
            /// </summary>
            /// <value>The revision.</value>
            public String Revision { get; set; }

            /// <summary>
            /// Gets or sets the supersession state.
            /// </summary>
            /// <value>The supersession state.</value>
            public String SupersessionState { get; set; }
            #endregion

            #region Methods
            /*
        public UInt32 GetProperty(UInt32 LanguageId, String PropertyName, out String PropertyValue)
        {
            PropertyValue = "";
            return 0;
        }*/
            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_SoftwareBase" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            public CCM_AppDeploymentType(PSObject WMIObject)
                : base(WMIObject)
            {

                this.WMIObject = WMIObject;

                this.AllowedActions = WMIObject.Properties["AllowedActions"].Value as string[];
                this.ApplicabilityState = WMIObject.Properties["ApplicabilityState"].Value as string;

                //Get Dependencies Objects
                try
                {
                    //baseInit oNewBase = new baseInit(remoteRunspace, pSCode);

                    if (WMIObject.Properties["Dependencies"].Value == null)
                    {
                        this.Dependencies = null;
                    }
                    else
                    {
                        List<CCM_AppDeploymentType> lAppDTs = new List<CCM_AppDeploymentType>();
                        foreach (PSObject pAppDT in WMIObject.Properties["Dependencies"].Value as PSObject[])
                        {
                            lAppDTs.Add(new CCM_AppDeploymentType(pAppDT));
                        }
                        this.Dependencies = lAppDTs.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

                this.DeploymentReport = WMIObject.Properties["DeploymentReport"].Value as string;
                this.Id = WMIObject.Properties["Id"].Value as string;
                this.InstallState = WMIObject.Properties["InstallState"].Value as String;
                string sLastEvalTime = WMIObject.Properties["LastEvalTime"].Value as string;
                if (string.IsNullOrEmpty(sLastEvalTime))
                    this.LastEvalTime = null;
                else
                    this.LastEvalTime = ManagementDateTimeConverter.ToDateTime(sLastEvalTime) as DateTime?;

                this.PostInstallAction = WMIObject.Properties["PostInstallAction"].Value as string;
                this.ResolvedState = WMIObject.Properties["ResolvedState"].Value as string;
                this.RetriesRemaining = WMIObject.Properties["RetriesRemaining"].Value as uint?;
                this.Revision = WMIObject.Properties["Revision"].Value as string;
                this.SupersessionState = WMIObject.Properties["SupersessionState"].Value as String;
            }

        }

        /// <summary>
        /// CCM_Application from ROOT\ccm\ClientSDK
        /// </summary>
        public class CCM_Application : CCM_SoftwareBase
        {
            internal baseInit oNewBase;

            #region Properties

#pragma warning disable 1591 // Disable warnings about missing XML comments

            public String[] AllowedActions { get; set; }
            public CCM_AppDeploymentType[] AppDTs { get; set; }
            public String ApplicabilityState { get; set; }
            public String DeploymentReport { get; set; }
            public UInt32? EnforcePreference { get; set; }
            public String FileTypes { get; set; }
            public String Icon { get; set; }
            public String Id { get; set; }
            public String InformativeUrl { get; set; }
            public String[] InProgressActions { get; set; }
            public String InstallState { get; set; }
            public Boolean? IsMachineTarget { get; set; }
            public Boolean? IsPreflightOnly { get; set; }
            public DateTime? LastEvalTime { get; set; }
            public DateTime? LastInstallTime { get; set; }
            public Boolean? NotifyUser { get; set; }
            public DateTime? ReleaseDate { get; set; }
            public String ResolvedState { get; set; }
            public String Revision { get; set; }
            public String SoftwareVersion { get; set; }
            public DateTime? StartTime { get; set; }
            public String SupersessionState { get; set; }
            public Boolean? UserUIExperience { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            /// <summary>
            /// Transalated EvaluationState into text from MSDN (http://msdn.microsoft.com/en-us/library/jj874280.aspx)
            /// </summary>
            public string EvaluationStateText
            {
                get
                {
                    switch (EvaluationState)
                    {
                        case 0:
                            return "No state information is available.";
                        case 1:
                            return "Application is enforced to desired/resolved state.";
                        case 2:
                            return "Application is not required on the client.";
                        case 3:
                            return "Application is available for enforcement (install or uninstall based on resolved state). Content may/may not have been downloaded.";
                        case 4:
                            return "Application last failed to enforce (install/uninstall).";
                        case 5:
                            return "Application is currently waiting for content download to complete.";
                        case 6:
                            return "Application is currently waiting for content download to complete.";
                        case 7:
                            return "Application is currently waiting for its dependencies to download.";
                        case 8:
                            return "Application is currently waiting for a service (maintenance) window.";
                        case 9:
                            return "Application is currently waiting for a previously pending reboot.";
                        case 10:
                            return "Application is currently waiting for serialized enforcement.";
                        case 11:
                            return "Application is currently enforcing dependencies.";
                        case 12:
                            return "Application is currently enforcing.";
                        case 13:
                            return "Application install/uninstall enforced and soft reboot is pending.";
                        case 14:
                            return "Application installed/uninstalled and hard reboot is pending.";
                        case 15:
                            return "Update is available but pending installation.";
                        case 16:
                            return "Application failed to evaluate.";
                        case 17:
                            return "Application is currently waiting for an active user session to enforce.";
                        case 18:
                            return "Application is currently waiting for all users to logoff.";
                        case 19:
                            return "Application is currently waiting for a user logon.";
                        case 20:
                            return "Application in progress, waiting for retry.";
                        case 21:
                            return "Application is waiting for presentation mode to be switched off.";
                        case 22:
                            return "Application is pre-downloading content (downloading outside of install job).";
                        case 23:
                            return "Application is pre-downloading dependent content (downloading outside of install job).";
                        case 24:
                            return "Application download failed (downloading during install job).";
                        case 25:
                            return "Application pre-downloading failed (downloading outside of install job).";
                        case 26:
                            return "Download success (downloading during install job).";
                        case 27:
                            return "Post-enforce evaluation.";
                        case 28:
                            return "Waiting for network connectivity.";
                        default:
                            return "Unknown state information.";

                    }
                }
            }
            /*
            public CCM_AppDeploymentType[] AppDTs
            {
                get
                {

                    //Get AppDTs sub Objects
                    List<CCM_AppDeploymentType> lAppDTs = new List<CCM_AppDeploymentType>();

                    if (remoteRunspace != null)
                    {
                        List<PSObject> lPSAppDts = oNewBase.GetProperties(@"ROOT\ccm\clientsdk:" + this.__RELPATH, "AppDTs");

                        foreach (PSObject pAppDT in lPSAppDts)
                        {
                            lAppDTs.Add(new CCM_AppDeploymentType(pAppDT));
                        }
                    }

                    return lAppDTs.ToArray();
                }

                set
                {
                } 
            } 
             * */
            #endregion

            /*
            #region Properties
            public String[] AllowedActions { get; set; }
            public String ApplicabilityState { get; set; }
            public String CurrentState { get; set; }
            public String DeploymentReport { get; set; }
            public UInt32? EnforcePreference { get; set; }
            public String FileTypes { get; set; }
            public String Icon { get; set; }
            public String Id { get; set; }
            public String InformativeUrl { get; set; }
            public Boolean? IsMachineTarget { get; set; }
            public DateTime? LastEvalTime { get; set; }
            public DateTime? LastInstallTime { get; set; }
            public Boolean? NotifyUser { get; set; }
            public String ProgressState { get; set; }
            public DateTime? ReleaseDate { get; set; }
            public String ResolvedState { get; set; }
            public String Revision { get; set; }
            public String SoftwareVersion { get; set; }
            public Boolean? UserUIExperience { get; set; }

            public CCM_AppDeploymentType[] AppDTs
            {
                get
                {
                    //Get AppDTs sub Objects
                    List<CCM_AppDeploymentType> lAppDTs = new List<CCM_AppDeploymentType>();

                    if (remoteRunspace != null)
                    {
                        List<PSObject> lPSAppDts = oNewBase.GetProperties(@"ROOT\ccm\clientsdk:" + this.__RELPATH, "AppDTs");

                        foreach (PSObject pAppDT in lPSAppDts)
                        {
                            lAppDTs.Add(new CCM_AppDeploymentType(pAppDT));
                        }
                    }
                    return lAppDTs.ToArray();
                }
                set
                {
                    AppDTs = value;
                }
            }
            #endregion

             * */
            #region Methods
            /*
        public UInt32 GetProperty(UInt32 LanguageId, String PropertyName, out String PropertyValue)
        {
            return 0;
        }
        public UInt32 DownloadContents(String Id, Boolean IsMachineTarget, String Priority, String Revision, out String JobId)
        {
            return 0;
        }
        public UInt32 Install(UInt32 EnforcePreference, String Id, Boolean IsMachineTarget, Boolean IsRebootIfNeeded, String Priority, String Revision, out String JobId)
        {
            return 0;
        }
        public UInt32 Cancel(String Id, Boolean IsMachineTarget, String Revision)
        {
            return 0;
        }
        public UInt32 Uninstall(UInt32 EnforcePreference, String Id, Boolean IsMachineTarget, Boolean IsRebootIfNeeded, String Priority, String Revision, out String JobId)
        {
            return 0;
        }
        public UInt32 GetPendingComponentList(String AppDeliveryTypeId, UInt32 Revision, out String PendingComponentList)
        {
            return 0;
        } */

            /// <summary>
            /// Install an Application
            /// </summary>
            /// <returns></returns>
            public string Install()
            {
                return Install(AppPriority.Normal, false);
            }

            /// <summary>
            /// Install an Application
            /// </summary>
            /// <param name="AppPriority">Foreground, High, Normal , Low</param>
            /// <param name="isRebootIfNeeded"></param>
            /// <returns></returns>
            public string Install(string AppPriority, bool isRebootIfNeeded)
            {
                if (string.IsNullOrEmpty(AppPriority))
                    AppPriority = "Normal";

                string sJobID = "";
                PSObject oResult = oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", "Install", "'" + Id + "', " + Revision + ", $" + IsMachineTarget.ToString() + ", " + AppEnforcePreference.Immediate + ", " + "'" + AppPriority + "'" + ", $" + isRebootIfNeeded.ToString());
                //sJobID = oResult.Properties["JobID"].Value.ToString();
                return sJobID;
            }

            /// <summary>
            /// Uninstall an Application
            /// </summary>
            /// <returns></returns>
            public string Uninstall()
            {
                return Uninstall(AppPriority.Normal, false);
            }

            /// <summary>
            /// Uninstall an Application
            /// </summary>
            /// <param name="AppPriority">Foreground, High, Normal , Low</param>
            /// <param name="isRebootIfNeeded"></param>
            /// <returns></returns>
            public string Uninstall(string AppPriority, bool isRebootIfNeeded)
            {
                bool bProcessed = false;
                List<PSObject> oObj = new List<PSObject>();
                List<CCM_ApplicationCIAssignment> lAssignments = new List<CCM_ApplicationCIAssignment>();

                oObj = oNewBase.GetObjects(@"ROOT\ccm\Policy\Machine\ActualConfig", "SELECT * FROM CCM_ApplicationCIAssignment", true);
                foreach (PSObject PSObj in oObj)
                {
                    try
                    {
                        //Get CCM_ApplicationCIAssignment

                        CCM_ApplicationCIAssignment oApp = new CCM_ApplicationCIAssignment(PSObj, remoteRunspace, pSCode);
                        oApp.remoteRunspace = remoteRunspace;
                        oApp.pSCode = pSCode;
                        if (Array.FindAll(oApp.AssignedCIs, s => s.IndexOf(Id.Split('_')[2]) >= 0).Count() > 0)
                        {
                            lAssignments.Add(oApp);
                            oNewBase.SetProperty(oApp.__NAMESPACE + ":" + oApp.__RELPATH.Replace("\"", "`\""), "EnforcementDeadline", "$null");
                            bProcessed = true;
                        }

                    }
                    catch { }
                }

                if (bProcessed)
                    System.Threading.Thread.Sleep(2000); //sleep 2s to process the changes

                foreach (PSObject PSObj in oObj)
                {
                    try
                    {
                        PSObject opResult = oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", "Uninstall", "'" + Id + "', " + Revision + ", $" + IsMachineTarget.ToString() + ", " + 1 + ", " + "'" + AppPriority + "'" + ", $" + isRebootIfNeeded.ToString());
                    }
                    catch { }
                }

                if (bProcessed)
                {
                    System.Threading.Thread.Sleep(2000); //sleep 2s to process the changes

                    foreach (PSObject PSObj in oObj)
                    {
                        try
                        {
                            //Get CCM_ApplicationCIAssignment
                            CCM_ApplicationCIAssignment oApp = new CCM_ApplicationCIAssignment(PSObj, remoteRunspace, pSCode);
                            oApp.remoteRunspace = remoteRunspace;
                            oApp.pSCode = pSCode;
                            if (Array.FindAll(oApp.AssignedCIs, s => s.IndexOf(Id.Split('_')[2]) >= 0).Count() > 0)
                            {
                                foreach (var oldAssignment in lAssignments.Where(t => t.AssignedCIs == oApp.AssignedCIs))
                                {
                                    oNewBase.SetProperty(oApp.__NAMESPACE + ":" + oApp.__RELPATH.Replace("\"", "`\""), "EnforcementDeadline", "\"" + oldAssignment.WMIObject.Properties["EnforcementDeadline"].Value.ToString() + "\"");
                                }
                            }
                        }
                        catch { }
                    }
                }

                /* if (!bProcessed)
                 {
                     PSObject opResult = oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", "Uninstall", "'" + Id + "', " + Revision + ", $" + IsMachineTarget.ToString() + ", " + AppEnforcePreference.Immediate + ", " + "'" + AppPriority + "'" + ", $" + isRebootIfNeeded.ToString());
                 }*/

                return "";
            }

            /// <summary>
            /// Cancel a Job -> Does not work !
            /// </summary>
            /// <returns></returns>
            public string Cancel()
            {
                PSObject oResult = oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", "Cancel", "'" + Id + "', " + Revision + ", $" + IsMachineTarget.ToString());

                return oResult.Properties["ReturnValue"].ToString();
            }

            /// <summary>
            /// Download Content
            /// </summary>
            /// <returns></returns>
            public string DownloadContents()
            {
                PSObject oResult = oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", "DownloadContents", "'" + Id + "', " + Revision + ", $" + IsMachineTarget.ToString() + ", 'Low'");

                return oResult.Properties["ReturnValue"].ToString();
            }

            #endregion


            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Application"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_Application(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                : base(WMIObject)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;
                oNewBase = new baseInit(remoteRunspace, pSCode);

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;

                List<string> lActions = new List<string>();
                foreach (string sAction in ((WMIObject.Properties["AllowedActions"].Value as PSObject).BaseObject as System.Collections.ArrayList))
                {
                    lActions.Add(sAction);
                }

                this.AllowedActions = lActions.ToArray();

                //Get AppDTs
                try
                {
                    //Get AppDTs sub Objects
                    List<CCM_AppDeploymentType> lAppDTs = new List<CCM_AppDeploymentType>();

                    if (remoteRunspace != null)
                    {
                        List<PSObject> lPSAppDts = oNewBase.GetProperties(@"ROOT\ccm\clientsdk:" + this.__RELPATH, "AppDTs");

                        foreach (PSObject pAppDT in lPSAppDts)
                        {
                            lAppDTs.Add(new CCM_AppDeploymentType(pAppDT));
                        }
                    }
                    this.AppDTs = lAppDTs.ToArray();
                }
                catch { }

                this.ApplicabilityState = WMIObject.Properties["ApplicabilityState"].Value as String;
                this.DeploymentReport = WMIObject.Properties["DeploymentReport"].Value as String;
                this.EnforcePreference = WMIObject.Properties["EnforcePreference"].Value as UInt32?;
                this.FileTypes = WMIObject.Properties["FileTypes"].Value as String;
                this.Icon = WMIObject.Properties["Icon"].Value as String;
                this.Id = WMIObject.Properties["Id"].Value as String;
                this.InformativeUrl = WMIObject.Properties["InformativeUrl"].Value as String;
                this.InProgressActions = WMIObject.Properties["InProgressActions"].Value as String[];
                this.InstallState = WMIObject.Properties["InstallState"].Value as String;
                this.IsMachineTarget = WMIObject.Properties["IsMachineTarget"].Value as Boolean?;
                this.IsPreflightOnly = WMIObject.Properties["IsPreflightOnly"].Value as Boolean?;
                string sLastEvalTime = WMIObject.Properties["LastEvalTime"].Value as string;
                if (string.IsNullOrEmpty(sLastEvalTime))
                    this.LastEvalTime = null;
                else
                {
                    try { this.LastEvalTime = ManagementDateTimeConverter.ToDateTime(sLastEvalTime) as DateTime?; }
                    catch { }
                }

                string sLastInstallTime = WMIObject.Properties["LastInstallTime"].Value as string;
                if (string.IsNullOrEmpty(sLastInstallTime))
                    this.LastInstallTime = null;
                else
                {
                    try { this.LastInstallTime = ManagementDateTimeConverter.ToDateTime(sLastInstallTime) as DateTime?; }
                    catch { }
                }
                this.NotifyUser = WMIObject.Properties["NotifyUser"].Value as Boolean?;
                string sReleaseDate = WMIObject.Properties["ReleaseDate"].Value as string;
                if (string.IsNullOrEmpty(sReleaseDate))
                    this.ReleaseDate = null;
                else
                {
                    try { this.ReleaseDate = ManagementDateTimeConverter.ToDateTime(sReleaseDate) as DateTime?; }
                    catch { };
                }
                this.ResolvedState = WMIObject.Properties["ResolvedState"].Value as String;
                this.Revision = WMIObject.Properties["Revision"].Value as String;
                this.SoftwareVersion = WMIObject.Properties["SoftwareVersion"].Value as String;
                string sStartTime = WMIObject.Properties["StartTime"].Value as string;
                if (string.IsNullOrEmpty(sStartTime))
                    this.StartTime = null;
                else
                {
                    try { this.StartTime = ManagementDateTimeConverter.ToDateTime(sStartTime) as DateTime?; }
                    catch { }
                }
                this.SupersessionState = WMIObject.Properties["SupersessionState"].Value as String;
                this.UserUIExperience = WMIObject.Properties["UserUIExperience"].Value as Boolean?;

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
            public CCM_Policy(PSObject WMIObject)
            {
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
        public class CCM_SoftwareDistribution : CCM_Policy
        {
            internal baseInit oNewBase;

            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_SoftwareDistribution"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_SoftwareDistribution(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                : base(WMIObject)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;
                oNewBase = new baseInit(remoteRunspace, pSCode);

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                string sADV_ActiveTime = WMIObject.Properties["ADV_ActiveTime"].Value as string;
                if (string.IsNullOrEmpty(sADV_ActiveTime))
                    this.ADV_ActiveTime = null;
                else
                    this.ADV_ActiveTime = ManagementDateTimeConverter.ToDateTime(sADV_ActiveTime) as DateTime?;
                this.ADV_ActiveTimeIsGMT = WMIObject.Properties["ADV_ActiveTimeIsGMT"].Value as Boolean?;
                this.ADV_ADF_Published = WMIObject.Properties["ADV_ADF_Published"].Value as Boolean?;
                this.ADV_ADF_RunNotification = WMIObject.Properties["ADV_ADF_RunNotification"].Value as Boolean?;
                this.ADV_AdvertisementID = WMIObject.Properties["ADV_AdvertisementID"].Value as String;
                string sADV_ExpirationTime = WMIObject.Properties["ADV_ExpirationTime"].Value as string;
                if (string.IsNullOrEmpty(sADV_ExpirationTime))
                    this.ADV_ExpirationTime = null;
                else
                    this.ADV_ExpirationTime = ManagementDateTimeConverter.ToDateTime(sADV_ExpirationTime) as DateTime?;
                this.ADV_ExpirationTimeIsGMT = WMIObject.Properties["ADV_ExpirationTimeIsGMT"].Value as Boolean?;
                this.ADV_FirstRunBehavior = WMIObject.Properties["ADV_FirstRunBehavior"].Value as String;
                this.ADV_MandatoryAssignments = WMIObject.Properties["ADV_MandatoryAssignments"].Value as Boolean?;
                this.ADV_ProgramWindowIsGMT = WMIObject.Properties["ADV_ProgramWindowIsGMT"].Value as Boolean?;
                string sADV_ProgramWindowStartTime = WMIObject.Properties["ADV_ProgramWindowStartTime"].Value as string;
                if (string.IsNullOrEmpty(sADV_ProgramWindowStartTime))
                    this.ADV_ProgramWindowStartTime = null;
                else
                    this.ADV_ProgramWindowStartTime = ManagementDateTimeConverter.ToDateTime(sADV_ProgramWindowStartTime) as DateTime?;
                string sADV_ProgramWindowStopTime = WMIObject.Properties["ADV_ProgramWindowStopTime"].Value as string;
                if (string.IsNullOrEmpty(sADV_ProgramWindowStopTime))
                    this.ADV_ProgramWindowStopTime = null;
                else
                    this.ADV_ProgramWindowStopTime = ManagementDateTimeConverter.ToDateTime(sADV_ProgramWindowStopTime) as DateTime?;
                this.ADV_RCF_InstallFromCDOptions = WMIObject.Properties["ADV_RCF_InstallFromCDOptions"].Value as String;
                this.ADV_RCF_InstallFromLocalDPOptions = WMIObject.Properties["ADV_RCF_InstallFromLocalDPOptions"].Value as String;
                this.ADV_RCF_InstallFromRemoteDPOptions = WMIObject.Properties["ADV_RCF_InstallFromRemoteDPOptions"].Value as String;
                this.ADV_RCF_PostponeToAC = WMIObject.Properties["ADV_RCF_PostponeToAC"].Value as Boolean?;
                this.ADV_RebootLogoffNotification = WMIObject.Properties["ADV_RebootLogoffNotification"].Value as Boolean?;
                this.ADV_RebootLogoffNotificationCountdownDuration = WMIObject.Properties["ADV_RebootLogoffNotificationCountdownDuration"].Value as UInt32?;
                this.ADV_RebootLogoffNotificationFinalWindow = WMIObject.Properties["ADV_RebootLogoffNotificationFinalWindow"].Value as UInt32?;
                this.ADV_RepeatRunBehavior = WMIObject.Properties["ADV_RepeatRunBehavior"].Value as String;
                this.ADV_RetryCount = WMIObject.Properties["ADV_RetryCount"].Value as UInt32?;
                this.ADV_RetryInterval = WMIObject.Properties["ADV_RetryInterval"].Value as UInt32?;
                this.ADV_RunNotificationCountdownDuration = WMIObject.Properties["ADV_RunNotificationCountdownDuration"].Value as UInt32?;
                this.PKG_ContentSize = WMIObject.Properties["PKG_ContentSize"].Value as UInt32?;
                this.PKG_Language = WMIObject.Properties["PKG_Language"].Value as String;
                this.PKG_Manufacturer = WMIObject.Properties["PKG_Manufacturer"].Value as String;
                this.PKG_MIFChecking = WMIObject.Properties["PKG_MIFChecking"].Value as Boolean?;
                this.PKG_MifFileName = WMIObject.Properties["PKG_MifFileName"].Value as String;
                this.PKG_MIFName = WMIObject.Properties["PKG_MIFName"].Value as String;
                this.PKG_MIFPublisher = WMIObject.Properties["PKG_MIFPublisher"].Value as String;
                this.PKG_MIFVersion = WMIObject.Properties["PKG_MIFVersion"].Value as String;
                this.PKG_Name = WMIObject.Properties["PKG_Name"].Value as String;
                this.PKG_PackageID = WMIObject.Properties["PKG_PackageID"].Value as String;
                this.PKG_PSF_ContainsSourceFiles = WMIObject.Properties["PKG_PSF_ContainsSourceFiles"].Value as Boolean?;
                this.PKG_SourceHash = WMIObject.Properties["PKG_SourceHash"].Value as String;
                this.PKG_SourceVersion = WMIObject.Properties["PKG_SourceVersion"].Value as String;
                this.PKG_version = WMIObject.Properties["PKG_version"].Value as String;
                this.PRG_Category = WMIObject.Properties["PRG_Category"].Value as String[];
                this.PRG_CommandLine = WMIObject.Properties["PRG_CommandLine"].Value as String;
                this.PRG_Comment = WMIObject.Properties["PRG_Comment"].Value as String;
                this.PRG_CustomLogoffReturnCodes = WMIObject.Properties["PRG_CustomLogoffReturnCodes"].Value as UInt32?[];
                this.PRG_CustomRebootReturnCodes = WMIObject.Properties["PRG_CustomRebootReturnCodes"].Value as UInt32?[];
                this.PRG_CustomSuccessReturnCodes = WMIObject.Properties["PRG_CustomSuccessReturnCodes"].Value as UInt32?[];
                this.PRG_DependentPolicy = WMIObject.Properties["PRG_DependentPolicy"].Value as Boolean?;
                this.PRG_DependentProgramPackageID = WMIObject.Properties["PRG_DependentProgramPackageID"].Value as String;
                this.PRG_DependentProgramProgramID = WMIObject.Properties["PRG_DependentProgramProgramID"].Value as String;
                this.PRG_DiskSpaceReq = WMIObject.Properties["PRG_DiskSpaceReq"].Value as String;
                this.PRG_DriveLetter = WMIObject.Properties["PRG_DriveLetter"].Value as String;
                this.PRG_ForceDependencyRun = WMIObject.Properties["PRG_ForceDependencyRun"].Value as Boolean?;
                this.PRG_HistoryLocation = WMIObject.Properties["PRG_HistoryLocation"].Value as String;
                this.PRG_MaxDuration = WMIObject.Properties["PRG_MaxDuration"].Value as UInt32?;
                this.PRG_PRF_AfterRunning = WMIObject.Properties["PRG_PRF_AfterRunning"].Value as String;
                this.PRG_PRF_Disabled = WMIObject.Properties["PRG_PRF_Disabled"].Value as Boolean?;
                this.PRG_PRF_InstallsApplication = WMIObject.Properties["PRG_PRF_InstallsApplication"].Value as Boolean?;
                this.PRG_PRF_MappedDriveRequired = WMIObject.Properties["PRG_PRF_MappedDriveRequired"].Value as Boolean?;
                this.PRG_PRF_PersistMappedDrive = WMIObject.Properties["PRG_PRF_PersistMappedDrive"].Value as Boolean?;
                this.PRG_PRF_RunNotification = WMIObject.Properties["PRG_PRF_RunNotification"].Value as Boolean?;
                this.PRG_PRF_RunWithAdminRights = WMIObject.Properties["PRG_PRF_RunWithAdminRights"].Value as Boolean?;
                this.PRG_PRF_ShowWindow = WMIObject.Properties["PRG_PRF_ShowWindow"].Value as String;
                this.PRG_PRF_UserInputRequired = WMIObject.Properties["PRG_PRF_UserInputRequired"].Value as Boolean?;
                this.PRG_PRF_UserLogonRequirement = WMIObject.Properties["PRG_PRF_UserLogonRequirement"].Value as String;
                this.PRG_ProgramID = WMIObject.Properties["PRG_ProgramID"].Value as String;
                this.PRG_ProgramName = WMIObject.Properties["PRG_ProgramName"].Value as String;
                this.PRG_Requirements = WMIObject.Properties["PRG_Requirements"].Value as String;
                this.PRG_ReturnCodesSource = WMIObject.Properties["PRG_ReturnCodesSource"].Value as String;
                this.PRG_WorkingDirectory = WMIObject.Properties["PRG_WorkingDirectory"].Value as String;

                this._RawObject = WMIObject;
            }

            #region Properties

#pragma warning disable 1591 // Disable warnings about missing XML comments

            public DateTime? ADV_ActiveTime { get; set; }
            public Boolean? ADV_ActiveTimeIsGMT { get; set; }
            public Boolean? ADV_ADF_Published { get; set; }
            public Boolean? ADV_ADF_RunNotification { get; set; }
            public String ADV_AdvertisementID { get; set; }
            public DateTime? ADV_ExpirationTime { get; set; }
            public Boolean? ADV_ExpirationTimeIsGMT { get; set; }
            public String ADV_FirstRunBehavior { get; set; }
            public Boolean? ADV_MandatoryAssignments { get; set; }
            public Boolean? ADV_ProgramWindowIsGMT { get; set; }
            public DateTime? ADV_ProgramWindowStartTime { get; set; }
            public DateTime? ADV_ProgramWindowStopTime { get; set; }
            public String ADV_RCF_InstallFromCDOptions { get; set; }
            public String ADV_RCF_InstallFromLocalDPOptions { get; set; }
            public String ADV_RCF_InstallFromRemoteDPOptions { get; set; }
            public Boolean? ADV_RCF_PostponeToAC { get; set; }
            public Boolean? ADV_RebootLogoffNotification { get; set; }
            public UInt32? ADV_RebootLogoffNotificationCountdownDuration { get; set; }
            public UInt32? ADV_RebootLogoffNotificationFinalWindow { get; set; }
            public String ADV_RepeatRunBehavior { get; set; }
            public UInt32? ADV_RetryCount { get; set; }
            public UInt32? ADV_RetryInterval { get; set; }
            public UInt32? ADV_RunNotificationCountdownDuration { get; set; }
            public UInt32? PKG_ContentSize { get; set; }
            public String PKG_Language { get; set; }
            public String PKG_Manufacturer { get; set; }
            public Boolean? PKG_MIFChecking { get; set; }
            public String PKG_MifFileName { get; set; }
            public String PKG_MIFName { get; set; }
            public String PKG_MIFPublisher { get; set; }
            public String PKG_MIFVersion { get; set; }
            public String PKG_Name { get; set; }
            public String PKG_PackageID { get; set; }
            public Boolean? PKG_PSF_ContainsSourceFiles { get; set; }
            public String PKG_SourceHash { get; set; }
            public String PKG_SourceVersion { get; set; }
            public String PKG_version { get; set; }
            public String[] PRG_Category { get; set; }
            public String PRG_CommandLine { get; set; }
            public String PRG_Comment { get; set; }
            public UInt32?[] PRG_CustomLogoffReturnCodes { get; set; }
            public UInt32?[] PRG_CustomRebootReturnCodes { get; set; }
            public UInt32?[] PRG_CustomSuccessReturnCodes { get; set; }
            public Boolean? PRG_DependentPolicy { get; set; }
            public String PRG_DependentProgramPackageID { get; set; }
            public String PRG_DependentProgramProgramID { get; set; }
            public String PRG_DiskSpaceReq { get; set; }
            public String PRG_DriveLetter { get; set; }
            public Boolean? PRG_ForceDependencyRun { get; set; }
            public String PRG_HistoryLocation { get; set; }
            public UInt32? PRG_MaxDuration { get; set; }
            public String PRG_PRF_AfterRunning { get; set; }
            public Boolean? PRG_PRF_Disabled { get; set; }
            public Boolean? PRG_PRF_InstallsApplication { get; set; }
            public Boolean? PRG_PRF_MappedDriveRequired { get; set; }
            public Boolean? PRG_PRF_PersistMappedDrive { get; set; }
            public Boolean? PRG_PRF_RunNotification { get; set; }
            public Boolean? PRG_PRF_RunWithAdminRights { get; set; }
            public String PRG_PRF_ShowWindow { get; set; }
            public Boolean? PRG_PRF_UserInputRequired { get; set; }
            public String PRG_PRF_UserLogonRequirement { get; set; }
            public String PRG_ProgramID { get; set; }
            public String PRG_ProgramName { get; set; }
            public String PRG_Requirements { get; set; }
            public String PRG_ReturnCodesSource { get; set; }
            public String PRG_WorkingDirectory { get; set; }

            public PSObject _RawObject { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            #endregion

            #region Methods

            /// <summary>
            /// CCM_Scheduler_ScheduledMessage object
            /// </summary>
            /// <returns></returns>
            public CCM_Scheduler_ScheduledMessage _ScheduledMessage()
            {
                try
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(PRG_Requirements);
                    string sSchedID = xDoc.SelectSingleNode("/SWDReserved/ScheduledMessageID").InnerText.ToString();
                    foreach (PSObject oObj in oNewBase.GetObjects(WMIObject.Properties["__NAMESPACE"].Value.ToString(), "SELECT * FROM CCM_Scheduler_ScheduledMessage WHERE ScheduledMessageID='" + sSchedID + "'"))
                    {
                        try
                        {
                            CCM_Scheduler_ScheduledMessage oMsg = new CCM_Scheduler_ScheduledMessage(oObj, remoteRunspace, pSCode);
                            oMsg.remoteRunspace = remoteRunspace;
                            oMsg.pSCode = pSCode;
                            return oMsg;
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
                return null;
            }

            /// <summary>
            /// Run (trigger) an advertisement
            /// </summary>
            public void TriggerSchedule(Boolean enforce)
            {
                try
                {
                    string sSchedule = _ScheduledMessage().ScheduledMessageID;

                    if (enforce)
                    {
                        string sPrgReq = PRG_Requirements;

                        sPrgReq = sPrgReq.Replace("<OverrideServiceWindows>FALSE</OverrideServiceWindows>", "<OverrideServiceWindows>TRUE</OverrideServiceWindows>");
                        string sPreReq2 = sPrgReq;
                        sPrgReq = sPrgReq.Replace("\r", "");
                        sPrgReq = sPrgReq.Replace("\n", "");
                        sPrgReq = sPrgReq.Replace("\t", "");
                        //sPrgReq = sPrgReq.Replace("\"", "\"\"");
                        sPrgReq = sPrgReq.Replace("\'", "\'\'");

                        try
                        {
                            oNewBase.SetProperty(this.__NAMESPACE + ":" + this.__RELPATH.Replace("\"", "'"), "ADV_RepeatRunBehavior", "'RerunAlways'");
                        }
                        catch { }
                        try
                        {
                            oNewBase.SetProperty(this.__NAMESPACE + ":" + this.__RELPATH.Replace("\"", "'"), "ADV_MandatoryAssignments", "$True");
                        }
                        catch { }
                        try
                        {
                            oNewBase.SetProperty(this.__NAMESPACE + ":" + this.__RELPATH.Replace("\"", "'"), "PRG_Requirements", "'" + sPrgReq + "'");
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.Message);
                        }

                        this.ADV_RepeatRunBehavior = "RerunAlways";
                        this.ADV_MandatoryAssignments = true;
                        this.PRG_Requirements = sPreReq2;

                        //Evaluate machine policy...
                        //oNewBase.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000022}'", true);

                        //Wait 0.5s and hope that to policy is updated...
                        System.Threading.Thread.Sleep(500);

                    }
                    //this.oNewBase.CallClassMethod(@"root\ccm\ClientSDK:CCM_ProgramsManager", "ExecuteProgram", "'" + PRG_ProgramID + "', '" + PKG_PackageID + "'");
                    this.oNewBase.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'" + sSchedule + "'");
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }

            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_TaskSequence : CCM_SoftwareDistribution
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_SoftwareDistribution" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_TaskSequence(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                : base(WMIObject, RemoteRunspace, PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.TS_BootImageID = WMIObject.Properties["TS_BootImageID"].Value as String;
                string sTS_Deadline = WMIObject.Properties["TS_Deadline"].Value as string;
                if (string.IsNullOrEmpty(sTS_Deadline))
                    this.TS_Deadline = null;
                else
                    this.TS_Deadline = ManagementDateTimeConverter.ToDateTime(sTS_Deadline) as DateTime?;
                this.TS_MandatoryCountdown = WMIObject.Properties["TS_MandatoryCountdown"].Value as UInt32?;
                this.TS_PopupReminderInterval = WMIObject.Properties["TS_PopupReminderInterval"].Value as UInt32?;
                this.TS_References = WMIObject.Properties["TS_References"].Value as String[];
                this.TS_Sequence = WMIObject.Properties["TS_Sequence"].Value as String;
                this.TS_Type = WMIObject.Properties["TS_Type"].Value as UInt32?;
                this.TS_UserNotificationFlags = WMIObject.Properties["TS_UserNotificationFlags"].Value as UInt32?;
            }

            #region Properties

#pragma warning disable 1591 // Disable warnings about missing XML comments

            public String Reserved { get; set; }
            public String TS_BootImageID { get; set; }
            public DateTime? TS_Deadline { get; set; }
            public UInt32? TS_MandatoryCountdown { get; set; }
            public UInt32? TS_PopupReminderInterval { get; set; }
            public String[] TS_References { get; set; }
            public String TS_Sequence { get; set; }
            public UInt32? TS_Type { get; set; }
            public UInt32? TS_UserNotificationFlags { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\ClientSDK
        /// </summary>
        public class CCM_Program : CCM_SoftwareBase
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="WMIObject"></param>
            /// <param name="RemoteRunspace"></param>
            /// <param name="PSCode"></param>
            public CCM_Program(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                : base(WMIObject)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                string sActivationTime = WMIObject.Properties["ActivationTime"].Value as string;
                if (string.IsNullOrEmpty(sActivationTime))
                    this.ActivationTime = null;
                else
                    this.ActivationTime = ManagementDateTimeConverter.ToDateTime(sActivationTime) as DateTime?;
                this.AdvertisedDirectly = WMIObject.Properties["AdvertisedDirectly"].Value as Boolean?;
                this.Categories = WMIObject.Properties["Categories"].Value as String[];
                this.CompletionAction = WMIObject.Properties["CompletionAction"].Value as UInt32?;
                this.Dependencies = WMIObject.Properties["Dependencies"].Value as CCM_Program[];
                this.DependentPackageID = WMIObject.Properties["DependentPackageID"].Value as String;
                this.DependentProgramID = WMIObject.Properties["DependentProgramID"].Value as String;
                this.DiskSpaceRequired = WMIObject.Properties["DiskSpaceRequired"].Value as String;
                this.Duration = WMIObject.Properties["Duration"].Value as UInt32?;
                string sExpirationTime = WMIObject.Properties["ExpirationTime"].Value as string;
                if (string.IsNullOrEmpty(sExpirationTime))
                    this.ExpirationTime = null;
                else
                    this.ExpirationTime = ManagementDateTimeConverter.ToDateTime(sExpirationTime) as DateTime?;
                this.ForceDependencyToRun = WMIObject.Properties["ForceDependencyToRun"].Value as Boolean?;
                this.HighImpact = WMIObject.Properties["HighImpact"].Value as Boolean?;
                this.LastExitCode = WMIObject.Properties["LastExitCode"].Value as UInt32?;
                this.LastRunStatus = WMIObject.Properties["LastRunStatus"].Value as String;
                string sLastRunTime = WMIObject.Properties["LastRunTime"].Value as string;
                if (string.IsNullOrEmpty(sLastRunTime))
                    this.LastRunTime = null;
                else
                    this.LastRunTime = ManagementDateTimeConverter.ToDateTime(sLastRunTime) as DateTime?;
                this.Level = WMIObject.Properties["Level"].Value as UInt32?;
                this.NotifyUser = WMIObject.Properties["NotifyUser"].Value as Boolean?;
                this.PackageID = WMIObject.Properties["PackageID"].Value as String;
                this.PackageLanguage = WMIObject.Properties["PackageLanguage"].Value as String;
                this.PackageName = WMIObject.Properties["PackageName"].Value as String;
                this.ProgramID = WMIObject.Properties["ProgramID"].Value as String;
                this.Published = WMIObject.Properties["Published"].Value as Boolean?;
                this.RepeatRunBehavior = WMIObject.Properties["RepeatRunBehavior"].Value as String;
                this.RequiresUserInput = WMIObject.Properties["RequiresUserInput"].Value as Boolean?;
                this.RunAtLogoff = WMIObject.Properties["RunAtLogoff"].Value as Boolean?;
                this.RunAtLogon = WMIObject.Properties["RunAtLogon"].Value as Boolean?;
                this.RunDependent = WMIObject.Properties["RunDependent"].Value as Boolean?;
                this.TaskSequence = WMIObject.Properties["TaskSequence"].Value as Boolean?;
                this.Version = WMIObject.Properties["Version"].Value as String;
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
            public DateTime? ActivationTime { get; set; }
            public Boolean? AdvertisedDirectly { get; set; }
            public String[] Categories { get; set; }
            public UInt32? CompletionAction { get; set; }
            public CCM_Program[] Dependencies { get; set; }
            public String DependentPackageID { get; set; }
            public String DependentProgramID { get; set; }
            public String DiskSpaceRequired { get; set; }
            public UInt32? Duration { get; set; }
            public DateTime? ExpirationTime { get; set; }
            public Boolean? ForceDependencyToRun { get; set; }
            public Boolean? HighImpact { get; set; }
            public UInt32? LastExitCode { get; set; }
            public String LastRunStatus { get; set; }
            public DateTime? LastRunTime { get; set; }
            public UInt32? Level { get; set; }
            public Boolean? NotifyUser { get; set; }
            public String PackageID { get; set; }
            public String PackageLanguage { get; set; }
            public String PackageName { get; set; }
            public String ProgramID { get; set; }
            public Boolean? Published { get; set; }
            public String RepeatRunBehavior { get; set; }
            public Boolean? RequiresUserInput { get; set; }
            public Boolean? RunAtLogoff { get; set; }
            public Boolean? RunAtLogon { get; set; }
            public Boolean? RunDependent { get; set; }
            public Boolean? TaskSequence { get; set; }
            public String Version { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            /// <summary>
            /// Transalated EvaluationState into text from MSDN (http://msdn.microsoft.com/en-us/library/jj874280.aspx)
            /// </summary>
            public string EvaluationStateText
            {
                get
                {
                    return "Unknown state information.";
                }
            }
            #endregion
        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_Scheduler_ScheduledMessage : CCM_Policy
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Scheduler_ScheduledMessage"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_Scheduler_ScheduledMessage(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                : base(WMIObject)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ActiveMessage = WMIObject.Properties["ActiveMessage"].Value as String;
                string sActiveTime = WMIObject.Properties["ActiveTime"].Value as string;
                if (string.IsNullOrEmpty(sActiveTime))
                    this.ActiveTime = null;
                else
                    this.ActiveTime = ManagementDateTimeConverter.ToDateTime(sActiveTime) as DateTime?;
                this.ActiveTimeIsGMT = WMIObject.Properties["ActiveTimeIsGMT"].Value as Boolean?;
                this.DeliverMode = WMIObject.Properties["DeliverMode"].Value as String;
                this.ExpireMessage = WMIObject.Properties["ExpireMessage"].Value as String;
                string sExpireTime = WMIObject.Properties["ExpireTime"].Value as string;
                if (string.IsNullOrEmpty(sExpireTime))
                    this.ExpireTime = null;
                else
                    this.ExpireTime = ManagementDateTimeConverter.ToDateTime(sExpireTime) as DateTime?;
                this.ExpireTimeIsGMT = WMIObject.Properties["ExpireTimeIsGMT"].Value as Boolean?;
                this.MessageName = WMIObject.Properties["MessageName"].Value as String;
                this.MessageTimeout = WMIObject.Properties["MessageTimeout"].Value as String;
                this.ReplyToEndpoint = WMIObject.Properties["ReplyToEndpoint"].Value as String;
                this.ScheduledMessageID = WMIObject.Properties["ScheduledMessageID"].Value as String;
                this.TargetEndpoint = WMIObject.Properties["TargetEndpoint"].Value as String;
                this.TriggerMessage = WMIObject.Properties["TriggerMessage"].Value as String;
                this.Triggers = WMIObject.Properties["Triggers"].Value as String[];

                this._RawObject = WMIObject;
            }

            #region Properties

#pragma warning disable 1591 // Disable warnings about missing XML comments

            public String ActiveMessage { get; set; }
            public DateTime? ActiveTime { get; set; }
            public Boolean? ActiveTimeIsGMT { get; set; }
            public String DeliverMode { get; set; }
            public String ExpireMessage { get; set; }
            public DateTime? ExpireTime { get; set; }
            public Boolean? ExpireTimeIsGMT { get; set; }
            public String MessageName { get; set; }
            public String MessageTimeout { get; set; }
            public String ReplyToEndpoint { get; set; }
            public String ScheduledMessageID { get; set; }
            public String TargetEndpoint { get; set; }
            public String TriggerMessage { get; set; }
            public String[] Triggers { get; set; }

            public PSObject _RawObject { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Scheduler
        /// </summary>
        public class CCM_Scheduler_History
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_Scheduler_History"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_Scheduler_History(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                string sActivationMessageSent = WMIObject.Properties["ActivationMessageSent"].Value as string;
                if (string.IsNullOrEmpty(sActivationMessageSent))
                    this.ActivationMessageSent = null;
                else
                    this.ActivationMessageSent = ManagementDateTimeConverter.ToDateTime(sActivationMessageSent) as DateTime?;
                this.ActivationMessageSentIsGMT = WMIObject.Properties["ActivationMessageSentIsGMT"].Value as Boolean?;
                string sExpirationMessageSent = WMIObject.Properties["ExpirationMessageSent"].Value as string;
                if (string.IsNullOrEmpty(sExpirationMessageSent))
                    this.ExpirationMessageSent = null;
                else
                    this.ExpirationMessageSent = ManagementDateTimeConverter.ToDateTime(sExpirationMessageSent) as DateTime?;
                this.ExpirationMessageSentIsGMT = WMIObject.Properties["ExpirationMessageSentIsGMT"].Value as Boolean?;
                string sFirstEvalTime = WMIObject.Properties["FirstEvalTime"].Value as string;
                if (string.IsNullOrEmpty(sFirstEvalTime))
                    this.FirstEvalTime = null;
                else
                    this.FirstEvalTime = ManagementDateTimeConverter.ToDateTime(sFirstEvalTime) as DateTime?;
                string sLastTriggerTime = WMIObject.Properties["LastTriggerTime"].Value as string;
                if (string.IsNullOrEmpty(sLastTriggerTime))
                    this.LastTriggerTime = null;
                else
                    this.LastTriggerTime = ManagementDateTimeConverter.ToDateTime(sLastTriggerTime) as DateTime?;
                this.ScheduleID = WMIObject.Properties["ScheduleID"].Value as String;
                this.TriggerState = WMIObject.Properties["TriggerState"].Value as String;
                this.UserSID = WMIObject.Properties["UserSID"].Value as String;
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

            public DateTime? ActivationMessageSent { get; set; }
            public Boolean? ActivationMessageSentIsGMT { get; set; }
            public DateTime? ExpirationMessageSent { get; set; }
            public Boolean? ExpirationMessageSentIsGMT { get; set; }
            public DateTime? FirstEvalTime { get; set; }
            public DateTime? LastTriggerTime { get; set; }
            public String ScheduleID { get; set; }
            public String TriggerState { get; set; }
            public String UserSID { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\ClientSDK
        /// </summary>
        public class CCM_SoftwareUpdate : CCM_SoftwareBase
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_SoftwareUpdate"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_SoftwareUpdate(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                : base(WMIObject)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ArticleID = WMIObject.Properties["ArticleID"].Value as String;
                this.BulletinID = WMIObject.Properties["BulletinID"].Value as String;
                this.ComplianceState = WMIObject.Properties["ComplianceState"].Value as UInt32?;
                this.ExclusiveUpdate = WMIObject.Properties["ExclusiveUpdate"].Value as Boolean?;
                this.MaxExecutionTime = WMIObject.Properties["MaxExecutionTime"].Value as UInt32?;
                this.NotifyUser = WMIObject.Properties["NotifyUser"].Value as Boolean?;
                this.OverrideServiceWindows = WMIObject.Properties["OverrideServiceWindows"].Value as Boolean?;
                this.RebootOutsideServiceWindows = WMIObject.Properties["RebootOutsideServiceWindows"].Value as Boolean?;
                string sRestartDeadline = WMIObject.Properties["RestartDeadline"].Value as string;
                if (string.IsNullOrEmpty(sRestartDeadline))
                    this.RestartDeadline = null;
                else
                    this.RestartDeadline = ManagementDateTimeConverter.ToDateTime(sRestartDeadline) as DateTime?;
                string sStartTime = WMIObject.Properties["StartTime"].Value as string;
                if (string.IsNullOrEmpty(sStartTime))
                    this.StartTime = null;
                else
                    this.StartTime = ManagementDateTimeConverter.ToDateTime(sStartTime) as DateTime?;
                this.UpdateID = WMIObject.Properties["UpdateID"].Value as String;
                this.URL = WMIObject.Properties["URL"].Value as String;
                this.UserUIExperience = WMIObject.Properties["UserUIExperience"].Value as Boolean?;
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

            public String ArticleID { get; set; }
            public String BulletinID { get; set; }
            public UInt32? ComplianceState { get; set; }
            public Boolean? ExclusiveUpdate { get; set; }
            public UInt32? MaxExecutionTime { get; set; }
            public Boolean? NotifyUser { get; set; }
            public Boolean? OverrideServiceWindows { get; set; }
            public Boolean? RebootOutsideServiceWindows { get; set; }
            public DateTime? RestartDeadline { get; set; }
            public DateTime? StartTime { get; set; }
            public String UpdateID { get; set; }
            public String URL { get; set; }
            public Boolean? UserUIExperience { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            /// <summary>
            /// Transalated EvaluationState into text from MSDN (http://msdn.microsoft.com/en-us/library/jj874280.aspx)
            /// </summary>
            public string EvaluationStateText
            {
                get
                {
                    switch (EvaluationState)
                    {
                        case 0: return "ciJobStateNone";
                        case 1: return "ciJobStateAvailable";
                        case 2: return "ciJobStateSubmitted";
                        case 3: return "ciJobStateDetecting";
                        case 4: return "ciJobStatePreDownload";
                        case 5: return "ciJobStateDownloading";
                        case 6: return "ciJobStateWaitInstall";
                        case 7: return "ciJobStateInstalling";
                        case 8: return "ciJobStatePendingSoftReboot";
                        case 9: return "ciJobStatePendingHardReboot";
                        case 10: return "ciJobStateWaitReboot";
                        case 11: return "ciJobStateVerifying";
                        case 12: return "ciJobStateInstallComplete";
                        case 13: return "ciJobStateError";
                        case 14: return "ciJobStateWaitServiceWindow";
                        case 15: return "ciJobStateWaitUserLogon";
                        case 16: return "ciJobStateWaitUserLogoff";
                        case 17: return "ciJobStateWaitJobUserLogon";
                        case 18: return "ciJobStateWaitUserReconnect";
                        case 19: return "ciJobStatePendingUserLogoff";
                        case 20: return "ciJobStatePendingUpdate";
                        case 21: return "ciJobStateWaitingRetry";
                        case 22: return "ciJobStateWaitPresModeOff";
                        case 23: return "ciJobStateWaitForOrchestration";
                        default:
                            return "Unknown state information.";
                    }
                }
            }
            #endregion

        }

        /// <summary>
        /// Application Priorities
        /// </summary>
        public static class AppPriority
        {
            /// <summary>
            /// Gets the low AppPriority.
            /// </summary>
            /// <value>The low AppPriority.</value>
            public static string Low
            {
                get { return "Low"; }
            }

            /// <summary>
            /// Gets the normal AppPriority.
            /// </summary>
            /// <value>The normal AppPriority.</value>
            public static string Normal
            {
                get { return "Normal"; }
            }

            /// <summary>
            /// Gets the high AppPriority.
            /// </summary>
            /// <value>The high AppPriority.</value>
            public static string High
            {
                get { return "High"; }
            }

            /// <summary>
            /// Gets the foreground AppPriority.
            /// </summary>
            /// <value>The foreground AppPriority.</value>
            public static string Foreground
            {
                get { return "Foreground"; }
            }
        }

        /// <summary>
        /// AppEnforcePreference
        /// </summary>
        public static class AppEnforcePreference
        {
            /// <summary>
            /// Gets the immediate AppEnforcePreference.
            /// </summary>
            /// <value>The immediate AppEnforcePreference.</value>
            public static UInt32 Immediate
            {
                get { return 0; }
            }

            /// <summary>
            /// Gets the non business hours AppEnforcePreference.
            /// </summary>
            /// <value>The non business hours AppEnforcePreference.</value>
            public static UInt32 NonBusinessHours
            {
                get { return 1; }
            }

            /// <summary>
            /// Gets the admin schedule AppEnforcePreference.
            /// </summary>
            /// <value>The admin schedule AppEnforcePreference.</value>
            public static UInt32 AdminSchedule
            {
                get { return 2; }
            }
        }

        /// <summary>
        /// Execution History
        /// </summary>
        public class REG_ExecutionHistory
        {
            internal baseInit oNewBase;

            internal string __RegPATH { get; set; }
            internal PSObject RegObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;

#pragma warning disable 1591 // Disable warnings about missing XML comments

            public String _ProgramID { get; set; }
            public String _State { get; set; }
            public DateTime? _RunStartTime { get; set; }
            public int? SuccessOrFailureCode { get; set; }
            public string SuccessOrFailureReason { get; set; }
            public string UserID { get; set; }
            public string PackageID { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            /// <summary>
            /// Initializes a new instance of the <see cref="REG_ExecutionHistory"/> class.
            /// </summary>
            /// <param name="RegObject">The reg object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public REG_ExecutionHistory(PSObject RegObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;
                oNewBase = new baseInit(remoteRunspace, pSCode);

                this.__RegPATH = (RegObject.Properties["PSPath"].Value as string).Replace("Microsoft.PowerShell.Core\\Registry::", "");
                this._ProgramID = RegObject.Properties["_ProgramID"].Value as string;
                this._State = RegObject.Properties["_State"].Value as string;
                this._RunStartTime = DateTime.Parse(RegObject.Properties["_RunStartTime"].Value as string);

                string sSuccessOrFailureCode = RegObject.Properties["SuccessOrFailureCode"].Value as string;
                if (!string.IsNullOrEmpty(sSuccessOrFailureCode))
                {
                    this.SuccessOrFailureCode = int.Parse(RegObject.Properties["SuccessOrFailureCode"].Value as string);
                }

                string sSuccessOrFailureReason = RegObject.Properties["SuccessOrFailureReason"].Value as string;
                if (!string.IsNullOrEmpty(sSuccessOrFailureReason))
                {
                    this.SuccessOrFailureReason = RegObject.Properties["SuccessOrFailureReason"].Value as string;
                }

                this.UserID = __RegPATH.Substring(__RegPATH.IndexOf("Execution History", StringComparison.CurrentCultureIgnoreCase)).Split('\\')[1];
                this.PackageID = __RegPATH.Substring(__RegPATH.IndexOf(UserID, StringComparison.CurrentCultureIgnoreCase)).Split('\\')[1];

            }

            /// <summary>
            /// Delete the Execution-History Item
            /// </summary>
            public void Delete()
            {
                try
                {
                    string sReg = __RegPATH.Replace(@"HKEY_LOCAL_MACHINE\", @"HKLM:\");
                    oNewBase.GetObjectsFromPS("Remove-Item \"" + sReg + "\" -Recurse", true, new TimeSpan(0, 0, 1));
                }
                catch { }
            }

            /// <summary>
            /// Translate the SID to a readable Username
            /// </summary>
            public void GetUserFromSID()
            {
                if (this.UserID.StartsWith("S-1-5-21-"))
                {
                    string sUser = oNewBase.GetStringFromPS(string.Format("((New-Object System.Security.Principal.SecurityIdentifier(\"{0}\")).Translate([System.Security.Principal.NTAccount])).value", this.UserID), false);
                    this.UserID = sUser;
                }
            }

        }

        /// <summary>
        /// Class SoftwareStatus.
        /// </summary>
        public class SoftwareStatus
        {
            /// <summary>
            /// Gets or sets the SoftwareStatus icon.
            /// </summary>
            /// <value>The SoftwareStatus icon.</value>
            public string Icon { get; set; }

            /// <summary>
            /// Gets or sets the SoftwareStatus name.
            /// </summary>
            /// <value>The SoftwareStatus name.</value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the SoftwareStatus type.
            /// </summary>
            /// <value>The SoftwareStatus type.</value>
            public string Type { get; set; }

            /// <summary>
            /// Gets or sets the SoftwareStatus publisher.
            /// </summary>
            /// <value>The SoftwareStatus publisher.</value>
            public string Publisher { get; set; }

            /// <summary>
            /// Gets or sets the SoftwareStatus availability.
            /// </summary>
            /// <value>The SoftwareStatus availability.</value>
            public DateTime? AvailableAfter { get; set; }

            /// <summary>
            /// Gets or sets the SoftwareStatus status.
            /// </summary>
            /// <value>The SoftwareStatus status.</value>
            public string Status { get; set; }

            /// <summary>
            /// Gets or sets the SoftwareStatus percent complete.
            /// </summary>
            /// <value>The SoftwareStatus percent complete.</value>
            public UInt32 PercentComplete { get; set; }

            /// <summary>
            /// Gets or sets the SoftwareStatus error code.
            /// </summary>
            /// <value>The SoftwareStatus error code.</value>
            public UInt32 ErrorCode { get; set; }

            private CCM_SoftwareBase _rawObject { get; set; }

            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            internal baseInit oNewBase;

            /// <summary>
            /// Initializes a new instance of the <see cref="SoftwareStatus"/> class.
            /// </summary>
            /// <param name="SWObject">The sw object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public SoftwareStatus(PSObject SWObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                try
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;
                    oNewBase = new baseInit(remoteRunspace, pSCode);
                    int iType = 0;
                    try
                    {
                        if (SWObject.Properties["Type"].Value == null)
                            Type = "";
                        string sType = SWObject.Properties["Type"].Value.ToString();
                        if (string.IsNullOrEmpty(sType))
                            sType = "99";

                        iType = int.Parse(sType);
                        switch (iType)
                        {
                            case 0:
                                Type = "Program";
                                _rawObject = new CCM_Program(SWObject, RemoteRunspace, PSCode);
                                Icon = "";
                                AvailableAfter = ((CCM_Program)_rawObject).ActivationTime;
                                Status = ((CCM_Program)_rawObject).LastRunStatus;
                                Name = ((CCM_Program)_rawObject).PackageName + ";" + ((CCM_Program)_rawObject).ProgramID;
                                if (Status == "Succeeded")
                                    Status = "Installed";
                                if (((CCM_Program)_rawObject).RepeatRunBehavior == "RerunAlways")
                                {
                                    if (((CCM_Program)_rawObject).Deadline != null)
                                    {
                                        if (((CCM_Program)_rawObject).Deadline > DateTime.Now)
                                        {
                                            Status = Status + "; waiting to install again at " + ((CCM_Program)_rawObject).Deadline.ToString();
                                        }
                                    }
                                }

                                break;
                            case 1:
                                Type = "Application";
                                _rawObject = new CCM_Application(SWObject, RemoteRunspace, PSCode);
                                Icon = SWObject.Properties["Icon"].Value as string;
                                AvailableAfter = ((CCM_Application)_rawObject).StartTime;
                                Name = SWObject.Properties["Name"].Value as string;
                                int EvaluationState = int.Parse(SWObject.Properties["EvaluationState"].Value.ToString());
                                switch (EvaluationState)
                                {
                                    case 0:
                                        Status = "Not Installed";
                                        break;
                                    case 1:
                                        Status = "Installed";
                                        break;
                                    case 2:
                                        Status = "Not required";
                                        break;
                                    case 3:
                                        Status = "Ready";
                                        break;
                                    case 4:
                                        Status = "Failed";
                                        break;
                                    case 5:
                                        Status = "Downloading content";
                                        break;
                                    case 6:
                                        Status = "Downloading content";
                                        break;
                                    case 7:
                                        Status = "Waiting";
                                        break;
                                    case 8:
                                        Status = "Waiting";
                                        break;
                                    case 9:
                                        Status = "Waiting";
                                        break;
                                    case 10:
                                        Status = "Waiting";
                                        break;
                                    case 11:
                                        Status = "Waiting";
                                        break;
                                    case 12:
                                        Status = "Installing";
                                        break;
                                    case 13:
                                        Status = "Reboot pending";
                                        break;
                                    case 14:
                                        Status = "Reboot pending";
                                        break;
                                    case 15:
                                        Status = "Waiting";
                                        break;
                                    case 16:
                                        Status = "Failed";
                                        break;
                                    case 17:
                                        Status = "Waiting";
                                        break;
                                    case 18:
                                        Status = "Waiting";
                                        break;
                                    case 19:
                                        Status = "Waiting";
                                        break;
                                    case 20:
                                        Status = "Waiting";
                                        break;
                                    case 21:
                                        Status = "Waiting";
                                        break;
                                    case 22:
                                        Status = "Downloading content";
                                        break;
                                    case 23:
                                        Status = "Downloading content";
                                        break;
                                    case 24:
                                        Status = "Failed";
                                        break;
                                    case 25:
                                        Status = "Failed";
                                        break;
                                    case 26:
                                        Status = "Waiting"; ;
                                        break;
                                    case 27:
                                        Status = "Waiting";
                                        break;
                                    case 28:
                                        Status = "Waiting";
                                        break;
                                    default:
                                        Status = "Unknown state information.";
                                        break;

                                }
                                break;
                            case 2:
                                Type = "Software Update";
                                _rawObject = new CCM_SoftwareUpdate(SWObject, RemoteRunspace, PSCode);
                                Icon = "Updates";
                                AvailableAfter = ((CCM_SoftwareUpdate)_rawObject).StartTime;
                                Name = SWObject.Properties["Name"].Value as string;
                                int EvaluationState2 = int.Parse(SWObject.Properties["EvaluationState"].Value.ToString());
                                switch (EvaluationState2)
                                {
                                    case 0:
                                        Status = "No State";
                                        try
                                        {
                                            if (SWObject.Properties["ComplianceState"].Value.ToString() == "0")
                                            {
                                                Status = "Missing";
                                            }
                                        }
                                        catch { }
                                        break;
                                    case 1:
                                        Status = "Missing";
                                        break;
                                    case 2:
                                        Status = "Ready";
                                        break;
                                    case 3:
                                        Status = "Detecting";
                                        break;
                                    case 4:
                                        Status = "Downloading content";
                                        break;
                                    case 5:
                                        Status = "Downloading content";
                                        break;
                                    case 6:
                                        Status = "Waiting";
                                        break;
                                    case 7:
                                        Status = "Installing";
                                        break;
                                    case 8:
                                        Status = "Reboot pending";
                                        break;
                                    case 9:
                                        Status = "Reboot pending";
                                        break;
                                    case 10:
                                        Status = "Reboot pending";
                                        break;
                                    case 11:
                                        Status = "Verifying";
                                        break;
                                    case 12:
                                        Status = "Installed";
                                        break;
                                    case 13:
                                        Status = "Failed";
                                        break;
                                    case 14:
                                        Status = "Waiting";
                                        break;
                                    case 15:
                                        Status = "Waiting";
                                        break;
                                    case 16:
                                        Status = "Waiting";
                                        break;
                                    case 17:
                                        Status = "Waiting";
                                        break;
                                    case 18:
                                        Status = "Waiting";
                                        break;
                                    case 19:
                                        Status = "Waiting";
                                        break;
                                    case 20:
                                        Status = "Pending";
                                        break;
                                    case 21:
                                        Status = "Waiting";
                                        break;
                                    case 22:
                                        Status = "Waiting";
                                        break;
                                    case 23:
                                        Status = "Waiting";
                                        break;
                                    default:
                                        Status = "Unknown state information.";
                                        break;
                                }
                                break;
                            case 3:
                                Type = "Program";
                                _rawObject = new CCM_Program(SWObject, RemoteRunspace, PSCode);
                                Icon = "";
                                if (((CCM_Program)_rawObject).TaskSequence == true)
                                {
                                    Type = "Operating System";
                                }
                                Status = ((CCM_Program)_rawObject).LastRunStatus;
                                AvailableAfter = ((CCM_Program)_rawObject).ActivationTime;
                                Name = SWObject.Properties["Name"].Value as string;
                                if (Status == "Succeeded")
                                    Status = "Installed";
                                if (((CCM_Program)_rawObject).RepeatRunBehavior == "RerunAlways")
                                {
                                    if (((CCM_Program)_rawObject).Deadline != null)
                                    {
                                        if (((CCM_Program)_rawObject).Deadline > DateTime.Now)
                                        {
                                            Status = Status + "; waiting to install again at " + ((CCM_Program)_rawObject).Deadline.ToString();
                                        }
                                    }
                                }
                                break;
                            default:
                                Type = "Unknown";
                                Icon = "";
                                Status = "";
                                break;
                        }
                    }
                    catch { }


                    Publisher = SWObject.Properties["Publisher"].Value as string;

                    try
                    {
                        ErrorCode = UInt32.Parse(SWObject.Properties["ErrorCode"].Value.ToString());
                    }
                    catch { }

                    try
                    {
                        PercentComplete = UInt32.Parse(SWObject.Properties["PercentComplete"].Value.ToString());
                    }
                    catch { }
                }
                catch { }
            }
        }

        /// <summary>
        /// Source:ROOT\ccm\cimodels
        /// </summary>
        public class Synclet
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="Synclet"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ClassName = WMIObject.Properties["ClassName"].Value as String;
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
            /// Gets or sets the name of the class.
            /// </summary>
            /// <value>The name of the class.</value>
            public String ClassName { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\cimodels
        /// </summary>
        public class CCM_HandlerSynclet : Synclet
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="Synclet" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_HandlerSynclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                : base(WMIObject, RemoteRunspace, PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ActionType = WMIObject.Properties["ActionType"].Value as String;
                this.AppDeliveryTypeId = WMIObject.Properties["AppDeliveryTypeId"].Value as String;
                this.ExecutionContext = WMIObject.Properties["ExecutionContext"].Value as String;
                this.RequiresLogOn = WMIObject.Properties["RequiresLogOn"].Value as String;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.Revision = WMIObject.Properties["Revision"].Value as UInt32?;
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

            public String ActionType { get; set; }
            public String AppDeliveryTypeId { get; set; }
            public String ExecutionContext { get; set; }
            public String RequiresLogOn { get; set; }
            public String Reserved { get; set; }
            public UInt32? Revision { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            #endregion

        }


        /// <summary>
        /// Source:ROOT\ccm\cimodels
        /// </summary>
        public class CCM_LocalInstallationSynclet : CCM_HandlerSynclet
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="Synclet" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_LocalInstallationSynclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                : base(WMIObject, RemoteRunspace, PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AllowedTarget = WMIObject.Properties["AllowedTarget"].Value as String;
                this.ExecuteTime = WMIObject.Properties["ExecuteTime"].Value as UInt32?;
                this.FastRetryExitCodes = WMIObject.Properties["FastRetryExitCodes"].Value as UInt32?[];
                this.HardRebootExitCodes = WMIObject.Properties["HardRebootExitCodes"].Value as UInt32?[];
                this.InstallCommandLine = WMIObject.Properties["InstallCommandLine"].Value as String;
                this.MaxExecuteTime = WMIObject.Properties["MaxExecuteTime"].Value as UInt32?;
                this.PostInstallbehavior = WMIObject.Properties["PostInstallbehavior"].Value as String;
                this.RebootExitCodes = WMIObject.Properties["RebootExitCodes"].Value as UInt32?[];
                this.RequiresElevatedRights = WMIObject.Properties["RequiresElevatedRights"].Value as Boolean?;
                this.RequiresReboot = WMIObject.Properties["RequiresReboot"].Value as Boolean?;
                this.RequiresUserInteraction = WMIObject.Properties["RequiresUserInteraction"].Value as Boolean?;
                this.RunAs32Bit = WMIObject.Properties["RunAs32Bit"].Value as Boolean?;
                this.SuccessExitCodes = WMIObject.Properties["SuccessExitCodes"].Value as UInt32?[];
                this.UserInteractionMode = WMIObject.Properties["UserInteractionMode"].Value as String;
                this.WorkingDirectory = WMIObject.Properties["WorkingDirectory"].Value as String;
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

            public String AllowedTarget { get; set; }
            public UInt32? ExecuteTime { get; set; }
            public UInt32?[] FastRetryExitCodes { get; set; }
            public UInt32?[] HardRebootExitCodes { get; set; }
            public String InstallCommandLine { get; set; }
            public UInt32? MaxExecuteTime { get; set; }
            public String PostInstallbehavior { get; set; }
            public UInt32?[] RebootExitCodes { get; set; }
            public Boolean? RequiresElevatedRights { get; set; }
            public Boolean? RequiresReboot { get; set; }
            public Boolean? RequiresUserInteraction { get; set; }
            public Boolean? RunAs32Bit { get; set; }
            public UInt32?[] SuccessExitCodes { get; set; }
            public String UserInteractionMode { get; set; }
            public String WorkingDirectory { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            #endregion

        }


        /// <summary>
        /// Source:ROOT\ccm\cimodels
        /// </summary>
        public class CCM_AppEnforceStatus
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_AppEnforceStatus"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_AppEnforceStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AppDeliveryTypeId = WMIObject.Properties["AppDeliveryTypeId"].Value as String;
                this.ExecutionStatus = WMIObject.Properties["ExecutionStatus"].Value as String;
                this.ExitCode = WMIObject.Properties["ExitCode"].Value as UInt32?;
                this.ReconnectData = WMIObject.Properties["ReconnectData"].Value as CCM_AppReconnectData_Base;
                this.Revision = WMIObject.Properties["Revision"].Value as UInt32?;
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

            public String AppDeliveryTypeId { get; set; }
            public String ExecutionStatus { get; set; }
            public UInt32? ExitCode { get; set; }
            public CCM_AppReconnectData_Base ReconnectData { get; set; }
            public UInt32? Revision { get; set; }

#pragma warning restore 1591 // Enable warnings about missing XML comments

            #endregion

        }


        /// <summary>
        /// Source:ROOT\ccm\cimodels
        /// </summary>
        public class CCM_AppReconnectData_Base
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_AppReconnectData_Base"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_AppReconnectData_Base(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AppDeliveryTypeId = WMIObject.Properties["AppDeliveryTypeId"].Value as String;
                this.Revision = WMIObject.Properties["Revision"].Value as UInt32?;
                this.UserSid = WMIObject.Properties["UserSid"].Value as String;
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
            /// Gets or sets the application delivery type identifier.
            /// </summary>
            /// <value>The application delivery type identifier.</value>
            public String AppDeliveryTypeId { get; set; }

            /// <summary>
            /// Gets or sets the revision.
            /// </summary>
            /// <value>The revision.</value>
            public UInt32? Revision { get; set; }

            /// <summary>
            /// Gets or sets the user sid.
            /// </summary>
            /// <value>The user sid.</value>
            public String UserSid { get; set; }
            #endregion

        }


        /// <summary>
        /// Source:ROOT\ccm\cimodels
        /// </summary>
        public class CCM_AppDeliveryType
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_AppDeliveryType"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_AppDeliveryType(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AppDeliveryTypeId = WMIObject.Properties["AppDeliveryTypeId"].Value as String;
                this.AppId = WMIObject.Properties["AppId"].Value as String;
                this.HostType = WMIObject.Properties["HostType"].Value as String;
                this.Revision = WMIObject.Properties["Revision"].Value as UInt32?;
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
            /// Gets or sets the application delivery type identifier.
            /// </summary>
            /// <value>The application delivery type identifier.</value>
            public String AppDeliveryTypeId { get; set; }

            /// <summary>
            /// Gets or sets the application identifier.
            /// </summary>
            /// <value>The application identifier.</value>
            public String AppId { get; set; }

            /// <summary>
            /// Gets or sets the type of the host.
            /// </summary>
            /// <value>The type of the host.</value>
            public String HostType { get; set; }

            /// <summary>
            /// Gets or sets the revision.
            /// </summary>
            /// <value>The revision.</value>
            public UInt32? Revision { get; set; }
            #endregion

            #region Methods
            /*
            public UInt32 GetContentInfo(String ActionType, String AppDeliveryTypeId, UInt32 Revision, out String ContentId, out String ContentVersion, out String ExcludedFileList, out Boolean ForceFileExclusion)
            {
                return 0;
            }
            public UInt32 EnforceApp(String ActionType, String AppDeliveryTypeId, String ContentPath, UInt32 Revision, UInt32 SessionId, String UserSid)
            {
                return 0;
            }
            public UInt32 GetMaxExecuteTime(String ActionType, String AppDeliveryTypeId, UInt32 Revision, out UInt32 MaxExecuteTime)
            {
                return 0;
            }
            public UInt32 GetPendingComponentList(String AppDeliveryTypeId, UInt32 Revision, out String PendingComponentList)
            {
                return 0;
            }
             * */
            #endregion
        }


        /// <summary>
        /// Source:ROOT\ccm\cimodels
        /// </summary>
        public class CCM_AppDeliveryTypeSynclet : Synclet
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="Synclet" /> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_AppDeliveryTypeSynclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode) : base(WMIObject, RemoteRunspace, PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AppDeliveryTypeId = WMIObject.Properties["AppDeliveryTypeId"].Value as String;
                this.AppDeliveryTypeName = WMIObject.Properties["AppDeliveryTypeName"].Value as String;
                this.AppId = WMIObject.Properties["AppId"].Value as String;
                this.DiscAction = WMIObject.Properties["DiscAction"].Value as CCM_AppAction;
                this.HostType = WMIObject.Properties["HostType"].Value as String;
                this.InstallAction = WMIObject.Properties["InstallAction"].Value as CCM_AppAction;
                this.Revision = WMIObject.Properties["Revision"].Value as UInt32?;
                this.UninstallAction = WMIObject.Properties["UninstallAction"].Value as CCM_AppAction;
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
            /// Gets or sets the application delivery type identifier.
            /// </summary>
            /// <value>The application delivery type identifier.</value>
            public String AppDeliveryTypeId { get; set; }

            /// <summary>
            /// Gets or sets the name of the application delivery type.
            /// </summary>
            /// <value>The name of the application delivery type.</value>
            public String AppDeliveryTypeName { get; set; }

            /// <summary>
            /// Gets or sets the application identifier.
            /// </summary>
            /// <value>The application identifier.</value>
            public String AppId { get; set; }

            /// <summary>
            /// Gets or sets the disc action.
            /// </summary>
            /// <value>The disc action.</value>
            public CCM_AppAction DiscAction { get; set; }

            /// <summary>
            /// Gets or sets the type of the host.
            /// </summary>
            /// <value>The type of the host.</value>
            public String HostType { get; set; }

            /// <summary>
            /// Gets or sets the install action.
            /// </summary>
            /// <value>The install action.</value>
            public CCM_AppAction InstallAction { get; set; }

            /// <summary>
            /// Gets or sets the revision.
            /// </summary>
            /// <value>The revision.</value>
            public UInt32? Revision { get; set; }

            /// <summary>
            /// Gets or sets the uninstall action.
            /// </summary>
            /// <value>The uninstall action.</value>
            public CCM_AppAction UninstallAction { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\cimodels
        /// </summary>
        public class CCM_AppAction
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_AppAction"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_AppAction(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ActionType = WMIObject.Properties["ActionType"].Value as String;
                this.Content = WMIObject.Properties["Content"].Value as ContentInfo;
                this.HandlerName = WMIObject.Properties["HandlerName"].Value as String;
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
            /// Gets or sets the type of the action.
            /// </summary>
            /// <value>The type of the action.</value>
            public String ActionType { get; set; }

            /// <summary>
            /// Gets or sets the content.
            /// </summary>
            /// <value>The content.</value>
            public ContentInfo Content { get; set; }

            /// <summary>
            /// Gets or sets the name of the handler.
            /// </summary>
            /// <value>The name of the handler.</value>
            public String HandlerName { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\cimodels
        /// </summary>
        public class ContentInfo
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="ContentInfo"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public ContentInfo(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ContentId = WMIObject.Properties["ContentId"].Value as String;
                this.ContentVersion = WMIObject.Properties["ContentVersion"].Value as String;
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
            /// Gets or sets the content identifier.
            /// </summary>
            /// <value>The content identifier.</value>
            public String ContentId { get; set; }

            /// <summary>
            /// Gets or sets the content version.
            /// </summary>
            /// <value>The content version.</value>
            public String ContentVersion { get; set; }
            #endregion

        }


        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_CIAssignment : CCM_Policy
        {
            /// <summary>
            /// CCM_CIAssignment
            /// </summary>
            /// <param name="WMIObject"></param>
            /// <param name="RemoteRunspace"></param>
            /// <param name="PSCode"></param>
            public CCM_CIAssignment(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode) : base(WMIObject)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;

                try
                {
                    //https://stackoverflow.com/questions/36422558/calling-dynamic-psobject-properties-in-c-sharp-returns-runtimebinderexception

                    object membersValue = WMIObject.Properties["AssignedCIs"].Value;
                    object[] members;
                    
                    // If the group has only one member, then it won't be an object array but rather a PSObject
                    if (membersValue.GetType() == typeof(PSObject))
                    {
                        members = new object[] { membersValue };
                    }
                    else
                    {
                        // The group has more than one member, in this case we can cast it to an object array
                        members = ((object[])membersValue);
                    }

                    this.AssignedCIs = Array.ConvertAll(members, x => x.ToString());
                }
                catch { }

                this.AssignmentAction = WMIObject.Properties["AssignmentAction"].Value as UInt32?;
                this.AssignmentFlags = WMIObject.Properties["AssignmentFlags"].Value as UInt64?;
                this.AssignmentID = WMIObject.Properties["AssignmentID"].Value as String;
                this.AssignmentName = WMIObject.Properties["AssignmentName"].Value as String;
                this.ConfigurationFlags = WMIObject.Properties["ConfigurationFlags"].Value as UInt64?;
                this.DesiredConfigType = WMIObject.Properties["DesiredConfigType"].Value as UInt32?;
                this.DisableMomAlerts = WMIObject.Properties["DisableMomAlerts"].Value as Boolean?;
                this.DPLocality = WMIObject.Properties["DPLocality"].Value as UInt32?;
                string sEnforcementDeadline = WMIObject.Properties["EnforcementDeadline"].Value as string;
                if (string.IsNullOrEmpty(sEnforcementDeadline))
                    this.EnforcementDeadline = null;
                else
                    this.EnforcementDeadline = ManagementDateTimeConverter.ToDateTime(sEnforcementDeadline) as DateTime?;
                string sExpirationTime = WMIObject.Properties["ExpirationTime"].Value as string;
                if (string.IsNullOrEmpty(sExpirationTime))
                    this.ExpirationTime = null;
                else
                    this.ExpirationTime = ManagementDateTimeConverter.ToDateTime(sExpirationTime) as DateTime?;
                this.LogComplianceToWinEvent = WMIObject.Properties["LogComplianceToWinEvent"].Value as Boolean?;
                this.NonComplianceCriticality = WMIObject.Properties["NonComplianceCriticality"].Value as UInt32?;
                this.NotifyUser = WMIObject.Properties["NotifyUser"].Value as Boolean?;
                this.OverrideServiceWindows = WMIObject.Properties["OverrideServiceWindows"].Value as Boolean?;
                this.PersistOnWriteFilterDevices = WMIObject.Properties["PersistOnWriteFilterDevices"].Value as Boolean?;
                this.RaiseMomAlertsOnFailure = WMIObject.Properties["RaiseMomAlertsOnFailure"].Value as Boolean?;
                this.RebootOutsideOfServiceWindows = WMIObject.Properties["RebootOutsideOfServiceWindows"].Value as Boolean?;
                this.Reserved1 = WMIObject.Properties["Reserved1"].Value as String;
                this.Reserved2 = WMIObject.Properties["Reserved2"].Value as String;
                this.Reserved3 = WMIObject.Properties["Reserved3"].Value as String;
                this.SendDetailedNonComplianceStatus = WMIObject.Properties["SendDetailedNonComplianceStatus"].Value as Boolean?;
                this.SettingTypes = WMIObject.Properties["SettingTypes"].Value as String;
                this.SoftDeadlineEnabled = WMIObject.Properties["SoftDeadlineEnabled"].Value as Boolean?;
                string sStartTime = WMIObject.Properties["StartTime"].Value as string;
                if (string.IsNullOrEmpty(sStartTime))
                    this.StartTime = null;
                else
                    this.StartTime = ManagementDateTimeConverter.ToDateTime(sStartTime) as DateTime?;
                this.StateMessagePriority = WMIObject.Properties["StateMessagePriority"].Value as UInt32?;
                this.SuppressReboot = WMIObject.Properties["SuppressReboot"].Value as UInt32?;
                string sUpdateDeadline = WMIObject.Properties["UpdateDeadline"].Value as string;
                if (string.IsNullOrEmpty(sUpdateDeadline))
                    this.UpdateDeadline = null;
                else
                    this.UpdateDeadline = ManagementDateTimeConverter.ToDateTime(sUpdateDeadline) as DateTime?;
                this.UseGMTTimes = WMIObject.Properties["UseGMTTimes"].Value as Boolean?;
                this.UseSiteEvaluation = WMIObject.Properties["UseSiteEvaluation"].Value as Boolean?;
                this.WoLEnabled = WMIObject.Properties["WoLEnabled"].Value as Boolean?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String[] AssignedCIs { get; set; }
            public UInt32? AssignmentAction { get; set; }
            public UInt64? AssignmentFlags { get; set; }
            public String AssignmentID { get; set; }
            public String AssignmentName { get; set; }
            public UInt64? ConfigurationFlags { get; set; }
            public UInt32? DesiredConfigType { get; set; }
            public Boolean? DisableMomAlerts { get; set; }
            public UInt32? DPLocality { get; set; }
            public DateTime? EnforcementDeadline { get; set; }
            public DateTime? ExpirationTime { get; set; }
            public Boolean? LogComplianceToWinEvent { get; set; }
            public UInt32? NonComplianceCriticality { get; set; }
            public Boolean? NotifyUser { get; set; }
            public Boolean? OverrideServiceWindows { get; set; }
            public Boolean? PersistOnWriteFilterDevices { get; set; }
            public Boolean? RaiseMomAlertsOnFailure { get; set; }
            public Boolean? RebootOutsideOfServiceWindows { get; set; }
            public String Reserved1 { get; set; }
            public String Reserved2 { get; set; }
            public String Reserved3 { get; set; }
            public Boolean? SendDetailedNonComplianceStatus { get; set; }
            public String SettingTypes { get; set; }
            public Boolean? SoftDeadlineEnabled { get; set; }
            public DateTime? StartTime { get; set; }
            public UInt32? StateMessagePriority { get; set; }
            public UInt32? SuppressReboot { get; set; }
            public DateTime? UpdateDeadline { get; set; }
            public Boolean? UseGMTTimes { get; set; }
            public Boolean? UseSiteEvaluation { get; set; }
            public Boolean? WoLEnabled { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_ApplicationCIAssignment : CCM_CIAssignment
        {
            /// <summary>
            /// CCM_ApplicationCIAssignment
            /// </summary>
            /// <param name="WMIObject"></param>
            /// <param name="RemoteRunspace"></param>
            /// <param name="PSCode"></param>
            public CCM_ApplicationCIAssignment(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode) : base(WMIObject, RemoteRunspace, PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.Priority = WMIObject.Properties["Priority"].Value as UInt32?;
                this.Reserved4 = WMIObject.Properties["Reserved4"].Value as String;
                this.UserUIExperience = WMIObject.Properties["UserUIExperience"].Value as Boolean?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public UInt32? Priority { get; set; }
            public String Reserved4 { get; set; }
            public Boolean? UserUIExperience { get; set; }
            #endregion

        }

        /// <summary>
        /// AppDetailView
        /// </summary>
        public class AppDetailView
        {
            public string Category;
            public string Publisher;
            public string ApplicationId;
            public string Description;
            public string Name;
            public string AppVersion;
            public string AvailableDate;
            public string ApplicationDefinitionVersion;
            public string IsAppModelApplication;
            public string PackageName;
            public string ImagePath;
            public string AutoApproval;
            public string SupersededApplications;
            public string Icon;
            public string DeploymentTypeExperience;
            public string InstallationState;
            public string InstallationErrorCode;

        }


    }
#endif
}