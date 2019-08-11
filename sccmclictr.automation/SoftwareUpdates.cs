//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2018 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

#define CM2012
#define CM2007

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;

namespace sccmclictr.automation.functions
{
    /// <summary>
    /// Software Update Class
    /// </summary>
    public class softwareupdates : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="softwareupdates"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        /// <param name="oClient">A CCM Client object.</param>
        public softwareupdates(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Get all known Software Updates with status
        /// </summary>
        public List<CCM_UpdateStatus> UpdateStatus
        {
            get
            {
                //retrun cached results
                return GetUpdateStatus(false);
            }
        }

        /// <summary>
        /// Get all known Software Updates with status
        /// </summary>
        /// <param name="reLoad">true = force reload; false = use cached results</param>
        /// <returns></returns>
        public List<CCM_UpdateStatus> GetUpdateStatus(bool reLoad)
        {
                List<CCM_UpdateStatus> lCache = new List<CCM_UpdateStatus>();

            //Cache for 2min
            List<PSObject> oObj = GetObjects(@"root\ccm\SoftwareUpdates\UpdatesStore", "SELECT * FROM CCM_UpdateStatus", reLoad, new TimeSpan(0, 2, 0));
                foreach (PSObject PSObj in oObj)
                {
                    CCM_UpdateStatus oUpdStat = new CCM_UpdateStatus(PSObj, remoteRunspace, pSCode);

                    oUpdStat.remoteRunspace = remoteRunspace;
                    oUpdStat.pSCode = pSCode;
                    lCache.Add(oUpdStat);
                }
                return lCache;
            }

        /// <summary>
        /// Get all mandatory Updates
        /// </summary>
        public List<CCM_TargetedUpdateEx1> TargetUpdates
        {
            get
            {
                List<CCM_TargetedUpdateEx1> lCache = new List<CCM_TargetedUpdateEx1>();
                List<PSObject> oObj = GetObjects(@"root\ccm\SoftwareUpdates\DeploymentAgent", "SELECT * FROM CCM_TargetedUpdateEx1");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_TargetedUpdateEx1 oUpdStat = new CCM_TargetedUpdateEx1(PSObj, remoteRunspace, pSCode);

                    oUpdStat.remoteRunspace = remoteRunspace;
                    oUpdStat.pSCode = pSCode;
                    lCache.Add(oUpdStat);
                }
                return lCache;
            }
        }

