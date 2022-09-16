// //File: ILogger.cs
// //
// //  Copyright (c) 2018 ERC LLC
// //
// //Author: woo
// //
// // Since: 2022-9-15
// //
using System;
namespace TestMapX
{
    interface ILogger
    {
        void Write(int msgLevel, string logMessage);
        void Terminate(string terminateMessage);
        void Init();
    }
}
