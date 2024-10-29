using System;
using System.IO;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Xml;

namespace AzureBlobService
{
    class Config
    {
        private readonly string logFile;
        private readonly string configFile;

        readonly Log log = new Log();

        private readonly string storageAccount;
        private readonly string glabelsPath;
        private readonly string sasKey;
        private readonly string ptouchPath;
        private readonly string templatePath;

        public string StorageAccount => storageAccount;
        public string SasKey => sasKey;
        public string GlabelsPath => glabelsPath;
        public string PtouchPath => ptouchPath;

        public string TemplatePath => templatePath;

        public Config()
        {
            string baseDir = Environment.CurrentDirectory;
            baseDir += @"\";
            logFile = baseDir + @"log.txt";
            configFile = baseDir + @"config.xml";

            XmlDocument xdConfig = new XmlDocument();

            try
            {
                xdConfig.Load(configFile);

                var main = xdConfig.GetElementsByTagName("main");
                foreach (XmlElement child in main[0].ChildNodes) 
                {
                    if (child.Name == "storageaccount")
                        storageAccount = child.InnerText;
                    else if (child.Name == "saskey")
                        sasKey = child.InnerText;
                    else if (child.Name == "templatepath")
                        templatePath = child.InnerText;
                    else if (child.Name == "glabelspath")
                        glabelsPath = child.InnerText;
                    else if (child.Name == "ptouchpath")
                        ptouchPath = child.InnerText;
                }

            }
            catch (Exception e)
            {
                log.Add("ERROR; Configuratiebestand niet aanwezig (" + e.Message + ")");
                return;
            }

        }

        public string GetLogFile()
        {
            return logFile;
        }

        public string GetConfigFile()
        {
            return configFile;
        }
    }
}
