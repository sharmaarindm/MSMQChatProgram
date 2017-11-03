/*
* FILE : Program.cs
* PROJECT : WinProg - Assignment #4
* PROGRAMMER :  Arindm Sharma & Zivojin Pecin 
* DESCRIPTION : This file creates a thread for each client, in order for the server to be able to process multiple requests at once.
*               Instead of processing each message and then accepting the other message then processing that one and so on..
*              
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Threading;
using System.Windows;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            IPCMQServer server = new IPCMQServer();

            Console.Write("Waiting for a connection... ");

            //create thread for server and message
            Thread t = new Thread(server.GetMessages);
            //start the thread processing
            t.Start();
            Console.WriteLine("Connected!");
            //create server
            IPCMQServer server2 = new IPCMQServer();

            //make user press any key to continue
            Console.ReadKey();
            

        }

    }
}

