using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
namespace Lab2_Purnyn
{
    /// <summary>
    /// PortSet that accepts items of int, string, double
    /// </summary>
    public class CcrConsolePort : PortSet<int, string, double>
    {
    }
    /// <summary>
    /// Simple example of a CCR component that uses a PortSet to abstract
    /// its API for message passing
    /// </summary>
    public class CcrConsoleService
    {
        CcrConsolePort _mainPort;
        DispatcherQueue _taskQueue;

        /// <summary>
        /// Creates an instance of the service class, returning only a PortSet
        /// instance for communication with the service
        /// </summary>
        /// <param name="taskQueue"></param>
        /// <returns></returns>
        public static CcrConsolePort Create(DispatcherQueue taskQueue)
        {
            var console = new CcrConsoleService(taskQueue);
            console.Initialize();
            return console._mainPort;
        }

        /// <summary>
        /// Initialization constructor
        /// </summary>
        /// <param name="taskQueue">DispatcherQueue instance used for scheduling</param>
        private CcrConsoleService(DispatcherQueue taskQueue)
        {
            // create PortSet instance used by external callers to post items
            _mainPort = new CcrConsolePort();
            // cache dispatcher queue used to schedule tasks
            _taskQueue = taskQueue;
        }

        private void Initialize()
        {
            // Activate three persisted receivers (single item arbiters)
            // that will run concurrently to each other,
            // one for each item/message type
            Arbiter.Activate(_taskQueue,
                Arbiter.Receive<int>(true, _mainPort, IntWriteLineHandler),
                Arbiter.Receive<string>(true, _mainPort, StringWriteLineHandler),
                Arbiter.Receive<double>(true, _mainPort, DoubleWriteLineHandler)
            );
        }

        void IntWriteLineHandler(int item)
        {
            Console.WriteLine("Received integer:" + item);
        }
        void StringWriteLineHandler(string item)
        {
            Console.WriteLine("Received string:" + item);
        }
        void DoubleWriteLineHandler(double item)
        {
            Console.WriteLine("Received double:" + item);
        }
    }

}
