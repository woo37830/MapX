// //File: CurrentCodeRevision.cs
// //
// //  Copyright (c) 2018 ERC LLC
// //
// //Author: woo
// //
// // Since: 2022-9-14
// //
using System;
namespace TestMapX
{
    public class CurrentCodeRevision
    {
        public SystemCommands commandManager;

        public CurrentCodeRevision()
        {
            commandManager = new SystemCommands();
        }

        public string getLastUpdate()
        {
            return commandManager.RunCommand("/usr/local/bin/git", "log -1 --date=format:\"%Y/%m/%d\" --format=\"%ad\" ").Trim();
        }

        public string getRevision()
        {
            return commandManager.RunCommand("/usr/local/bin/git", "rev-parse --short HEAD").Trim();
        }

        public string getBranch()
        {
            return commandManager.RunCommand("/usr/local/bin/git", "rev-parse --abbrev-ref HEAD").Trim();
        }

        public string getCodeStatus()
        {
            string last = getLastUpdate();
            string rev = getRevision();
            string branch = getBranch();
            return "Last Update: " + last + "  Commit: " + rev + "   Branch: " + branch;
        }
    }
}
