using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

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
                MD5CryptoServiceProvider objMD5 = new MD5CryptoServiceProvider();
                objDES.Key = objMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strKey));
                objDES.Mode = CipherMode.ECB;
                ICryptoTransform objDESEncrypt = objDES.CreateEncryptor();
                byte[] arrBuffer = ASCIIEncoding.ASCII.GetBytes(strPlainText);
                return Convert.ToBase64String(objDESEncrypt.TransformFinalBlock(arrBuffer, 0, arrBuffer.Length));

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
                MD5CryptoServiceProvider objMD5 = new MD5CryptoServiceProvider();
                objDES.Key = objMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strKey));
                objDES.Mode = CipherMode.ECB;
                ICryptoTransform objDESEncrypt = objDES.CreateDecryptor();
                byte[] arrBuffer = Convert.FromBase64String(strBase64Text);
                return ASCIIEncoding.ASCII.GetString(objDESEncrypt.TransformFinalBlock(arrBuffer, 0, arrBuffer.Length));
            }
            catch (System.Exception ex)
            {
                ex.Message.ToString();
            }
            return "";

        }
    }
}
