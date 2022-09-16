// //File: FileLogger.cs
// //
// //  Copyright (c) 2018 ERC LLC
// //
// //Author: woo
// //
// // Since: 2022-9-15
// //
using System;
using System.IO;
using TestMapX;
namespace TestMapX
{
    class FileLogger : ILogger
    {
        private string mFileName;
        private StreamWriter mLogFile;
        public string FileName
        {
            get
            {
                return mFileName;
            }
        }
        public FileLogger(string fileName)
        {
            mFileName = fileName;
        }
        public void Init()
        {
            mLogFile = new StreamWriter(mFileName);
        }
        public void Terminate(string message)
        {
            Write(1, message);
            mLogFile.Close();
        }
        public void Write(int msgLevel, string logMessage)
        {
            // FileLogger implements the ProcessLogMessage method by
            // writing the incoming message to a file.
            mLogFile.WriteLine(logMessage);
        }
    }

}
