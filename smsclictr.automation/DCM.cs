//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Xml;

namespace smsclictr.automation
{
    /// <summary>
    /// Class to manage Desired Configuration Monitoring
    /// </summary>
    public class SCCMDCM
    {
        #region Internal
        private WMIProvider oWMIProvider;
        private ManagementObjectCollection oDCMBaselines;
        #endregion

        #region Constructor

        /// <summary>
        /// SMSSchedules Constructor
        /// </summary>
        /// <param name="oProvider"></param>
        public SCCMDCM(WMIProvider oProvider)
        {
            oWMIProvider = new WMIProvider(oProvider.mScope.Clone());
        }

        #endregion

        #region Functions

        /// <summary>
        /// Get DCM Baselines (Class SMS_DesiredConfiguration)
        /// </summary>
        /// <returns>Class SMS_DesiredConfiguration</returns>
        public ManagementObjectCollection CCM_DCMBaselines()
        {
            if (oDCMBaselines == null)
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\dcm";
                ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM SMS_DesiredConfiguration");
                oDCMBaselines = MOC;
                return MOC;
            }
            else
            {
                return oDCMBaselines;
            }

        }

        /// <summary>
        /// Get DCM Baselines (Class SMS_DesiredConfiguration)
        /// </summary>
        /// <param name="refresh">true=reload;false=get cached objects</param>
        /// <returns>Class SMS_DesiredConfiguration</returns>
        public ManagementObjectCollection CCM_DCMBaselines(bool refresh)
        {
            if (refresh)
            {
                oDCMBaselines = null;
            }
            return CCM_DCMBaselines();
        }

        #endregion

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
        public List<ConfigItem> ConfigItems(ManagementObject DCMBaseline)
        {
            List<ConfigItem> oResult = new List<ConfigItem>();

            try
            {

                XmlDocument xDoc = new XmlDocument();
                DCMBaseline.Get();
                xDoc.LoadXml(DCMBaseline.Properties["ComplianceDetails"].Value.ToString());
                XmlNodeList xNodes = xDoc.SelectNodes(@"//DiscoveryReport/BaselineCIComplianceState/PartsCompliance/PartCIComplianceState");
                foreach (XmlNode xNode in xNodes)
                {
                    ConfigItem oItem = new ConfigItem();
                    oItem.LogicalName = xNode.Attributes["LogicalName"].Value.ToString();
                    oItem.Applicable = bool.Parse(xNode.Attributes["Applicable"].Value.ToString());
                    oItem.Compliant = bool.Parse(xNode.Attributes["Compliant"].Value.ToString());
                    oItem.Detected = bool.Parse(xNode.Attributes["Detected"].Value.ToString());
                    oItem.Type = xNode.Attributes["Type"].Value.ToString();
                    oItem.Version = xNode.Attributes["Version"].Value.ToString();

                    oItem.CIName = xNode.SelectSingleNode("./CIProperties/LocalizableText[@PropertyName='CIName']").InnerText;
                    oItem.CIDescription = xNode.SelectSingleNode("./CIProperties/LocalizableText[@PropertyName='CIDescription']").InnerText;

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



    }

}