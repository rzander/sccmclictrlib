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
    public class processes : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public processes(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        public List<Win32_Process> Win32_Processes
        {
            get
            {
                List<Win32_Process> lCache = new List<Win32_Process>();
                List<PSObject> oObj = GetObjects(@"ROOT\cimv2", "SELECT * FROM Win32_Process", false);
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    Win32_Process oCIEx = new Win32_Process(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }

            set { }
        }

        internal List<ExtProcess> LoadExtProcess(bool Reload)
        {
            List<ExtProcess> lCache = new List<ExtProcess>();
            TimeSpan orgTime = cacheTime;
            //Set Cache TTL to 10seconds.
            cacheTime = new TimeSpan(0, 0, 10);
            List<PSObject> oObj = GetObjectsFromPS("Get-WMIObject win32_Process | Foreach {  $owner = $_.GetOwner();  $_ | Add-Member -MemberType \"Noteproperty\" -name \"Owner\" -value $(\"{0}\\{1}\" -f $owner.Domain, $owner.User) -passthru }", Reload);
            foreach (PSObject PSObj in oObj)
            {
                //Get AppDTs sub Objects
                ExtProcess oCIEx = new ExtProcess(PSObj, remoteRunspace, pSCode);

                oCIEx.remoteRunspace = remoteRunspace;
                oCIEx.pSCode = pSCode;
                lCache.Add(oCIEx);
            }
            cacheTime = orgTime;
            return lCache;
        }

        internal List<ExtProcess> extProcess
        {
            get
            {
                return LoadExtProcess(false);
            }

            set { }

        }

        public List<ExtProcess> ExtProcesses(bool Reload)
        {
            return LoadExtProcess(Reload);
        }

        /// <summary>
        /// Get a single Process
        /// </summary>
        /// <param name="ProcessID">ProcessID of the process</param>
        /// <returns></returns>
        public ExtProcess GetExtProcess(string ProcessID)
        {
            TimeSpan orgTime = cacheTime;
            List<PSObject> oObj = GetObjectsFromPS("Get-WMIObject win32_Process -filter \"ProcessId = " + ProcessID +"\" | Foreach {  $owner = $_.GetOwner();  $_ | Add-Member -MemberType \"Noteproperty\" -name \"Owner\" -value $(\"{0}\\{1}\" -f $owner.Domain, $owner.User) -passthru }", true);
            foreach (PSObject PSObj in oObj)
            {
                //Get AppDTs sub Objects
                ExtProcess oCIEx = new ExtProcess(PSObj, remoteRunspace, pSCode);

                oCIEx.remoteRunspace = remoteRunspace;
                oCIEx.pSCode = pSCode;
                return oCIEx;
            }

            return null;
        }

        /// <summary>
        /// Create a new Process
        /// </summary>
        /// <param name="Command">Command to start</param>
        /// <returns>ProcessId of the started process</returns>
        public UInt32? CreateProcess(string Command)
        {
            try
            {
                // (start-process "notepad.exe" -PassThru).Id
                string sRes = GetStringFromPS("(start-process " + Command + " -PassThru).Id");
                if (!string.IsNullOrEmpty(sRes))
                {
                    return UInt32.Parse(sRes);
                }
            }
            catch { }

            return null;
        }
    }

    /// <summary>
    /// Extended Win32_Process with Owner attribute
    /// </summary>
    public class ExtProcess : Win32_Process
    {
        //Constructor
        public ExtProcess(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            : base(WMIObject, RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;

            //oNewBase = new baseInit(remoteRunspace, pSCode);

            this.Owner = WMIObject.Properties["Owner"].Value as string;
        }

        //internal baseInit oNewBase;

        #region Properties

        public string Owner { get; set; }

        #endregion

    }

    /// <summary>
    /// Source:ROOT\cimv2
    /// </summary>
    public class Win32_Process : CIM_Process
    {
        //Constructor
        public Win32_Process(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            : base(WMIObject, RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            oNewBase = new baseInit(remoteRunspace, pSCode);

            this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
            this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
            this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
            this.__INSTANCE = true;
            this.WMIObject = WMIObject;
            this.CommandLine = WMIObject.Properties["CommandLine"].Value as String;
            this.ExecutablePath = WMIObject.Properties["ExecutablePath"].Value as String;
            this.HandleCount = WMIObject.Properties["HandleCount"].Value as UInt32?;
            this.MaximumWorkingSetSize = WMIObject.Properties["MaximumWorkingSetSize"].Value as UInt32?;
            this.MinimumWorkingSetSize = WMIObject.Properties["MinimumWorkingSetSize"].Value as UInt32?;
            this.OtherOperationCount = WMIObject.Properties["OtherOperationCount"].Value as UInt64?;
            this.OtherTransferCount = WMIObject.Properties["OtherTransferCount"].Value as UInt64?;
            this.PageFaults = WMIObject.Properties["PageFaults"].Value as UInt32?;
            this.PageFileUsage = WMIObject.Properties["PageFileUsage"].Value as UInt32?;
            this.ParentProcessId = WMIObject.Properties["ParentProcessId"].Value as UInt32?;
            this.PeakPageFileUsage = WMIObject.Properties["PeakPageFileUsage"].Value as UInt32?;
            this.PeakVirtualSize = WMIObject.Properties["PeakVirtualSize"].Value as UInt64?;
            this.PeakWorkingSetSize = WMIObject.Properties["PeakWorkingSetSize"].Value as UInt32?;
            this.PrivatePageCount = WMIObject.Properties["PrivatePageCount"].Value as UInt64?;
            this.ProcessId = WMIObject.Properties["ProcessId"].Value as UInt32?;
            this.QuotaNonPagedPoolUsage = WMIObject.Properties["QuotaNonPagedPoolUsage"].Value as UInt32?;
            this.QuotaPagedPoolUsage = WMIObject.Properties["QuotaPagedPoolUsage"].Value as UInt32?;
            this.QuotaPeakNonPagedPoolUsage = WMIObject.Properties["QuotaPeakNonPagedPoolUsage"].Value as UInt32?;
            this.QuotaPeakPagedPoolUsage = WMIObject.Properties["QuotaPeakPagedPoolUsage"].Value as UInt32?;
            this.ReadOperationCount = WMIObject.Properties["ReadOperationCount"].Value as UInt64?;
            this.ReadTransferCount = WMIObject.Properties["ReadTransferCount"].Value as UInt64?;
            this.SessionId = WMIObject.Properties["SessionId"].Value as UInt32?;
            this.ThreadCount = WMIObject.Properties["ThreadCount"].Value as UInt32?;
            this.VirtualSize = WMIObject.Properties["VirtualSize"].Value as UInt64?;
            this.WindowsVersion = WMIObject.Properties["WindowsVersion"].Value as String;
            this.WriteOperationCount = WMIObject.Properties["WriteOperationCount"].Value as UInt64?;
            this.WriteTransferCount = WMIObject.Properties["WriteTransferCount"].Value as UInt64?;
        }

        internal baseInit oNewBase;

        #region Properties

        //internal string __CLASS { get; set; }
        //internal string __NAMESPACE { get; set; }
        //internal bool __INSTANCE { get; set; }
        //internal string __RELPATH { get; set; }
        //internal PSObject WMIObject { get; set; }
        //internal Runspace remoteRunspace;
        //internal TraceSource pSCode;
        public String CommandLine { get; set; }
        public String ExecutablePath { get; set; }
        public UInt32? HandleCount { get; set; }
        public UInt32? MaximumWorkingSetSize { get; set; }
        public UInt32? MinimumWorkingSetSize { get; set; }
        public UInt64? OtherOperationCount { get; set; }
        public UInt64? OtherTransferCount { get; set; }
        public UInt32? PageFaults { get; set; }
        public UInt32? PageFileUsage { get; set; }
        public UInt32? ParentProcessId { get; set; }
        public UInt32? PeakPageFileUsage { get; set; }
        public UInt64? PeakVirtualSize { get; set; }
        public UInt32? PeakWorkingSetSize { get; set; }
        public UInt64? PrivatePageCount { get; set; }
        public UInt32? ProcessId { get; set; }
        public UInt32? QuotaNonPagedPoolUsage { get; set; }
        public UInt32? QuotaPagedPoolUsage { get; set; }
        public UInt32? QuotaPeakNonPagedPoolUsage { get; set; }
        public UInt32? QuotaPeakPagedPoolUsage { get; set; }
        public UInt64? ReadOperationCount { get; set; }
        public UInt64? ReadTransferCount { get; set; }
        public UInt32? SessionId { get; set; }
        public UInt32? ThreadCount { get; set; }
        public UInt64? VirtualSize { get; set; }
        public String WindowsVersion { get; set; }
        public UInt64? WriteOperationCount { get; set; }
        public UInt64? WriteTransferCount { get; set; }
        #endregion

        #region Methods

        public UInt32 Terminate()
        {
            //Remove cached results
            string sHash1 = oNewBase.CreateHash(@"Get-Process | Where { $_.Id -Eq " + ProcessId + @" } | Kill -Force");
            oNewBase.Cache.Remove(sHash1);

            oNewBase.GetStringFromPS(@"Get-Process | Where { $_.Id -Eq '" + ProcessId + @"' } | Kill -Force");

            //Remove cached results
            oNewBase.Cache.Remove(sHash1);

            return 0;
        }

        /*
        public UInt32 Create(String CommandLine, String CurrentDirectory, Object ProcessStartupInformation, out UInt32 ProcessId)
        {
            return 0;
        }

        public UInt32 GetOwner(out String Domain, out String User)
        {
            return 0;
        }
        public UInt32 GetOwnerSid(out String Sid)
        {
            return 0;
        }
        public UInt32 SetPriority(SInt32 Priority)
        {
            return 0;
        }
        public UInt32 AttachDebugger()
        {
            return 0;
        } */



        #endregion
    }

    /// <summary>
    /// Source:ROOT\cimv2
    /// </summary>
    public class CIM_Process : CIM_LogicalElement
    {
        //Constructor
        public CIM_Process(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            : base(WMIObject, RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;

            this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
            this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
            this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
            this.__INSTANCE = true;
            this.WMIObject = WMIObject;
            this.CreationClassName = WMIObject.Properties["CreationClassName"].Value as String;
            string sCreationDate = WMIObject.Properties["CreationDate"].Value as string;
            if (string.IsNullOrEmpty(sCreationDate))
                this.CreationDate = null;
            else
                this.CreationDate = ManagementDateTimeConverter.ToDateTime(sCreationDate) as DateTime?;
            this.CSCreationClassName = WMIObject.Properties["CSCreationClassName"].Value as String;
            this.CSName = WMIObject.Properties["CSName"].Value as String;
            this.ExecutionState = WMIObject.Properties["ExecutionState"].Value as UInt16?;
            this.Handle = WMIObject.Properties["Handle"].Value as String;
            this.KernelModeTime = WMIObject.Properties["KernelModeTime"].Value as UInt64?;
            this.OSCreationClassName = WMIObject.Properties["OSCreationClassName"].Value as String;
            this.OSName = WMIObject.Properties["OSName"].Value as String;
            this.Priority = WMIObject.Properties["Priority"].Value as UInt32?;
            string sTerminationDate = WMIObject.Properties["TerminationDate"].Value as string;
            if (string.IsNullOrEmpty(sTerminationDate))
                this.TerminationDate = null;
            else
                this.TerminationDate = ManagementDateTimeConverter.ToDateTime(sTerminationDate) as DateTime?;
            this.UserModeTime = WMIObject.Properties["UserModeTime"].Value as UInt64?;
            this.WorkingSetSize = WMIObject.Properties["WorkingSetSize"].Value as UInt64?;
        }

        #region Properties

        //internal string __CLASS { get; set; }
        //internal string __NAMESPACE { get; set; }
        //internal bool __INSTANCE { get; set; }
        //internal string __RELPATH { get; set; }
        //internal PSObject WMIObject { get; set; }
        //internal Runspace remoteRunspace;
        //internal TraceSource pSCode;
        public String CreationClassName { get; set; }
        public DateTime? CreationDate { get; set; }
        public String CSCreationClassName { get; set; }
        public String CSName { get; set; }
        public UInt16? ExecutionState { get; set; }
        public String Handle { get; set; }
        public UInt64? KernelModeTime { get; set; }
        public String OSCreationClassName { get; set; }
        public String OSName { get; set; }
        public UInt32? Priority { get; set; }
        public DateTime? TerminationDate { get; set; }
        public UInt64? UserModeTime { get; set; }
        public UInt64? WorkingSetSize { get; set; }
        #endregion

    }
}
