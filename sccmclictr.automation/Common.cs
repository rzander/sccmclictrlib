//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2018 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;
using System.Management;

namespace sccmclictr.automation
{
    /// <summary>
    /// Class common.
    /// </summary>
    static public class common
    {
        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="strPlainText"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string Encrypt(string strPlainText, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDES = new TripleDESCryptoServiceProvider();
                
                SHA1CryptoServiceProvider objSHA1 = new SHA1CryptoServiceProvider();
                byte[] bHash = objSHA1.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strKey));

                byte[] bRes = System.Security.Cryptography.ProtectedData.Protect(ASCIIEncoding.ASCII.GetBytes(strPlainText), bHash, DataProtectionScope.CurrentUser);

                return Convert.ToBase64String(bRes);
            }
            catch (System.Exception ex)
            {
                ex.Message.ToString();
            }
            return "";
        }

        /// <summary>
        /// Decrypt a string
        /// </summary>
        /// <param name="strBase64Text"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string Decrypt(string strBase64Text, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDES = new TripleDESCryptoServiceProvider();
                
                SHA1CryptoServiceProvider objSHA1 = new SHA1CryptoServiceProvider();
                byte[] bHash = objSHA1.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strKey));

                byte[] arrBuffer = Convert.FromBase64String(strBase64Text);
                return ASCIIEncoding.ASCII.GetString(System.Security.Cryptography.ProtectedData.Unprotect(arrBuffer, bHash, DataProtectionScope.CurrentUser));
            }
            catch (System.Exception ex)
            {
                ex.Message.ToString();
            }
            return "";

        }

        /// <summary>
        /// Gets the sha1 hash of the supplied value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string GetSha1(string value)
        {
            var data = Encoding.ASCII.GetBytes(value);
            var hashData = new SHA1Managed().ComputeHash(data);

            var hash = string.Empty;

            foreach (var b in hashData)
                hash += b.ToString("X2");

            return hash;
        }


        // Image converter functions found here: http://www.dailycoding.com/Posts/convert_image_to_base64_string_and_base64_string_to_image.aspx

        /// <summary>
        /// Get Image from String
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static Image Base64ToImage(string base64String)
        {

            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        /// <summary>
        /// Convert Image to string
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        /// <summary>
        /// Converts a WMI DateTime string to a C# DateTime object
        /// </summary>
        /// <param name="ManagementDateTime">The WMI DateTime string.</param>
        /// <returns>System.Nullable{DateTime}.</returns>
        public static DateTime? WMIDateToDateTime(string ManagementDateTime)
        {
            try
            {
                if (string.IsNullOrEmpty(ManagementDateTime))
                    return null;
                else
                    return ManagementDateTimeConverter.ToDateTime(ManagementDateTime) as DateTime?;
            }
            catch { }

            return null;
        }


    }
}
