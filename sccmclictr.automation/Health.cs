//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2018 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

#define CM2012
#define CM2007

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;

namespace sccmclictr.automation.functions
{
    /// <summary>
    /// Agent health and repair functions
    /// </summary>
    public class health : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="health"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        /// <param name="oClient">a CCM Client object.</param>
        public health(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /*public new void Dispose()
        {
            //baseClient.Dispose();
            remoteRunspace.Dispose();
            //base.Dispose();
        } */

        /// <summary>
        /// Verify WMI Repository (winmgmt /verifyrepository).
        /// </summary>
        /// <returns>Command results as string</returns>
        public string WMIVerifyRepository()
        {
            TimeSpan toldCacheTime = base.cacheTime;

            //Cache for 30 seconds
            base.cacheTime = new TimeSpan(0, 0, 30);
            string sResult = base.GetStringFromPS("winmgmt /verifyrepository");
            base.cacheTime = toldCacheTime;

            return sResult;
        }

        /// <summary>
        /// Performs a consistency check on the WMI repository (winmgmt /salvagerepository).
        /// </summary>
        /// <returns>Command results as string</returns>
        public string WMISalvageRepository()
        {
            string sResult = base.GetStringFromPS("winmgmt /salvagerepository", true);

            return sResult;
        }

        /// <summary>
        /// The repository is reset to the initial state when the operating system is first installed (winmgmt /resetrepository).
        /// </summary>
        /// <returns>Command results as string</returns>
        public string WMIResetRepository()
        {
            string sResult = base.GetStringFromPS("Stop-Service winmgmt -Force; winmgmt /resetrepository", true);

            return sResult;
        }

        /// <summary>
        /// Registers the system performance libraries with WMI (winmgmt /resyncperf).
        /// </summary>
        /// <returns>Command results as string</returns>
        public string WMIRegPerfLibraries()
        {
            string sResult = base.GetStringFromPS("winmgmt /resyncperf", true);

            return sResult;
        }

        /// <summary>
        /// Rgister DLL using Regsvr32
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public string RegsiertDLL(string File)
        {
            string sResult = base.GetStringFromPS(string.Format("regsvr32.exe /s \"{0}\"", Environment.ExpandEnvironmentVariables(File)), true);

            return sResult;
        }

        /// <summary>
        /// State if DCOM is enabled (HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Ole:EnableDCOM)
        /// </summary>
        public bool EnableDCOM
        {
            get
            {
                TimeSpan toldCacheTime = base.cacheTime;

                //Cache for 30 seconds
                base.cacheTime = new TimeSpan(0, 0, 30);
                string sResult = base.GetStringFromPS("(get-ItemProperty \"HKLM:\\SOFTWARE\\Microsoft\\Ole\").\"EnableDCOM\"");
                base.cacheTime = toldCacheTime;
                if (string.Compare(sResult, "Y", true) == 0)
                    return true;
                else
                    return false;
            }

            set
            {
                string sValue = value ? "Y" : "N";
                string sResult = base.GetStringFromPS(string.Format("set-ItemProperty -Path \"HKLM:\\SOFTWARE\\Microsoft\\Ole\" -Name \"EnableDCOM\" -Value \"{0}\"", sValue));
            }
        }

        /// <summary>
        /// Get the DCOM Permission ACL from a Binary Registry Key.
        /// </summary>
        /// <param name="HKLMKey">HKLM Key like "software\microsoft\ole"</param>
        /// <param name="RegValue">Registry Value that contains the DCOM Permission like "MachineLaunchRestriction"</param>
        /// <returns></returns>
        public string GetDCOMPerm(string HKLMKey, string RegValue)
        {
            TimeSpan toldCacheTime = base.cacheTime;

            //Cache for 30 seconds
            base.cacheTime = new TimeSpan(0, 0, 30);
            string sResult = base.GetStringFromPS(string.Format(Properties.Settings.Default.PSGetDCOMPerm, HKLMKey, RegValue));
            
            base.cacheTime = toldCacheTime;
            return sResult;
        }

        /// <summary>
        /// Set the DCOM Permission ACL to a Binary Registry Key.
        /// O:BAG:BAD:(A;;CCDCSW;;;WD)(A;;CCDCLCSWRP;;;BA)(A;;CCDCLCSWRP;;;LU)(A;;CCDCLCSWRP;;;S-1-5-32-562)
        /// </summary>
        /// <param name="HKLMKey">HKLM Key like "software\microsoft\ole"</param>
        /// <param name="RegValue">Registry Value that contains the DCOM Permission like "MachineLaunchRestriction"</param>
        /// <param name="ACL">ACL like "O:BAG:BAD:(A;;CCDCSW;;;WD)(A;;CCDCLCSWRP;;;BA)(A;;CCDCLCSWRP;;;LU)(A;;CCDCLCSWRP;;;S-1-5-32-562)"</param>
        /// <returns></returns>
        public string SetDCOMPerm(string HKLMKey, string RegValue, string ACL)
        {
            string sResult = base.GetStringFromPS(string.Format(Properties.Settings.Default.PSSetDCOMPerm, HKLMKey, RegValue, ACL), true);
            return "";
        }

        /// <summary>
        /// Run a "basic" Powershell Health check 
        /// </summary>
        /// <returns></returns>
        public string RunHealthCheck()
        {
            try
            {
                List<PSObject> res = base.GetObjectsFromPS(Properties.Resources.HealthCheck);
                return res[0].ToString();
            }
            catch { }

            return "";
        }

        /// <summary>
        /// Delete root\ccm namespace in WMI
        /// </summary>
        public void DeleteCCMNamespace()
        {
            base.GetStringFromPS("gwmi -query \"SELECT * FROM __Namespace WHERE Name='CCM'\" -Namespace \"root\" | Remove-WmiObject");
        }

        /// <summary>
        /// Delete all machine certificates from the SMS Folder
        /// </summary>
        public void DeleteSMSCertificates()
        {
            try
            {
                base.GetStringFromPS(@"Remove-Item -path HKLM:\SOFTWARE\Microsoft\SystemCertificates\SMS\* -Recurse");
            }
            catch
            {
            }
        }

        /// <summary>
        /// Run CCMEval.exe
        /// </summary>
        public void RunCCMEval()
        { 
            baseClient.Process.CreateProcess(baseClient.AgentProperties.LocalSCCMAgentPath + "ccmeval.exe");
        }

        /// <summary>
        /// DateTime of last CCMEval cycle 
        /// </summary>
        public DateTime LastCCMEval
        {
            get 
            {
                try
                {
                    string sPScript = "[xml]$ccmeval = Get-Content \"" + baseClient.AgentProperties.LocalSCCMAgentPath + "CcmEvalReport.xml\"; $ccmeval.ClientHealthReport.Summary.EvaluationTime"; // | % { $_ }; ";
                    string sResult = base.GetStringFromPS(sPScript);
                    return DateTime.Parse(sResult); //"yyyy-mm-ddThh:MM:ssZ"
                }
                catch { }

                return new DateTime();
            }
        }

        /// <summary>
        /// Show results of the CCMEval taks
        /// </summary>
        /// <returns>List of CCMEval results</returns>
        public List<ccmeval> GetCCMEvalStatus()
        {
            List<ccmeval> lResult = new List<ccmeval>();

            string sPScript = "[xml]$ccmeval = Get-Content \"" + baseClient.AgentProperties.LocalSCCMAgentPath + "CcmEvalReport.xml\"; $ccmeval.ClientHealthReport.HealthChecks.HealthCheck"; // | % { $_ }; ";

            //Always reload XML...
            List<PSObject> res = base.GetObjectsFromPS(sPScript, true);
            foreach (PSObject oObj in res)
            {
                try
                {
                    ccmeval oEval = new ccmeval();
                    oEval.ID = oObj.Properties["ID"].Value.ToString();
                    oEval.Description = oObj.Properties["Description"].Value.ToString();
                    oEval.ResultCode = oObj.Properties["ResultCode"].Value.ToString();
                    oEval.ResultType = oObj.Properties["ResultType"].Value.ToString();
                    oEval.ResultDetail = oObj.Properties["ResultDetail"].Value.ToString();
                    oEval.StepDetail = oObj.Properties["StepDetail"].Value.ToString();
                    oEval.text = oObj.Properties["#text"].Value.ToString();

                    lResult.Add(oEval);
                }
                catch { }
            }

            return lResult;
        }

        /// <summary>
        /// ccmeval result entry
        /// </summary>
        public class ccmeval
        {
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            public string ID { get; set; }
            public string Description { get; set; }
            public string ResultCode { get; set; }
            public string ResultType{ get; set; }
            public string ResultDetail { get; set; }
            public string StepDetail { get; set; }
            public string text { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/cc146437.aspx; This causes the client to resend a full compliance report to the Configuration Manager server
        /// </summary>
        public void RefreshServerComplianceState()
        {
            try
            {
                string sResult = base.GetStringFromPS("(New-Object -ComObject Microsoft.CCM.UpdatesStore).RefreshServerComplianceState()", true);
            }
            catch { }
        }
    }
}
