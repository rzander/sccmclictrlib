using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
//using System.Windows.Media.Imaging;
using System.Management;

namespace sccmclictr.automation
{
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
