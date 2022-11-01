using System;
using System.Collections.Generic;
using System.IO;

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
            string strExeFilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string configFile = strExeFilePath + "\\config\\myconfig.cfg";
            Dictionary<string, string> argValues = ParseCommandLine(args);
            configuration = new ConfigurationBuilder(configFile).Build(argValues["Note"], argValues["INTERACTIVE"]);
            AddCommandLine(argValues);
            return configuration;
        }
        public Configuration getConfiguration()
        {
            return configuration;
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
                    string key = bits[0].Trim();
                    string dataAndComment = text.Substring(bits[0].Length + 1).Trim();
                    bits = dataAndComment.Split('#');
                    string data = bits[0].Trim();
                    //string data = text.Substring(bits[0].Length + 1).Trim();
                    config.add(key, data);
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

        /*
         * ParseCommandLine will use a name / value pair to set parameters in the config file.
         * Thus, -processArea="  ... . a sql string ...."  will become "processArea"="the sql string" in the config object
         * They are preceeded by a '-' to distinguish them from positional parameters?
         */
        public static Dictionary<string, string> ParseCommandLine(string[] args)
        {
            Dictionary<int, string> positionalItems = new Dictionary<int, string>();
            positionalItems.Add(0, "ConfigFile");
            positionalItems.Add(1, "Note");
            positionalItems.Add(2, "INTERACTIVE");


            Dictionary<string, string> returnValues = new Dictionary<string, string>();
            returnValues["INTERACTIVE"] = "NO";
            returnValues["Note"] = "";
            if (args != null)
            {
                int k = 0;
                foreach (string arg in args)
                {
                    if (arg.StartsWith("-"))
                    { // This is a named argument
                        // Remove the leading '-' and store the name/value in the returnValues Dictionary
                        int equalsIndex = arg.IndexOf('=');
                        string name = arg.Substring(1, equalsIndex - 1);
                        returnValues.Add(name, arg.Substring(equalsIndex + 1));
                    }
                    else
                    { // This is a positional argument, not counting the named ones
                        if (k > positionalItems.Count)
                        {
                            throw new Exception("Too many positional arguments on command.  Must be " + positionalItems.Count + " or less!");
                        }
                        returnValues[positionalItems[k]] = args[k];
                        ++k;
                    }
                }
            }
            return returnValues;
        }
        public static void AddCommandLine(Dictionary<string, string> args)
        {
            foreach (string arg in args.Keys)
            {
                configuration.add(arg, args[arg]);
            }
            configuration.update();
        }
    }
}

