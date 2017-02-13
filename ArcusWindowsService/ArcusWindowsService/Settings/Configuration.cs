using System;
using System.IO;
using System.Xml.Serialization;

namespace BEArcus.Agent
{
    public partial class Configuration
    {
        public static string CONFIGURATION_XML_PATH = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Settings\Configuration.xml");

        #region Instance

        private static Configuration instance;
        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    try
                    {
                        instance = new Configuration();
                        using (StreamReader streamReader = new StreamReader(CONFIGURATION_XML_PATH))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(instance.GetType());
                            instance = xmlSerializer.Deserialize(streamReader) as Configuration;
                        }
                    }
                    catch
                    {
                        instance = null;
                    }
                }
                return instance;
            }
        }

        #endregion
    }
}


