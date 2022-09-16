using System;
using System.IO;
using System.Collections.Generic;
using TestMapX;


namespace TestMapX
{
    public class ConfigurationBuilder
    {
        private static Configuration configuration;


        public static Configuration CreateDefault(string[] args)
        {
            if (configuration != null)
            {
                Console.WriteLine("Cannot create another instance of Configuraton");
                return configuration;
            }
            string configFile = "/Users/woo/Development/workspaces/TestMapX/TestMapX/myconfig.cfg";
            string noteStr = "";
            string interactiveFlag = "NO";
            if (args != null)
            {
                if (args.Length > 0)
                {
                    configFile = args[0]; // location of configFile
                }
                if (args.Length > 1)
                {
                    noteStr = args[1];
                }
                if (args.Length == 2)
                {
                    interactiveFlag = args[2];
                }
            }
            Configuration config = new ConfigurationBuilder(configFile).Build(noteStr, interactiveFlag);
            string dataSource = config.get("dataSource");
            string dataUser = config.get("dataUser");
            string dataPassword = config.get("dataPassword");
            string connString = "Data Source=" + dataSource + "; user id=" + dataUser + "; password=" + dataPassword + ";";
            config.add("connString", connString);
            Logger Logger = Logger.Instance;
            String formattedString = String.Format("\n\t-------------Configuration---------\n{0}\n------------------------\n", config);
            Console.WriteLine(formattedString);
            configuration = config;
            return config;
        }
        public ConfigurationBuilder(string fileName)
        {
            if (fileName.EndsWith(".xml"))
                this.AddXMLFile(fileName);
            else if (fileName.EndsWith(".cfg"))
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
            if (configuration != null)
            {
                Console.WriteLine("Cannot create another instance of Configuraton");
                return;
            }
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
            configuration = config;
        }

        public Configuration Build(string noteStr = "", string interactiveFlag = "NO")
        {
            configuration.add("Note", noteStr);
            configuration.add("INTERACTIVE", interactiveFlag);
            configuration.update();
            return configuration;
        }
    }
}
