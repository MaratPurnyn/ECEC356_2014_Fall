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
    [Contract(Contract.Identifier)]
    [DisplayName("Lab2_Purnyn")]
    [Description("Lab2_Purnyn service (no description provided)")]
    class Lab2_PurnynService : DsspServiceBase
    {
        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        Lab2_PurnynState _state = new Lab2_PurnynState();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/Lab2_Purnyn", AllowMultipleInstances = true)]
        Lab2_PurnynOperations _mainPort = new Lab2_PurnynOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// Service constructor
        /// </summary>
        public Lab2_PurnynService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {

            // 
            // Add service specific initialization here
            // 

            base.Start();
            int exampleNum = 3;

            switch (exampleNum)
            {
                case 1:
                    // Create port that accepts instances of System.Int32
                    Port<int> portInt1 = new Port<int>();

                    // Add the number 10 to the port
                    portInt1.Post(10);

                    // Display number of items to the console
                    Console.WriteLine(portInt1.ItemCount);
                    break;
                case 2:
                    // Create port that accepts instances of System.Int32
                    var portInt2 = new Port<int>();

                    // Add the number 10 to the port
                    portInt2.Post(10);

                    // Display number of items to the console
                    Console.WriteLine(portInt2.ItemCount);

                    // retrieve the item using Test
                    int item2;
                    var hasItem2 = portInt2.Test(out item2);
                    if (hasItem2)
                    {
                        Console.WriteLine("Found item in port:" + item2);
                    }
                    portInt2.Post(11);
                    // alternative to using Test is just assignment of port to variable using
                    // implicit operator
                    var nextItem = portInt2;

                    Console.WriteLine("Found item in port:" + nextItem);
                    break;
                case 3:
                    // Create port that accepts instances of System.Int32
                    var portInt3 = new Port<int>();

                    // Add the number 10 to the port
                    portInt3.Post(10);

                    // Display number of items to the console
                    Console.WriteLine(portInt3.ItemCount);

                    // create dispatcher and dispatcher queue for scheduling tasks
                    Dispatcher dispatcher = new Dispatcher();
                    DispatcherQueue taskQueue = new DispatcherQueue("sample queue", dispatcher);

                    // retrieve the item by attaching a one time receiver
                    Arbiter.Activate(
                        taskQueue,
                        portInt3.Receive(delegate(int item3) // anonymous method
                        {
                            // this code executes in parallel with the method that
                            // activated it                        
                            Console.WriteLine("Received item:" + item3);
                        }
                    ));
                    // any code below runs in parallel with delegate

                    break;
                case 4:
                    // Create a PortSet using generic type arguments
                    var genericPortSet4 = new PortSet<int, string, double>();
                    genericPortSet4.Post(10);
                    genericPortSet4.Post("hello");
                    genericPortSet4.Post(3.14159);

                    // Create a runtime PortSet, using the initialization 
                    // constructor to supply an array of types
                    PortSet runtimePortSet4 = new PortSet(
                        typeof(int),
                        typeof(string),
                        typeof(double)
                        );

                    runtimePortSet4.PostUnknownType(10);
                    runtimePortSet4.PostUnknownType("hello");
                    runtimePortSet4.PostUnknownType(3.14159);
                    break;
                case 5:
                    // create dispatcher and dispatcher queue for scheduling tasks
                    Dispatcher dispatcher5 = new Dispatcher();
                    DispatcherQueue taskQueue5 = new DispatcherQueue("sample queue", dispatcher5);
                    CcrConsolePort port5 = CcrConsoleService.Create(taskQueue5);
                    break;
                case 6:
                    Dispatcher dispatcher6 = new Dispatcher();
                    DispatcherQueue taskQueue6 = new DispatcherQueue("sample queue", dispatcher6);
                    CcrConsolePort port6 = CcrConsoleService.Create(taskQueue6);
                    var portSet6 = new PortSet<int, string, double>();
                    // the following statement compiles because of the implicit assignment operators
                    // that "extract" the instance of Port<int> from the PortSet
                    var portInt6 = portSet6;

                    // the implicit assignment operator is used below to "extract" the Port<int>
                    // instance so the int receiver can be registered
                    Arbiter.Activate(taskQueue6,
                        Arbiter.Receive<int>(true, portSet6, item => Console.WriteLine(item))
                    );

                    break;
                case 7:
                    Dispatcher dispatcher7 = new Dispatcher();
                    DispatcherQueue taskQueue7 = new DispatcherQueue("sample queue", dispatcher7);
                    var port7 = new Port<int>();
                    Arbiter.Activate(taskQueue7,
                       Arbiter.Receive(
                           true,
                           port7,
                           item => Console.WriteLine(item)
                        /** older syntax
                         *    delegate(int item){
                         *        Console.WriteLine(item);
                         *    }
                         *    
                         **/

                       )
                    );

                    // post item, so delegate executes
                    port7.Post(5);
                    break;
                case 8:
                    Dispatcher dispatcher8 = new Dispatcher();
                    DispatcherQueue taskQueue8 = new DispatcherQueue("sample queue", dispatcher8);
                    var port8 = new Port<int>();
                    // alternate version that explicitly constructs a Receiver by passing
                    // Arbiter class factory methods
                    var persistedReceiver = new Receiver<int>(
                           true, // persisted
                           port8,
                           null, // no predicate
                           new Task<int>(item => Console.WriteLine(item)) // task to execute
                        );
                    Arbiter.Activate(taskQueue8, persistedReceiver);
                    break;
                case 9:
                    Dispatcher dispatcher9 = new Dispatcher();
                    DispatcherQueue taskQueue9 = new DispatcherQueue("sample queue", dispatcher9);
                    // create a simple service listening on a port
                    ServicePort servicePort9 = SimpleService.Create(taskQueue9);

                    // create request
                    GetState get = new GetState();

                    // post request
                    servicePort9.Post(get);

                    // use the extension method on the PortSet that creates a choice
                    // given two types found on one PortSet. This a common use of 
                    // Choice to deal with responses that have success or failure
                    Arbiter.Activate(taskQueue9,
                        get.ResponsePort.Choice(
                            s => Console.WriteLine(s), // delegate for success
                            ex => Console.WriteLine(ex) // delegate for failure
                    ));
                    break;
                case 10:
                    Dispatcher dispatcher10 = new Dispatcher();
                    DispatcherQueue taskQueue10 = new DispatcherQueue("sample queue", dispatcher10);
                    var portDouble = new Port<double>();
                    var portString = new Port<string>();

                    // activate a joined receiver that will execute only when one
                    // item is available in each port.
                    Arbiter.Activate(taskQueue10,
                        portDouble.Join(
                            portString, // port to join with
                            (value, stringValue) => // delegate
                            {
                                value /= 2.0;
                                stringValue = value.ToString();
                                // post back updated values
                                portDouble.Post(value);
                                portString.Post(stringValue);
                            })
                        );

                    // post items. The order does not matter, which is what Join its power
                    portDouble.Post(3.14159);
                    portString.Post("0.1");

                    //after the last post the delegate above will execute 
                    break;
            }
        }

        /// <summary>
        /// Handles Subscribe messages
        /// </summary>
        /// <param name="subscribe">the subscribe request</param>
        [ServiceHandler]
        public void SubscribeHandler(Subscribe subscribe)
        {
            SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);
        }
    }
}


