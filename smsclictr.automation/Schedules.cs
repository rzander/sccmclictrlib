//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace smsclictr.automation
{
    /// <summary>
    /// Class to manage SMS Schedules
    /// </summary>
    public class SMSSchedules
    {
        #region Internal
        private WMIProvider oWMIProvider;
        private ManagementClass SMSClass;
        private ManagementBaseObject inParams;
        private string sDCMScanScheduleID;
        //private string sRequestEvaluateMachinePolicyID;
        #endregion

        #region Constructor

        /// <summary>
        /// SMSSchedules Constructor
        /// </summary>
        /// <param name="oProvider"></param>
        public SMSSchedules(WMIProvider oProvider)
        {
            oWMIProvider = new WMIProvider(oProvider.mScope.Clone());
        }

        #endregion

        #region Functions

        /// <summary>
        /// Delete Inventory History
        /// </summary>
        /// <param name="ScheduleID"></param>
        private void SMSDelInvHist(string ScheduleID)
        {
            try
            {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\CCM\invagt";
                    ManagementObject MO = oProv.GetObject(@"InventoryActionStatus.InventoryActionID='" + ScheduleID + "'");
                    MO.Delete();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Request UserPolicy if a User is logged on
        /// </summary>
        /// <returns>false = no user is logged on or a fialure occured; true = success</returns>
        public bool UserPolicyReq()
        {
            try
            {
                SMSClient oClient = new SMSClient(new WMIProvider(oWMIProvider.mScope.Clone()));
                return UserPolicyReq(oClient.ComputerSystem.LoggedOnUser);
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Request UserPolicy for a specified Account
        /// </summary>
        /// <param name="Username">Username (Domain\Username)</param>
        /// <returns>false = no user is logged on or a fialure occured; true = success</returns>
        public bool UserPolicyReq(string Username)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                System.Security.Principal.NTAccount ntAc = new System.Security.Principal.NTAccount(Username);
                System.Security.Principal.SecurityIdentifier sid = (System.Security.Principal.SecurityIdentifier)ntAc.Translate(typeof(System.Security.Principal.SecurityIdentifier));
                string sPath = "root\\ccm\\Policy\\" + sid.ToString().Replace('-','_') + "\\ActualConfig";
                oProv.mScope.Path.NamespacePath = sPath;
                ManagementObject MO = new ManagementObject(oProv.mScope, new ManagementPath("CCM_Scheduler_ScheduledMessage.ScheduledMessageID='{00000000-0000-0000-0000-000000000026}'"), new ObjectGetOptions());
                MO.Properties["Triggers"].Value = new string[] { "SimpleInterval;Minutes=1;MaxRandomDelayMinutes=0" };
                MO.Put();
            }
            catch(Exception ex)
            {
                ex.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Request UserPolicy if a User is logged on
        /// </summary>
        /// <returns>false = no user is logged on or a fialure occured; true = success</returns>
        public bool UserPolicyEval()
        {
            try
            {
                SMSClient oClient = new SMSClient(new WMIProvider(oWMIProvider.mScope.Clone()));
                return UserPolicyEval(oClient.ComputerSystem.LoggedOnUser);
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Request UserPolicy for a specified Account
        /// </summary>
        /// <param name="Username">Username (Domain\Username)</param>
        /// <returns>false = no user is logged on or a fialure occured; true = success</returns>
        public bool UserPolicyEval(string Username)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                System.Security.Principal.NTAccount ntAc = new System.Security.Principal.NTAccount(Username);
                System.Security.Principal.SecurityIdentifier sid = (System.Security.Principal.SecurityIdentifier)ntAc.Translate(typeof(System.Security.Principal.SecurityIdentifier));
                string sPath = "root\\ccm\\Policy\\" + sid.ToString().Replace('-', '_') + "\\ActualConfig";
                oProv.mScope.Path.NamespacePath = sPath;
                ManagementObject MO = new ManagementObject(oProv.mScope, new ManagementPath("CCM_Scheduler_ScheduledMessage.ScheduledMessageID='{00000000-0000-0000-0000-000000000027}'"), new ObjectGetOptions());
                MO.Properties["Triggers"].Value = new string[] { "SimpleInterval;Minutes=1" };
                MO.Put();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Initiate a HardwareInventory cycle
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.HardwareInventory();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.HardwareInventory()
        /// </code>
        /// </example>
        public void HardwareInventory()
        {
            HardwareInventory(false);
        }

        /// <summary>
        /// Initiate a HardwareInventory cycle
        /// </summary>
        /// <param name="FullInventory">Force a Full-Inventory if TRUE</param>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.HardwareInventory(True);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.HardwareInventory("TRUE")
        /// </code>
        /// </example>
        public void HardwareInventory(bool FullInventory)
        {
            if (FullInventory)
                SMSDelInvHist("{00000000-0000-0000-0000-000000000001}");
            RunScheduleID("{00000000-0000-0000-0000-000000000001}", true);
        }

        /// <summary>
        /// Initiate a SoftwareInventory cycle
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.SoftwareInventory();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.SoftwareInventory()
        /// </code>
        /// </example>
        public void SoftwareInventory()
        {
            SoftwareInventory(false);
        }

        /// <summary>
        /// Initiate a SoftwareInventory cycle
        /// </summary>
        /// <param name="FullInventory">Force a Full-Inventory if TRUE</param>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.SoftwareInventory(True);
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.SoftwareInventory("TRUE")
        /// </code>
        /// </example>
        public void SoftwareInventory(bool FullInventory)
        {
            if (FullInventory)
                SMSDelInvHist("{00000000-0000-0000-0000-000000000002}");
            RunScheduleID("{00000000-0000-0000-0000-000000000002}", true);
        }

        /// <summary>
        /// Initiate a DataDiscovery cycle
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.DataDiscovery();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.DataDiscovery()
        /// </code>
        /// </example>
        public void DataDiscovery()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000003}", true);
        }

        /// <summary>
        /// Software Inventory, Collect Files
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.SinvCollFile();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.SinvCollFile()
        /// </code>
        /// </example>
        public void SinvCollFile()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000010}");
        }

        /// <summary>
        /// Hardware Inventory, Collect IDMIF Files (if this option is enabled)
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.IDMIFColl();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.IDMIFColl()
        /// </code>
        /// </example>
        public void IDMIFColl()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000011}");
        }

        /// <summary>
        /// Triggers a "Machine Policy Retrieval + Evaluation Cycle"
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.RequestMachineAssignments();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.RequestMachineAssignments()
        /// </code>
        /// </example>
        public void RequestMachineAssignments()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000021}", true);
        }

        /// <summary>
        /// Triggers "Machine Policy Evaluator"
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.MachinePolicyEvaluator();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.MachinePolicyEvaluator()
        /// </code>
        /// </example>
        public void MachinePolicyEvaluator()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000022}", true);
        }

        /// <summary>
        /// Refresh Default MP
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.LSRefreshDefaultMP();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.LSRefreshDefaultMP()
        /// </code>
        /// </example>
        public void LSRefreshDefaultMP()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000023}");
        }

        /// <summary>
        /// Refresh Locations
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.LSRefreshLocations();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.LSRefreshLocations()
        /// </code>
        /// </example>
        public void LSRefreshLocations()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000024}");
        }

        /// <summary>
        /// TimeOut Requests
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.LSTimeOutRequests();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.LSTimeOutRequests()
        /// </code>
        /// </example>
        public void LSTimeOutRequests()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000025}");
        }

        /// <summary>
        /// Generate a Software Meetering Report
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.SWMTRReportGen();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.SWMTRReportGen()
        /// </code>
        /// </example>
        public void SWMTRReportGen()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000031}");
        }

        /// <summary>
        /// Initiate a WindowsInstaller Source Update cycle
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.MSISourceUpdate();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.MSISourceUpdate()
        /// </code>
        /// </example>
        public void MSISourceUpdate()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000032}");
        }

        /// <summary>
        /// Refresh Proxy MP
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.ProxyMaintenanceEndpoint();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.ProxyMaintenanceEndpoint()
        /// </code>
        /// </example>
        public void ProxyMaintenanceEndpoint()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000037}");
        }

        /// <summary>
        /// Policy Cleanup
        /// </summary>
        public void PolicyCleanup()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000040}");
        }

        /// <summary>
        /// Validate assignments
        /// </summary>
        public void ValidateAssignments()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000042}");
        }

        /// <summary>
        /// Certificate Maintenance Task
        /// </summary>
        public void CertMaintenance()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000051}");
        }

        /// <summary>
        /// Branch DP scheduled maintenance
        /// </summary>
        public void PDPStatusTask()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000061}");
        }

        /// <summary>
        /// Branch DP provisioning status reporting
        /// </summary>
        public void PDPPkgProvisioningStatus()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000062}");
        }

        /// <summary>
        /// NAP Compliance Interval Enforcement
        /// </summary>
        public void NAPComplianceEnforcement()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000071}");
        }

        /// <summary>
        /// Initiate a Software Update Evaluation cycle
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.UpdateAssignmentEval();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.UpdateAssignmentEval()
        /// </code>
        /// </example>
        public void UpdateAssignmentEval()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000108}");
        }

        /// <summary>
        /// Initiate a Peer DP Maintenance Task
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.PeerDPMaintenanceTask();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.PeerDPMaintenanceTask()
        /// </code>
        /// </example>
        public void PeerDPMaintenanceTask()
        {
            //Seems that this Schedule id is not present over WMI
            RunScheduleID("{00000000-0000-0000-0000-000000000109}");
        }

        /// <summary>
        /// State message upload
        /// </summary>
        public void SendUnsent()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000111}");
        }

        /// <summary>
        /// State message cache cleanup
        /// </summary>
        public void MessageCacheCleanout()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000112}");
        }

        /// <summary>
        /// Initiate a Updates Source Scan Cycle
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.UpdatesSourceScan();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.UpdatesSourceScan()
        /// </code>
        /// </example>
        public void UpdatesSourceScan()
        {
            UpdatesSourceScan(false);
        }

        /// <summary>
        /// Initiate a Updates Source Scan Cycle
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                oClient.Schedules.UpdatesSourceScan();
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.UpdatesSourceScan()
        /// </code>
        /// </example>
        public void UpdatesSourceScan(bool enforce)
        {
            //Delete LastScanToolHistory
            if (enforce)
            {
                oWMIProvider.DeleteQueryResults(@"root\ccm\scanagent","SELECT * FROM CCM_ScanToolHistory");
            }

            RunScheduleID("{00000000-0000-0000-0000-000000000113}");
        }

        /// <summary>
        /// Software Update deployment re-eval
        /// </summary>
        public void UpdatesSourceStore()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000114}");
        }

        /// <summary>
        /// OutOfBand Discovery
        /// </summary>
        public void OOBDiscovery()
        {
            RunScheduleID("{00000000-0000-0000-0000-000000000120}");
        }

        /// <summary>
        /// DesiredConfigurationMonitoring Scan
        /// </summary>
        public void DCMScan()
        {
            if (string.IsNullOrEmpty(sDCMScanScheduleID))
            {
                ManagementScope mScope = this.oWMIProvider.mScope.Clone();
                mScope.Path.NamespacePath = @"root\ccm\policy\machine\actualconfig";
                ManagementObjectCollection MOC = new ManagementObjectSearcher(mScope, new ObjectQuery("SELECT * FROM CCM_Scheduler_ScheduledMessage WHERE TargetEndpoint = 'direct:DCMAgent'")).Get();
                foreach (ManagementObject MO in MOC)
                {
                    try
                    {
                        string sID = MO["ScheduledMessageID"].ToString();
                        if (!string.IsNullOrEmpty(sID))
                        {
                            sDCMScanScheduleID = sID;
                            if (string.IsNullOrEmpty(sDCMScanScheduleID))
                            {
                                throw new System.Exception("Not found!");
                            }
                            else
                            {
                                RunScheduleID(sDCMScanScheduleID);
                            }
                        }
                    }
                    catch { }
                }

            }

            
        }

        /// <summary>
        /// Run a ScheduleID
        /// </summary>
        /// <param name="ScheduleID">Schedule Identifier</param>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                 //Run a HardwareInventory Schedule
        ///                oClient.Schedules.RunScheduleID("{00000000-0000-0000-0000-000000000001}");
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.RunScheduleID("{00000000-0000-0000-0000-000000000001}")
        /// </code>
        /// </example>
        public void RunScheduleID(string ScheduleID)
        {
            RunScheduleID(ScheduleID, false);
        }

        /// <summary>
        /// Run a ScheduleID and update the Schedule-History
        /// </summary>
        /// <param name="ScheduleID"></param>
        /// <param name="UpdateHistory"></param>
        public void RunScheduleID(string ScheduleID, Boolean UpdateHistory)
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\CCM";

            SMSClass = oProv.GetClass("SMS_Client");
            inParams = SMSClass.GetMethodParameters("TriggerSchedule");

            try
            {
                inParams["sScheduleID"] = ScheduleID;
                oProv.ExecuteMethod("SMS_Client", "TriggerSchedule", inParams);
            }
            catch (Exception ex)
            {
               ex.Message.ToString();
            }

            if (UpdateHistory)
            {
                
                oProv.mScope.Path.NamespacePath = @"ROOT\ccm\Scheduler";
                ManagementObject MO = null;
                try
                {
                    MO = oProv.GetObject("CCM_Scheduler_History.ScheduleID='" + ScheduleID + "',UserSID='Machine'");
                }
                catch { }
                if (MO == null)
                {
                    MO = oProv.GetClass("CCM_Scheduler_History").CreateInstance();
                    MO.SetPropertyValue("ScheduleID", ScheduleID);
                    MO.SetPropertyValue("UserSID", "Machine");
                    MO.SetPropertyValue("FirstEvalTime", System.Management.ManagementDateTimeConverter.ToDmtfDateTime(DateTime.Now.ToUniversalTime()));
                }
                MO.SetPropertyValue("LastTriggerTime", System.Management.ManagementDateTimeConverter.ToDmtfDateTime(DateTime.Now.ToUniversalTime()));
                
                MO.Put();
            }
        }

        /// <summary>
        /// Get the last HardwareInventory TimeStamp
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("Last HW Inventory: " + oClient.Schedules.LastHWInv;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.LastHWInv
        /// </code>
        /// </example>
        public DateTime LastHWInv
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"Root\CCM\Scheduler";
                ManagementObject MO = oProv.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000001}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(MO.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        /// <summary>
        /// Get the last Software Inventory TimeStamp
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("Last SW Inventory: " + oClient.Schedules.LastSWInv;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.LastSWInv
        /// </code>
        /// </example>
        public DateTime LastSWInv
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"Root\CCM\Scheduler";
                ManagementObject MO = oProv.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000002}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(MO.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        /// <summary>
        /// Get the last DataDiscovery TimeStamp
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("Last SW Inventory: " + oClient.Schedules.LastDDR;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.LastDDR
        /// </code>
        /// </example>
        public DateTime LastDDR
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"Root\CCM\Scheduler";
                ManagementObject MO = oProv.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000003}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(MO.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        /// <summary>
        /// Get the last Machine Policy Download TimeStamp
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("Last Policy MachineAssignments " + oClient.Schedules.LastMachineAssignments;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.LastMachineAssignments
        /// </code>
        /// </example>
        public DateTime LastMachineAssignments
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"Root\CCM\Scheduler";
                ManagementObject MO = oProv.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000021}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(MO.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        /// <summary>
        /// Get the last Machine Policy Evaluation TimeStamp
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("Last Policy MachineAssignments " + oClient.Schedules.LastMachinePolicyEvaluator;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.LastMachinePolicyEvaluator
        /// </code>
        /// </example>
        public DateTime LastMachinePolicyEvaluator
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"Root\CCM\Scheduler";
                ManagementObject MO = oProv.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000022}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(MO.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        /// <summary>
        /// Get the last UpdateAssignment Evaluation TimeStamp
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("Last HW Inventory: " + oClient.Schedules.LastUpdateAssignmentEval;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.LastUpdateAssignmentEval
        /// </code>
        /// </example>
        public DateTime LastUpdateAssignmentEval
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"Root\CCM\Scheduler";
                ManagementObject MO = oProv.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000108}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(MO.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        /// <summary>
        /// Get the last UpdatesSourceScan  TimeStamp
        /// </summary>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                SMSClient oClient = new SMSClient("workstation01");
        ///                Console.WriteLine("Last HW Inventory: " + oClient.Schedules.LastUpdatesSourceScan;
        ///             }
        ///       }
        /// }
        /// </code>
        /// PowerShell:
        /// <para>(copy smsclictr.automation.dll to the %HOMEDRIVE%\%HOMEPATH% Folder of the current User)</para>
        /// <code>
        /// [void][System.Reflection.Assembly]::LoadFile("$HOME\smsclictr.automation.dll")
        /// $SMSClient = New-Object -TypeName smsclictr.automation.SMSClient("workstation01")
        /// $SMSClient.Schedules.LastUpdatesSourceScan
        /// </code>
        /// </example>
        public DateTime LastUpdatesSourceScan
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"Root\CCM\Scheduler";
                ManagementObject MO = oProv.GetObject("CCM_Scheduler_History.ScheduleID='{00000000-0000-0000-0000-000000000113}',UserSID='Machine'");
                return ManagementDateTimeConverter.ToDateTime(MO.GetPropertyValue("LastTriggerTime").ToString());
            }
        }

        /// <summary>
        /// Gat LastRun DateTime from a Package/Program
        /// </summary>
        /// <param name="PackageID"></param>
        /// <param name="ProgramID"></param>
        /// <returns>LastRun DateTime</returns>
        public DateTime LastRun(string PackageID, string ProgramID)
        {
            WMIRegistry oReg = new WMIRegistry(oWMIProvider);
            string sRegKey = ExecutionHistoryRegKey(PackageID, ProgramID);
            if (!string.IsNullOrEmpty(sRegKey))
            {
                return DateTime.Parse(oReg.GetString(2147483650, sRegKey, "_RunStartTime"));
            }
            return new DateTime();
        }

        /// <summary>
        /// Check if a Package executed successfully
        /// </summary>
        /// <param name="PackageID"></param>
        /// <param name="ProgramID"></param>
        /// <returns>true=successful, false=failed</returns>
        public Boolean PkgSuccessful(string PackageID, string ProgramID)
        {
            WMIRegistry oReg = new WMIRegistry(oWMIProvider);
            string sRegKey = ExecutionHistoryRegKey(PackageID, ProgramID);
            if (!string.IsNullOrEmpty(sRegKey))
            {
                string sState = oReg.GetString(2147483650, sRegKey, "_State");
                switch (sState)
                {
                    case("Success"):
                        return true;
                    case("Failed"):
                        return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the ExecutionHistory Reg Key (HKLM)
        /// </summary>
        /// <param name="PackageID"></param>
        /// <param name="ProgramID"></param>
        /// <returns>Execution History Reg Path</returns>
        public string ExecutionHistoryRegKey(string PackageID, string ProgramID)
        {
            WMIRegistry oReg = new WMIRegistry(oWMIProvider);
            if (oWMIProvider.isX86)
            {
                System.Collections.ArrayList aList = oReg.RegKeys(2147483650, @"SOFTWARE\Microsoft\SMS\Mobile Client\Software Distribution\Execution History\System\" + PackageID);
                foreach (string s in aList)
                {
                    if (string.Compare(ProgramID, oReg.GetString(2147483650, @"SOFTWARE\Microsoft\SMS\Mobile Client\Software Distribution\Execution History\System\" + PackageID + @"\" + s, "_ProgramID"), true) == 0)
                    {
                        return @"SOFTWARE\Microsoft\SMS\Mobile Client\Software Distribution\Execution History\System\" + PackageID + @"\" + s;
                    }
                }
            }
            else
            {
                System.Collections.ArrayList aList = oReg.RegKeys(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\SMS\Mobile Client\Software Distribution\Execution History\System\" + PackageID);
                foreach (string s in aList)
                {
                    if (string.Compare(ProgramID, oReg.GetString(2147483650, @"SOFTWARE\Wow6432Node\Microsoft\SMS\Mobile Client\Software Distribution\Execution History\System\" + PackageID + @"\" + s, "_ProgramID"), true) == 0)
                    {
                        return @"SOFTWARE\Wow6432Node\Microsoft\SMS\Mobile Client\Software Distribution\Execution History\System\" + PackageID + @"\" + s;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get all instances of root\ccm\Policy\Machine\ActualConfig:CCM_Scheduler_ScheduledMessage
        /// </summary>
        public ManagementObjectCollection CCM_Scheduler_ScheduledMessage{
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\ActualConfig";
                return oProv.ExecuteQuery("SELECT * FROM CCM_Scheduler_ScheduledMessage");
            }
        }

        /// <summary>
        /// Get all Advertisement Schedules of root\ccm\Policy\Machine\ActualConfig:CCM_Scheduler_ScheduledMessage 
        /// </summary>
        /*public ManagementObjectCollection CCM_Scheduler_ScheduledMessage_ExecMgr
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\ActualConfig";
                return oProv.ExecuteQuery("SELECT * FROM CCM_Scheduler_ScheduledMessage WHERE TargetEndpoint = 'direct:execmgr'");
            }
        }*/

        /// <summary>
        /// Get all Advertisement Schedules of root\ccm\Policy\[SID]\ActualConfig:CCM_Scheduler_ScheduledMessage 
        /// </summary>
        /// <param name="SID">User SID or "Machine" for Computer Schedules</param>
        /// <returns>CCM_Scheduler_ScheduledMessage</returns>
        public ManagementObjectCollection CCM_Scheduler_ScheduledMessage_ExecMgr(string SID)
        {
            if(string.IsNullOrEmpty(SID))
                SID = "Machine";
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = string.Format(@"root\ccm\Policy\{0}\ActualConfig", SID);

            return oProv.ExecuteQuery("SELECT * FROM CCM_Scheduler_ScheduledMessage WHERE TargetEndpoint = 'direct:execmgr'");
        }

        /// <summary>
        /// Get all ServiceWindow Objects of ROOT\ccm\Policy\Machine\RequestedConfig:CCM_ServiceWindow
        /// </summary>
        public ManagementObjectCollection CCM_ServiceWindow
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\ccm\Policy\Machine\RequestedConfig";
                return oProv.ExecuteQuery("SELECT * FROM CCM_ServiceWindow");
            }

        }


        #endregion

        #region ScheduleID
        //Schedule ID decoding was possible because of the reverse engineering work from Jeff Huston 
        //<http://myitforum.com/cs2/blogs/jhuston/archive/2007/07/30/sms-schedule-token-strings.aspx>

        /// <summary>
        /// Chech if ScheduleID is a NonRecuring Schedule
        /// </summary>
        /// <param name="ScheduleID"></param>
        /// <returns></returns>
        internal static Boolean isNonRecurring(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Check if bit signature is 001 on Position 19
            if ((lSchedID >> 19 & 7) == 1)
                return true;
            return false;
        }

        internal static Boolean isRecurInterval(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Check if bit signature is 010 on Position 19
            if ((lSchedID >> 19 & 7) == 2)
                return true;
            return false;
        }

        internal static Boolean isRecurWeekly(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Check if bit signature is 011 on Position 19
            if ((lSchedID >> 19 & 7) == 3)
                return true;
            return false;
        }

        internal static Boolean isRecurMonthlyByWeekday(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Check if bit signature is 100 on Position 19
            if ((lSchedID >> 19 & 7) == 4)
                return true;
            return false;
        }

        internal static Boolean isRecurMonthlyByDate(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Check if bit signature is 101 on Position 19
            if ((lSchedID >> 19 & 7) == 5)
                return true;
            return false;
        }

        internal static Boolean isgmt(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 1Bit from position 1
            long lstart = (lSchedID  & 1);
            return System.Convert.ToBoolean(lstart);
        }

        internal static int dayspan(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 3
            long lstart = (lSchedID >> 3 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int hourpan(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 8
            long lstart = (lSchedID >> 8 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int minutespan(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 6Bit's from position 13
            long lstart = (lSchedID >> 13 & 63);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int weekorder(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 3Bit's from position 9
            long lstart = (lSchedID >> 9 & 7);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int fornumberofweeks(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 3Bit's from position 13
            long lstart = (lSchedID >> 13 & 7);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int fornumberofmonths(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 4Bit's from position 12
            long lstart = (lSchedID >> 12 & 15);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int fornumberofmonths2(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 4Bit's from position 10
            long lstart = (lSchedID >> 10 & 15);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int iDay(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 3Bit's from position 16
            long lstart = (lSchedID >> 16 & 7);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int monthday(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 14
            long lstart = (lSchedID >> 14 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int dayduration(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 22
            long lstart = (lSchedID >> 22 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int hourduration(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 27
            long lstart = (lSchedID >> 27 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int minuteduration(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 6Bit's from position 32
            long lstart = (lSchedID >> 32 & 63);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int startyear(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 6Bit's from position 38
            long lstart = (lSchedID >> 38 & 63);
            int iRes = System.Convert.ToInt16(lstart + 1970);
            return iRes;
        }

        internal static int startmonth(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 4Bit's from position 44
            long lstart = (lSchedID >> 44 & 15);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int startday(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 48
            long lstart = (lSchedID >> 48 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int starthour(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 5Bit's from position 53
            long lstart = (lSchedID >> 53 & 31);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static int startminute(string ScheduleID)
        {
            long lSchedID = long.Parse(ScheduleID, System.Globalization.NumberStyles.AllowHexSpecifier);
            //Get 6Bit's from position 58
            long lstart = (lSchedID >> 58 & 63);
            int iRes = System.Convert.ToInt16(lstart);
            return iRes;
        }

        internal static string encodeID(object Schedule)
        {
            long lSchedID = new long();
            int lo = 0;
            int hi = 0;

            SMS_ST_NonRecurring oSched = Schedule as SMS_ST_NonRecurring;

            #region BaseSchedule
            //IsGMT Flag
            if (oSched.IsGMT)
            {
                lo |= 1;
            }

            //StartTime Year (check if in a valid range)
            if ((oSched.StartTime.Year > 1970) & (oSched.StartTime.Year < 2033))
            {
                hi |= (oSched.StartTime.Year - 1970) << 6;
            }
            else
            {
                hi |= 63 << 6;
            }

            //StartTime Month
            hi |= oSched.StartTime.Month << 12;

            //StartTime Day
            hi |= oSched.StartTime.Day << 16;

            //StartTime Hour
            hi |= oSched.StartTime.Hour << 21;

            //StartTime Minute
            hi |= oSched.StartTime.Minute << 26;

            //Day duration
            lo |= oSched.DayDuration << 22;

            //hourduration duration
            lo |= oSched.HourDuration << 27;

            //Minute duration
            hi |= oSched.MinuteDuration;
            #endregion

            switch (Schedule.GetType().Name)
            {

                case ("SMS_ST_NonRecurring"):
                    //Set type to SMS_ST_NonRecurring
                    lo |= 1 << 19;
                    break;

                case ("SMS_ST_RecurInterval"):
                    SMS_ST_RecurInterval oSchedRI = oSched as SMS_ST_RecurInterval;

                    //DaySpan
                    lo |= oSchedRI.DaySpan << 3;

                    //HourSpan
                    lo |= oSchedRI.HourSpan << 8;

                    //MinuteSpan
                    lo |= oSchedRI.MinuteSpan << 13;

                    //Set type to SMS_ST_RecurInterval
                    lo |= 2 << 19;
                    break;
            }

            //Convert to HEX
            lSchedID = (((long)hi) << 32) | ((uint)lo);
            return lSchedID.ToString("X");
        }

        /// <summary>
        /// Decode an SMS ScheduleID string
        /// </summary>
        /// <param name="ScheduleID">SMS encoded 64bit ScheduleID string</param>
        /// <returns>object of type: SMS_ST_NonRecurring, SMS_ST_RecurInterval, SMS_ST_RecurWeekly, SMS_ST_RecurMonthlyByWeekday or SMS_ST_RecurMonthlyByDate</returns>
        public static object DecodeScheduleID(string ScheduleID)
        {
            try
            {
                int year = startyear(ScheduleID);
                int month = startmonth(ScheduleID);
                int day = startday(ScheduleID);
                int hour = starthour(ScheduleID);
                int minute = startminute(ScheduleID);

                if (isNonRecurring(ScheduleID))
                {
                    SMS_ST_NonRecurring oRes = new SMS_ST_NonRecurring();
                    oRes.IsGMT = isgmt(ScheduleID);
                    oRes.StartTime = new DateTime(year, month, day, hour, minute, 0);
                    oRes.DayDuration = dayduration(ScheduleID);
                    oRes.HourDuration = hourduration(ScheduleID);
                    oRes.MinuteDuration = minuteduration(ScheduleID);
                    return oRes;
                }

                if (isRecurInterval(ScheduleID))
                {
                    SMS_ST_RecurInterval oRes = new SMS_ST_RecurInterval();
                    oRes.IsGMT = isgmt(ScheduleID);
                    oRes.StartTime = new DateTime(year, month, day, hour, minute, 0);
                    oRes.DayDuration = dayduration(ScheduleID);
                    oRes.DaySpan = dayspan(ScheduleID);
                    oRes.HourDuration = hourduration(ScheduleID);
                    oRes.HourSpan = hourpan(ScheduleID);
                    oRes.MinuteDuration = minuteduration(ScheduleID);
                    oRes.MinuteSpan = minutespan(ScheduleID);
                    return oRes;
                }

                if (isRecurWeekly(ScheduleID))
                {
                    SMS_ST_RecurWeekly oRes = new SMS_ST_RecurWeekly();
                    oRes.IsGMT = isgmt(ScheduleID);
                    oRes.StartTime = new DateTime(year, month, day, hour, minute, 0);
                    oRes.Day = iDay(ScheduleID);
                    oRes.ForNumberOfWeeks = fornumberofweeks(ScheduleID);
                    oRes.DayDuration = dayduration(ScheduleID);
                    oRes.HourDuration = hourduration(ScheduleID);
                    oRes.MinuteDuration = minuteduration(ScheduleID);
                    return oRes;
                }

                if (isRecurMonthlyByWeekday(ScheduleID))
                {
                    SMS_ST_RecurMonthlyByWeekday oRes = new SMS_ST_RecurMonthlyByWeekday();
                    oRes.IsGMT = isgmt(ScheduleID);
                    oRes.StartTime = new DateTime(year, month, day, hour, minute, 0);
                    oRes.WeekOrder = weekorder(ScheduleID);
                    oRes.Day = iDay(ScheduleID);
                    oRes.ForNumberOfMonths = fornumberofmonths(ScheduleID);
                    oRes.DayDuration = dayduration(ScheduleID);
                    oRes.HourDuration = hourduration(ScheduleID);
                    oRes.MinuteDuration = minuteduration(ScheduleID);
                    return oRes;
                }

                if (isRecurMonthlyByDate(ScheduleID))
                {
                    SMS_ST_RecurMonthlyByDate oRes = new SMS_ST_RecurMonthlyByDate();
                    oRes.IsGMT = isgmt(ScheduleID);
                    oRes.StartTime = new DateTime(year, month, day, hour, minute, 0);
                    oRes.ForNumberOfMonths = fornumberofmonths2(ScheduleID);
                    oRes.MonthDay = monthday(ScheduleID);
                    oRes.DayDuration = dayduration(ScheduleID);
                    oRes.HourDuration = hourduration(ScheduleID);
                    oRes.MinuteDuration = minuteduration(ScheduleID);
                    return oRes;
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// split the scheduleID string into 16char substrings
        /// </summary>
        /// <param name="ScheduleID"></param>
        /// <returns>16char ScheduleIDs</returns>
        public static string[] GetScheduleIDs(string ScheduleID)
        {
            string[] aSchedIds;
            if (ScheduleID.Length < 16)
            {
                aSchedIds = new string[1];
                aSchedIds[0] = ScheduleID;
                return aSchedIds;
            }
            else
            {
                aSchedIds = new string[(ScheduleID.Length) / 16];
            }
            int i = 0;
            while ((i+1)*16 <= ScheduleID.Length)
            {
                aSchedIds[i] = ScheduleID.Substring(i * 16 , 16);
                i++;
            }
            return aSchedIds;
        }

        /// <summary>
        /// Non recuring schedule
        /// </summary>
        public class SMS_ST_NonRecurring
        {
            //public SMS_ST_NonRecurring() { }

            /// <summary>
            /// duration in Days
            /// </summary>
            public int DayDuration { get; set; }

            /// <summary>
            /// duration in hours
            /// </summary>
            public int HourDuration { get; set; }

            /// <summary>
            /// Time is GMT Time
            /// </summary>
            public Boolean IsGMT { get; set; }

            /// <summary>
            /// duration in minutes
            /// </summary>
            public int MinuteDuration { get; set; }

            /// <summary>
            /// Get or set the start time
            /// </summary>
            public DateTime StartTime { get; set; }

            /// <summary>
            /// Get the next start time
            /// </summary>
            public DateTime NextStartTime
            {
                get 
                {
                    return StartTime; 
                }
            }

            /// <summary>
            /// get the ScheduleID
            /// </summary>
            public string ScheduleID
            {
                get
                {
                    return encodeID(this);
                }
            }
        }

        /// <summary>
        /// Interval Schedule (day, hour, minute)
        /// </summary>
        public class SMS_ST_RecurInterval: SMS_ST_NonRecurring
        {
            //public SMS_ST_RecurInterval() { }

            /// <summary>
            /// Interval span in days
            /// </summary>
            public int DaySpan { get; set; }

            /// <summary>
            /// Interval span in hours
            /// </summary>
            public int HourSpan { get; set; }

            /// <summary>
            /// Interval span in minutes
            /// </summary>
            public int MinuteSpan { get; set; }

            /// <summary>
            /// get the next start time
            /// </summary>
            public new DateTime NextStartTime
            {
                get
                {
                    DateTime dEndTime = new DateTime();

                    //determine the new start date-time
                    DateTime oNextStartTime = base.StartTime.Subtract(new TimeSpan(DaySpan, HourSpan, MinuteSpan, 0));
                    dEndTime = oNextStartTime + new TimeSpan(this.DayDuration, this.HourDuration, this.MinuteDuration, 0);
                    while (dEndTime < DateTime.Now)
                    {
                        dEndTime = dEndTime + new TimeSpan(DaySpan, HourSpan, MinuteSpan, 0);
                        oNextStartTime = oNextStartTime + new TimeSpan(DaySpan, HourSpan, MinuteSpan, 0);
                    }
                    return oNextStartTime;

                }
            }
        }

        /// <summary>
        /// Weekly Interval
        /// </summary>
        public class SMS_ST_RecurWeekly: SMS_ST_NonRecurring
        {
            //public SMS_ST_RecurWeekly() { }

            /// <summary>
            /// Day of the Week
            /// </summary>
            public int Day { get; set; }

            /// <summary>
            /// interval in weeks
            /// </summary>
            public int ForNumberOfWeeks { get; set; }

            /// <summary>
            /// Get the next start time
            /// </summary>
            public new DateTime NextStartTime
            {
                get
                {
                    if (base.StartTime > DateTime.Now)
                        return base.StartTime;
                    else
                    {
                        //determine the new start date-time
                        DateTime oNextStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, StartTime.Hour, StartTime.Minute, 0);
                        while (((int)oNextStartTime.DayOfWeek + 1 != Day) | (oNextStartTime < DateTime.Now) )
                        {
                            oNextStartTime = oNextStartTime + new TimeSpan(1, 0, 0, 0);
                        }
                        return oNextStartTime;
                    }
                }
            }
        }

        /// <summary>
        /// Monthly interval (by date)
        /// </summary>
        public class SMS_ST_RecurMonthlyByDate: SMS_ST_NonRecurring
        {
            //public SMS_ST_RecurMonthlyByDate() { }

            /// <summary>
            /// interval in months
            /// </summary>
            public int ForNumberOfMonths { get; set; }

            /// <summary>
            /// Day of the month
            /// </summary>
            public int MonthDay { get; set; }

            /// <summary>
            /// get next start time
            /// </summary>
            public new DateTime NextStartTime
            {
                get
                {
                    if (base.StartTime > DateTime.Now)
                        return base.StartTime;
                    else
                    {
                        //determine the new start date-time
                        if (MonthDay == 0)
                        {
                            //Last Day of Month...
                            DateTime oNextStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, StartTime.Hour, StartTime.Minute, 0);
                            while ((int)oNextStartTime.Day != DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                            {
                                oNextStartTime = oNextStartTime + new TimeSpan(1, 0, 0, 0);
                            }
                            return oNextStartTime;
                        }
                        else
                        {
                            DateTime oNextStartTime = new DateTime(DateTime.Now.Year, StartTime.Month, MonthDay, StartTime.Hour, StartTime.Minute, 0);
                            while (oNextStartTime < DateTime.Now)
                            {
                                oNextStartTime = oNextStartTime.AddMonths(ForNumberOfMonths);
                            }
                            return oNextStartTime;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Monthly interval (by weekday)
        /// </summary>
        public class SMS_ST_RecurMonthlyByWeekday: SMS_ST_NonRecurring
        {
            //public SMS_ST_RecurMonthlyByWeekday() { }

            /// <summary>
            /// Week day
            /// </summary>
            public int Day { get; set; }

            /// <summary>
            /// interval in months
            /// </summary>
            public int ForNumberOfMonths { get; set; }

            /// <summary>
            /// WeekOrder
            /// </summary>
            public int WeekOrder { get; set; }

            /// <summary>
            /// Get next start time
            /// </summary>
            public new DateTime NextStartTime
            {
                get
                {
                    if (base.StartTime > DateTime.Now)
                        return base.StartTime;
                    else
                    {
                        //determine the new start date-time
                        DateTime oNextStartTime = new DateTime(StartTime.Year, StartTime.Month, 1, StartTime.Hour, StartTime.Minute, 0);
                        bool ReRun = false;

                        //Verify that the nextStartTime is in the future
                        while ((oNextStartTime < DateTime.Now) | ReRun)
                        {
                            //Check if next month cycle is still in the past
                            while (oNextStartTime.AddMonths(ForNumberOfMonths) < DateTime.Now)
                            {
                                //Add number of months
                                oNextStartTime = oNextStartTime.AddMonths(ForNumberOfMonths);
                            }

                            int iCount = WeekOrder;
                            while (iCount > 0)
                            {
                                if ((int)oNextStartTime.DayOfWeek + 1 == Day)
                                {
                                    iCount = iCount - 1;
                                    if (iCount != 0)
                                        oNextStartTime = oNextStartTime.AddDays(1);
                                }
                                else
                                {
                                    oNextStartTime = oNextStartTime.AddDays(1);
                                }
                            }

                            //Check if schedule is still in the past ?
                            if (oNextStartTime < DateTime.Now)
                            {
                                oNextStartTime = oNextStartTime.AddMonths(ForNumberOfMonths);
                                oNextStartTime = new DateTime(oNextStartTime.Year, oNextStartTime.Month, 1, oNextStartTime.Hour, oNextStartTime.Minute, oNextStartTime.Second);
                                ReRun = true;
                            }
                            else
                            {
                                //Do not rerun the cycle
                                ReRun = false;
                            }

                        }
                        return oNextStartTime;
                    }
                }
            }
        }

        #endregion
    }
}
