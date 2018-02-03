//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2018 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

#define CM2012
#define CM2007

using System;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Management.Automation;
using static sccmclictr.automation.functions.agentproperties;
using System.Net;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace sccmclictr.automation.functions
{

    /// <summary>
    /// Class agentactions.
    /// </summary>
    public class agentactions : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="agentactions"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The ps code.</param>
        /// <param name="oClient">The CCM client object.</param>
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
            catch { }

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
        /// Trigger Discovery Inventory
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
        /// IDMIF Collection
        /// </summary>
        /// <param name="Full"></param>
        /// <returns></returns>
        public bool IDMIFCollection(Boolean Full)
        {
            try
            {
                if (Full)
                {
                    SMSDelInvHist("{00000000-0000-0000-0000-000000000011}");
                }
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000011}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Client Machine Authentication
        /// </summary>
        /// <param name="Full"></param>
        /// <returns></returns>
        public bool ClientMachineAuthentication(Boolean Full)
        {
            try
            {
                if (Full)
                {
                    SMSDelInvHist("{00000000-0000-0000-0000-000000000012}");
                }
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000012}'");
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
        /// LS (Location Service) Timeout Refresh Task
        /// </summary>
        /// <returns>false=Error</returns>
        public bool TimeoutLocationServicesTask()
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
        /// Policy Agent Request Assignment (User)
        /// </summary>
        /// <returns>false=Error</returns>
        public bool RequestUserAssignments()
        {
            try
            {
                foreach (string sSID in baseClient.AgentProperties.GetLoggedOnUserSIDs())
                {
                    try
                    {
                        string sPath = "root\\ccm\\Policy\\" + sSID.Replace('-', '_') + "\\ActualConfig:CCM_Scheduler_ScheduledMessage.ScheduledMessageID='{00000000-0000-0000-0000-000000000026}'";
                        baseClient.SetProperty(sPath, "Triggers", "@('SimpleInterval;Minutes=1;MaxRandomDelayMinutes=0')");
                    }
                    catch { }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Policy Agent Evaluate Assignment (User)
        /// </summary>
        /// <returns>false=Error</returns>
        public bool EvaluateUserPolicies()
        {
            try
            {
                foreach (string sSID in baseClient.AgentProperties.GetLoggedOnUserSIDs())
                {
                    try
                    {
                        string sPath = "root\\ccm\\Policy\\" + sSID.Replace('-', '_') + "\\ActualConfig:CCM_Scheduler_ScheduledMessage.ScheduledMessageID='{00000000-0000-0000-0000-000000000027}'";
                        baseClient.SetProperty(sPath, "Triggers", "@('SimpleInterval;Minutes=1')");
                    }
                    catch { }
                }
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
        /// Source Update Message
        /// </summary>
        /// <returns>false=Error</returns>
        public bool SourceUpdateMessage()
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
        /// Clearing proxy settings cache
        /// </summary>
        /// <returns></returns>
        public bool ClearingProxySettingsCache()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000037}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Machine Policy Agent Cleanup
        /// </summary>
        /// <returns>false=Error</returns>
        public bool MachinePolicyAgentCleanupCycle()
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
        /// User Policy Agent Cleanup
        /// </summary>
        /// <returns></returns>
        public bool UserPolicyAgentCleanupCycle()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000041}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Policy Agent Validate Machine Policy / Assignment
        /// </summary>
        /// <returns>false=Error</returns>
        public bool ValidateMachineAssignments()
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
        /// Policy Agent Validate User Policy / Assignment
        /// </summary>
        /// <returns></returns>
        public bool ValidateUserAssignments()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000043}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retrying/Refreshing certificates in AD on MP
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
        /// Peer DP Status reporting
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
        /// Peer DP Pending package check schedule
        /// </summary>
        /// <returns>false=Error</returns>
        public bool PeerDPPackageCheck()
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
        /// SUM Updates install schedule
        /// </summary>
        /// <returns></returns>
        public bool SUMUpdatesInstall()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000063}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// NAP Compliance Interval Enforcement
        /// </summary>
        /// <returns>false=Error</returns>
        public bool NAPIntervalEnforcement()
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
        /// Hardware Inventory Collection Cycle
        /// </summary>
        /// <returns></returns>
        public bool HWInvCollection()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000101}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Software Inventory Collection Cycle
        /// </summary>
        /// <returns></returns>
        public bool SWInvCollection()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000102}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Discovery Data Collection Cycle
        /// </summary>
        /// <returns></returns>
        public bool DDRCollection()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000103}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// File Collection Cycle
        /// </summary>
        /// <returns></returns>
        public bool FileCollection()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000104}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// IDMIF Collection Cycle
        /// </summary>
        /// <returns></returns>
        public bool IDMIFCollection()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000105}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Software Metering Usage Report Cycle
        /// </summary>
        /// <returns></returns>
        public bool SWMeteringUsageReport()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000106}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Windows Installer Source List Update Cycle
        /// </summary>
        /// <returns></returns>
        public bool MSISourceListUpdate()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000107}'");
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
        /// Branch Distribution Point Maintenance Task
        /// </summary>
        /// <returns></returns>
        public bool BranchDPMaintenanceTask()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000109}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// DCM policy
        /// </summary>
        /// <returns></returns>
        public bool DCMPolicy()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000110}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send Unsent State Messages
        /// </summary>
        /// <returns>false=Error</returns>
        public bool SendUnsentStatusMessages()
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
        /// State System policy cache cleanout
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
        /// Scan by Update Source
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
        /// Update Store Policy
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
        /// State system policy bulk send high
        /// </summary>
        /// <returns>false=Error</returns>
        public bool StateSystemPolicyBulksendHigh()
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
        /// State system policy bulk send low
        /// </summary>
        /// <returns>false=Error</returns>
        public bool StateSystemPolicyBulksendLow()
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
        /// AMT Status Check Policy
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
        /// Application manager policy action
        /// </summary>
        /// <returns>false=Error</returns>
        public bool AppManPolicyAction()
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
        /// Application manager user policy action
        /// </summary>
        /// <returns></returns>
        public bool AppManUserPolicyAction()
        {
            /*try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000122}'");
            }
            catch
            {
                return false;
            } */

            try
            {
                foreach (string sSID in baseClient.AgentProperties.GetLoggedOnUserSIDs())
                {
                    try
                    {
                        string sPath = "root\\ccm\\Policy\\" + sSID.Replace('-', '_') + "\\ActualConfig:CCM_Scheduler_ScheduledMessage.ScheduledMessageID='{00000000-0000-0000-0000-000000000122}'";
                        baseClient.SetProperty(sPath, "Triggers", "@('SimpleInterval;Minutes=1;MaxRandomDelayMinutes=0')");
                    }
                    catch { }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Application manager global evaluation action
        /// </summary>
        /// <returns></returns>
        public bool AppManGlobalEvaluation()
        {
            try
            {
                //WMI Schedule does not exist: base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000123}'");
                //Using COM Object instead of WMI
                base.GetObjectsFromPS("((New-Object -comobject \"CPApplet.CPAppletMgr\").GetClientActions() | Where-Object { $_.ActionID -eq '{00000000-0000-0000-0000-000000000123}' }).PerformAction()", true);
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
        /// Endpoint deployment reevaluate
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
        /// Endpoint AM policy reevaluate
        /// </summary>
        /// <returns></returns>
        public bool EndpointAMPolicyreevaluate()
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000222}'");
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// External event detection
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
                }
                else
                {
                    base.CallClassMethod(@"ROOT\ccm:SMS_Client", "ResetPolicy", "0", true);
                }

                this.MachinePolicyAgentCleanupCycle();
                this.RequestMachinePolicyAssignments();
            }
            catch
            {
                return false;
            }

            return true;

        }

        /// <summary>
        /// Repairs the SCCM/CM12 agent.
        /// </summary>
        /// <returns><c>true</c> if method attempted to repair the agent, <c>false</c> otherwise.</returns>
        public bool RepairAgent()
        {
            string sProdCode = baseClient.AgentProperties.ProductCode;
            if (sProdCode.StartsWith("{"))
            {
                string sResult = baseClient.GetStringFromPS("Invoke-Expression(\"msiexec.exe /fpecms '" + sProdCode + "'\")");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove the SCCM/CM12 Agent
        /// </summary>
        /// <returns></returns>
        public bool UninstallAgent()
        {
            string sProdCode = baseClient.AgentProperties.ProductCode;
            if (sProdCode.StartsWith("{"))
            {
                string sResult = baseClient.GetStringFromPS("Invoke-Expression(\"msiexec.exe /x '" + sProdCode + "' REBOOT=ReallySuppress /q\")");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset the Pauses Software Distribution flag
        /// </summary>
        /// <returns>true = success; false = error</returns>
        public bool ResetPausedSWDist()
        {
            try
            {
                string sResult = base.GetStringFromPS("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\State\" -Name \"Paused\" -Type DWORD -force -value 0", true);
                sResult = base.GetStringFromPS("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\State\" -Name \"PausedCookie\" -Type DWORD -force -value 0", true);

                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Reset the ProvisioningMode Flag
        /// </summary>
        /// <returns>true = success; false = error</returns>
        public bool ResetProvisioningMode()
        {
            try
            {
                string sResult = base.GetStringFromPS("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\\CcmExec\" -Name \"ProvisioningMode\" -Type string -force -value \"false\"", true);

                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Remove SystemTaskExclude entries from Registry
        /// </summary>
        /// <returns>true = success; false = error</returns>
        public bool SystemTaskExclude()
        {
            try
            {
                string sResult = base.GetStringFromPS("New-ItemProperty -path \"HKLM:\\SOFTWARE\\Microsoft\\CCM\\CcmExec\" -Name \"SystemTaskExcludes\" -Type string -force -value \"\"", true);

                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Remove IsCacheCopyNeededCallBack entry from Registry
        /// </summary>
        /// <returns>true = success; false = error</returns>
        public bool IsCacheCopyNeededCallBack()
        {
            try
            {
                string sResult = base.GetStringFromPS(@"Remove-ItemProperty 'hklm:\Software\Microsoft\SMS\Mobile Client\Software Distribution\' 'IsCacheCopyNeededCallBack' -ea SilentlyContinue", true);

                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Import a local Application Policy
        /// </summary>
        /// <param name="Body">Policy Body as XML</param>
        /// <param name="BodySignature">Body Signature</param>
        /// <param name="BodySource">'LOCAL' if not specified</param>
        /// <returns></returns>
        public bool ApplyPolicyEx(string Body, string BodySignature, string BodySource = "LOCAL")
        {
            try
            {
                string sResult = baseClient.GetStringFromPS(@"([wmiclass]'ROOT\ccm\ClientSdk:CCM_SoftwareCatalogUtilities').ApplyPolicyEx('" + Body + "','" + BodySignature + "','" + BodySource + "').Id");
                if (!string.IsNullOrEmpty(sResult))
                    return true;
                else
                    return false;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Import an Application into the users policy store
        /// </summary>
        /// <param name="ApplicationID"></param>
        /// <returns></returns>
        public bool ImportApplicationPolicy(string ApplicationID)
        {
            try
            {
                DeviceId oID = baseClient.AgentProperties.GetDeviceId;
                string sPortalURL = baseClient.AgentProperties.PortalURL;
                string SOAPResult = "";

                //Console.WriteLine(sPortalURL);

                if (!string.IsNullOrEmpty(sPortalURL))
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(sPortalURL + "/applicationviewservice.asmx");
                    webRequest.Headers.Add("SOAPAction","http://schemas.microsoft.com/5.0.0.0/ConfigurationManager/SoftwareCatalog/Website/InstallApplication");
                    webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                    webRequest.Accept = "text/xml";
                    webRequest.Method = "POST";
                    webRequest.UseDefaultCredentials = true;

                    XmlDocument soapEnvelopeXml = new XmlDocument();
                    soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?><s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body><InstallApplication xmlns=""http://schemas.microsoft.com/5.0.0.0/ConfigurationManager/SoftwareCatalog/Website"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><applicationID>" +
                        ApplicationID + "</applicationID><deviceID>" + oID.ClientId + "," + oID.SignedClientId +
                        "</deviceID><reserved/></InstallApplication></s:Body></s:Envelope>");

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
                    return false;

                if(!string.IsNullOrEmpty(SOAPResult))
                {


                    //Console.WriteLine(SOAPResult);
                    /*string sPSCode = "[xml]$out = '" + SOAPResult + "';" +
                        "$BodySignature = $out.Envelope.Body.InstallApplicationResponse.InstallApplicationResult.BodySignature;" +
                        "$Body64 = $out.Envelope.Body.InstallApplicationResponse.InstallApplicationResult.PolicyAssignmentsDocument;" +
                        "$Body = [System.Text.Encoding]::Unicode.GetString([System.Convert]::FromBase64String($Body64));" +
                        @"([wmiclass]'ROOT\ccm\ClientSdk:CCM_SoftwareCatalogUtilities').ApplyPolicyEx($Body, $BodySignature, 'LOCAL')";*/
                    //List<PSObject> oRes = baseClient.GetObjectsFromPS(sPSCode, true);

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(SOAPResult);
                    XmlNode xNode2 = xDoc.SelectSingleNode("//*[local-name()='InstallApplicationResult']");
                    string s2 = xNode2["PolicyAssignmentsDocument"].InnerText;
                    //[System.Text.Encoding]::Unicode.GetString([System.Convert]::FromBase64String($Body64))
                    string sPolicy = System.Text.Encoding.Unicode.GetString(System.Convert.FromBase64String(s2));
                    string sPSCode = getPSWMIScript(sPolicy);
                    string sRes = baseClient.GetStringFromPS(sPSCode, true);
                    //Console.WriteLine(oRes[0].Properties["Id"].Value.ToString());
                    System.Threading.Thread.Sleep(1000);
                    baseClient.AgentActions.AppManGlobalEvaluation();
                    return true;
                }

            }
            catch(Exception ex) { Console.WriteLine(ex.Message);  }

            return false;
        }

        /// <summary>
        /// Generate PS to import App Policy
        /// </summary>
        /// <param name="sXMLBody"></param>
        /// <returns></returns>
        public string getPSWMIScript(string sXMLBody)
        {
            File.WriteAllText(Environment.ExpandEnvironmentVariables(@"%temp%\body.xml"), sXMLBody);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sXMLBody);

            List<string> lResult = new List<string>();

            foreach (XmlNode xNode in xDoc.SelectNodes("/ReplyAssignments/EPolicy/PolicyXML"))
            {
                try
                {
                    string sComp = xNode.InnerXml.ToString();
                    string sXMLPolicy = sccmclictr.automation.policy.localpolicy.DecompressPolicy(sComp);
                    sXMLPolicy.ToString();

                    XmlDocument xBody = new XmlDocument();
                    xBody.LoadXml(sXMLPolicy);
                    foreach (XmlNode xPolicy in xBody.SelectNodes("/Policy/PolicyRule/PolicyAction/instance"))
                    {
                        try
                        {
                            string sClassName = xPolicy.Attributes["class"].Value.ToString();
                            lResult.Add(@"$ruleClass = ([WMICLASS]'ROOT\ccm\Policy\Machine\ActualConfig:" + sClassName + "').CreateInstance();");

                            foreach (XmlNode xProperty in xPolicy.SelectNodes("property"))
                            {
                                try
                                {
                                    string sType = xProperty.Attributes["type"].Value as string;
                                    if (!string.IsNullOrEmpty(xProperty.InnerText))
                                    {
                                        switch (sType)
                                        {
                                            case "8200":
                                                lResult.Add("$ruleClass[\"" + xProperty.Attributes["name"].Value.ToString() + "\"] = @(\"" + xProperty.InnerText.Trim().Replace("\r\n", "`r`n").Replace("'", "`'").Replace("\"", "`\"") + "\");");
                                                continue;
                                            case "19":
                                                lResult.Add("$ruleClass[\"" + xProperty.Attributes["name"].Value.ToString() + "\"] = " + xProperty.InnerText.Trim().Replace("\r\n", "`r`n").Replace("'", "`'").Replace("\"", "`\"") + ";");
                                                continue;
                                            default:
                                                lResult.Add("$ruleClass[\"" + xProperty.Attributes["name"].Value.ToString() + "\"] = \"" + xProperty.InnerText.Trim().Replace("\r\n", "`r`n").Replace("'", "`'").Replace("\"", "`\"") + "\";");
                                                continue;
                                        }
                                    }
                                }
                                catch { }
                            }

                            lResult.Add("$ruleClass.Put();");
                        }
                        catch { }
                    }
                }
                catch { }
            }
            string sResult = string.Join("", lResult.ToArray());
            File.WriteAllText(Environment.ExpandEnvironmentVariables(@"%temp%\sccmclictr.ps1"), sResult);
            return sResult;
        }
    }
}
