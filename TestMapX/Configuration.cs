namespace TestMapX
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Security.Principal;

    public class Configuration
    {
        Dictionary<string, string> nameValues;
        string fileName;
        string currentUser = Environment.UserName;
        string currentWorkingDirectory = Environment.CurrentDirectory;
        DateTime now = DateTime.Now;
        bool altered = false;

        public Configuration(string fileName)
        {
            this.fileName = fileName;
            nameValues = new Dictionary<string, string>();
        }
        public void add(string text)
        {
            string[] bits = text.Split('=');
            this.add(bits[0].Trim(), bits[1].Trim());
        }
        public void add(string name, string value)
        {
            nameValues[name] = value;
        }

        public string get(string name)
        {
            return nameValues[name];
        }

        public void update()
        {
            String line = "";
            this.add("Configfile", this.fileName);
            this.add("Working Directory", this.currentWorkingDirectory);
            this.add("Note", "");
            Console.WriteLine("config currently has {0}", this);
            this.add("Config file", this.fileName);
            this.add("Working Directory", this.currentWorkingDirectory);
            this.add("Note", "");
            Console.Write("\nEnter name=value to edit, '.' to quit editing: ");
            while (true)
            {
                line = Console.ReadLine();
                if (line.StartsWith("."))
                {
                    if (this.altered)
                    {
                        this.add("Date", now.ToString());
                        this.add("Author", currentUser);

                    }
                    Console.WriteLine("config is now {0}", this);
                    return;
                }
                string[] bits = line.Split('=');

                Console.WriteLine("The variable {0}'s value of {1} is replaced by {2}", bits[0], this.get(bits[0]), bits[1]);
                this.add(line);
                this.altered = true;
            }
        }

        public override string ToString()
        {
            string returnStr = "\n\t";
            foreach (KeyValuePair<string, string> pair in nameValues)
            {
                returnStr += pair.Key + ": " + pair.Value + "\n\t";
            }
            return returnStr;
        }

        public void addComment()
        {
            Console.Write("\nAdd comment, '.' to quit adding: ");
            string comment = "\t";
            string line;
            while (true)
            {
                line = Console.ReadLine();
                if (line.StartsWith("."))
                {
                    if (line.Length > 0)
                    {
                        nameValues["Comment"] = comment + "\n";
                    }
                    break;
                }
                comment += line + "\n\t";
            } // End while
        }

        public void summarize()
        {
            string authorStr = "";
            string dateStr = this.now.ToString();
            string commentStr = "";
            if (nameValues["Comment"] != null)
            {
                commentStr = "\n\t\tComments: " + nameValues["Comment"];
            }

            authorStr = "\n\t\t\tAuthor: " + nameValues["Author"];
            dateStr = nameValues["Date"].ToString();

            Console.WriteLine("\n\t\t\tSummary of Run\n\t\t\tDate: {0}\n\t\tNote: {1} {2} {3}", dateStr, nameValues["Note"], authorStr, commentStr);
        }
    }
}
//