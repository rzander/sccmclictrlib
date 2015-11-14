//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

#define CM2012
#define CM2007

using System;
using System.Collections.Generic;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;

namespace sccmclictr.automation.functions
{
    /// <summary>
    /// Class processes.
    /// </summary>
    public class processes : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="processes"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        /// <param name="oClient">A CCM CLient object.</param>
        public processes(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Gets or sets the win32_ processes.
        /// </summary>
        /// <value>The win32_ processes.</value>
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

        /// <summary>
        /// Extends the processes.
        /// </summary>
        /// <param name="Reload">if set to <c>true</c> [reload].</param>
        /// <returns>List{ExtProcess}.</returns>
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtProcess"/> class.
        /// </summary>
        /// <param name="WMIObject">The WMI object.</param>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
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

        /// <summary>
        /// Gets or sets the process owner.
        /// </summary>
        /// <value>The process owner.</value>
        public string Owner { get; set; }

        #endregion

    }

    /// <summary>
    /// Source:ROOT\cimv2
    /// </summary>
    public class Win32_Process : CIM_Process
    {
        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CIM_Process" /> class.
        /// </summary>
        /// <param name="WMIObject">The WMI object.</param>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
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

        /// <summary>
        /// Gets or sets the command line.
        /// </summary>
        /// <value>Command line used to start a specific process, if applicable.</value>
        public String CommandLine { get; set; }

        /// <summary>
        /// Gets or sets the executable path.
        /// </summary>
        /// <value>Path to the executable file of the process.</value>
        public String ExecutablePath { get; set; }

