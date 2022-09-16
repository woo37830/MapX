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
    class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
        }
        public void Init()
        {
        }
        public void Terminate(String message)
        {
            Write(1, message);
        }
        public void Write(int msgLevel, string logMessage)
        {
            Console.WriteLine(logMessage);
        }
    }

}