        /// <summary>
        /// Get the Content Locations for Software Updates
        /// </summary>
        public List<CCM_UpdateSource> UpdateSource
        {
            get
            {
                List<CCM_UpdateSource> lCache = new List<CCM_UpdateSource>();
                List<PSObject> oObj = GetObjects(@"root\ccm\SoftwareUpdates\WUAHandler", "SELECT * FROM CCM_UpdateSource");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_UpdateSource oUpdStat = new CCM_UpdateSource(PSObj, remoteRunspace, pSCode);

                    oUpdStat.remoteRunspace = remoteRunspace;
                    oUpdStat.pSCode = pSCode;
                    lCache.Add(oUpdStat);
                }
                return lCache;
            }
        }

        /// <summary>
        /// Show required Software Updates and the current state (from cache)
        /// </summary>
        public List<CCM_SoftwareUpdate> SoftwareUpdate
        {
            get
            {
                return GetSoftwareUpdate(false);
            }
        }

        /// <summary>
        /// Show required Software Updates and the current state
        /// </summary>
        /// <param name="reLoad">Enforce reloading the cache</param>
        /// <returns></returns>
        public List<CCM_SoftwareUpdate> GetSoftwareUpdate(bool reLoad)
        {
            return GetSoftwareUpdate(reLoad, new TimeSpan(0, 2, 0));
        }

        /// <summary>
        /// Show required Software Updates and the current state
        /// </summary>
        /// <param name="reLoad">Enforce reloading the cache</param>
        /// <param name="tCache">TTL to keep Items in Cache</param>
        /// <returns></returns>
        public List<CCM_SoftwareUpdate> GetSoftwareUpdate(bool reLoad, TimeSpan tCache)
        {
            List<CCM_SoftwareUpdate> lCache = new List<CCM_SoftwareUpdate>();
            List<PSObject> oObj = GetObjects(@"ROOT\ccm\ClientSDK", "SELECT * FROM CCM_SoftwareUpdate", reLoad, tCache);
            foreach (PSObject PSObj in oObj)
            {
                //Get AppDTs sub Objects
                CCM_SoftwareUpdate oUpdStat = new CCM_SoftwareUpdate(PSObj, remoteRunspace, pSCode);

                oUpdStat.remoteRunspace = remoteRunspace;
                oUpdStat.pSCode = pSCode;
                lCache.Add(oUpdStat);
            }
            return lCache;
        }

        /// <summary>
        /// Source:root\ccm\SoftwareUpdates\UpdatesStore
        /// </summary>
        public class CCM_UpdateStatus
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_UpdateStatus"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_UpdateStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;

                this.Article = WMIObject.Properties["Article"].Value as string;
                this.Bulletin = WMIObject.Properties["Bulletin"].Value as string;
                this.Language = WMIObject.Properties["Language"].Value as string;
                this.ProductID = WMIObject.Properties["ProductID"].Value as string;
                this.RevisionNumber = WMIObject.Properties["RevisionNumber"].Value as UInt32?;
                string sScanTime = WMIObject.Properties["ScanTime"].Value as string;
                if (string.IsNullOrEmpty(sScanTime))
                    this.ScanTime = null;
                else
                    this.ScanTime = ManagementDateTimeConverter.ToDateTime(sScanTime) as DateTime?;

                //Not implemented
                this.Sources = null;

                this.SourceType = WMIObject.Properties["SourceType"].Value as UInt32?;
                this.SourceUniqueId = WMIObject.Properties["SourceUniqueId"].Value as string;
                this.SourceVersion = WMIObject.Properties["SourceVersion"].Value as UInt32?;
                this.Status = WMIObject.Properties["Status"].Value as string;
                this.Title = WMIObject.Properties["Title"].Value as string;
                this.UniqueId = WMIObject.Properties["UniqueId"].Value as string;

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
            /// Gets or sets the article.
            /// </summary>
            /// <value>The article.</value>
            public String Article { get; set; }

            /// <summary>
            /// Gets or sets the bulletin.
            /// </summary>
            /// <value>The bulletin.</value>
            public String Bulletin { get; set; }

            /// <summary>
            /// Gets or sets the language.
            /// </summary>
            /// <value>The language.</value>
            public String Language { get; set; }

            /// <summary>
            /// Gets or sets the product identifier.
            /// </summary>
            /// <value>The product identifier.</value>
            public String ProductID { get; set; }

            /// <summary>
            /// Gets or sets the revision number.
            /// </summary>
            /// <value>The revision number.</value>
            public UInt32? RevisionNumber { get; set; }

            /// <summary>
            /// Gets or sets the scan time.
            /// </summary>
            /// <value>The scan time.</value>
            public DateTime? ScanTime { get; set; }

            /// <summary>
            /// Gets or sets the sources.
            /// </summary>
            /// <value>The sources.</value>
            public CCM_SourceStatus[] Sources { get; set; }

            /// <summary>
            /// Gets or sets the type of the source.
            /// </summary>
            /// <value>The type of the source.</value>
            public UInt32? SourceType { get; set; }

            /// <summary>
            /// Gets or sets the source unique identifier.
            /// </summary>
            /// <value>The source unique identifier.</value>
            public String SourceUniqueId { get; set; }

            /// <summary>
            /// Gets or sets the source version.
            /// </summary>
            /// <value>The source version.</value>
            public UInt32? SourceVersion { get; set; }

            /// <summary>
            /// Gets or sets the status.
            /// </summary>
            /// <value>The status.</value>
            public String Status { get; set; }

            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            /// <value>The title.</value>
            public String Title { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier.
            /// </summary>
            /// <value>The unique identifier.</value>
            public String UniqueId { get; set; }

            #endregion

        }

        /// <summary>
        /// Source:root\ccm\SoftwareUpdates\UpdatesStore
        /// </summary>
        public class CCM_SourceStatus
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_SourceStatus"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_SourceStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.RevisionNumber = WMIObject.Properties["RevisionNumber"].Value as UInt32?;
                this.ScanTime = WMIObject.Properties["ScanTime"].Value as DateTime?;
                this.SourceType = WMIObject.Properties["SourceType"].Value as UInt32?;
                this.SourceUniqueId = WMIObject.Properties["SourceUniqueId"].Value as String;
                this.SourceVersion = WMIObject.Properties["SourceVersion"].Value as UInt32?;
                this.Status = WMIObject.Properties["Status"].Value as String;
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
            /// Gets or sets the revision number.
            /// </summary>
            /// <value>The revision number.</value>
            public UInt32? RevisionNumber { get; set; }

            /// <summary>
            /// Gets or sets the scan time.
            /// </summary>
            /// <value>The scan time.</value>
            public DateTime? ScanTime { get; set; }

            /// <summary>
            /// Gets or sets the type of the source.
            /// </summary>
            /// <value>The type of the source.</value>
            public UInt32? SourceType { get; set; }

            /// <summary>
            /// Gets or sets the source unique identifier.
            /// </summary>
            /// <value>The source unique identifier.</value>
            public String SourceUniqueId { get; set; }

            /// <summary>
            /// Gets or sets the source version.
            /// </summary>
            /// <value>The source version.</value>
            public UInt32? SourceVersion { get; set; }

            /// <summary>
            /// Gets or sets the status.
            /// </summary>
            /// <value>The status.</value>
            public String Status { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_AssignmentCompliance
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_AssignmentCompliance"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_AssignmentCompliance(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AssignmentId = WMIObject.Properties["AssignmentId"].Value as String;
                this.IsCompliant = WMIObject.Properties["IsCompliant"].Value as Boolean?;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.Signature = WMIObject.Properties["Signature"].Value as String;
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
            /// Gets or sets the assignment identifier.
            /// </summary>
            /// <value>The assignment identifier.</value>
            public String AssignmentId { get; set; }

            /// <summary>
            /// Gets or sets the is compliant.
            /// </summary>
            /// <value>The is compliant.</value>
            public Boolean? IsCompliant { get; set; }

            /// <summary>
            /// Gets or sets the reserved.
            /// </summary>
            /// <value>The reserved.</value>
            public String Reserved { get; set; }

            /// <summary>
            /// Gets or sets the signature.
            /// </summary>
            /// <value>The signature.</value>
            public String Signature { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_AssignmentJobEx1
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_AssignmentJobEx1"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_AssignmentJobEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AssignmentId = WMIObject.Properties["AssignmentId"].Value as String;
                this.AssignmentState = WMIObject.Properties["AssignmentState"].Value as UInt32?;
                this.AssignmentSubState = WMIObject.Properties["AssignmentSubState"].Value as UInt32?;
                this.AssignmentTrigger = WMIObject.Properties["AssignmentTrigger"].Value as UInt32?;
                this.JobId = WMIObject.Properties["JobId"].Value as String;
                this.ReEvaluateTrigger = WMIObject.Properties["ReEvaluateTrigger"].Value as UInt32?;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
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
            /// Gets or sets the assignment identifier.
            /// </summary>
            /// <value>The assignment identifier.</value>
            public String AssignmentId { get; set; }

            /// <summary>
            /// Gets or sets the state of the assignment.
            /// </summary>
            /// <value>The state of the assignment.</value>
            public UInt32? AssignmentState { get; set; }

            /// <summary>
            /// Gets or sets the state of the assignment sub.
            /// </summary>
            /// <value>The state of the assignment sub.</value>
            public UInt32? AssignmentSubState { get; set; }

            /// <summary>
            /// Gets or sets the assignment trigger.
            /// </summary>
            /// <value>The assignment trigger.</value>
            public UInt32? AssignmentTrigger { get; set; }

            /// <summary>
            /// Gets or sets the job identifier.
            /// </summary>
            /// <value>The job identifier.</value>
            public String JobId { get; set; }

            /// <summary>
            /// Gets or sets the re evaluate trigger.
            /// </summary>
            /// <value>The re evaluate trigger.</value>
            public UInt32? ReEvaluateTrigger { get; set; }

            /// <summary>
            /// Gets or sets the reserved.
            /// </summary>
            /// <value>The reserved.</value>
            public String Reserved { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_ComponentState
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_ComponentState"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_ComponentState(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ComponentName = WMIObject.Properties["ComponentName"].Value as String;
                this.MaxPauseDuration = WMIObject.Properties["MaxPauseDuration"].Value as UInt32?;
                this.PauseCookie = WMIObject.Properties["PauseCookie"].Value as UInt32?;
                string sPauseStartTime = WMIObject.Properties["PauseStartTime"].Value as string;
                if (string.IsNullOrEmpty(sPauseStartTime))
                    this.PauseStartTime = null;
                else
                    this.PauseStartTime = ManagementDateTimeConverter.ToDateTime(sPauseStartTime) as DateTime?;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
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
            /// Gets or sets the name of the component.
            /// </summary>
            /// <value>The name of the component.</value>
            public String ComponentName { get; set; }

            /// <summary>
            /// Gets or sets the maximum duration of the pause.
            /// </summary>
            /// <value>The maximum duration of the pause.</value>
            public UInt32? MaxPauseDuration { get; set; }

            /// <summary>
            /// Gets or sets the pause cookie.
            /// </summary>
            /// <value>The pause cookie.</value>
            public UInt32? PauseCookie { get; set; }

            /// <summary>
            /// Gets or sets the pause start time.
            /// </summary>
            /// <value>The pause start time.</value>
            public DateTime? PauseStartTime { get; set; }

            /// <summary>
            /// Gets or sets the reserved.
            /// </summary>
            /// <value>The reserved.</value>
            public String Reserved { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_DeploymentTaskEx1
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_DeploymentTaskEx1"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_DeploymentTaskEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AssignmentId = WMIObject.Properties["AssignmentId"].Value as String;
                this.DetectJobId = WMIObject.Properties["DetectJobId"].Value as String;
                this.IsEnforcedInstall = WMIObject.Properties["IsEnforcedInstall"].Value as Boolean?;
                this.JobId = WMIObject.Properties["JobId"].Value as String;
                this.nKey = WMIObject.Properties["nKey"].Value as UInt32?;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
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
            /// Gets or sets the assignment identifier.
            /// </summary>
            /// <value>The assignment identifier.</value>
            public String AssignmentId { get; set; }

            /// <summary>
            /// Gets or sets the detect job identifier.
            /// </summary>
            /// <value>The detect job identifier.</value>
            public String DetectJobId { get; set; }

            /// <summary>
            /// Gets or sets the is enforced install.
            /// </summary>
            /// <value>The is enforced install.</value>
            public Boolean? IsEnforcedInstall { get; set; }

            /// <summary>
            /// Gets or sets the job identifier.
            /// </summary>
            /// <value>The job identifier.</value>
            public String JobId { get; set; }

            /// <summary>
            /// Gets or sets the n key.
            /// </summary>
            /// <value>The n key.</value>
            public UInt32? nKey { get; set; }

            /// <summary>
            /// Gets or sets the reserved.
            /// </summary>
            /// <value>The reserved.</value>
            public String Reserved { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_SUMLocalSettings
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_SUMLocalSettings"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_SUMLocalSettings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.nKey = WMIObject.Properties["nKey"].Value as UInt32?;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.UserExperience = WMIObject.Properties["UserExperience"].Value as UInt32?;
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
            /// Gets or sets the n key.
            /// </summary>
            /// <value>The n key.</value>
            public UInt32? nKey { get; set; }

            /// <summary>
            /// Gets or sets the reserved.
            /// </summary>
            /// <value>The reserved.</value>
            public String Reserved { get; set; }

            /// <summary>
            /// Gets or sets the user experience.
            /// </summary>
            /// <value>The user experience.</value>
            public UInt32? UserExperience { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_TargetedUpdateEx1
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_TargetedUpdateEx1"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_TargetedUpdateEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                string sDeadline = WMIObject.Properties["Deadline"].Value as string;
                if (string.IsNullOrEmpty(sDeadline))
                    this.Deadline = null;
                else
                    this.Deadline = ManagementDateTimeConverter.ToDateTime(sDeadline) as DateTime?;
                this.DisableMomAlerts = WMIObject.Properties["DisableMomAlerts"].Value as Boolean?;
                this.DownloadSize = WMIObject.Properties["DownloadSize"].Value as UInt32?;
                this.DPLocality = WMIObject.Properties["DPLocality"].Value as UInt32?;
                this.IsDeleted = WMIObject.Properties["IsDeleted"].Value as Boolean?;
                this.KeepHidden = WMIObject.Properties["KeepHidden"].Value as Boolean?;
                this.LimitStateMessageVerbosity = WMIObject.Properties["LimitStateMessageVerbosity"].Value as Boolean?;
                this.OverrideServiceWindows = WMIObject.Properties["OverrideServiceWindows"].Value as Boolean?;
                this.PercentComplete = WMIObject.Properties["PercentComplete"].Value as UInt32?;
                this.RaiseMomAlertsOnFailure = WMIObject.Properties["RaiseMomAlertsOnFailure"].Value as Boolean?;
                string sRebootDeadline = WMIObject.Properties["RebootDeadline"].Value as string;
                if (string.IsNullOrEmpty(sRebootDeadline))
                    this.RebootDeadline = null;
                else
                    this.RebootDeadline = ManagementDateTimeConverter.ToDateTime(sRebootDeadline) as DateTime?;
                this.RebootOutsideOfServiceWindows = WMIObject.Properties["RebootOutsideOfServiceWindows"].Value as Boolean?;
                this.RefAssignments = WMIObject.Properties["RefAssignments"].Value as String;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.ScheduledUpdateOptions = WMIObject.Properties["ScheduledUpdateOptions"].Value as UInt32?;
                string sStartTime = WMIObject.Properties["StartTime"].Value as string;
                if (string.IsNullOrEmpty(sStartTime))
                    this.StartTime = null;
                else
                    this.StartTime = ManagementDateTimeConverter.ToDateTime(sStartTime) as DateTime?;
                this.UpdateAction = WMIObject.Properties["UpdateAction"].Value as UInt32?;
                this.UpdateApplicability = WMIObject.Properties["UpdateApplicability"].Value as UInt32?;
                this.UpdateId = WMIObject.Properties["UpdateId"].Value as String;
                this.UpdateState = WMIObject.Properties["UpdateState"].Value as UInt32?;
                this.UpdateStatus = WMIObject.Properties["UpdateStatus"].Value as UInt32?;
                this.UpdateVersion = WMIObject.Properties["UpdateVersion"].Value as String;
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

            /// <summary>
            /// Gets or sets the deadline.
            /// </summary>
            /// <value>The deadline.</value>
            public DateTime? Deadline { get; set; }

            /// <summary>
            /// Gets or sets the disable mom alerts.
            /// </summary>
            /// <value>The disable mom alerts.</value>
            public Boolean? DisableMomAlerts { get; set; }

            /// <summary>
            /// Gets or sets the size of the download.
            /// </summary>
            /// <value>The size of the download.</value>
            public UInt32? DownloadSize { get; set; }

            /// <summary>
            /// Gets or sets the dp locality.
            /// </summary>
            /// <value>The dp locality.</value>
            public UInt32? DPLocality { get; set; }

            /// <summary>
            /// Gets or sets the is deleted.
            /// </summary>
            /// <value>The is deleted.</value>
            public Boolean? IsDeleted { get; set; }

            /// <summary>
            /// Gets or sets the keep hidden.
            /// </summary>
            /// <value>The keep hidden.</value>
            public Boolean? KeepHidden { get; set; }

            /// <summary>
            /// Gets or sets the limit state message verbosity.
            /// </summary>
            /// <value>The limit state message verbosity.</value>
            public Boolean? LimitStateMessageVerbosity { get; set; }

            /// <summary>
            /// Gets or sets the override service windows.
            /// </summary>
            /// <value>The override service windows.</value>
            public Boolean? OverrideServiceWindows { get; set; }

            /// <summary>
            /// Gets or sets the percent complete.
            /// </summary>
            /// <value>The percent complete.</value>
            public UInt32? PercentComplete { get; set; }

            /// <summary>
            /// Gets or sets the raise mom alerts on failure.
            /// </summary>
            /// <value>The raise mom alerts on failure.</value>
            public Boolean? RaiseMomAlertsOnFailure { get; set; }

            /// <summary>
            /// Gets or sets the reboot deadline.
            /// </summary>
            /// <value>The reboot deadline.</value>
            public DateTime? RebootDeadline { get; set; }

            /// <summary>
            /// Gets or sets the reboot outside of service windows.
            /// </summary>
            /// <value>The reboot outside of service windows.</value>
            public Boolean? RebootOutsideOfServiceWindows { get; set; }

            /// <summary>
            /// Gets or sets the reference assignments.
            /// </summary>
            /// <value>The reference assignments.</value>
            public String RefAssignments { get; set; }

            /// <summary>
            /// Gets or sets the reserved.
            /// </summary>
            /// <value>The reserved.</value>
            public String Reserved { get; set; }

            /// <summary>
            /// Gets or sets the scheduled update options.
            /// </summary>
            /// <value>The scheduled update options.</value>
            public UInt32? ScheduledUpdateOptions { get; set; }

            /// <summary>
            /// Gets or sets the start time.
            /// </summary>
            /// <value>The start time.</value>
            public DateTime? StartTime { get; set; }

            /// <summary>
            /// Gets or sets the update action.
            /// </summary>
            /// <value>The update action.</value>
            public UInt32? UpdateAction { get; set; }

            /// <summary>
            /// Gets or sets the update applicability.
            /// </summary>
            /// <value>The update applicability.</value>
            public UInt32? UpdateApplicability { get; set; }

            /// <summary>
            /// Gets or sets the update identifier.
            /// </summary>
            /// <value>The update identifier.</value>
            public String UpdateId { get; set; }

            /// <summary>
            /// Gets or sets the state of the update.
            /// </summary>
            /// <value>The state of the update.</value>
            public UInt32? UpdateState { get; set; }

            /// <summary>
            /// Gets or sets the update status.
            /// </summary>
            /// <value>The update status.</value>
            public UInt32? UpdateStatus { get; set; }

            /// <summary>
            /// Gets or sets the update version.
            /// </summary>
            /// <value>The update version.</value>
            public String UpdateVersion { get; set; }

            /// <summary>
            /// Gets or sets the user UI experience.
            /// </summary>
            /// <value>The user UI experience.</value>
            public Boolean? UserUIExperience { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\Handler
        /// </summary>
        public class CCM_AtomicUpdateEx1
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_AtomicUpdateEx1"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_AtomicUpdateEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.CachedContentInUse = WMIObject.Properties["CachedContentInUse"].Value as UInt32?;
                this.ContentPriority = WMIObject.Properties["ContentPriority"].Value as UInt32?;
                this.ContentRequestId = WMIObject.Properties["ContentRequestId"].Value as String;
                this.ExecutionRequestId = WMIObject.Properties["ExecutionRequestId"].Value as String;
                this.FailureDetail = WMIObject.Properties["FailureDetail"].Value as String;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.UpdateID = WMIObject.Properties["UpdateID"].Value as String;
                this.UpdateResult = WMIObject.Properties["UpdateResult"].Value as UInt32?;
                this.UpdateState = WMIObject.Properties["UpdateState"].Value as UInt32?;
                this.UpdateVersion = WMIObject.Properties["UpdateVersion"].Value as String;
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
            /// Gets or sets the cached content in use.
            /// </summary>
            /// <value>The cached content in use.</value>
            public UInt32? CachedContentInUse { get; set; }

            /// <summary>
            /// Gets or sets the content priority.
            /// </summary>
            /// <value>The content priority.</value>
            public UInt32? ContentPriority { get; set; }

            /// <summary>
            /// Gets or sets the content request identifier.
            /// </summary>
            /// <value>The content request identifier.</value>
            public String ContentRequestId { get; set; }

            /// <summary>
            /// Gets or sets the execution request identifier.
            /// </summary>
            /// <value>The execution request identifier.</value>
            public String ExecutionRequestId { get; set; }

            /// <summary>
            /// Gets or sets the failure detail.
            /// </summary>
            /// <value>The failure detail.</value>
            public String FailureDetail { get; set; }

            /// <summary>
            /// Gets or sets the reserved.
            /// </summary>
            /// <value>The reserved.</value>
            public String Reserved { get; set; }

            /// <summary>
            /// Gets or sets the update identifier.
            /// </summary>
            /// <value>The update identifier.</value>
            public String UpdateID { get; set; }

            /// <summary>
            /// Gets or sets the update result.
            /// </summary>
            /// <value>The update result.</value>
            public UInt32? UpdateResult { get; set; }

            /// <summary>
            /// Gets or sets the state of the update.
            /// </summary>
            /// <value>The state of the update.</value>
            public UInt32? UpdateState { get; set; }

            /// <summary>
            /// Gets or sets the update version.
            /// </summary>
            /// <value>The update version.</value>
            public String UpdateVersion { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\Handler
        /// </summary>
        public class CCM_BundledUpdateEx1
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_BundledUpdateEx1"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The ps code.</param>
            public CCM_BundledUpdateEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.CachedContentInUse = WMIObject.Properties["CachedContentInUse"].Value as UInt32?;
                this.ContentPriority = WMIObject.Properties["ContentPriority"].Value as UInt32?;
                this.ContentRequestId = WMIObject.Properties["ContentRequestId"].Value as String;
                this.ExecutionRequestId = WMIObject.Properties["ExecutionRequestId"].Value as String;
                this.FailureDetail = WMIObject.Properties["FailureDetail"].Value as String;
                this.IsSelfDownloadComplete = WMIObject.Properties["IsSelfDownloadComplete"].Value as Boolean?;
                this.IsSelfInstalling = WMIObject.Properties["IsSelfInstalling"].Value as Boolean?;
                this.RelatedUpdates = WMIObject.Properties["RelatedUpdates"].Value as String;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.UpdateID = WMIObject.Properties["UpdateID"].Value as String;
                this.UpdateResult = WMIObject.Properties["UpdateResult"].Value as UInt32?;
                this.UpdateState = WMIObject.Properties["UpdateState"].Value as UInt32?;
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
            /// Gets or sets the cached content in use.
            /// </summary>
            /// <value>The cached content in use.</value>
            public UInt32? CachedContentInUse { get; set; }

            /// <summary>
            /// Gets or sets the content priority.
            /// </summary>
            /// <value>The content priority.</value>
            public UInt32? ContentPriority { get; set; }

            /// <summary>
            /// Gets or sets the content request identifier.
            /// </summary>
            /// <value>The content request identifier.</value>
            public String ContentRequestId { get; set; }

            /// <summary>
            /// Gets or sets the execution request identifier.
            /// </summary>
            /// <value>The execution request identifier.</value>
            public String ExecutionRequestId { get; set; }

            /// <summary>
            /// Gets or sets the failure detail.
            /// </summary>
            /// <value>The failure detail.</value>
            public String FailureDetail { get; set; }

            /// <summary>
            /// Gets or sets the is self download complete.
            /// </summary>
            /// <value>The is self download complete.</value>
            public Boolean? IsSelfDownloadComplete { get; set; }

            /// <summary>
            /// Gets or sets the is self installing.
            /// </summary>
            /// <value>The is self installing.</value>
            public Boolean? IsSelfInstalling { get; set; }

            /// <summary>
            /// Gets or sets the related updates.
            /// </summary>
            /// <value>The related updates.</value>
            public String RelatedUpdates { get; set; }

            /// <summary>
            /// Gets or sets the reserved.
            /// </summary>
            /// <value>The reserved.</value>
            public String Reserved { get; set; }

            /// <summary>
            /// Gets or sets the update identifier.
            /// </summary>
            /// <value>The update identifier.</value>
            public String UpdateID { get; set; }

            /// <summary>
            /// Gets or sets the update result.
            /// </summary>
            /// <value>The update result.</value>
            public UInt32? UpdateResult { get; set; }

            /// <summary>
            /// Gets or sets the state of the update.
            /// </summary>
            /// <value>The state of the update.</value>
            public UInt32? UpdateState { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\Handler
        /// </summary>
        public class CCM_UpdatesDeploymentJobEx1
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_UpdatesDeploymentJobEx1"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_UpdatesDeploymentJobEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.HandleForMTC = WMIObject.Properties["HandleForMTC"].Value as String;
                this.IsCompleted = WMIObject.Properties["IsCompleted"].Value as Boolean?;
                this.IsOwnerOfMTCTask = WMIObject.Properties["IsOwnerOfMTCTask"].Value as Boolean?;
                this.IsRequestSumbittedToMTC = WMIObject.Properties["IsRequestSumbittedToMTC"].Value as Boolean?;
                this.JobID = WMIObject.Properties["JobID"].Value as String;
                this.JobState = WMIObject.Properties["JobState"].Value as UInt32?;
                this.JobType = WMIObject.Properties["JobType"].Value as UInt32?;
                this.JobUpdates = WMIObject.Properties["JobUpdates"].Value as String;
                this.NotifyDownloadComplete = WMIObject.Properties["NotifyDownloadComplete"].Value as String;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.TaskPriorityForMTC = WMIObject.Properties["TaskPriorityForMTC"].Value as UInt32?;
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
            /// Gets or sets the handle for MTC.
            /// </summary>
            /// <value>The handle for MTC.</value>
            public String HandleForMTC { get; set; }

            /// <summary>
            /// Gets or sets the is completed.
            /// </summary>
            /// <value>The is completed.</value>
            public Boolean? IsCompleted { get; set; }

            /// <summary>
            /// Gets or sets the is owner of MTC task.
            /// </summary>
            /// <value>The is owner of MTC task.</value>
            public Boolean? IsOwnerOfMTCTask { get; set; }

            /// <summary>
            /// Gets or sets the is request sumbitted to MTC.
            /// </summary>
            /// <value>The is request sumbitted to MTC.</value>
            public Boolean? IsRequestSumbittedToMTC { get; set; }

            /// <summary>
            /// Gets or sets the job identifier.
            /// </summary>
            /// <value>The job identifier.</value>
            public String JobID { get; set; }

            /// <summary>
            /// Gets or sets the state of the job.
            /// </summary>
            /// <value>The state of the job.</value>
            public UInt32? JobState { get; set; }

            /// <summary>
            /// Gets or sets the type of the job.
            /// </summary>
            /// <value>The type of the job.</value>
            public UInt32? JobType { get; set; }

            /// <summary>
            /// Gets or sets the job updates.
            /// </summary>
            /// <value>The job updates.</value>
            public String JobUpdates { get; set; }

            /// <summary>
            /// Gets or sets the notify download complete.
            /// </summary>
            /// <value>The notify download complete.</value>
            public String NotifyDownloadComplete { get; set; }

            /// <summary>
            /// Gets or sets the reserved.
            /// </summary>
            /// <value>The reserved.</value>
            public String Reserved { get; set; }

            /// <summary>
            /// Gets or sets the task priority for MTC.
            /// </summary>
            /// <value>The task priority for MTC.</value>
            public UInt32? TaskPriorityForMTC { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\WUAHandler
        /// </summary>
        public class CCM_UpdateSource
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_UpdateSource"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_UpdateSource(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ContentLocation = WMIObject.Properties["ContentLocation"].Value as String;
                this.ContentType = WMIObject.Properties["ContentType"].Value as UInt32?;
                this.ContentVersion = WMIObject.Properties["ContentVersion"].Value as UInt32?;
                this.ServiceId = WMIObject.Properties["ServiceId"].Value as String;
                this.UniqueId = WMIObject.Properties["UniqueId"].Value as String;
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
            /// Gets or sets the content location.
            /// </summary>
            /// <value>The content location.</value>
            public String ContentLocation { get; set; }

            /// <summary>
            /// Gets or sets the type of the content.
            /// </summary>
            /// <value>The type of the content.</value>
            public UInt32? ContentType { get; set; }

            /// <summary>
            /// Gets or sets the content version.
            /// </summary>
            /// <value>The content version.</value>
            public UInt32? ContentVersion { get; set; }

            /// <summary>
            /// Gets or sets the service identifier.
            /// </summary>
            /// <value>The service identifier.</value>
            public String ServiceId { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier.
            /// </summary>
            /// <value>The unique identifier.</value>
            public String UniqueId { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\ClientSDK
        /// </summary>
        public class CCM_SoftwareUpdate : softwaredistribution.CCM_SoftwareBase
        {
            internal baseInit oNewBase;

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
                oNewBase = new baseInit(remoteRunspace, pSCode);

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
                {
                    this.RestartDeadline = ManagementDateTimeConverter.ToDateTime(sRestartDeadline) as DateTime?;
                    this.RestartDeadline = ((DateTime)this.RestartDeadline).ToUniversalTime();
                }
                string sStartTime = WMIObject.Properties["StartTime"].Value as string;
                if (string.IsNullOrEmpty(sStartTime))
                    this.StartTime = null;
                else
                {
                    this.StartTime = ManagementDateTimeConverter.ToDateTime(sStartTime) as DateTime?;
                    this.StartTime = ((DateTime)this.StartTime).ToUniversalTime();
                }
                this.UpdateID = WMIObject.Properties["UpdateID"].Value as String;
                this.URL = WMIObject.Properties["URL"].Value as String;
                this.UserUIExperience = WMIObject.Properties["UserUIExperience"].Value as Boolean?;
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
            /// Gets or sets the article identifier.
            /// </summary>
            /// <value>The article identifier.</value>
            public String ArticleID { get; set; }

            /// <summary>
            /// Gets or sets the bulletin identifier.
            /// </summary>
            /// <value>The bulletin identifier.</value>
            public String BulletinID { get; set; }

            /// <summary>
            /// Gets or sets the state of the compliance.
            /// </summary>
            /// <value>The state of the compliance.</value>
            public UInt32? ComplianceState { get; set; }

            /// <summary>
            /// Gets or sets the exclusive update.
            /// </summary>
            /// <value>The exclusive update.</value>
            public Boolean? ExclusiveUpdate { get; set; }


            /// <summary>
            /// Maximum time required for the software update to run.
            /// </summary>
            public UInt32? MaxExecutionTime { get; set; }


            /// <summary>
            /// true if notifications for the software update are shown to the user; otherwise, false.
            /// Ignored if UserUIExperiene is set to false.
            /// </summary>
            public Boolean? NotifyUser { get; set; }

            /// <summary>
            /// Gets or sets the override service windows.
            /// </summary>
            /// <value>The override service windows.</value>
            public Boolean? OverrideServiceWindows { get; set; }

            /// <summary>
            /// Gets or sets the reboot outside service windows.
            /// </summary>
            /// <value>The reboot outside service windows.</value>
            public Boolean? RebootOutsideServiceWindows { get; set; }

            /// <summary>
            /// Gets or sets the restart deadline.
            /// </summary>
            /// <value>The restart deadline.</value>
            public DateTime? RestartDeadline { get; set; }


            /// <summary>
            /// Date and time when the software update is made available to the user.
            /// </summary>
            public DateTime? StartTime { get; set; }

            /// <summary>
            /// Gets or sets the update identifier.
            /// </summary>
            /// <value>The update identifier.</value>
            public String UpdateID { get; set; }

            /// <summary>
            /// Gets or sets the URL.
            /// </summary>
            /// <value>The URL.</value>
            public String URL { get; set; }

            /// <summary>
            /// Gets or sets the user UI experience.
            /// </summary>
            /// <value>The user UI experience.</value>
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

            /// <summary>
            /// Installs this instance.
            /// </summary>
            public void Install()
            {
                string sCode = string.Format("$a = get-wmiobject -query \"SELECT * FROM CCM_SoftwareUpdate WHERE UpdateID='{0}'\" -namespace \"ROOT\\ccm\\ClientSDK\";([wmiclass]'ROOT\\ccm\\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates($a)", this.UpdateID);
                oNewBase.GetObjectsFromPS(sCode, true);
            }

        }

        /// <summary>
        /// Install all required updates with a deadline (mandatory).
        /// </summary>
        public void InstallAllRequiredUpdates()
        {
            string sCode = string.Format("([wmiclass]'ROOT\\ccm\\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates()");
            baseClient.GetObjectsFromPS(sCode, true);
        }

        /// <summary>
        /// Install all approved updates even if they do not have a deadline.
        /// </summary>
        public void InstallAllApprovedUpdates()
        {
            string sCode = string.Format(@"([wmiclass]'ROOT\ccm\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates([System.Management.ManagementObject[]] (get-wmiobject -query 'SELECT * FROM CCM_SoftwareUpdate' -namespace 'ROOT\ccm\ClientSDK'))");
            baseClient.GetObjectsFromPS(sCode, true);
        }

        /// <summary>
        /// Check if there are updates with pending reboot
        /// </summary>
        /// <returns></returns>
        public Boolean UpdateWithPendingReboot()
        {
            //8 = ciJobStatePendingSoftReboot
            //9 = ciJobStatePendingHardReboot
            //10= ciJobStateWaitReboot
            List<CCM_SoftwareUpdate> oPendingReboot = GetSoftwareUpdate(true).Where(t => (t.EvaluationState == 8) | (t.EvaluationState == 9) | (t.EvaluationState == 10)).ToList();

            if (oPendingReboot.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if there are pending Updates (Installation not started)
        /// </summary>
        /// <returns></returns>
        public Boolean UpdatesInstallationNotStarted()
        {
            //0 = ciJobStateNone
            //1 = ciJobStateAvailable
            //14= ciJobStateWaitServiceWindow
            //21= ciJobStateWaitingRetry
            List<CCM_SoftwareUpdate> oUpdStat = GetSoftwareUpdate(true).Where(t => (t.EvaluationState == 1) | (t.EvaluationState == 0) | (t.EvaluationState == 14) | (t.EvaluationState == 21)).ToList();

            if (oUpdStat.Count > 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Chek if there an update installation is running
        /// </summary>
        /// <returns></returns>
        public Boolean UpdateInstallationRunning()
        {
            //0 = ciJobStateNone
            //1 = ciJobStateAvailable
            List<CCM_SoftwareUpdate> oUpdStat = GetSoftwareUpdate(true).Where(t => (t.EvaluationState == 2) | (t.EvaluationState == 3) | (t.EvaluationState == 4)
                | (t.EvaluationState == 5) | (t.EvaluationState == 6) | (t.EvaluationState == 7) | (t.EvaluationState == 11)).ToList();

            if (oUpdStat.Count > 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Check if there are failed updates
        /// </summary>
        /// <returns></returns>
        public Boolean UpdateInstallationErrors()
        {
            //13= ciJobStateError
            List<CCM_SoftwareUpdate> oUpdStat = GetSoftwareUpdate(true).Where(t => (t.EvaluationState == 13)).ToList();

            if (oUpdStat.Count > 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Installs the updates.
        /// </summary>
        /// <param name="Updates">The updates.</param>
        public void InstallUpdates(List<CCM_SoftwareUpdate> Updates)
        {
            List<string> sUpdateIDs = new List<string>();
            sUpdateIDs = Updates.Select(t => t.UpdateID).ToList();

            string sIDs = string.Join("' OR UpdateID='", sUpdateIDs);

            string sCode = string.Format("[System.Management.ManagementObject[]] $a = get-wmiobject -query \"SELECT * FROM CCM_SoftwareUpdate WHERE UpdateID like '{0}'\" -namespace \"ROOT\\ccm\\ClientSDK\";([wmiclass]'ROOT\\ccm\\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates($a)", sIDs);
            baseClient.GetObjectsFromPS(sCode, true);
        }

    }



}
