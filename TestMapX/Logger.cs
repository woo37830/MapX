// //File: Logger.cs
// //
// //  Copyright (c) 2018 ERC LLC
// //
// //Author: woo
// //
// // Since: 2022-9-15
// //
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace TestMapX
{
    class Logger
    {
        private static object mLock = new object();
        private static Logger mLogger = null;
        private static List<ILogger> mObservers;
        private static int LoggerStatus = 1;
        private static string loggerFormat = "{0} - {1}";
        public static int loggerLevel = 1; // Logger will log all messages equal to or lower than this level

        public static Configuration config;

        private static string solutionName = "Test";
        private static string outputType = "CONSOLE";
        private static string message = "Starting " + solutionName + " at " +
                                 DateTime.Now.ToString();

        public static void Configure(Configuration configuration)
        {
            if (config != null)
            {
                Console.WriteLine("Cannot reconfigure the logger");
            }
            config = configuration;
        }

        public static Configuration getConfiguration()
        {
            return config;
        }
        // the public Instance property everyone uses to access the Logger
        public static Logger Instance
        {
            get
            {
                if (config != null)
                {
                    solutionName = config.get("SOLUTION_NAME");
                    outputType = config.get("OUTPUT_TYPE");
                    message = "Starting " + solutionName + " at " +
                                 DateTime.Now.ToString();
                }
                // If this is the first time we're referring to the
                // singleton object, the private variable will be null.
                if (mLogger == null)
                {
                    // for thread safety, lock an object when
                    // instantiating the new Logger object. This prevents
                    // other threads from performing the same block at the
                    // same time.
                    lock (mLock)
                    {
                        // Two or more threads might have found a null
                        // mLogger and are therefore trying to create a 
                        // new one. One thread will get to lock first, and
                        // the other one will wait until mLock is released.
                        // Once the second thread can get through, mLogger
                        // will have already been instantiated by the first
                        // thread so test the variable again. 
                        if (mLogger == null)
                        {
                            mLogger = new Logger(solutionName,
                                 LoggerStatus,
                                 outputType,
                                 true,
                                 message);
                        }
                    }
                }
                return mLogger;
            }
        }
        // the constructor. usually public, this time it is private to ensure 
        // no one except this class can use it.
        private Logger(string _solutionName, int _status, string _outputType, bool val, string _message)
        {
            solutionName = _solutionName;
            outputType = _outputType;
            LoggerStatus = _status;
            message = _message;
            mLock = new object();
            mObservers = new List<ILogger>();
        }
        public void RegisterObserver(ILogger observer)
        {
            if (!mObservers.Contains(observer))
            {
                mObservers.Add(observer);
                observer.Write(1, message);
            }
        }

        public void Write(int level, string message)
        {
            if (level <= loggerLevel)
                for (int i = 0; i < mObservers.Count; i++)
                {
                    ILogger observer = mObservers[i];
                    observer.Write(level, String.Format(loggerFormat, DateTime.Now.ToString(),
                        message));
                }
        }

        public void Terminate(string message)
        {
            for (int i = 0; i < mObservers.Count; i++)
            {
                ILogger observer = mObservers[i];
                observer.Write(1, message);
            }

        }

    }
    public class ExampleWorkerClass
    {
        // any class wanting to use the Logger just has to create a 
        // Logger object by pointing it to the one and only Logger
        // instance.
        private Logger mLogger = Logger.Instance;
    }
}
