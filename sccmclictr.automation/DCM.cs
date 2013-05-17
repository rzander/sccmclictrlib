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
using System.Xml;

namespace sccmclictr.automation.functions
{
    public class dcm : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="RemoteRunspace"></param>
        /// <param name="PSCode"></param>
        /// <param name="oClient"></param>
        public dcm(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        public List<SMS_DesiredConfiguration> DCMBaselines
        {
            get
            {
                List<SMS_DesiredConfiguration> lCache = new List<SMS_DesiredConfiguration>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\dcm", "SELECT * FROM SMS_DesiredConfiguration", true);
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    SMS_DesiredConfiguration oCIEx = new SMS_DesiredConfiguration(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }

                return lCache;
            }
        }
        /// <summary>
        /// Source:ROOT\ccm\dcm
        /// </summary>
        public class SMS_DesiredConfiguration
        {
            internal baseInit oNewBase;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="WMIObject"></param>
            /// <param name="RemoteRunspace"></param>
            /// <param name="PSCode"></param>
            public SMS_DesiredConfiguration(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;
                oNewBase = new baseInit(remoteRunspace, pSCode);

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ComplianceDetails = WMIObject.Properties["ComplianceDetails"].Value as String;
                this.DisplayName = WMIObject.Properties["DisplayName"].Value as String;
                this.IsMachineTarget = WMIObject.Properties["IsMachineTarget"].Value as Boolean?;
                this.LastComplianceStatus = WMIObject.Properties["LastComplianceStatus"].Value as UInt32?;
                string sLastEvalTime = WMIObject.Properties["LastEvalTime"].Value as string;
                if (string.IsNullOrEmpty(sLastEvalTime))
                    this.LastEvalTime = null;
                else
                    this.LastEvalTime = ManagementDateTimeConverter.ToDateTime(sLastEvalTime) as DateTime?;
                this.Name = WMIObject.Properties["Name"].Value as String;
                this.Status = WMIObject.Properties["Status"].Value as UInt32?;
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
            public String ComplianceDetails { get; set; }
            public String DisplayName { get; set; }
            public Boolean? IsMachineTarget { get; set; }
            public UInt32? LastComplianceStatus { get; set; }
            public DateTime? LastEvalTime { get; set; }
            public String Name { get; set; }
            public UInt32? Status { get; set; }
            public String Version { get; set; }
            #endregion

            #region Methods

            /// <summary>
            /// Trigger the Baseline evaluation
            /// </summary>
            /// <param name="IsEnforced"></param>
            /// <param name="JobId"></param>
            /// <returns></returns>
            public UInt32 TriggerEvaluation(Boolean IsEnforced, out String JobId)
            {
                JobId = "";
                try
                {
                    PSObject oResult = oNewBase.CallClassMethod("ROOT\\ccm\\dcm:SMS_DesiredConfiguration", "TriggerEvaluation", "'" + Name + "', '" + Version + "', $" + IsMachineTarget.ToString() + " , $" + IsEnforced.ToString());
                    JobId = oResult.Properties["JobID"].Value.ToString();
                    return (UInt32)oResult.Properties["ReturnValue"].Value;
                }
                catch { }

                return 1;
            }

            /// <summary>
            /// ConfigItem Object
            /// </summary>
            public class ConfigItem
            {
                /// <summary>
                /// Logical Name
                /// </summary>
                public string LogicalName { get; set; }

                /// <summary>
                /// Display Name
                /// </summary>
                public string CIName { get; set; }

                /// <summary>
                /// Description
                /// </summary>
                public string CIDescription { get; set; }

                /// <summary>
                /// CI Version
                /// </summary>
                public string Version { get; set; }

                /// <summary>
                /// CI Type
                /// </summary>
                public string Type { get; set; }

                /// <summary>
                /// Compliance state
                /// </summary>
                public bool Compliant { get; set; }

                /// <summary>
                /// Detection state
                /// </summary>
                public bool Detected { get; set; }

                /// <summary>
                /// Applicable state
                /// </summary>
                public bool Applicable { get; set; }

                /// <summary>
                /// Violation status
                /// </summary>
                public string ConstraintViolation { get; set; }


            }

            /// <summary>
            /// List of Config Items (Class ConfigItem)
            /// </summary>
            /// <param name="DCMBaseline">Instance of SMS_DesiredConfiguration ManagementObject</param>
            /// <returns>Confg Items</returns>
            public List<ConfigItem> ConfigItems()
            {
                List<ConfigItem> oResult = new List<ConfigItem>();

                try
                {

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(ComplianceDetails);
                    XmlNodeList xNodes = xDoc.SelectNodes(@"//ConfigurationItemReport/ReferencedConfigurationItems/ConfigurationItemReport");
                    foreach (XmlNode xNode in xNodes)
                    {
                        ConfigItem oItem = new ConfigItem();
                        oItem.LogicalName = xNode.Attributes["LogicalName"].Value.ToString();
                        oItem.Applicable = (xNode.Attributes["CIApplicablityState"].Value == "Applicable");
                        oItem.Compliant = (xNode.Attributes["CIComplianceState"].Value == "Compliant");
                        oItem.Detected = bool.Parse(xNode.Attributes["IsDetected"].Value.ToString());
                        oItem.Type = xNode.Attributes["Type"].Value.ToString();
                        oItem.Version = xNode.Attributes["Version"].Value.ToString();

                        oItem.CIName = xNode.SelectSingleNode("./CIProperties/Name").InnerText;
                        oItem.CIDescription = xNode.SelectSingleNode("./CIProperties/Description").InnerText;

                        if (xNode.SelectSingleNode("./ConstraintViolations[@Count > 0]") != null)
                        {
                            string sSeverity = "";
                            //Need to fix !
                            foreach (XmlNode xSeverity in xNode.SelectNodes("./ConstraintViolations/ConstraintViolation"))
                            {
                                string sSev = xSeverity.Attributes["Severity"].Value.ToString();

                                if (string.Compare(sSev, "Information", true) == 0)
                                {
                                    if ((string.Compare(sSeverity, "Error", true) != 0) & (string.Compare(sSeverity, "Warning", true) != 0))
                                        sSeverity = sSev;
                                }

                                if (string.Compare(sSev, "Warning", true) == 0)
                                {
                                    if (string.Compare(sSeverity, "Error", true) != 0)
                                        sSeverity = sSev;
                                }

                                if (string.Compare(sSev, "Error", true) == 0)
                                {
                                    sSeverity = sSev;
                                }
                            }

                            //oItem.ConstraintViolation = xNode.SelectSingleNode("./ConstraintViolations/ConstraintViolation").Attributes["Severity"].Value.ToString();
                            oItem.ConstraintViolation = sSeverity;
                        }
                        else
                        {
                            oItem.ConstraintViolation = "";
                        }

                        oResult.Add(oItem);
                    }
                }
                catch { }

                return oResult;
            }

            /*
            public UInt32 GetProperty(UInt32 LanguageId, String PropertyName, out String PropertyValue)
            {
                return 0;
            }
            public UInt32 GetUserReport(String Name, String UserSID, String Version, out String ComplianceDetails)
            {
                return 0;
            }
            public UInt32 EvaluatePoliciesOfType(Boolean IsEnforced, Boolean IsSynchronousRequest, Boolean IsUserLogonBlocked, String PolicyType, UInt64 RequestTimeout, UInt64 UserSessionId, UInt64 UserToken, UInt64 UserTokenSourceProcessId, out String JobId)
            {
                return 0;
            }
            public UInt32 GetComplianceForPolicyType(String PolicyType, String UserSID, out String ComplianceState)
            {
                return 0;
            }  */
            #endregion
        }

    }
}
