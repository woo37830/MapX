using System;
using System.IO;
using System.Collections.Generic;
using TestMapX;

namespace TestMapX
{
    public class ConfigurationBuilder
    {
        private Configuration configuration;

        public ConfigurationBuilder(string fileName)
        {
            if (fileName.EndsWith(".xml"))
                this.AddXMLFile(fileName);
            else if (fileName.EndsWith(".txt"))
                this.AddTextFile(fileName);
            else if (fileName.EndsWith(".json"))
                this.AddJsonFile(fileName);
            else
                throw new NotImplementedException();
        }

        public void AddJsonFile(string jsonFile)
        {
            throw new NotImplementedException();
        }

        public void AddXMLFile(string xmlFile)
        {
            throw new NotImplementedException();
        }

        public void AddTextFile(string textFile)
        {
            Configuration config = new Configuration(textFile);
            StreamReader dataStream = new StreamReader(textFile);
            string text = "";
            while ((text = dataStream.ReadLine()) != null)
            {
                if (!text.StartsWith("#"))
                {
                    string[] bits = text.Split('=');
                    config.add(bits[0].Trim(), bits[1].Trim());
                }
            }
            this.configuration = config;
        }

        public Configuration Build(string noteStr = "", string interactiveFlag = "NO")
        {
            configuration.add("Note", noteStr);
            configuration.add("INTERACTIVE", interactiveFlag);
            configuration.update();
            return this.configuration;
        }
    }
}
