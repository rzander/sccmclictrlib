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
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Web;


namespace sccmclictr.automation.functions
{

    public class agentactions: baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;
        
        public agentactions(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Delete InventoryActionStatus from Root\ccm\invagt:InventoryActionStatus
        /// </summary>
        /// <param name="ScheduleID">SCCM Schedule ID</param>
        private void SMSDelInvHist(string ScheduleID)
        {
            try
            {
                base.GetStringFromPS(string.Format("[wmi]\"ROOT\\ccm\\invagt:InventoryActionStatus.InventoryActionID='{0}'\" | remove-wmiobject", ScheduleID));
            }
            catch
            {
            }
        }

        /// <summary>
        /// Trigger Hardware-Inventory
        /// </summary>
        /// <param name="Full">Enforce a full inventory (Default=Delta)</param>
        /// <returns>false=Error</returns>
        public bool HardwareInventory(Boolean Full)
        {
            try
            {
                if (Full)
                {
                    SMSDelInvHist("{00000000-0000-0000-0000-000000000001}");
                }
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000001}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Trigger Software-Inventory
        /// </summary>
        /// <param name="Full">Enforce a full inventory (Default=Delta)</param>
        /// <returns>false=Error</returns>
        public bool SoftwareInventory(Boolean Full)
        {
            try
            {
                if (Full)
                {
                    SMSDelInvHist("{00000000-0000-0000-0000-000000000002}");
                }
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000002}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Trigger DataDiscovery (Heartbeat)
        /// </summary>
        /// <param name="Full">Enforce a full Discovery (Delete History-Timestamp)</param>
        /// <returns>false=Error</returns>
        public bool DataDiscovery(Boolean Full)
        {
            try
            {
                if (Full)
                {
                    SMSDelInvHist("{00000000-0000-0000-0000-000000000003}");
                }
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000003}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Trigger File Collection
        /// </summary>
        /// <param name="Full">Enforce a full File Colelction (Delete History-Timestamp)</param>
        /// <returns>false=Error</returns>
        public bool FileCollection(Boolean Full)
        {
            try
            {
                if (Full)
                {
                    SMSDelInvHist("{00000000-0000-0000-0000-000000000010}");
                }
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000010}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Request Machine Policy Assignments
        /// </summary>
        /// <returns>false=Error</returns>
        public bool RequestMachinePolicyAssignments()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000021}'", true);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Evaluate Machine Policy Assignments
        /// </summary>
        /// <returns>false=Error</returns>
        public bool EvaluateMachinePolicyAssignments()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000022}'", true);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Refresh Default MP Task
        /// </summary>
        /// <returns>false=Error</returns>
        public bool RefreshDefaultMPTask()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000023}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Refresh Location Services Task
        /// </summary>
        /// <returns>false=Error</returns>
        public bool RefreshLocationServicesTask()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000024}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Location Services Cleanup Task
        /// </summary>
        /// <returns>false=Error</returns>
        public bool LocationServicesCleanupTask()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000025}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Software Metering Report Cycle
        /// </summary>
        /// <returns>false=Error</returns>
        public bool SoftwareMeteringReportCycle()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000031}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Source Update Manage Update Cycle
        /// </summary>
        /// <returns>false=Error</returns>
        public bool SourceUpdateManageUpdateCycle()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000032}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Policy Agent Cleanup Cycle
        /// </summary>
        /// <returns>false=Error</returns>
        public bool PolicyAgentCleanupCycle()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000040}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate Assignments
        /// </summary>
        /// <returns>false=Error</returns>
        public bool ValidateAssignments()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000042}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Certificate Maintenance Cycle
        /// </summary>
        /// <returns>false=Error</returns>
        public bool CertificateMaintenanceCycle()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000051}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Peer Distribution Point Status Task
        /// </summary>
        /// <returns>false=Error</returns>
        public bool PeerDistributionPointStatusTask()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000061}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Peer Distribution Point Provisioning Status Task
        /// </summary>
        /// <returns>false=Error</returns>
        public bool PeerDistributionPointProvisioningStatusTask()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000062}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compliance Interval Enforcement
        /// </summary>
        /// <returns>false=Error</returns>
        public bool ComplianceIntervalEnforcement()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000071}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Software Updates Agent Assignment Evaluation Cycle
        /// </summary>
        /// <returns>false=Error</returns>
        public bool SoftwareUpdatesAgentAssignmentEvaluationCycle()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000108}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send Unsent State Messages with Priority 5
        /// </summary>
        /// <returns>false=Error</returns>
        public bool SendUnsentStatusMessages_Prio5()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000111}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// State Message Manager Task
        /// </summary>
        /// <returns>false=Error</returns>
        public bool StateMessageManagerTask()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000112}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Force Update Scan
        /// </summary>
        /// <returns>false=Error</returns>
        public bool ForceUpdateScan()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000113}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update Status Refresh
        /// </summary>
        /// <returns>false=Error</returns>
        public bool UpdateStatusRefresh()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000114}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send Unsent Status Messages with Priority 1
        /// </summary>
        /// <returns>false=Error</returns>
        public bool SendUnsentStatusMessages_Prio1()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000115}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send Unsent Status Messages with Priority 10
        /// </summary>
        /// <returns>false=Error</returns>
        public bool SendUnsentStatusMessages_Prio10()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000116}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// AMT Provision Cycle
        /// </summary>
        /// <returns>false=Error</returns>
        public bool AMTProvisionCycle()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000120}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// DCM Policy Enforcement
        /// </summary>
        /// <returns>false=Error</returns>
        public bool DCMPolicyEnforcement()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000121}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// PowerMgmt Start Summarization Task
        /// </summary>
        /// <returns>false=Error</returns>
        public bool PowerMgmtStartSummarizationTask()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000131}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Endpoint Deployment Message
        /// </summary>
        /// <returns>false=Error</returns>
        public bool EndpointDeploymentMessage()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000221}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// External Event Detection Message
        /// </summary>
        /// <returns>false=Error</returns>
        public bool ExternalEventDetectionMessage()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000223}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Cleanup the Message Queue from the SCCM Agent
        /// </summary>
        /// <returns></returns>
        public bool CleanupMessageQueue()
        {
            try
            {
                baseClient.Services.GetService("CcmExec").StopService();
                string sQueuePath = System.IO.Path.Combine(baseClient.AgentProperties.LocalSCCMAgentPath, @"ServiceData\Messaging\EndpointQueues");
                base.GetStringFromPS("get-childitem '" + sQueuePath + @"' -include *.msg,*.que -recurse | foreach ($_) {remove-item $_.fullname -force}");
                baseClient.Services.GetService("CcmExec").StartService();
                StateMessageManagerTask();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reset Policy
        /// </summary>
        /// <param name="Hardreset"></param>
        /// <returns>false=Error</returns>
        public bool ResetPolicy(bool Hardreset)
        {
            try
            {
                if (Hardreset)
                {
                    base.CallClassMethod(@"ROOT\ccm:SMS_Client", "ResetPolicy", "1", true);
                    this.CertificateMaintenanceCycle();
                    this.PolicyAgentCleanupCycle();
                }
                else
                {
                    base.CallClassMethod(@"ROOT\ccm:SMS_Client", "ResetPolicy", "0", true);
                    this.PolicyAgentCleanupCycle();
                }
            }
            catch
            {
                return false;
            }

            return true;

        }
    }
}
