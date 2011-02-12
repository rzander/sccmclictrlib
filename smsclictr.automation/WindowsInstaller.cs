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
    /// WindowsInstaller Functions and Properties 
    /// </summary>
    public class WindowsInstaller
    {
        #region Internal

        private WMIProvider oWMIProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// WindowsInstaller Constructor
        /// </summary>
        /// <param name="oProvider"></param>
        public WindowsInstaller(WMIProvider oProvider)
        {
            oWMIProvider = oProvider;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Allow Windows Installer Source Update to use remote SMS Distribution Points
        /// </summary>
        public bool EnableRemoteDPs
        {
            get
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SourceUpdateClientConfig.SiteSettingsKey=1");
                return Boolean.Parse(MO.GetPropertyValue("RemoteDPs").ToString());
            }
            set
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\ccm\Policy\Machine\ActualConfig";
                ManagementObject MO = oProv.GetObject("CCM_SourceUpdateClientConfig.SiteSettingsKey=1");
                MO.SetPropertyValue("RemoteDPs", value);
                MO.Put();
            }
        }

        /// <summary>
        /// Uninstall a Windows Installer Package based on the Packgae Name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>MSI Exit Code or 99 if Product was not found</returns>
        public UInt32 UninstallMSI_Name(string Name)
        {
            ManagementObjectCollection CliAgents;
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\CIMV2";
            CliAgents = oProv.ExecuteQuery("SELECT * FROM Win32_Product WHERE Name ='" + Name + "'");

            foreach (ManagementObject CliAgent in CliAgents)
            {
                ManagementBaseObject inParams = CliAgent.GetMethodParameters("Uninstall");
                ManagementBaseObject result = CliAgent.InvokeMethod("Uninstall", inParams, null);
                return UInt32.Parse(result.GetPropertyValue("ReturnValue").ToString());
            }
            return 99;
        }

        /// <summary>
        /// Uninstall a Windows Installer Package based on the MSI-ID
        /// </summary>
        /// <param name="MSIID"></param>
        /// <returns>MSI Exit Code or 99 if MSIID was not found</returns>
        public UInt32 UninstallMSI_ID(string MSIID)
        {
            ManagementObjectCollection CliAgents;
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            CliAgents = oProv.ExecuteQuery("SELECT * FROM Win32_Product WHERE IdentifyingNumber ='" + MSIID + "'");

            foreach (ManagementObject CliAgent in CliAgents)
            {
                ManagementBaseObject inParams = CliAgent.GetMethodParameters("Uninstall");
                ManagementBaseObject result = CliAgent.InvokeMethod("Uninstall", inParams, null);
                return UInt32.Parse(result.GetPropertyValue("ReturnValue").ToString());
            }
            return 99;
        }

        /// <summary>
        /// Install a Windows Installer Package
        /// </summary>
        /// <param name="sPath"></param>
        /// <param name="sOptions"></param>
        /// <returns>MSI Exit Code</returns>
        public UInt32 InstallMSI(string sPath, string sOptions)
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            ManagementClass MC = oProv.GetClass("Win32_Product");
            ManagementBaseObject inParams = MC.GetMethodParameters("Install");
            inParams.SetPropertyValue("PackageLocation", sPath);
            inParams.SetPropertyValue("Options", sOptions);
            ManagementBaseObject Result = MC.InvokeMethod("Install", inParams, null);
            return (UInt32)Result.GetPropertyValue("ReturnValue");
        }

        /// <summary>
        /// MSI Reinstall Modes
        /// </summary>
        public enum reinstallMode : int
        {
            /// <summary>
            /// Reinstall only if the file is missing.
            /// </summary>
            FileMissing = 1,
            /// <summary>
            /// Reinstall if the file is missing or is an older version.
            /// </summary>
            FileOlderVersion = 2,
            /// <summary>
            /// Reinstall if the file is missing, or is an equal or older version.
            /// </summary>
            FileEqualVersion = 3,
            /// <summary>
            /// Reinstall if the file is missing or a different version is present.
            /// </summary>
            FileExact = 4,
            /// <summary>
            /// Verify the checksum values, and reinstall the file if they are missing or corrupt. This flag only repairs files that have msidbFileAttributesChecksum in the Attributes column of the File Table.
            /// </summary>
            FileVerify = 5,
            /// <summary>
            /// Force all files to be reinstalled, regardless of checksum or version
            /// </summary>
            FileReplace = 6,
            /// <summary>
            /// Rewrite all required registry entries from the Registry Table that go to the HKEY_CURRENT_USER or HKEY_USERS registry hive.
            /// </summary>
            UserData = 7,
            /// <summary>
            /// Rewrite all required registry entries from the Registry Table that go to the 
            /// HKEY_LOCAL_MACHINE or HKEY_CLASSES_ROOT registry hive. 
            /// Rewrite all information from the Class Table, Verb Table, PublishComponent Table, ProgID Table, MIME Table, Icon Table, Extension Table, and AppID Table regardless of machine or user assignment. 
            /// Reinstall all qualified components. When reinstalling an application, this option runs the RegisterTypeLibraries and InstallODBC actions.
            /// </summary>
            MachineData = 8,
            /// <summary>
            /// Reinstall all shortcuts and re-cache all icons overwriting any existing shortcuts and icons.
            /// </summary>
            Shortcut = 9,
            /// <summary>
            /// Use to run from the source package and re-cache the local package. Do not use this option for the first installation of an application or feature.
            /// </summary>
            Package = 10,
            /// <summary>
            /// None
            /// </summary>
            None = 0
        }

        /// <summary>
        /// Reinstall/Repair a Windows Installer Package based on the Packagename
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ReinstallMode"></param>
        /// <returns>MSI Exit Code or 99 if the PRoduct was not found</returns>
        public UInt32 ReinstallMSI_Name(string Name, reinstallMode ReinstallMode)
        {
            ManagementObjectCollection CliAgents;
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            CliAgents = oProv.ExecuteQuery("SELECT * FROM Win32_Product WHERE Name ='" + Name + "'");
            foreach (ManagementObject CliAgent in CliAgents)
            {
                ManagementBaseObject inParams = CliAgent.GetMethodParameters("Reinstall");
                inParams.SetPropertyValue("ReinstallMode", (UInt32)ReinstallMode);
                ManagementBaseObject result = CliAgent.InvokeMethod("Reinstalll", inParams, null);
                return UInt32.Parse(result.GetPropertyValue("ReturnValue").ToString());
            }
            return 99;
        }

        /// <summary>
        /// Reinstall/Repair a Windows Installer Package based on the MSI-ID
        /// </summary>
        /// <param name="MSIID"></param>
        /// <param name="ReinstallMode"></param>
        /// <returns>MSI Exit Code or 99 if MSIID was not found</returns>
        public UInt32 ReinstallMSI_ID(string MSIID, reinstallMode ReinstallMode)
        {
            ManagementObjectCollection CliAgents;
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            CliAgents = oProv.ExecuteQuery("SELECT * FROM Win32_Product WHERE IdentifyingNumber ='" + MSIID + "'");

            foreach (ManagementObject CliAgent in CliAgents)
            {
                ManagementBaseObject inParams = CliAgent.GetMethodParameters("Reinstall");
                inParams.SetPropertyValue("ReinstallMode", (UInt32)ReinstallMode);
                ManagementBaseObject result = CliAgent.InvokeMethod("Reinstalll", inParams, null);
                return UInt32.Parse(result.GetPropertyValue("ReturnValue").ToString());
            }
            return 99;
        }

        #endregion
    }
}
