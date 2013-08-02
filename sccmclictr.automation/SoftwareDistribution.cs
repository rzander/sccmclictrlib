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
                List<CCM_Application> lApps = new List<CCM_Application>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\ClientSDK", "SELECT * FROM CCM_Application");
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
        }

        /// <summary>
        /// List of the System Execution History (only Machine based !)
        /// </summary>
        public List<REG_ExecutionHistory> ExecutionHistory
        {
            get
            {
                
                List<REG_ExecutionHistory> lExec = new List<REG_ExecutionHistory>();
                List<PSObject> oObj = new List<PSObject>();
                Boolean bisSCCM2012 = baseClient.AgentProperties.isSCCM2012;
                Boolean bisx64OS = true;
                
                //Only Get Architecture if SCCM < 2012
                if(!bisSCCM2012)
                    bisx64OS = baseClient.Inventory.isx64OS;
                
                if(bisSCCM2012)
                    oObj = GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\\System\" -Recurse | % { get-itemproperty -path  $_.PsPath }", false, new TimeSpan(0, 0, 10));
                if (!bisSCCM2012 & bisx64OS)
                    oObj = GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\\System\" -Recurse | % { get-itemproperty -path  $_.PsPath }", false, new TimeSpan(0, 0, 10));
                if (!bisSCCM2012 & !bisx64OS)
                    oObj = GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\\System\" -Recurse | % { get-itemproperty -path  $_.PsPath }", false, new TimeSpan(0, 0, 10));

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
        }

        /// <summary>
        /// List of Package-Deployments (old Package-Model)
        /// </summary>
        public List<CCM_SoftwareDistribution> Advertisements
        {
            get
            {
                List<CCM_SoftwareDistribution> lApps = new List<CCM_SoftwareDistribution>();
                List<PSObject> oObj = GetObjects(@"root\ccm\policy\machine", "SELECT * FROM CCM_SoftwareDistribution");
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
        }

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
            #endregion

            #region Methods

            #endregion

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

        public class CCM_AppDeploymentType : CCM_SoftwareBase
        {
            #region Properties

            public String[] AllowedActions { get; set; }
            public String ApplicabilityState { get; set; }

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

            public String DeploymentReport { get; set; }
            public String Id { get; set; }
            public String InstallState { get; set; }
            public DateTime? LastEvalTime { get; set; }
            public String PostInstallAction { get; set; }
            public String ResolvedState { get; set; }
            public UInt32? RetriesRemaining { get; set; }
            public String Revision { get; set; }
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
                catch(Exception ex)
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

        public class CCM_Application : CCM_SoftwareBase
        {
            internal baseInit oNewBase;

            #region Properties

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
            /// Constructor
            /// </summary>
            /// <param name="WMIObject"></param>
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
            //Constructor
            public CCM_SoftwareDistribution(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode) : base(WMIObject)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

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
            }

            #region Properties

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
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_TaskSequence : CCM_SoftwareDistribution
        {
            //Constructor
            public CCM_TaskSequence(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode) : base(WMIObject, RemoteRunspace, PSCode)
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

            public String Reserved { get; set; }
            public String TS_BootImageID { get; set; }
            public DateTime? TS_Deadline { get; set; }
            public UInt32? TS_MandatoryCountdown { get; set; }
            public UInt32? TS_PopupReminderInterval { get; set; }
            public String[] TS_References { get; set; }
            public String TS_Sequence { get; set; }
            public UInt32? TS_Type { get; set; }
            public UInt32? TS_UserNotificationFlags { get; set; }
            #endregion

        }


        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_Scheduler_ScheduledMessage : CCM_Policy
        {
            //Constructor
            public CCM_Scheduler_ScheduledMessage(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode) : base(WMIObject)
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
            }

            #region Properties

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
            #endregion

        }

        /// <summary>
        /// Application Priorities
        /// </summary>
        public static class AppPriority
        {
            public static string Low
            {
                get { return "Low"; }
            }

            public static string Normal
            {
                get { return "Normal"; }
            }

            public static string High
            {
                get { return "High"; }
            }

            public static string Foreground
            {
                get { return "Foreground"; }
            }
        }

        public static class AppEnforcePreference
        {
            public static UInt32 Immediate
            {
                get { return 0; }
            }

            public static UInt32 NonBusinessHours
            {
                get { return 1; }
            }

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

            public String _ProgramID { get; set; }
            public String _State { get; set; }
            public DateTime? _RunStartTime { get; set; }
            public int? SuccessOrFailureCode { get; set; }
            public string SuccessOrFailureReason { get; set; }
            public string UserID { get; set; }
            public string PackageID { get; set; }

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

        }
    }



#endif
}
