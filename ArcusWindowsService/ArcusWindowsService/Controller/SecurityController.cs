/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using System.IO;
using Logging;

namespace BEArcus.Agent
{
    class SecurityController
    {
        static readonly byte[] entropy = { 1, 2, 3, 4, 5, 6 }; //the entropy
        ILog Log = Logger.Instance;

        /// <summary>
        /// Encrypts the text using Current user.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Encrypt(string text)
        {
            try
            {
                LogUtility.LogInfoFunction("Entered Encrypt.");
                // first, convert the text to byte array 
                byte[] originalText = Encoding.Unicode.GetBytes(text);

                // then use Protect() to encrypt your data 
                byte[] encryptedText = ProtectedData.Protect(originalText, entropy, DataProtectionScope.CurrentUser);

                LogUtility.LogInfoFunctionFinished();
                //and return the encrypted message 
                return Convert.ToBase64String(encryptedText);
            }
            catch (Exception)
            {
                LogUtility.LogInfoFunction("Data was not Encrypted.");
                return null;
            }
        }

        /// <summary>
        /// Decrypts the text using Current User.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Decrypt(string text)
        {
            try
            {
                LogUtility.LogInfoFunction("Entered Decrypt.");
                // the encrypted text, converted to byte array 
                byte[] encryptedText = Convert.FromBase64String(text);

                // calling Unprotect() that returns the original text 
                byte[] originalText = ProtectedData.Unprotect(encryptedText, entropy, DataProtectionScope.CurrentUser);

                LogUtility.LogInfoFunctionFinished();

                // finally, returning the result 
                return Encoding.Unicode.GetString(originalText);
            }
            catch (Exception e)
            {
                LogUtility.LogInfoFunction("Data was not decrypted.");
                LogUtility.LogInfoFunction(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Updates the configuration.xml
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="authorizationKey"></param>
        public static void UpdateConfiguration(string endpointUrl, string authorizationKey)
        {
            try
            {
                LogUtility.LogInfoFunction("Entered UpdateConfiguration.");
                XmlDocument xmlDoc = new XmlDocument();
                string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                //xmlDoc.Load(path + @"\Settings\Configuration.xml");            
                xmlDoc.Load(@"Settings\Configuration.xml");
                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("ns", "http://schemas.arcus.com/configuration");
                XmlNode documentDBCredentials = xmlDoc.SelectSingleNode("//ns:DataStores/ns:DocumentDB", nsmgr);

                LogUtility.LogInfoFunction("Updating the Endpoint Url and Authorization Key in Configuration.xml");
                documentDBCredentials.Attributes["EndPointUrl"].Value = endpointUrl;
                documentDBCredentials.Attributes["AuthorizationKey"].Value = authorizationKey;

                //xmlDoc.Save(path + @"\Settings\Configuration.xml");  
                xmlDoc.Save(@"Settings\Configuration.xml");

                LogUtility.LogInfoFunctionFinished();
            }
            catch (Exception)
            {
                LogUtility.LogInfoFunction("Configuration.xml is not updated.");
            }
        }
    }
}
