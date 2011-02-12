//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Management;

namespace smsclictr.automation
{
    /// <summary>
    /// Manage SMS Components
    /// </summary>
    public class SMSComponents
    {
        private WMIProvider oWMIProvider;

        private ManagementObject oCCM_SoftwareDistributionClientConfig;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oProvider"></param>
        public SMSComponents(WMIProvider oProvider)
        {
            oWMIProvider = oProvider;
        }

        /// <summary>
        /// The cached CCM_SoftwareDistributionClientConfig Class.
        /// </summary>
        /// <returns>root\ccm\policy\machine\requestedconfig:CCM_SoftwareDistributionClientConfig</returns>
        /// <seealso cref="M:smsclictr.automation.SMSComponents.CCM_SoftwareDistributionClientConfig(System.Boolean)"/>
        public ManagementObject CCM_SoftwareDistributionClientConfig()
        {
            if (oCCM_SoftwareDistributionClientConfig == null)
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";
                ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM CCM_SoftwareDistributionClientConfig");
                foreach (ManagementObject MO in MOC)
                {
                    oCCM_SoftwareDistributionClientConfig = MO;
                    return MO;
                }
                return null;
            }
            else
            {
                return oCCM_SoftwareDistributionClientConfig;
            }

        }

        /// <summary>
        /// The CCM_SoftwareDistributionClientConfig Class
        /// </summary>
        /// <param name="refresh">Refresh the cached CCM_SoftwareDistributionClientConfig Object</param>
        /// <returns>root\ccm\policy\machine\requestedconfig:CCM_SoftwareDistributionClientConfig</returns>
        public ManagementObject CCM_SoftwareDistributionClientConfig(bool refresh)
        {
            if (refresh)
            {
                oCCM_SoftwareDistributionClientConfig = null;
            }
            return CCM_SoftwareDistributionClientConfig();
        }

        /// <summary>
        /// Get the status or disable SoftwareDistribution
        /// </summary>
        public bool DisableSoftwareDistribution
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SoftwareDistributionClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;

            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM ccm_SoftwareDistributionClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("ccm_SoftwareDistributionClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsSoftwareDistribution");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("LockSettings", "true");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");

            }
        }

        /// <summary>
        /// Get the status or disable SoftwareMeetering
        /// </summary>
        public bool DisableSoftwareMeetering
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SoftwareMeteringClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_SoftwareMeteringClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_SoftwareMeteringClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsSoftwareMetering");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// Get the status or disable MSISourceUpdate
        /// </summary>
        public bool DisableMSISourceUpdate
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SourceUpdateClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_SourceUpdateClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_SourceUpdateClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsSourceUpdateAgent");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// Get the status or disable Inventory
        /// </summary>
        public bool DisableInventory
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_InventoryClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_InventoryClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_InventoryClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsInventory");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// Get the status or disable remote tools
        /// </summary>
        public bool DisableRemoteTools
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_RemoteToolsConfig.Type=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_RemoteToolsConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_RemoteToolsConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsRemoteTools");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("Type", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// SMSSystemHealthAgent status
        /// </summary>
        public bool DisableHealthAgent
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SystemHealthClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_SystemHealthClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_SystemHealthClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SMSSystemHealthAgent");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// SoftwareUpdatesClientConfig Agent status
        /// </summary>
        public bool DisableSoftwareUpdate
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SoftwareUpdatesClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_SoftwareUpdatesClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_SoftwareUpdatesClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsSoftwareUpdate");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// ConfigurationManagementClientConfig Agent status
        /// </summary>
        public bool DisableConfigurationManagement
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_ConfigurationManagementClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;
            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_ConfigurationManagementClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_ConfigurationManagementClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SMSConfigurationManagementAgent");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// ClientAgentConfig Status
        /// </summary>
        public bool ClientAgentConfig
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_ClientAgentConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;

            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_ClientAgentConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_ClientAgentConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "ClientAgent");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// Disable OutOfBand
        /// </summary>
        public bool DisableOOB
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_OutOfBandManagementClientConfig.SiteSettingsKey=1");
                if (!bool.Parse(MO.GetPropertyValue("Enabled").ToString()))
                {
                    return true;
                }
                return false;

            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\policy\machine\requestedconfig";

                //Cleanup old "local" policies...
                oProv.DeleteQueryResults("SELECT * FROM CCM_OutOfBandManagementClientConfig WHERE PolicySource = 'local'");

                if (value)
                {
                    //Create a local policy
                    ManagementClass MC = oProv.GetClass("CCM_OutOfBandManagementClientConfig");
                    ManagementObject MO = MC.CreateInstance();
                    MO.SetPropertyValue("ComponentName", "SmsOutOfBandManagement");
                    //MO.SetPropertyValue("PolicyID", "{local}");
                    MO.SetPropertyValue("Enabled", "false");
                    MO.SetPropertyValue("PolicySource", "local");
                    MO.SetPropertyValue("PolicyVersion", "1.0");
                    MO.SetPropertyValue("SiteSettingsKey", "1");

                    //Save the new Instance
                    MO.Put();
                }

                //Evaluate Machine Policy
                oProv.mScope.Path.NamespacePath = @"ROOT\CCM";
                oProv.ExecuteMethod("SMS_Client", "EvaluateMachinePolicy");
            }
        }

        /// <summary>
        /// return all instances of ROOT\ccm:CCM_InstalledComponent
        /// </summary>
        public ManagementObjectCollection CCM_InstalledComponent
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\ccm";
                return oProv.ExecuteQuery("SELECT * FROM CCM_InstalledComponent");
            }
        }

        /// <summary>
        /// CCM_ComponentClientConfig from actual policy
        /// </summary>
        /// <param name="ComponentName"></param>
        /// <returns>ROOT\ccm\Policy\Machine\ActualConfig:CCM_ComponentClientConfig</returns>
        public ManagementObject Component_Actual(string ComponentName)
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\ccm\Policy\Machine\ActualConfig";
            ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM CCM_ComponentClientConfig WHERE ComponentName = '" + ComponentName + "'");
            foreach (ManagementObject MO in MOC)
            {
                return MO;
            }
            return null;
        }

        /// <summary>
        /// local CCM_ComponentClientConfig policy from requested policy
        /// </summary>
        /// <param name="ComponentName"></param>
        /// <returns>ROOT\ccm\Policy\Machine\RequestedConfig:CCM_ComponentClientConfig</returns>
        public ManagementObjectCollection Component_Requested(string ComponentName)
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\ccm\Policy\Machine\RequestedConfig";
            return oProv.ExecuteQuery("SELECT * FROM CCM_ComponentClientConfig WHERE ComponentName = '" + ComponentName + "'");
        }

    }
}
