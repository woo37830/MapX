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
        string currentUser;
        string currentWorkingDirectory;
        DateTime now = DateTime.Now;
        string authorStr = "";
        string dateStr = "";
        bool altered = false;

        public Configuration(string fileName, string noteStr = "", string interactiveFlag = "NO")
        {
            this.fileName = fileName;
            currentUser = Environment.UserName;
            currentWorkingDirectory = Environment.CurrentDirectory;
            nameValues = new Dictionary<string, string>();
            this.add("Configfile", this.fileName);
            this.add("Working Directory", this.currentWorkingDirectory);
            this.add("Note", noteStr);
            this.add("Author", this.currentUser);
            this.add("Date", now.ToString());
            this.authorStr = "\n\t\t\tAuthor: " + nameValues["Author"];
            this.dateStr = nameValues["Date"].ToString();
            this.add("INTERACTIVE", interactiveFlag);

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
            if (this.nameValues["INTERACTIVE"].ToUpper().Equals("NO"))
            {
                return;
            }
            String line = "";
            Console.WriteLine("config currently has {0}", this);
            bool altered = false;
            this.add("Config file", this.fileName);
            this.add("Working Directory", this.currentWorkingDirectory);
            this.add("Note", "");
            while (true)
            {
                Console.Write("\nEnter name=value to edit, '.' to quit editing: ");
                line = Console.ReadLine();
                if (line.StartsWith("."))
                {
                    if (altered)
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
                altered = true;
            }
        }

        public void addComment()
        {
            if (this.nameValues["INTERACTIVE"].ToUpper().Equals("NO"))
            {
                nameValues["Comment"] = "\tNot Interactive, no run comment\n";
                return;
            }
            Console.Write("\nAdd comment, '.' to quit adding: ");
            string comment = "\t\t";
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
                comment += line + "\n\n\t";
            } // End while
        }

        public void summarize()
        {
            string authorStr = "";
            string dateStr = this.now.ToString();
            string commentStr = "";
            if (nameValues["Comment"] != null)
            {
                commentStr = "\n\t\t\tComments: " + nameValues["Comment"];
            }

            authorStr = "\n\t\t\tAuthor: " + nameValues["Author"];
            string noteString = nameValues["Note"];
            Logger.Instance.Write(1, $"\n\t-------------Configuration---------\n{this.ToString()}\n------------------------\n");
            Logger.Instance.Write(1, $"\n\t\t\tSummary of Run\n\t\t\tNote: {noteString} {authorStr} {commentStr}");
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
    }
}
//