        /// <summary>
        /// Gets or sets the handle count.
        /// </summary>
        /// <value>Total number of open handles owned by the process. HandleCount is the sum of the handles currently open by each thread in this process.</value>
        public UInt32? HandleCount { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the working set.
        /// </summary>
        /// <value>Maximum working set size of the process. The working set of a process is the set of memory pages visible to the process in physical RAM. These pages are resident, and available for an application to use without triggering a page fault.</value>
        public UInt32? MaximumWorkingSetSize { get; set; }

        /// <summary>
        /// Gets or sets the minimum size of the working set.
        /// </summary>
        /// <value>Minimum working set size of the process. The working set of a process is the set of memory pages visible to the process in physical RAM. These pages are resident and available for an application to use without triggering a page fault.</value>
        public UInt32? MinimumWorkingSetSize { get; set; }

        /// <summary>
        /// Gets or sets the other operation count.
        /// </summary>
        /// <value>Number of I/O operations performed that are not read or write operations.</value>
        public UInt64? OtherOperationCount { get; set; }

        /// <summary>
        /// Gets or sets the other transfer count.
        /// </summary>
        /// <value>Amount of data transferred during operations that are not read or write operations.</value>
        public UInt64? OtherTransferCount { get; set; }

        /// <summary>
        /// Gets or sets the page faults.
        /// </summary>
        /// <value>Number of page faults that a process generates.</value>
        public UInt32? PageFaults { get; set; }

        /// <summary>
        /// Gets or sets the page file usage.
        /// </summary>
        /// <value>Maximum amount of page file space used during the life of a process.</value>
        public UInt32? PageFileUsage { get; set; }

        /// <summary>
        /// Gets or sets the parent process identifier.
        /// </summary>
        /// <value>Unique identifier of the process that creates a process.</value>
        public UInt32? ParentProcessId { get; set; }

        /// <summary>
        /// Gets or sets the peak page file usage.
        /// </summary>
        /// <value>Maximum amount of page file space used during the life of a process.</value>
        public UInt32? PeakPageFileUsage { get; set; }

        /// <summary>
        /// Gets or sets the size of the peak virtual.
        /// </summary>
        /// <value>Maximum virtual address space a process uses at any one time.</value>
        public UInt64? PeakVirtualSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the peak working set.
        /// </summary>
        /// <value>Peak working set size of a process in Kilobytes</value>
        public UInt32? PeakWorkingSetSize { get; set; }

        /// <summary>
        /// Gets or sets the private page count.
        /// </summary>
        /// <value>Current number of pages allocated that are only accessible to the process represented by this Win32_Process instance</value>
        public UInt64? PrivatePageCount { get; set; }

        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        /// <value>Global process identifier that is used to identify a process. The value is valid from the time a process is created until it is terminated.</value>
        public UInt32? ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the quota non paged pool usage.
        /// </summary>
        /// <value>Quota amount of nonpaged pool usage for a process.</value>
        public UInt32? QuotaNonPagedPoolUsage { get; set; }

        /// <summary>
        /// Gets or sets the quota paged pool usage.
        /// </summary>
        /// <value>Quota amount of paged pool usage for a process.</value>
        public UInt32? QuotaPagedPoolUsage { get; set; }

        /// <summary>
        /// Gets or sets the quota peak non paged pool usage.
        /// </summary>
        /// <value>Peak quota amount of nonpaged pool usage for a process.</value>
        public UInt32? QuotaPeakNonPagedPoolUsage { get; set; }

        /// <summary>
        /// Gets or sets the quota peak paged pool usage.
        /// </summary>
        /// <value>Peak quota amount of paged pool usage for a process.</value>
        public UInt32? QuotaPeakPagedPoolUsage { get; set; }

        /// <summary>
        /// Gets or sets the read operation count.
        /// </summary>
        /// <value>Number of read operations performed.</value>
        public UInt64? ReadOperationCount { get; set; }

        /// <summary>
        /// Gets or sets the read transfer count.
        /// </summary>
        /// <value>Amount of data read.</value>
        public UInt64? ReadTransferCount { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>Unique identifier that an operating system generates when a session is created. A session spans a period of time from logon until logoff from a specific system.</value>
        public UInt32? SessionId { get; set; }

        /// <summary>
        /// Gets or sets the thread count.
        /// </summary>
        /// <value>Number of active threads in a process. An instruction is the basic unit of execution in a processor, and a thread is the object that executes an instruction. Each running process has at least one thread. </value>
        public UInt32? ThreadCount { get; set; }

        /// <summary>
        /// Gets or sets the size of the virtual.
        /// </summary>
        /// <value>Current size of the virtual address space that a process is using, not the physical or virtual memory actually used by the process.</value>
        public UInt64? VirtualSize { get; set; }

        /// <summary>
        /// Gets or sets the windows version.
        /// </summary>
        /// <value>Version of Windows in which the process is running.</value>
        public String WindowsVersion { get; set; }

        /// <summary>
        /// Gets or sets the write operation count.
        /// </summary>
        /// <value>Number of write operations performed.</value>
        public UInt64? WriteOperationCount { get; set; }

        /// <summary>
        /// Gets or sets the write transfer count.
        /// </summary>
        /// <value>Amount of data written.</value>
        public UInt64? WriteTransferCount { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Terminates a process and all of its threads.
        /// </summary>
        /// <returns>UInt32.</returns>
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
        /// <summary>
        /// Initializes a new instance of the <see cref="CIM_Process"/> class.
        /// </summary>
        /// <param name="WMIObject">The WMI object.</param>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
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

        /// <summary>
        /// Gets or sets the name of the creation class.
        /// </summary>
        /// <value>Name of the first concrete class in the inheritance chain that is used to create an instance.</value>
        public String CreationClassName { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>Date the process begins executing.</value>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the cs creation class.
        /// </summary>
        /// <value>Creation class name of the scoping computer system.</value>
        public String CSCreationClassName { get; set; }

        /// <summary>
        /// Gets or sets the name of the cs.
        /// </summary>
        /// <value>Name of the scoping computer system.</value>
        public String CSName { get; set; }

        /// <summary>
        /// Gets or sets the state of the execution.
        /// </summary>
        /// <value>This property is not implemented and does not get populated for any instance of this class. This property is always NULL.</value>
        public UInt16? ExecutionState { get; set; }

        /// <summary>
        /// Gets or sets the handle.
        /// </summary>
        /// <value>Process identifier.</value>
        public String Handle { get; set; }

        /// <summary>
        /// Gets or sets the kernel mode time.
        /// </summary>
        /// <value>Time in kernel mode, in 100 nanosecond units. If this information is not available, use a value of 0 (zero).</value>
        public UInt64? KernelModeTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the os creation class.
        /// </summary>
        /// <value>Creation class name of the scoping operating system.</value>
        public String OSCreationClassName { get; set; }

        /// <summary>
        /// Gets or sets the name of the os.
        /// </summary>
        /// <value>Name of the scoping operating system.</value>
        public String OSName { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>Scheduling priority of a process within an operating system. The higher the value, the higher priority a process receives. Priority values can range from 0 (zero), which is the lowest priority to 31, which is highest priority.</value>
        public UInt32? Priority { get; set; }

        /// <summary>
        /// Gets or sets the termination date.
        /// </summary>
        /// <value>Process was stopped or terminated. To get the termination time, a handle to the process must be held open. Otherwise, this property returns NULL.</value>
        public DateTime? TerminationDate { get; set; }

        /// <summary>
        /// Gets or sets the user mode time.
        /// </summary>
        /// <value>Time in user mode, in 100 nanosecond units. If this information is not available, use a value of 0 (zero).</value>
        public UInt64? UserModeTime { get; set; }

        /// <summary>
        /// Gets or sets the size of the working set.
        /// </summary>
        /// <value>Amount of memory in bytes that a process needs to execute efficiently—for an operating system that uses page-based memory management.</value>
        public UInt64? WorkingSetSize { get; set; }

        #endregion

    }
}
