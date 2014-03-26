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

using System.Drawing;
using System.Xml;

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
        public List<CCM_Application> Applications_(Boolean bReload)
        {

            List<CCM_Application> lApps = new List<CCM_Application>();
            List<PSObject> oObj = new List<PSObject>();

            oObj = GetObjects(@"ROOT\ccm\ClientSDK", "SELECT * FROM CCM_Application", bReload);

            foreach (PSObject PSObj in oObj)
            {
                //Get AppDTs sub Objects
                CCM_Application oApp = new CCM_Application(PSObj, remoteRunspace, pSCode);

                oApp.remoteRunspace = remoteRunspace;
                oApp.pSCode = pSCode;
                lApps.Add(oApp);
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
                    this.Deadline = ManagementDateTimeConverter.ToDateTime(sDeadline) as DateTime?;

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
                if (string.IsNullOrEmpty(AppPriority))
                    AppPriority = "Normal";

                string sJobID = "";
                PSObject oResult = oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", "Uninstall", "'" + Id + "', " + Revision + ", $" + IsMachineTarget.ToString() + ", " + AppEnforcePreference.Immediate + ", " + "'" + AppPriority + "'" + ", $" + isRebootIfNeeded.ToString());
                return sJobID;
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
                //foreach (string sAction in WMIObject.Properties["AllowedActions"].Value as string[])
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
                catch(Exception ex)
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


        /*
            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class AppV_Detect_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public AppV_Detect_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.PackageGUID = WMIObject.Properties["PackageGUID"].Value as String;
                    this.PublishComponents = WMIObject.Properties["PublishComponents"].Value as String[];
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
                public String PackageGUID { get; set; }
                public String[] PublishComponents { get; set; }
                public String VersionGUID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class AppV_Install_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public AppV_Install_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ContentFile = WMIObject.Properties["ContentFile"].Value as String;
                    this.ExecuteTime = WMIObject.Properties["ExecuteTime"].Value as UInt32?;
                    this.ExtendedData = WMIObject.Properties["ExtendedData"].Value as String;
                    this.ManifestFile = WMIObject.Properties["ManifestFile"].Value as String;
                    this.PackageGUID = WMIObject.Properties["PackageGUID"].Value as String;
                    this.PublishComponents = WMIObject.Properties["PublishComponents"].Value as String[];
                    this.RequireLoad = WMIObject.Properties["RequireLoad"].Value as Boolean?;
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
                public String ContentFile { get; set; }
                public UInt32? ExecuteTime { get; set; }
                public String ExtendedData { get; set; }
                public String ManifestFile { get; set; }
                public String PackageGUID { get; set; }
                public String[] PublishComponents { get; set; }
                public Boolean? RequireLoad { get; set; }
                public String VersionGUID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class AppV_Uninstall_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public AppV_Uninstall_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ContentFile = WMIObject.Properties["ContentFile"].Value as String;
                    this.ExtendedData = WMIObject.Properties["ExtendedData"].Value as String;
                    this.ManifestFile = WMIObject.Properties["ManifestFile"].Value as String;
                    this.PackageGUID = WMIObject.Properties["PackageGUID"].Value as String;
                    this.PublishComponents = WMIObject.Properties["PublishComponents"].Value as String[];
                    this.RequireLoad = WMIObject.Properties["RequireLoad"].Value as Boolean?;
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
                public String ContentFile { get; set; }
                public String ExtendedData { get; set; }
                public String ManifestFile { get; set; }
                public String PackageGUID { get; set; }
                public String[] PublishComponents { get; set; }
                public Boolean? RequireLoad { get; set; }
                public String VersionGUID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class AppV5X_Detect_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public AppV5X_Detect_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.PackageGUID = WMIObject.Properties["PackageGUID"].Value as String;
                    this.PackageName = WMIObject.Properties["PackageName"].Value as String;
                    this.PublishComponents = WMIObject.Properties["PublishComponents"].Value as String[];
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
                public String PackageGUID { get; set; }
                public String PackageName { get; set; }
                public String[] PublishComponents { get; set; }
                public String VersionGUID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class AppV5X_Install_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public AppV5X_Install_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ContentFile = WMIObject.Properties["ContentFile"].Value as String;
                    this.DeploymentConfigFile = WMIObject.Properties["DeploymentConfigFile"].Value as String;
                    this.ExecuteTime = WMIObject.Properties["ExecuteTime"].Value as UInt32?;
                    this.PackageGUID = WMIObject.Properties["PackageGUID"].Value as String;
                    this.PackageName = WMIObject.Properties["PackageName"].Value as String;
                    this.PublishComponents = WMIObject.Properties["PublishComponents"].Value as String[];
                    this.RequireLoad = WMIObject.Properties["RequireLoad"].Value as Boolean?;
                    this.UserConfigFile = WMIObject.Properties["UserConfigFile"].Value as String;
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
                public String ContentFile { get; set; }
                public String DeploymentConfigFile { get; set; }
                public UInt32? ExecuteTime { get; set; }
                public String PackageGUID { get; set; }
                public String PackageName { get; set; }
                public String[] PublishComponents { get; set; }
                public Boolean? RequireLoad { get; set; }
                public String UserConfigFile { get; set; }
                public String VersionGUID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class AppV5X_Uninstall_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public AppV5X_Uninstall_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ContentFile = WMIObject.Properties["ContentFile"].Value as String;
                    this.DeploymentConfigFile = WMIObject.Properties["DeploymentConfigFile"].Value as String;
                    this.PackageGUID = WMIObject.Properties["PackageGUID"].Value as String;
                    this.PackageName = WMIObject.Properties["PackageName"].Value as String;
                    this.PublishComponents = WMIObject.Properties["PublishComponents"].Value as String[];
                    this.UserConfigFile = WMIObject.Properties["UserConfigFile"].Value as String;
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
                public String ContentFile { get; set; }
                public String DeploymentConfigFile { get; set; }
                public String PackageGUID { get; set; }
                public String PackageName { get; set; }
                public String[] PublishComponents { get; set; }
                public String UserConfigFile { get; set; }
                public String VersionGUID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class AssemblyObj
            {
                //Constructor
                public AssemblyObj(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Culture = WMIObject.Properties["Culture"].Value as String;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.PublicKeyToken = WMIObject.Properties["PublicKeyToken"].Value as String;
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
                public String Culture { get; set; }
                public String InstancePath { get; set; }
                public String PublicKeyToken { get; set; }
                public String Version { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_ADQuery_Setting_Boolean : CCM_Setting_Boolean
            {
                //Constructor
                public CCM_ADQuery_Setting_Boolean(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_ADQuery_Setting_DateTime : CCM_Setting_DateTime
            {
                //Constructor
                public CCM_ADQuery_Setting_DateTime(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_ADQuery_Setting_Double : CCM_Setting_Double
            {
                //Constructor
                public CCM_ADQuery_Setting_Double(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_ADQuery_Setting_Integer : CCM_Setting_Integer
            {
                //Constructor
                public CCM_ADQuery_Setting_Integer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_ADQuery_Setting_IntegerArray : CCM_Setting_IntegerArray
            {
                //Constructor
                public CCM_ADQuery_Setting_IntegerArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_ADQuery_Setting_String : CCM_Setting_String
            {
                //Constructor
                public CCM_ADQuery_Setting_String(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_ADQuery_Setting_StringArray : CCM_Setting_StringArray
            {
                //Constructor
                public CCM_ADQuery_Setting_StringArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_ADQuery_Setting_Synclet
            {
                //Constructor
                public CCM_ADQuery_Setting_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Attribute = WMIObject.Properties["Attribute"].Value as String;
                    this.Depth = WMIObject.Properties["Depth"].Value as byte?;
                    this.dnName = WMIObject.Properties["dnName"].Value as String;
                    this.Filter = WMIObject.Properties["Filter"].Value as String;
                    this.ID = WMIObject.Properties["ID"].Value as String;
                    this.LDAPPrefix = WMIObject.Properties["LDAPPrefix"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String Attribute { get; set; }
                public byte? Depth { get; set; }
                public String dnName { get; set; }
                public String Filter { get; set; }
                public String ID { get; set; }
                public String LDAPPrefix { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_AppAction
            {
                //Constructor
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
                public String ActionType { get; set; }
                public ContentInfo Content { get; set; }
                public String HandlerName { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_AppDeliveryType
            {
                //Constructor
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
                public String AppDeliveryTypeId { get; set; }
                public String AppId { get; set; }
                public String HostType { get; set; }
                public UInt32? Revision { get; set; }
                #endregion

                #region Methods

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
                #endregion
            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_AppDeliveryTypeSynclet : Synclet
            {
                //Constructor
                public CCM_AppDeliveryTypeSynclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
                public String AppDeliveryTypeId { get; set; }
                public String AppDeliveryTypeName { get; set; }
                public String AppId { get; set; }
                public CCM_AppAction DiscAction { get; set; }
                public String HostType { get; set; }
                public CCM_AppAction InstallAction { get; set; }
                public UInt32? Revision { get; set; }
                public CCM_AppAction UninstallAction { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_AppEnforceStatus
            {
                //Constructor
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
                public String AppDeliveryTypeId { get; set; }
                public String ExecutionStatus { get; set; }
                public UInt32? ExitCode { get; set; }
                public CCM_AppReconnectData_Base ReconnectData { get; set; }
                public UInt32? Revision { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_AppHandlers
            {
                //Constructor
                public CCM_AppHandlers(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.HandlerCLSID = WMIObject.Properties["HandlerCLSID"].Value as String;
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
                public String HandlerCLSID { get; set; }
                public String HandlerName { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_AppProductSource
            {
                //Constructor
                public CCM_AppProductSource(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AppDeliveryTypeId = WMIObject.Properties["AppDeliveryTypeId"].Value as String;
                    this.ProductCode = WMIObject.Properties["ProductCode"].Value as String;
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
                public String AppDeliveryTypeId { get; set; }
                public String ProductCode { get; set; }
                public String Reserved { get; set; }
                public UInt32? Revision { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_AppReconnectData_Base
            {
                //Constructor
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
                public String AppDeliveryTypeId { get; set; }
                public UInt32? Revision { get; set; }
                public String UserSid { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_AppReconnectData_MSI : CCM_AppReconnectData_Base
            {
                //Constructor
                public CCM_AppReconnectData_MSI(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AdvertiseFileName = WMIObject.Properties["AdvertiseFileName"].Value as String;
                    string sCreationTime = WMIObject.Properties["CreationTime"].Value as string;
                    if (string.IsNullOrEmpty(sCreationTime))
                        this.CreationTime = null;
                    else
                        this.CreationTime = ManagementDateTimeConverter.ToDateTime(sCreationTime) as DateTime?;
                    this.ProcessId = WMIObject.Properties["ProcessId"].Value as UInt32?;
                    this.ProductCode = WMIObject.Properties["ProductCode"].Value as String;
                    this.ProgramRestart = WMIObject.Properties["ProgramRestart"].Value as Boolean?;
                    this.UserDT = WMIObject.Properties["UserDT"].Value as Boolean?;
                    this.UserInteraction = WMIObject.Properties["UserInteraction"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String AdvertiseFileName { get; set; }
                public DateTime? CreationTime { get; set; }
                public UInt32? ProcessId { get; set; }
                public String ProductCode { get; set; }
                public Boolean? ProgramRestart { get; set; }
                public Boolean? UserDT { get; set; }
                public Boolean? UserInteraction { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_AppReconnectData_Script : CCM_AppReconnectData_Base
            {
                //Constructor
                public CCM_AppReconnectData_Script(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AdvertiseFileName = WMIObject.Properties["AdvertiseFileName"].Value as String;
                    string sCreationTime = WMIObject.Properties["CreationTime"].Value as string;
                    if (string.IsNullOrEmpty(sCreationTime))
                        this.CreationTime = null;
                    else
                        this.CreationTime = ManagementDateTimeConverter.ToDateTime(sCreationTime) as DateTime?;
                    this.ProcessId = WMIObject.Properties["ProcessId"].Value as UInt32?;
                    this.ProductCode = WMIObject.Properties["ProductCode"].Value as String;
                    this.ProgramRestart = WMIObject.Properties["ProgramRestart"].Value as Boolean?;
                    this.UserDT = WMIObject.Properties["UserDT"].Value as Boolean?;
                    this.UserInteraction = WMIObject.Properties["UserInteraction"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String AdvertiseFileName { get; set; }
                public DateTime? CreationTime { get; set; }
                public UInt32? ProcessId { get; set; }
                public String ProductCode { get; set; }
                public Boolean? ProgramRestart { get; set; }
                public Boolean? UserDT { get; set; }
                public Boolean? UserInteraction { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_AppXInstallInfo
            {
                //Constructor
                public CCM_AppXInstallInfo(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AppDeliveryTypeId = WMIObject.Properties["AppDeliveryTypeId"].Value as String;
                    this.IsBundle = WMIObject.Properties["IsBundle"].Value as Boolean?;
                    this.IsDeepLink = WMIObject.Properties["IsDeepLink"].Value as Boolean?;
                    this.Name = WMIObject.Properties["Name"].Value as String;
                    this.ProcessorArchitecture = WMIObject.Properties["ProcessorArchitecture"].Value as String;
                    this.Publisher = WMIObject.Properties["Publisher"].Value as String;
                    this.ResourceId = WMIObject.Properties["ResourceId"].Value as String;
                    this.Revision = WMIObject.Properties["Revision"].Value as UInt32?;
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
                public String AppDeliveryTypeId { get; set; }
                public Boolean? IsBundle { get; set; }
                public Boolean? IsDeepLink { get; set; }
                public String Name { get; set; }
                public String ProcessorArchitecture { get; set; }
                public String Publisher { get; set; }
                public String ResourceId { get; set; }
                public UInt32? Revision { get; set; }
                public String Version { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Assembly_Setting : CCM_Setting
            {
                //Constructor
                public CCM_Assembly_Setting(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as AssemblyObj[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public AssemblyObj[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Assembly_Setting_Synclet
            {
                //Constructor
                public CCM_Assembly_Setting_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AssemblyName = WMIObject.Properties["AssemblyName"].Value as String;
                    this.ID = WMIObject.Properties["ID"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String AssemblyName { get; set; }
                public String ID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Certificate
            {
                //Constructor
                public CCM_Certificate(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Blob = WMIObject.Properties["Blob"].Value as String;
                    this.IsInstalled = WMIObject.Properties["IsInstalled"].Value as Boolean?;
                    this.StoreLocation = WMIObject.Properties["StoreLocation"].Value as byte?;
                    this.StoreName = WMIObject.Properties["StoreName"].Value as String;
                    this.Thumbprint = WMIObject.Properties["Thumbprint"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String Blob { get; set; }
                public Boolean? IsInstalled { get; set; }
                public byte? StoreLocation { get; set; }
                public String StoreName { get; set; }
                public String Thumbprint { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_CertificateEnrollment
            {
                //Constructor
                public CCM_CertificateEnrollment(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ConfigurationParameters = WMIObject.Properties["ConfigurationParameters"].Value as String;
                    this.EnhancedKeyUsages = WMIObject.Properties["EnhancedKeyUsages"].Value as String;
                    this.Error = WMIObject.Properties["Error"].Value as UInt32?;
                    this.ExpirationThreshold = WMIObject.Properties["ExpirationThreshold"].Value as UInt32?;
                    this.Issuers = WMIObject.Properties["Issuers"].Value as String;
                    this.RequestID = WMIObject.Properties["RequestID"].Value as String;
                    this.RequestParameters = WMIObject.Properties["RequestParameters"].Value as String;
                    this.SerialNumber = WMIObject.Properties["SerialNumber"].Value as String;
                    this.Status = WMIObject.Properties["Status"].Value as UInt32?;
                    this.StoreLocation = WMIObject.Properties["StoreLocation"].Value as byte?;
                    this.SubjectAlternativeNames = WMIObject.Properties["SubjectAlternativeNames"].Value as String;
                    this.SubjectName = WMIObject.Properties["SubjectName"].Value as String;
                    this.Thumbprint = WMIObject.Properties["Thumbprint"].Value as String;
                    string sValidFrom = WMIObject.Properties["ValidFrom"].Value as string;
                    if (string.IsNullOrEmpty(sValidFrom))
                        this.ValidFrom = null;
                    else
                        this.ValidFrom = ManagementDateTimeConverter.ToDateTime(sValidFrom) as DateTime?;
                    string sValidTo = WMIObject.Properties["ValidTo"].Value as string;
                    if (string.IsNullOrEmpty(sValidTo))
                        this.ValidTo = null;
                    else
                        this.ValidTo = ManagementDateTimeConverter.ToDateTime(sValidTo) as DateTime?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String ConfigurationParameters { get; set; }
                public String EnhancedKeyUsages { get; set; }
                public UInt32? Error { get; set; }
                public UInt32? ExpirationThreshold { get; set; }
                public String Issuers { get; set; }
                public String RequestID { get; set; }
                public String RequestParameters { get; set; }
                public String SerialNumber { get; set; }
                public UInt32? Status { get; set; }
                public byte? StoreLocation { get; set; }
                public String SubjectAlternativeNames { get; set; }
                public String SubjectName { get; set; }
                public String Thumbprint { get; set; }
                public DateTime? ValidFrom { get; set; }
                public DateTime? ValidTo { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_ExpressionValue_Setting
            {
                //Constructor
                public CCM_ExpressionValue_Setting(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ExpressionValue = WMIObject.Properties["ExpressionValue"].Value as Boolean?;
                    this.ID = WMIObject.Properties["ID"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? ExpressionValue { get; set; }
                public String ID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_File_Setting : CCM_FileSystem_Setting
            {
                //Constructor
                public CCM_File_Setting(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Company = WMIObject.Properties["Company"].Value as String;
                    this.ProductName = WMIObject.Properties["ProductName"].Value as String;
                    this.SHA1Hash = WMIObject.Properties["SHA1Hash"].Value as String;
                    this.Size = WMIObject.Properties["Size"].Value as UInt64;
                    this.SizeOnDisk = WMIObject.Properties["SizeOnDisk"].Value as UInt64;
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
                public String Company { get; set; }
                public String ProductName { get; set; }
                public String SHA1Hash { get; set; }
                public UInt64 Size { get; set; }
                public UInt64 SizeOnDisk { get; set; }
                public String Version { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_FileSystem_Setting
            {
                //Constructor
                public CCM_FileSystem_Setting(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Archived = WMIObject.Properties["Archived"].Value as Boolean?;
                    this.BasePath = WMIObject.Properties["BasePath"].Value as String;
                    this.Compressed = WMIObject.Properties["Compressed"].Value as Boolean?;
                    string sDateCreated = WMIObject.Properties["DateCreated"].Value as string;
                    if (string.IsNullOrEmpty(sDateCreated))
                        this.DateCreated = null;
                    else
                        this.DateCreated = ManagementDateTimeConverter.ToDateTime(sDateCreated) as DateTime?;
                    string sDateModified = WMIObject.Properties["DateModified"].Value as string;
                    if (string.IsNullOrEmpty(sDateModified))
                        this.DateModified = null;
                    else
                        this.DateModified = ManagementDateTimeConverter.ToDateTime(sDateModified) as DateTime?;
                    this.Encrypted = WMIObject.Properties["Encrypted"].Value as Boolean?;
                    this.FileSystemRedirectionMode = WMIObject.Properties["FileSystemRedirectionMode"].Value as byte?;
                    this.FullPath = WMIObject.Properties["FullPath"].Value as String;
                    this.Hidden = WMIObject.Properties["Hidden"].Value as Boolean?;
                    this.Name = WMIObject.Properties["Name"].Value as String;
                    this.Path = WMIObject.Properties["Path"].Value as String;
                    this.ReadOnly = WMIObject.Properties["ReadOnly"].Value as Boolean?;
                    this.SearchDepth = WMIObject.Properties["SearchDepth"].Value as byte?;
                    this.System = WMIObject.Properties["System"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? Archived { get; set; }
                public String BasePath { get; set; }
                public Boolean? Compressed { get; set; }
                public DateTime? DateCreated { get; set; }
                public DateTime? DateModified { get; set; }
                public Boolean? Encrypted { get; set; }
                public byte? FileSystemRedirectionMode { get; set; }
                public String FullPath { get; set; }
                public Boolean? Hidden { get; set; }
                public String Name { get; set; }
                public String Path { get; set; }
                public Boolean? ReadOnly { get; set; }
                public byte? SearchDepth { get; set; }
                public Boolean? System { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Folder_Setting : CCM_FileSystem_Setting
            {
                //Constructor
                public CCM_Folder_Setting(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_HandlerSynclet : Synclet
            {
                //Constructor
                public CCM_HandlerSynclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
                public String ActionType { get; set; }
                public String AppDeliveryTypeId { get; set; }
                public String ExecutionContext { get; set; }
                public String RequiresLogOn { get; set; }
                public String Reserved { get; set; }
                public UInt32? Revision { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_IisMetabase_Setting_Boolean : CCM_Setting_Boolean
            {
                //Constructor
                public CCM_IisMetabase_Setting_Boolean(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_IisMetabase_Setting_DateTime : CCM_Setting_DateTime
            {
                //Constructor
                public CCM_IisMetabase_Setting_DateTime(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_IisMetabase_Setting_Double : CCM_Setting_Double
            {
                //Constructor
                public CCM_IisMetabase_Setting_Double(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_IisMetabase_Setting_Integer : CCM_Setting_Integer
            {
                //Constructor
                public CCM_IisMetabase_Setting_Integer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_IisMetabase_Setting_IntegerArray : CCM_Setting_IntegerArray
            {
                //Constructor
                public CCM_IisMetabase_Setting_IntegerArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_IisMetabase_Setting_String : CCM_Setting_String
            {
                //Constructor
                public CCM_IisMetabase_Setting_String(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_IisMetabase_Setting_StringArray : CCM_Setting_StringArray
            {
                //Constructor
                public CCM_IisMetabase_Setting_StringArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_IisMetabase_Setting_Synclet
            {
                //Constructor
                public CCM_IisMetabase_Setting_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ID = WMIObject.Properties["ID"].Value as String;
                    this.MetaBasePath = WMIObject.Properties["MetaBasePath"].Value as String;
                    this.PropertyID = WMIObject.Properties["PropertyID"].Value as UInt32?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String ID { get; set; }
                public String MetaBasePath { get; set; }
                public UInt32? PropertyID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_LocalInstallationSynclet : CCM_HandlerSynclet
            {
                //Constructor
                public CCM_LocalInstallationSynclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_MSIProduct
            {
                //Constructor
                public CCM_MSIProduct(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.LocalPackage = WMIObject.Properties["LocalPackage"].Value as String;
                    this.ProductCode = WMIObject.Properties["ProductCode"].Value as String;
                    this.ProductName = WMIObject.Properties["ProductName"].Value as String;
                    this.ProductVersion = WMIObject.Properties["ProductVersion"].Value as String;
                    this.UpgradeCode = WMIObject.Properties["UpgradeCode"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String LocalPackage { get; set; }
                public String ProductCode { get; set; }
                public String ProductName { get; set; }
                public String ProductVersion { get; set; }
                public String UpgradeCode { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_OperatingSystem
            {
                //Constructor
                public CCM_OperatingSystem(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Architecture = WMIObject.Properties["Architecture"].Value as String;
                    this.ServerCore = WMIObject.Properties["ServerCore"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String Architecture { get; set; }
                public Boolean? ServerCore { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_PrimaryUser
            {
                //Constructor
                public CCM_PrimaryUser(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.User = WMIObject.Properties["User"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String User { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RAXInfo
            {
                //Constructor
                public CCM_RAXInfo(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AppID = WMIObject.Properties["AppID"].Value as String;
                    this.FeedURL = WMIObject.Properties["FeedURL"].Value as String;
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
                public String AppID { get; set; }
                public String FeedURL { get; set; }
                public String UserSID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RegistryKey_Setting
            {
                //Constructor
                public CCM_RegistryKey_Setting(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Hive = WMIObject.Properties["Hive"].Value as byte?;
                    this.key = WMIObject.Properties["key"].Value as String;
                    this.RegistryKeyExists = WMIObject.Properties["RegistryKeyExists"].Value as Boolean?;
                    this.RegistryPathRedirectionMode = WMIObject.Properties["RegistryPathRedirectionMode"].Value as byte?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public byte? Hive { get; set; }
                public String key { get; set; }
                public Boolean? RegistryKeyExists { get; set; }
                public byte? RegistryPathRedirectionMode { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RegistryValue_Setting
            {
                //Constructor
                public CCM_RegistryValue_Setting(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Hive = WMIObject.Properties["Hive"].Value as byte?;
                    this.key = WMIObject.Properties["key"].Value as String;
                    this.RegistryPathRedirectionMode = WMIObject.Properties["RegistryPathRedirectionMode"].Value as byte?;
                    this.RegistryValueExists = WMIObject.Properties["RegistryValueExists"].Value as Boolean?;
                    this.ResolvedKey = WMIObject.Properties["ResolvedKey"].Value as String;
                    this.ValueName = WMIObject.Properties["ValueName"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public byte? Hive { get; set; }
                public String key { get; set; }
                public byte? RegistryPathRedirectionMode { get; set; }
                public Boolean? RegistryValueExists { get; set; }
                public String ResolvedKey { get; set; }
                public String ValueName { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RegistryValue_Setting_Boolean : CCM_RegistryValue_Setting
            {
                //Constructor
                public CCM_RegistryValue_Setting_Boolean(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Value = WMIObject.Properties["Value"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RegistryValue_Setting_Datetime : CCM_RegistryValue_Setting
            {
                //Constructor
                public CCM_RegistryValue_Setting_Datetime(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    string sValue = WMIObject.Properties["Value"].Value as string;
                    if (string.IsNullOrEmpty(sValue))
                        this.Value = null;
                    else
                        this.Value = ManagementDateTimeConverter.ToDateTime(sValue) as DateTime?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public DateTime? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RegistryValue_Setting_Double : CCM_RegistryValue_Setting
            {
                //Constructor
                public CCM_RegistryValue_Setting_Double(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Value = WMIObject.Properties["Value"].Value as Decimal?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Decimal? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RegistryValue_Setting_Integer : CCM_RegistryValue_Setting
            {
                //Constructor
                public CCM_RegistryValue_Setting_Integer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Value = WMIObject.Properties["Value"].Value as Int64?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Int64? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RegistryValue_Setting_String : CCM_RegistryValue_Setting
            {
                //Constructor
                public CCM_RegistryValue_Setting_String(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Value = WMIObject.Properties["Value"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RegistryValue_Setting_StringArray : CCM_RegistryValue_Setting
            {
                //Constructor
                public CCM_RegistryValue_Setting_StringArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Value = WMIObject.Properties["Value"].Value as String[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String[] Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RegistryValue_Setting_Synclet
            {
                //Constructor
                public CCM_RegistryValue_Setting_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.CreateMissingPath = WMIObject.Properties["CreateMissingPath"].Value as Boolean?;
                    this.Hive = WMIObject.Properties["Hive"].Value as byte?;
                    this.key = WMIObject.Properties["key"].Value as String;
                    this.RegistryDataType = WMIObject.Properties["RegistryDataType"].Value as byte?;
                    this.RegistryPathRedirectionMode = WMIObject.Properties["RegistryPathRedirectionMode"].Value as byte?;
                    this.ValueName = WMIObject.Properties["ValueName"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? CreateMissingPath { get; set; }
                public byte? Hive { get; set; }
                public String key { get; set; }
                public byte? RegistryDataType { get; set; }
                public byte? RegistryPathRedirectionMode { get; set; }
                public String ValueName { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_RemotePCConnect_Settings
            {
                //Constructor
                public CCM_RemotePCConnect_Settings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.EnableNLA = WMIObject.Properties["EnableNLA"].Value as Boolean?;
                    this.EnablePrimaryUsers = WMIObject.Properties["EnablePrimaryUsers"].Value as Boolean?;
                    this.EnableTSConnection = WMIObject.Properties["EnableTSConnection"].Value as Boolean?;
                    this.EnableTSFirewallRule = WMIObject.Properties["EnableTSFirewallRule"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? EnableNLA { get; set; }
                public Boolean? EnablePrimaryUsers { get; set; }
                public Boolean? EnableTSConnection { get; set; }
                public Boolean? EnableTSFirewallRule { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Script_Setting_Boolean : CCM_Setting_Boolean
            {
                //Constructor
                public CCM_Script_Setting_Boolean(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Script_Setting_DateTime : CCM_Setting_DateTime
            {
                //Constructor
                public CCM_Script_Setting_DateTime(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Script_Setting_Double : CCM_Setting_Double
            {
                //Constructor
                public CCM_Script_Setting_Double(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Script_Setting_Integer : CCM_Setting_Integer
            {
                //Constructor
                public CCM_Script_Setting_Integer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Script_Setting_IntegerArray : CCM_Setting_IntegerArray
            {
                //Constructor
                public CCM_Script_Setting_IntegerArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Script_Setting_String : CCM_Setting_String
            {
                //Constructor
                public CCM_Script_Setting_String(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Script_Setting_StringArray : CCM_Setting_StringArray
            {
                //Constructor
                public CCM_Script_Setting_StringArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Script_Setting_Synclet
            {
                //Constructor
                public CCM_Script_Setting_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.DataType = WMIObject.Properties["DataType"].Value as byte?;
                    this.DiscoveryScript = WMIObject.Properties["DiscoveryScript"].Value as ScriptDefinition;
                    this.ExecutionContext = WMIObject.Properties["ExecutionContext"].Value as String;
                    this.ID = WMIObject.Properties["ID"].Value as String;
                    this.RemediationScript = WMIObject.Properties["RemediationScript"].Value as ScriptDefinition;
                    this.Use64BitScriptingHost = WMIObject.Properties["Use64BitScriptingHost"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public byte? DataType { get; set; }
                public ScriptDefinition DiscoveryScript { get; set; }
                public String ExecutionContext { get; set; }
                public String ID { get; set; }
                public ScriptDefinition RemediationScript { get; set; }
                public Boolean? Use64BitScriptingHost { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting
            {
                //Constructor
                public CCM_Setting(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ID = WMIObject.Properties["ID"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String ID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting_Boolean : CCM_Setting
            {
                //Constructor
                public CCM_Setting_Boolean(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as CCM_Value_Boolean[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public CCM_Value_Boolean[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting_BooleanArray : CCM_Setting
            {
                //Constructor
                public CCM_Setting_BooleanArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as CCM_Value_BooleanArray[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public CCM_Value_BooleanArray[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting_DateTime : CCM_Setting
            {
                //Constructor
                public CCM_Setting_DateTime(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as CCM_Value_DateTime[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public CCM_Value_DateTime[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting_DatetimeArray : CCM_Setting
            {
                //Constructor
                public CCM_Setting_DatetimeArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as CCM_Value_DatetimeArray[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public CCM_Value_DatetimeArray[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting_Double : CCM_Setting
            {
                //Constructor
                public CCM_Setting_Double(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as CCM_Value_Double[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public CCM_Value_Double[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting_DoubleArray : CCM_Setting
            {
                //Constructor
                public CCM_Setting_DoubleArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as CCM_Value_DoubleArray[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public CCM_Value_DoubleArray[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting_Integer : CCM_Setting
            {
                //Constructor
                public CCM_Setting_Integer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as CCM_Value_Integer[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public CCM_Value_Integer[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting_IntegerArray : CCM_Setting
            {
                //Constructor
                public CCM_Setting_IntegerArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as CCM_Value_IntegerArray[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public CCM_Value_IntegerArray[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting_String : CCM_Setting
            {
                //Constructor
                public CCM_Setting_String(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as CCM_Value_String[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public CCM_Value_String[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Setting_StringArray : CCM_Setting
            {
                //Constructor
                public CCM_Setting_StringArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SettingInstances = WMIObject.Properties["SettingInstances"].Value as CCM_Value_StringArray[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public CCM_Value_StringArray[] SettingInstances { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_SqlQuery_Setting_Boolean : CCM_Setting_Boolean
            {
                //Constructor
                public CCM_SqlQuery_Setting_Boolean(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_SqlQuery_Setting_DateTime : CCM_Setting_DateTime
            {
                //Constructor
                public CCM_SqlQuery_Setting_DateTime(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_SqlQuery_Setting_Double : CCM_Setting_Double
            {
                //Constructor
                public CCM_SqlQuery_Setting_Double(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_SqlQuery_Setting_Integer : CCM_Setting_Integer
            {
                //Constructor
                public CCM_SqlQuery_Setting_Integer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_SqlQuery_Setting_String : CCM_Setting_String
            {
                //Constructor
                public CCM_SqlQuery_Setting_String(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_SqlQuery_Setting_Synclet
            {
                //Constructor
                public CCM_SqlQuery_Setting_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ColumnName = WMIObject.Properties["ColumnName"].Value as String;
                    this.DatabaseName = WMIObject.Properties["DatabaseName"].Value as String;
                    this.ID = WMIObject.Properties["ID"].Value as String;
                    this.InstanceName = WMIObject.Properties["InstanceName"].Value as String;
                    this.Query = WMIObject.Properties["Query"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String ColumnName { get; set; }
                public String DatabaseName { get; set; }
                public String ID { get; set; }
                public String InstanceName { get; set; }
                public String Query { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_SUM_Setting : CCM_Setting
            {
                //Constructor
                public CCM_SUM_Setting(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.IsApplicable = WMIObject.Properties["IsApplicable"].Value as Boolean?;
                    this.IsInstalled = WMIObject.Properties["IsInstalled"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? IsApplicable { get; set; }
                public Boolean? IsInstalled { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_SUM_Setting_Synclet
            {
                //Constructor
                public CCM_SUM_Setting_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ID = WMIObject.Properties["ID"].Value as String;
                    this.UpdateID = WMIObject.Properties["UpdateID"].Value as String;
                    this.UpdateSourceID = WMIObject.Properties["UpdateSourceID"].Value as String;
                    this.UpdateSourceVersion = WMIObject.Properties["UpdateSourceVersion"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String ID { get; set; }
                public String UpdateID { get; set; }
                public String UpdateSourceID { get; set; }
                public String UpdateSourceVersion { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Value_Boolean
            {
                //Constructor
                public CCM_Value_Boolean(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public Boolean? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Value_DateTime
            {
                //Constructor
                public CCM_Value_DateTime(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    string sValue = WMIObject.Properties["Value"].Value as string;
                    if (string.IsNullOrEmpty(sValue))
                        this.Value = null;
                    else
                        this.Value = ManagementDateTimeConverter.ToDateTime(sValue) as DateTime?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public DateTime? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Value_Double
            {
                //Constructor
                public CCM_Value_Double(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as Decimal?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public Decimal? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Value_Integer
            {
                //Constructor
                public CCM_Value_Integer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as Int64?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public Int64? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Value_IntegerArray
            {
                //Constructor
                public CCM_Value_IntegerArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as Int64?[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public Int64?[] Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Value_String
            {
                //Constructor
                public CCM_Value_String(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public String Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_Value_StringArray
            {
                //Constructor
                public CCM_Value_StringArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as String[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public String[] Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_VirtualEnvironment
            {
                //Constructor
                public CCM_VirtualEnvironment(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Packages = WMIObject.Properties["Packages"].Value as String;
                    this.Revision = WMIObject.Properties["Revision"].Value as UInt32?;
                    this.VirtualEnvironmentId = WMIObject.Properties["VirtualEnvironmentId"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String Packages { get; set; }
                public UInt32? Revision { get; set; }
                public String VirtualEnvironmentId { get; set; }
                #endregion

                #region Methods

                public UInt32 EnforceVirtualEnvironment(String ActionType, String DisplayName, String Packages, UInt32 Revision, UInt32 SessionId, String UserSid, String VirtualEnvironmentId)
                {
                    return 0;
                }
                public UInt32 RemovePackageFromConnectionGroups(String PackageId, UInt32 SessionId, String UserSid, String VersionId)
                {
                    return 0;
                }
                #endregion
            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_VirtualEnvironmentSynclet : Synclet
            {
                //Constructor
                public CCM_VirtualEnvironmentSynclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Revision = WMIObject.Properties["Revision"].Value as UInt32?;
                    this.VirtualEnvironmentId = WMIObject.Properties["VirtualEnvironmentId"].Value as String;
                    this.VirtualEnvironmentXml = WMIObject.Properties["VirtualEnvironmentXml"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public UInt32? Revision { get; set; }
                public String VirtualEnvironmentId { get; set; }
                public String VirtualEnvironmentXml { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_VpnConnection
            {
                //Constructor
                public CCM_VpnConnection(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AllUserConnection = WMIObject.Properties["AllUserConnection"].Value as Boolean?;
                    this.ApplicationID = WMIObject.Properties["ApplicationID"].Value as String[];
                    this.AuthenticationMethod = WMIObject.Properties["AuthenticationMethod"].Value as String[];
                    this.CustomConfiguration = WMIObject.Properties["CustomConfiguration"].Value as String;
                    this.DnsConfig_DnsIPAddress = WMIObject.Properties["DnsConfig_DnsIPAddress"].Value as String[];
                    this.DnsConfig_DnsSuffix = WMIObject.Properties["DnsConfig_DnsSuffix"].Value as String[];
                    this.DnsSuffix = WMIObject.Properties["DnsSuffix"].Value as String;
                    this.DnsSuffixSearchList = WMIObject.Properties["DnsSuffixSearchList"].Value as String[];
                    this.EapConfigXmlStream = WMIObject.Properties["EapConfigXmlStream"].Value as String;
                    this.EncryptionLevel = WMIObject.Properties["EncryptionLevel"].Value as String;
                    this.IdleDisconnectSeconds = WMIObject.Properties["IdleDisconnectSeconds"].Value as UInt32?;
                    this.L2tpPsk = WMIObject.Properties["L2tpPsk"].Value as String;
                    this.MachineCertificateEKUFilter = WMIObject.Properties["MachineCertificateEKUFilter"].Value as String[];
                    this.MachineCertificateIssuerFilter = WMIObject.Properties["MachineCertificateIssuerFilter"].Value as String;
                    this.Name = WMIObject.Properties["Name"].Value as String;
                    this.PlugInApplicationID = WMIObject.Properties["PlugInApplicationID"].Value as String;
                    this.ProfileType = WMIObject.Properties["ProfileType"].Value as String;
                    this.ProvisioningAuthority = WMIObject.Properties["ProvisioningAuthority"].Value as String;
                    this.Proxy_AutoConfigurationScript = WMIObject.Properties["Proxy_AutoConfigurationScript"].Value as String;
                    this.Proxy_AutoDetect = WMIObject.Properties["Proxy_AutoDetect"].Value as Boolean?;
                    this.Proxy_BypassProxyForLocal = WMIObject.Properties["Proxy_BypassProxyForLocal"].Value as Boolean?;
                    this.Proxy_ExceptionPrefix = WMIObject.Properties["Proxy_ExceptionPrefix"].Value as String[];
                    this.Proxy_ProxyServer = WMIObject.Properties["Proxy_ProxyServer"].Value as String;
                    this.RememberCredential = WMIObject.Properties["RememberCredential"].Value as Boolean?;
                    this.Routes_DestinationPrefix_Metric = WMIObject.Properties["Routes_DestinationPrefix_Metric"].Value as String[];
                    this.ServerAddress = WMIObject.Properties["ServerAddress"].Value as String;
                    this.ServerList_FriendlyName = WMIObject.Properties["ServerList_FriendlyName"].Value as String[];
                    this.ServerList_ServerAddress = WMIObject.Properties["ServerList_ServerAddress"].Value as String[];
                    this.SplitTunneling = WMIObject.Properties["SplitTunneling"].Value as Boolean?;
                    this.TrustedNetwork = WMIObject.Properties["TrustedNetwork"].Value as String[];
                    this.TunnelType = WMIObject.Properties["TunnelType"].Value as String;
                    this.UseWinlogonCredential = WMIObject.Properties["UseWinlogonCredential"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? AllUserConnection { get; set; }
                public String[] ApplicationID { get; set; }
                public String[] AuthenticationMethod { get; set; }
                public String CustomConfiguration { get; set; }
                public String[] DnsConfig_DnsIPAddress { get; set; }
                public String[] DnsConfig_DnsSuffix { get; set; }
                public String DnsSuffix { get; set; }
                public String[] DnsSuffixSearchList { get; set; }
                public String EapConfigXmlStream { get; set; }
                public String EncryptionLevel { get; set; }
                public UInt32? IdleDisconnectSeconds { get; set; }
                public String L2tpPsk { get; set; }
                public String[] MachineCertificateEKUFilter { get; set; }
                public String MachineCertificateIssuerFilter { get; set; }
                public String Name { get; set; }
                public String PlugInApplicationID { get; set; }
                public String ProfileType { get; set; }
                public String ProvisioningAuthority { get; set; }
                public String Proxy_AutoConfigurationScript { get; set; }
                public Boolean? Proxy_AutoDetect { get; set; }
                public Boolean? Proxy_BypassProxyForLocal { get; set; }
                public String[] Proxy_ExceptionPrefix { get; set; }
                public String Proxy_ProxyServer { get; set; }
                public Boolean? RememberCredential { get; set; }
                public String[] Routes_DestinationPrefix_Metric { get; set; }
                public String ServerAddress { get; set; }
                public String[] ServerList_FriendlyName { get; set; }
                public String[] ServerList_ServerAddress { get; set; }
                public Boolean? SplitTunneling { get; set; }
                public String[] TrustedNetwork { get; set; }
                public String TunnelType { get; set; }
                public Boolean? UseWinlogonCredential { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_VpnConnectionXml
            {
                //Constructor
                public CCM_VpnConnectionXml(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AllUserConnection = WMIObject.Properties["AllUserConnection"].Value as Boolean?;
                    this.Name = WMIObject.Properties["Name"].Value as String;
                    this.Profile = WMIObject.Properties["Profile"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? AllUserConnection { get; set; }
                public String Name { get; set; }
                public String Profile { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_WebAppInstallInfo
            {
                //Constructor
                public CCM_WebAppInstallInfo(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AppDeliveryTypeId = WMIObject.Properties["AppDeliveryTypeId"].Value as String;
                    this.AppDtRevision = WMIObject.Properties["AppDtRevision"].Value as UInt32?;
                    this.TargetURL = WMIObject.Properties["TargetURL"].Value as String;
                    this.URLFileName = WMIObject.Properties["URLFileName"].Value as String;
                    this.URLPath = WMIObject.Properties["URLPath"].Value as String;
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
                public String AppDeliveryTypeId { get; set; }
                public UInt32? AppDtRevision { get; set; }
                public String TargetURL { get; set; }
                public String URLFileName { get; set; }
                public String URLPath { get; set; }
                public String UserSID { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_WqlQuery_Setting_Boolean : CCM_Setting_Boolean
            {
                //Constructor
                public CCM_WqlQuery_Setting_Boolean(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_WqlQuery_Setting_DateTime : CCM_Setting_DateTime
            {
                //Constructor
                public CCM_WqlQuery_Setting_DateTime(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_WqlQuery_Setting_Double : CCM_Setting_Double
            {
                //Constructor
                public CCM_WqlQuery_Setting_Double(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_WqlQuery_Setting_Integer : CCM_Setting_Integer
            {
                //Constructor
                public CCM_WqlQuery_Setting_Integer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_WqlQuery_Setting_IntegerArray : CCM_Setting_IntegerArray
            {
                //Constructor
                public CCM_WqlQuery_Setting_IntegerArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_WqlQuery_Setting_String : CCM_Setting_String
            {
                //Constructor
                public CCM_WqlQuery_Setting_String(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_WqlQuery_Setting_StringArray : CCM_Setting_StringArray
            {
                //Constructor
                public CCM_WqlQuery_Setting_StringArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_WqlQuery_Setting_Synclet
            {
                //Constructor
                public CCM_WqlQuery_Setting_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.DataType = WMIObject.Properties["DataType"].Value as byte?;
                    this.ID = WMIObject.Properties["ID"].Value as String;
                    this.IsArray = WMIObject.Properties["IsArray"].Value as Boolean?;
                    this.Namespace = WMIObject.Properties["Namespace"].Value as String;
                    this.Property = WMIObject.Properties["Property"].Value as String;
                    this.Query = WMIObject.Properties["Query"].Value as String;
                    this.WMIClass = WMIObject.Properties["WMIClass"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public byte? DataType { get; set; }
                public String ID { get; set; }
                public Boolean? IsArray { get; set; }
                public String Namespace { get; set; }
                public String Property { get; set; }
                public String Query { get; set; }
                public String WMIClass { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_XPathQuery_Setting_Boolean : CCM_Setting_Boolean
            {
                //Constructor
                public CCM_XPathQuery_Setting_Boolean(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_XPathQuery_Setting_DateTime : CCM_Setting_DateTime
            {
                //Constructor
                public CCM_XPathQuery_Setting_DateTime(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_XPathQuery_Setting_Double : CCM_Setting_Double
            {
                //Constructor
                public CCM_XPathQuery_Setting_Double(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_XPathQuery_Setting_Integer : CCM_Setting_Integer
            {
                //Constructor
                public CCM_XPathQuery_Setting_Integer(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_XPathQuery_Setting_IntegerArray : CCM_Setting_IntegerArray
            {
                //Constructor
                public CCM_XPathQuery_Setting_IntegerArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_XPathQuery_Setting_String : CCM_Setting_String
            {
                //Constructor
                public CCM_XPathQuery_Setting_String(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_XPathQuery_Setting_StringArray : CCM_Setting_StringArray
            {
                //Constructor
                public CCM_XPathQuery_Setting_StringArray(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CCM_XPathQuery_Setting_Synclet
            {
                //Constructor
                public CCM_XPathQuery_Setting_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ID = WMIObject.Properties["ID"].Value as String;
                    this.Is64Bit = WMIObject.Properties["Is64Bit"].Value as Boolean?;
                    this.NamespaceDeclarations = WMIObject.Properties["NamespaceDeclarations"].Value as Namespace[];
                    this.SearchDepth = WMIObject.Properties["SearchDepth"].Value as byte?;
                    this.WorkingFilter = WMIObject.Properties["WorkingFilter"].Value as String;
                    this.WorkingPath = WMIObject.Properties["WorkingPath"].Value as String;
                    this.XpathQuery = WMIObject.Properties["XpathQuery"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String ID { get; set; }
                public Boolean? Is64Bit { get; set; }
                public Namespace[] NamespaceDeclarations { get; set; }
                public byte? SearchDepth { get; set; }
                public String WorkingFilter { get; set; }
                public String WorkingPath { get; set; }
                public String XpathQuery { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CIM_ClassCreation : CIM_ClassIndication
            {
                //Constructor
                public CIM_ClassCreation(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CIM_ClassDeletion : CIM_ClassIndication
            {
                //Constructor
                public CIM_ClassDeletion(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CIM_ClassIndication : CIM_Indication
            {
                //Constructor
                public CIM_ClassIndication(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ClassDefinition = WMIObject.Properties["ClassDefinition"].Value as Object;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Object ClassDefinition { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CIM_ClassModification : CIM_ClassIndication
            {
                //Constructor
                public CIM_ClassModification(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.PreviousClassDefinition = WMIObject.Properties["PreviousClassDefinition"].Value as Object;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Object PreviousClassDefinition { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CIM_Error
            {
                //Constructor
                public CIM_Error(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.CIMStatusCode = WMIObject.Properties["CIMStatusCode"].Value as UInt32?;
                    this.CIMStatusCodeDescription = WMIObject.Properties["CIMStatusCodeDescription"].Value as String;
                    this.ErrorSource = WMIObject.Properties["ErrorSource"].Value as String;
                    this.ErrorSourceFormat = WMIObject.Properties["ErrorSourceFormat"].Value as UInt16;
                    this.ErrorType = WMIObject.Properties["ErrorType"].Value as UInt16;
                    this.Message = WMIObject.Properties["Message"].Value as String;
                    this.MessageArguments = WMIObject.Properties["MessageArguments"].Value as String[];
                    this.MessageID = WMIObject.Properties["MessageID"].Value as String;
                    this.OtherErrorSourceFormat = WMIObject.Properties["OtherErrorSourceFormat"].Value as String;
                    this.OtherErrorType = WMIObject.Properties["OtherErrorType"].Value as String;
                    this.OWningEntity = WMIObject.Properties["OWningEntity"].Value as String;
                    this.PerceivedSeverity = WMIObject.Properties["PerceivedSeverity"].Value as UInt16;
                    this.ProbableCause = WMIObject.Properties["ProbableCause"].Value as UInt16;
                    this.ProbableCauseDescription = WMIObject.Properties["ProbableCauseDescription"].Value as String;
                    this.RecommendedActions = WMIObject.Properties["RecommendedActions"].Value as String[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public UInt32? CIMStatusCode { get; set; }
                public String CIMStatusCodeDescription { get; set; }
                public String ErrorSource { get; set; }
                public UInt16 ErrorSourceFormat { get; set; }
                public UInt16 ErrorType { get; set; }
                public String Message { get; set; }
                public String[] MessageArguments { get; set; }
                public String MessageID { get; set; }
                public String OtherErrorSourceFormat { get; set; }
                public String OtherErrorType { get; set; }
                public String OWningEntity { get; set; }
                public UInt16 PerceivedSeverity { get; set; }
                public UInt16 ProbableCause { get; set; }
                public String ProbableCauseDescription { get; set; }
                public String[] RecommendedActions { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CIM_Indication
            {
                //Constructor
                public CIM_Indication(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.CorrelatedIndications = WMIObject.Properties["CorrelatedIndications"].Value as String[];
                    this.IndicationFilterName = WMIObject.Properties["IndicationFilterName"].Value as String;
                    this.IndicationIdentifier = WMIObject.Properties["IndicationIdentifier"].Value as String;
                    string sIndicationTime = WMIObject.Properties["IndicationTime"].Value as string;
                    if (string.IsNullOrEmpty(sIndicationTime))
                        this.IndicationTime = null;
                    else
                        this.IndicationTime = ManagementDateTimeConverter.ToDateTime(sIndicationTime) as DateTime?;
                    this.OtherSeverity = WMIObject.Properties["OtherSeverity"].Value as String;
                    this.PerceivedSeverity = WMIObject.Properties["PerceivedSeverity"].Value as UInt16;
                    this.SequenceContext = WMIObject.Properties["SequenceContext"].Value as String;
                    this.SequenceNumber = WMIObject.Properties["SequenceNumber"].Value as Int64?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String[] CorrelatedIndications { get; set; }
                public String IndicationFilterName { get; set; }
                public String IndicationIdentifier { get; set; }
                public DateTime? IndicationTime { get; set; }
                public String OtherSeverity { get; set; }
                public UInt16 PerceivedSeverity { get; set; }
                public String SequenceContext { get; set; }
                public Int64? SequenceNumber { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CIM_InstCreation : CIM_InstIndication
            {
                //Constructor
                public CIM_InstCreation(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CIM_InstDeletion : CIM_InstIndication
            {
                //Constructor
                public CIM_InstDeletion(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CIM_InstIndication : CIM_Indication
            {
                //Constructor
                public CIM_InstIndication(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.SourceInstance = WMIObject.Properties["SourceInstance"].Value as Object;
                    this.SourceInstanceHost = WMIObject.Properties["SourceInstanceHost"].Value as String;
                    this.SourceInstanceModelPath = WMIObject.Properties["SourceInstanceModelPath"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Object SourceInstance { get; set; }
                public String SourceInstanceHost { get; set; }
                public String SourceInstanceModelPath { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class CIM_InstModification : CIM_InstIndication
            {
                //Constructor
                public CIM_InstModification(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.PreviousInstance = WMIObject.Properties["PreviousInstance"].Value as Object;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Object PreviousInstance { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ContentInfo
            {
                //Constructor
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
                public String ContentId { get; set; }
                public String ContentVersion { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Device_OperatingSystem
            {
                //Constructor
                public Device_OperatingSystem(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.OSName = WMIObject.Properties["OSName"].Value as String;
                    this.OSType = WMIObject.Properties["OSType"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String OSName { get; set; }
                public String OSType { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Device_OSInformation
            {
                //Constructor
                public Device_OSInformation(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.APILevel = WMIObject.Properties["APILevel"].Value as UInt64;
                    this.Platform = WMIObject.Properties["Platform"].Value as String;
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
                public UInt64 APILevel { get; set; }
                public String Platform { get; set; }
                public String Version { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class DeviceProperty
            {
                //Constructor
                public DeviceProperty(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.DeviceOwnershipType = WMIObject.Properties["DeviceOwnershipType"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String DeviceOwnershipType { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class GLOBAL_MachineOU
            {
                //Constructor
                public GLOBAL_MachineOU(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.MachineOU = WMIObject.Properties["MachineOU"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String MachineOU { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Local_Detect_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public Local_Detect_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.DiscoverySourceXml = WMIObject.Properties["DiscoverySourceXml"].Value as String;
                    this.ExpressionXml = WMIObject.Properties["ExpressionXml"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String DiscoverySourceXml { get; set; }
                public String ExpressionXml { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class MSFT_ExtendedStatus : MSFT_WmiError
            {
                //Constructor
                public MSFT_ExtendedStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.original_error = WMIObject.Properties["original_error"].Value as Object;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Object original_error { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class MSFT_WmiError : CIM_Error
            {
                //Constructor
                public MSFT_WmiError(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.error_Category = WMIObject.Properties["error_Category"].Value as UInt16;
                    this.error_Code = WMIObject.Properties["error_Code"].Value as UInt32?;
                    this.error_Type = WMIObject.Properties["error_Type"].Value as String;
                    this.error_WindowsErrorMessage = WMIObject.Properties["error_WindowsErrorMessage"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public UInt16 error_Category { get; set; }
                public UInt32? error_Code { get; set; }
                public String error_Type { get; set; }
                public String error_WindowsErrorMessage { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class MSI_Detect_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public MSI_Detect_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.PackageCode = WMIObject.Properties["PackageCode"].Value as String;
                    this.PatchCodes = WMIObject.Properties["PatchCodes"].Value as String[];
                    this.ProductCode = WMIObject.Properties["ProductCode"].Value as String;
                    this.ProductVersion = WMIObject.Properties["ProductVersion"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String PackageCode { get; set; }
                public String[] PatchCodes { get; set; }
                public String ProductCode { get; set; }
                public String ProductVersion { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class MSI_Install_Synclet : CCM_LocalInstallationSynclet
            {
                //Constructor
                public MSI_Install_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class MSI_Uninstall_Synclet : CCM_LocalInstallationSynclet
            {
                //Constructor
                public MSI_Uninstall_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Namespace
            {
                //Constructor
                public Namespace(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Prefix = WMIObject.Properties["Prefix"].Value as String;
                    this.XMLNamespace = WMIObject.Properties["XMLNamespace"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String Prefix { get; set; }
                public String XMLNamespace { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class PrimaryDevice
            {
                //Constructor
                public PrimaryDevice(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.IsPrimaryDevice = WMIObject.Properties["IsPrimaryDevice"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? IsPrimaryDevice { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class RAX_Detect_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public RAX_Detect_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ApplicationID = WMIObject.Properties["ApplicationID"].Value as String;
                    this.ApplicationName = WMIObject.Properties["ApplicationName"].Value as String;
                    this.FeedURL = WMIObject.Properties["FeedURL"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String ApplicationID { get; set; }
                public String ApplicationName { get; set; }
                public String FeedURL { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class RAX_Install_Synclet : RAX_Detect_Synclet
            {
                //Constructor
                public RAX_Install_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.FastRetryExitCodes = WMIObject.Properties["FastRetryExitCodes"].Value as UInt32?[];
                    this.MaxExecuteTime = WMIObject.Properties["MaxExecuteTime"].Value as UInt32?;
                    this.SuccessExitCodes = WMIObject.Properties["SuccessExitCodes"].Value as UInt32?[];
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public UInt32?[] FastRetryExitCodes { get; set; }
                public UInt32? MaxExecuteTime { get; set; }
                public UInt32?[] SuccessExitCodes { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class RAX_Uninstall_Synclet : RAX_Install_Synclet
            {
                //Constructor
                public RAX_Uninstall_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_0d0bbb2e_1cf6_41a2_b7b8_5fc2838dafe7
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_0d0bbb2e_1cf6_41a2_b7b8_5fc2838dafe7(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as Int64?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public Int64? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_116c5bf1_53f3_4025_af55_43305c2cb7dd
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_116c5bf1_53f3_4025_af55_43305c2cb7dd(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public Boolean? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_24f48702_2ce9_49b8_9a65_9ecee21c1af8
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_24f48702_2ce9_49b8_9a65_9ecee21c1af8(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public Boolean? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_290dd3cc_8414_442d_b1a2_7c844ed0b5be
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_290dd3cc_8414_442d_b1a2_7c844ed0b5be(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public String Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_8c5052c0_049f_49cb_9a02_456946ea5bdc
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_8c5052c0_049f_49cb_9a02_456946ea5bdc(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public String Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_bdd64169_c026_456b_9b01_fd2fa11a774f
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_bdd64169_c026_456b_9b01_fd2fa11a774f(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public String Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_c53b0eb5_b533_4df2_b7e6_dee559b5979b
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_c53b0eb5_b533_4df2_b7e6_dee559b5979b(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public String Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_f47e9501_4f7f_4406_acf5_f7bb2fb2ef17
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_f47e9501_4f7f_4406_acf5_f7bb2fb2ef17(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public String Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_f9763508_e4a8_4072_9ee5_22e7616ff230
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_142fa09a_4a04_470c_8cf7_d07a6a065778_ScriptSetting_f9763508_e4a8_4072_9ee5_22e7616ff230(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public Boolean? Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_f1802fa8_aa4b_4326_9533_a6c134702013_ScriptSetting_0142664b_36d8_4edb_a9d8_d1ffaf0f4e6a
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_f1802fa8_aa4b_4326_9533_a6c134702013_ScriptSetting_0142664b_36d8_4edb_a9d8_d1ffaf0f4e6a(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public String Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_f1802fa8_aa4b_4326_9533_a6c134702013_ScriptSetting_7c7a8ec1_f9a0_45bc_8405_9566657763a5
            {
                //Constructor
                public ScopeId_C2498A4B_7C31_452B_8A2B_A2D22D73AF40_Application_f1802fa8_aa4b_4326_9533_a6c134702013_ScriptSetting_7c7a8ec1_f9a0_45bc_8405_9566657763a5(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.InstancePath = WMIObject.Properties["InstancePath"].Value as String;
                    this.Value = WMIObject.Properties["Value"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String InstancePath { get; set; }
                public String Value { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Script_Detect_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public Script_Detect_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.RunAs32Bit = WMIObject.Properties["RunAs32Bit"].Value as Boolean?;
                    this.ScriptBody = WMIObject.Properties["ScriptBody"].Value as String;
                    this.ScriptType = WMIObject.Properties["ScriptType"].Value as byte?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? RunAs32Bit { get; set; }
                public String ScriptBody { get; set; }
                public byte? ScriptType { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Script_Install_Synclet : CCM_LocalInstallationSynclet
            {
                //Constructor
                public Script_Install_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Script_Uninstall_Synclet : CCM_LocalInstallationSynclet
            {
                //Constructor
                public Script_Uninstall_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class ScriptDefinition
            {
                //Constructor
                public ScriptDefinition(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ScriptBody = WMIObject.Properties["ScriptBody"].Value as String;
                    this.ScriptType = WMIObject.Properties["ScriptType"].Value as byte?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String ScriptBody { get; set; }
                public byte? ScriptType { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Security_Ace
            {
                //Constructor
                public Security_Ace(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AccessMask = WMIObject.Properties["AccessMask"].Value as UInt32?;
                    this.AccessSacl = WMIObject.Properties["AccessSacl"].Value as Boolean?;
                    this.AceType = WMIObject.Properties["AceType"].Value as byte?;
                    this.ContainerInherit = WMIObject.Properties["ContainerInherit"].Value as Boolean?;
                    this.DeleteObject = WMIObject.Properties["DeleteObject"].Value as Boolean?;
                    this.DomainName = WMIObject.Properties["DomainName"].Value as String;
                    this.GenericAll = WMIObject.Properties["GenericAll"].Value as Boolean?;
                    this.GenericExecute = WMIObject.Properties["GenericExecute"].Value as Boolean?;
                    this.GenericRead = WMIObject.Properties["GenericRead"].Value as Boolean?;
                    this.GenericWrite = WMIObject.Properties["GenericWrite"].Value as Boolean?;
                    this.InheritedFrom = WMIObject.Properties["InheritedFrom"].Value as String;
                    this.InheritOnly = WMIObject.Properties["InheritOnly"].Value as Boolean?;
                    this.ObjectInherit = WMIObject.Properties["ObjectInherit"].Value as Boolean?;
                    this.ReadControl = WMIObject.Properties["ReadControl"].Value as Boolean?;
                    this.Synchronize = WMIObject.Properties["Synchronize"].Value as Boolean?;
                    this.Trustee = WMIObject.Properties["Trustee"].Value as String;
                    this.UserName = WMIObject.Properties["UserName"].Value as String;
                    this.WriteDACL = WMIObject.Properties["WriteDACL"].Value as Boolean?;
                    this.WriteOwner = WMIObject.Properties["WriteOwner"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public UInt32? AccessMask { get; set; }
                public Boolean? AccessSacl { get; set; }
                public byte? AceType { get; set; }
                public Boolean? ContainerInherit { get; set; }
                public Boolean? DeleteObject { get; set; }
                public String DomainName { get; set; }
                public Boolean? GenericAll { get; set; }
                public Boolean? GenericExecute { get; set; }
                public Boolean? GenericRead { get; set; }
                public Boolean? GenericWrite { get; set; }
                public String InheritedFrom { get; set; }
                public Boolean? InheritOnly { get; set; }
                public Boolean? ObjectInherit { get; set; }
                public Boolean? ReadControl { get; set; }
                public Boolean? Synchronize { get; set; }
                public String Trustee { get; set; }
                public String UserName { get; set; }
                public Boolean? WriteDACL { get; set; }
                public Boolean? WriteOwner { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Security_Acl
            {
                //Constructor
                public Security_Acl(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Aces = WMIObject.Properties["Aces"].Value as Security_Ace[];
                    this.AutoPropagate = WMIObject.Properties["AutoPropagate"].Value as Boolean?;
                    this.Protected = WMIObject.Properties["Protected"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Security_Ace[] Aces { get; set; }
                public Boolean? AutoPropagate { get; set; }
                public Boolean? Protected { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Security_DirectoryAce : Security_Ace
            {
                //Constructor
                public Security_DirectoryAce(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AddFile = WMIObject.Properties["AddFile"].Value as Boolean?;
                    this.AddSubdirectory = WMIObject.Properties["AddSubdirectory"].Value as Boolean?;
                    this.AppendData = WMIObject.Properties["AppendData"].Value as Boolean?;
                    this.DeleteChild = WMIObject.Properties["DeleteChild"].Value as Boolean?;
                    this.Execute = WMIObject.Properties["Execute"].Value as Boolean?;
                    this.ListDirectory = WMIObject.Properties["ListDirectory"].Value as Boolean?;
                    this.ReadAttributes = WMIObject.Properties["ReadAttributes"].Value as Boolean?;
                    this.ReadData = WMIObject.Properties["ReadData"].Value as Boolean?;
                    this.ReadExtendedAttributes = WMIObject.Properties["ReadExtendedAttributes"].Value as Boolean?;
                    this.Traverse = WMIObject.Properties["Traverse"].Value as Boolean?;
                    this.WriteAttributes = WMIObject.Properties["WriteAttributes"].Value as Boolean?;
                    this.WriteData = WMIObject.Properties["WriteData"].Value as Boolean?;
                    this.WriteExtendedAttributes = WMIObject.Properties["WriteExtendedAttributes"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? AddFile { get; set; }
                public Boolean? AddSubdirectory { get; set; }
                public Boolean? AppendData { get; set; }
                public Boolean? DeleteChild { get; set; }
                public Boolean? Execute { get; set; }
                public Boolean? ListDirectory { get; set; }
                public Boolean? ReadAttributes { get; set; }
                public Boolean? ReadData { get; set; }
                public Boolean? ReadExtendedAttributes { get; set; }
                public Boolean? Traverse { get; set; }
                public Boolean? WriteAttributes { get; set; }
                public Boolean? WriteData { get; set; }
                public Boolean? WriteExtendedAttributes { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Security_DirectorySecurityDescriptor : Security_SecurableObject
            {
                //Constructor
                public Security_DirectorySecurityDescriptor(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.BasePath = WMIObject.Properties["BasePath"].Value as String;
                    this.FileSystemRedirectionMode = WMIObject.Properties["FileSystemRedirectionMode"].Value as byte?;
                    this.Name = WMIObject.Properties["Name"].Value as String;
                    this.Path = WMIObject.Properties["Path"].Value as String;
                    this.SearchDepth = WMIObject.Properties["SearchDepth"].Value as byte?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String BasePath { get; set; }
                public byte? FileSystemRedirectionMode { get; set; }
                public String Name { get; set; }
                public String Path { get; set; }
                public byte? SearchDepth { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Security_FileAce : Security_Ace
            {
                //Constructor
                public Security_FileAce(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.AppendData = WMIObject.Properties["AppendData"].Value as Boolean?;
                    this.Execute = WMIObject.Properties["Execute"].Value as Boolean?;
                    this.ReadAttributes = WMIObject.Properties["ReadAttributes"].Value as Boolean?;
                    this.ReadData = WMIObject.Properties["ReadData"].Value as Boolean?;
                    this.ReadExtendedAttributes = WMIObject.Properties["ReadExtendedAttributes"].Value as Boolean?;
                    this.WriteAttributes = WMIObject.Properties["WriteAttributes"].Value as Boolean?;
                    this.WriteData = WMIObject.Properties["WriteData"].Value as Boolean?;
                    this.WriteExtendedAttributes = WMIObject.Properties["WriteExtendedAttributes"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? AppendData { get; set; }
                public Boolean? Execute { get; set; }
                public Boolean? ReadAttributes { get; set; }
                public Boolean? ReadData { get; set; }
                public Boolean? ReadExtendedAttributes { get; set; }
                public Boolean? WriteAttributes { get; set; }
                public Boolean? WriteData { get; set; }
                public Boolean? WriteExtendedAttributes { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Security_FileSecurityDescriptor : Security_SecurableObject
            {
                //Constructor
                public Security_FileSecurityDescriptor(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.BasePath = WMIObject.Properties["BasePath"].Value as String;
                    this.FileSystemRedirectionMode = WMIObject.Properties["FileSystemRedirectionMode"].Value as byte?;
                    this.Name = WMIObject.Properties["Name"].Value as String;
                    this.Path = WMIObject.Properties["Path"].Value as String;
                    this.SearchDepth = WMIObject.Properties["SearchDepth"].Value as byte?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String BasePath { get; set; }
                public byte? FileSystemRedirectionMode { get; set; }
                public String Name { get; set; }
                public String Path { get; set; }
                public byte? SearchDepth { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Security_RegistryKeyAce : Security_Ace
            {
                //Constructor
                public Security_RegistryKeyAce(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.CreateLink = WMIObject.Properties["CreateLink"].Value as Boolean?;
                    this.CreateSubKey = WMIObject.Properties["CreateSubKey"].Value as Boolean?;
                    this.EnumerateSubKeys = WMIObject.Properties["EnumerateSubKeys"].Value as Boolean?;
                    this.Notify = WMIObject.Properties["Notify"].Value as Boolean?;
                    this.QueryValue = WMIObject.Properties["QueryValue"].Value as Boolean?;
                    this.SetValue = WMIObject.Properties["SetValue"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? CreateLink { get; set; }
                public Boolean? CreateSubKey { get; set; }
                public Boolean? EnumerateSubKeys { get; set; }
                public Boolean? Notify { get; set; }
                public Boolean? QueryValue { get; set; }
                public Boolean? SetValue { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Security_RegistrySecurityDescriptor : Security_SecurableObject
            {
                //Constructor
                public Security_RegistrySecurityDescriptor(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Hive = WMIObject.Properties["Hive"].Value as byte?;
                    this.KeyPath = WMIObject.Properties["KeyPath"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public byte? Hive { get; set; }
                public String KeyPath { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Security_SecurableObject
            {
                //Constructor
                public Security_SecurableObject(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.ObjectId = WMIObject.Properties["ObjectId"].Value as String;
                    this.ObjectType = WMIObject.Properties["ObjectType"].Value as String;
                    this.Sd = WMIObject.Properties["Sd"].Value as Security_SecurityDescriptor;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String ObjectId { get; set; }
                public String ObjectType { get; set; }
                public Security_SecurityDescriptor Sd { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Security_SecurityDescriptor
            {
                //Constructor
                public Security_SecurityDescriptor(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.Dacl = WMIObject.Properties["Dacl"].Value as Security_Acl;
                    this.Group = WMIObject.Properties["Group"].Value as String;
                    this.Owner = WMIObject.Properties["Owner"].Value as String;
                    this.Sacl = WMIObject.Properties["Sacl"].Value as Security_Acl;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Security_Acl Dacl { get; set; }
                public String Group { get; set; }
                public String Owner { get; set; }
                public Security_Acl Sacl { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Synclet
            {
                //Constructor
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
                public String ClassName { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class UserProperty
            {
                //Constructor
                public UserProperty(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.IsCloudUser = WMIObject.Properties["IsCloudUser"].Value as Boolean?;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public Boolean? IsCloudUser { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class WebApp_Detect_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public WebApp_Detect_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.LocalAppId = WMIObject.Properties["LocalAppId"].Value as String;
                    this.LocalAppVersion = WMIObject.Properties["LocalAppVersion"].Value as UInt32?;
                    this.TargetUrl = WMIObject.Properties["TargetUrl"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String LocalAppId { get; set; }
                public UInt32? LocalAppVersion { get; set; }
                public String TargetUrl { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class WebApp_Install_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public WebApp_Install_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.LocalAppId = WMIObject.Properties["LocalAppId"].Value as String;
                    this.LocalAppVersion = WMIObject.Properties["LocalAppVersion"].Value as UInt32?;
                    this.TargetUrl = WMIObject.Properties["TargetUrl"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String LocalAppId { get; set; }
                public UInt32? LocalAppVersion { get; set; }
                public String TargetUrl { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class WebApp_Uninstall_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public WebApp_Uninstall_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.LocalAppId = WMIObject.Properties["LocalAppId"].Value as String;
                    this.LocalAppVersion = WMIObject.Properties["LocalAppVersion"].Value as UInt32?;
                    this.TargetUrl = WMIObject.Properties["TargetUrl"].Value as String;
                }

                #region Properties

                internal string __CLASS { get; set; }
                internal string __NAMESPACE { get; set; }
                internal bool __INSTANCE { get; set; }
                internal string __RELPATH { get; set; }
                internal PSObject WMIObject { get; set; }
                internal Runspace remoteRunspace;
                internal TraceSource pSCode;
                public String LocalAppId { get; set; }
                public UInt32? LocalAppVersion { get; set; }
                public String TargetUrl { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Windows8App_Detect_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public Windows8App_Detect_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.DisplayName = WMIObject.Properties["DisplayName"].Value as String;
                    this.Frameworks = WMIObject.Properties["Frameworks"].Value as String;
                    this.IsBundle = WMIObject.Properties["IsBundle"].Value as Boolean?;
                    this.IsDeepLink = WMIObject.Properties["IsDeepLink"].Value as Boolean?;
                    this.Name = WMIObject.Properties["Name"].Value as String;
                    this.ProcessorArchitecture = WMIObject.Properties["ProcessorArchitecture"].Value as String;
                    this.Publisher = WMIObject.Properties["Publisher"].Value as String;
                    this.ResourceId = WMIObject.Properties["ResourceId"].Value as String;
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
                public String DisplayName { get; set; }
                public String Frameworks { get; set; }
                public Boolean? IsBundle { get; set; }
                public Boolean? IsDeepLink { get; set; }
                public String Name { get; set; }
                public String ProcessorArchitecture { get; set; }
                public String Publisher { get; set; }
                public String ResourceId { get; set; }
                public String Version { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Windows8App_Install_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public Windows8App_Install_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.DisplayName = WMIObject.Properties["DisplayName"].Value as String;
                    this.FastRetryExitCodes = WMIObject.Properties["FastRetryExitCodes"].Value as UInt32?[];
                    this.Frameworks = WMIObject.Properties["Frameworks"].Value as String;
                    this.IsDeepLink = WMIObject.Properties["IsDeepLink"].Value as Boolean?;
                    this.MaxExecuteTime = WMIObject.Properties["MaxExecuteTime"].Value as UInt32?;
                    this.Name = WMIObject.Properties["Name"].Value as String;
                    this.PackageUri = WMIObject.Properties["PackageUri"].Value as String;
                    this.ProcessorArchitecture = WMIObject.Properties["ProcessorArchitecture"].Value as String;
                    this.Publisher = WMIObject.Properties["Publisher"].Value as String;
                    this.ResourceId = WMIObject.Properties["ResourceId"].Value as String;
                    this.SuccessExitCodes = WMIObject.Properties["SuccessExitCodes"].Value as UInt32?[];
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
                public String DisplayName { get; set; }
                public UInt32?[] FastRetryExitCodes { get; set; }
                public String Frameworks { get; set; }
                public Boolean? IsDeepLink { get; set; }
                public UInt32? MaxExecuteTime { get; set; }
                public String Name { get; set; }
                public String PackageUri { get; set; }
                public String ProcessorArchitecture { get; set; }
                public String Publisher { get; set; }
                public String ResourceId { get; set; }
                public UInt32?[] SuccessExitCodes { get; set; }
                public String Version { get; set; }
                #endregion

            }

            /// <summary>
            /// Source:ROOT\ccm\cimodels
            /// </summary>
            public class Windows8App_Uninstall_Synclet : CCM_HandlerSynclet
            {
                //Constructor
                public Windows8App_Uninstall_Synclet(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                {
                    remoteRunspace = RemoteRunspace;
                    pSCode = PSCode;

                    this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                    this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                    this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                    this.__INSTANCE = true;
                    this.WMIObject = WMIObject;
                    this.DisplayName = WMIObject.Properties["DisplayName"].Value as String;
                    this.FastRetryExitCodes = WMIObject.Properties["FastRetryExitCodes"].Value as UInt32?[];
                    this.IsDeepLink = WMIObject.Properties["IsDeepLink"].Value as Boolean?;
                    this.MaxExecuteTime = WMIObject.Properties["MaxExecuteTime"].Value as UInt32?;
                    this.Name = WMIObject.Properties["Name"].Value as String;
                    this.ProcessorArchitecture = WMIObject.Properties["ProcessorArchitecture"].Value as String;
                    this.Publisher = WMIObject.Properties["Publisher"].Value as String;
                    this.ResourceId = WMIObject.Properties["ResourceId"].Value as String;
                    this.SuccessExitCodes = WMIObject.Properties["SuccessExitCodes"].Value as UInt32?[];
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
                public String DisplayName { get; set; }
                public UInt32?[] FastRetryExitCodes { get; set; }
                public Boolean? IsDeepLink { get; set; }
                public UInt32? MaxExecuteTime { get; set; }
                public String Name { get; set; }
                public String ProcessorArchitecture { get; set; }
                public String Publisher { get; set; }
                public String ResourceId { get; set; }
                public UInt32?[] SuccessExitCodes { get; set; }
                public String Version { get; set; }
                #endregion

            }
            */

    }
#endif
}
