//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections;
using System.Management;

namespace smsclictr.automation
{
    /// <summary>
    /// FileIO Methods
    /// </summary>
    public class WMIFileIO
    {
        private WMIProvider oWMIProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oProvider"></param>
        public WMIFileIO(WMIProvider oProvider)
        {
            oWMIProvider = oProvider;
        }

        /// <summary>
        /// Delete a single File
        /// </summary>
        /// <param name="FilePath"></param>
        public void DeleteFile(string FilePath)
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"root\cimv2";
            ManagementObject MO = oProv.GetObject("CIM_DataFile.Name='" + FilePath + "'");
            MO.InvokeMethod("Delete", null);
        }

        /// <summary>
        /// Get an ArrayList of all Subfolders
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public ArrayList SubFolders(string Path)
        {
            lock (oWMIProvider)
            {
                ArrayList result = new ArrayList();
                try
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    ManagementObjectCollection MOC = oProv.ExecuteQuery(@"Associators of {Win32_Directory.Name='" + Path + @"'} where AssocClass=Win32_Subdirectory ResultRole=PartComponent");

                    foreach (ManagementObject MO in MOC)
                    {
                        try
                        {
                            result.Add(MO.GetPropertyValue("Name").ToString().ToLower());
                        }
                        catch { }
                    }
                    return result;
                }
                catch { }

                return result;

            }

        }

        /// <summary>
        /// Delete a Folder with all Subfolders and Files
        /// </summary>
        /// <param name="Path"></param>
        public void DeleteFolder(string Path)
        {
            if (!string.IsNullOrEmpty(Path))
            {
                try
                {
                    ManagementObjectCollection MOC;

                    //Delete all Subfolders
                    foreach (string sSub in SubFolders(Path))
                    {
                        DeleteFolder(sSub);
                    }
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"root\cimv2";
                    string FolderPath = Path.Replace(@"\", @"\\");
                    //ManagementObjectCollection MOC = oWMIProvider.ExecuteQuery("SELECT * FROM Win32_Directory WHERE Drive like '" + (FolderPath.Split('\\'))[0] + "' and Path like '" + (CachePath.Split(':'))[1] + @"\\' and FileType = 'File Folder'");
                    MOC = oProv.ExecuteQuery("SELECT * FROM Win32_Directory WHERE Name = '" + FolderPath + "'");

                    //Delete the root Folder
                    foreach (ManagementObject MO in MOC)
                    {
                        try
                        {
                            ManagementBaseObject inParams = MO.GetMethodParameters("DeleteEx");
                            ManagementBaseObject result = MO.InvokeMethod("DeleteEx", inParams, null);
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Check if Directory Exists
        /// </summary>
        /// <param name="Path"></param>
        /// <returns>True = Directory exists</returns>
        public Boolean DirExist(string Path)
        {
            if (!string.IsNullOrEmpty(Path))
            {
                try
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    string FolderPath = Path.Replace(@"\", @"\\");
                    ManagementObjectCollection MOC = oProv.ExecuteQuery("SELECT * FROM Win32_Directory WHERE Name = '" + FolderPath + "'");
                    foreach (ManagementObject MO in MOC)
                    {
                        return true;
                    }
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Delete multiple Files
        /// </summary>
        /// <param name="Drive">Disk Drive like 'c:'</param>
        /// <param name="Path">Path like '\\windows\\'</param>
        /// <param name="Filename">Filename like 'kb%'</param>
        /// <param name="Extension">Extension like 'log'</param>
        public void DeleteFiles(string Drive, string Path, string Filename, string Extension)
        {
            try
            {
                ManagementObjectCollection MOC;

                if (!Path.EndsWith(@"\"))
                    Path = Path + @"\";

                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"root\cimv2";
                string FolderPath = Path.Replace(@"\", @"\\");
                //ManagementObjectCollection MOC = oWMIProvider.ExecuteQuery("SELECT * FROM Win32_Directory WHERE Drive like '" + (FolderPath.Split('\\'))[0] + "' and Path like '" + (CachePath.Split(':'))[1] + @"\\' and FileType = 'File Folder'");
                MOC = oProv.ExecuteQuery(string.Format("SELECT * FROM CIM_DataFile WHERE Drive = '{0}' and Path = '{1}' and Filename like '{2}' and Extension like '{3}'", new object[] { Drive, FolderPath, Filename, Extension }));

                //Delete the root Folder
                foreach (ManagementObject MO in MOC)
                {
                    try
                    {
                        ManagementBaseObject inParams = MO.GetMethodParameters("Delete");
                        ManagementBaseObject result = MO.InvokeMethod("Delete", inParams, null);
                    }
                    catch { }
                }
            }
            catch { }
        }

    }
}
