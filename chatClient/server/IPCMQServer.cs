/*
* FILE : IPCMQServer.cs
* PROJECT : WinProg - Assignment #4
* PROGRAMMER : Zivojin Pecin & Arindm Sharma
* DESCRIPTION : This file is the server in a message que chat. Once the server gets created it will create a master que between multiple machines.
*               These machines will send a message to particular machine by indiacting the name of the machine that the message is intended to.
*               This message will go to the master que in the server. Once its gets to the server, the server will extract the information about the machine.
*               Which is , the machines name and the message, once it finds out the machine name it will send the message to that particular machine name. 
*/


using System;
using System.Messaging;

namespace server
{
    class IPCMQServer
    {
        string mQueueName = @".\private$\zyQueue";
        MessageQueue mq;

        /*
* FUNCTION    : IPCMQServer()
*
* DESCRIPTION : This is the constructor for the lcass which sets up the connection.
*
* PARAMETERS  : NA
* RETURNS     : NA
*/
        public IPCMQServer() // constructor for initilizing the class.
        {
            //check if que exists, if not create one and attach to it.
            if (!MessageQueue.Exists(mQueueName))
            {
                mq = MessageQueue.Create(mQueueName);
                Console.WriteLine("the message queue named zyQueue was created on the server!");
            }
            else
            {
                mq = new MessageQueue(mQueueName);
            }
        }

/*
* FUNCTION    : GetMessages
*
* DESCRIPTION : This function gets messages from the que
*
* PARAMETERS  : NA
* RETURNS     : NA
*/
        public void GetMessages()
        {
            bool finished = false;

            //constantly try to read messages from que
            while (!finished)
            {
                try
                {
                    //get the message from a particualr que
                    MessageQueue msgqueue_Get = new MessageQueue();
                    msgqueue_Get.Path = mQueueName;


                    Type[] target = new Type[1];
                    target[0] = typeof(string);

                    //Time out for 2 seconds
                    Message msg = msgqueue_Get.Receive();

                    //Formatter for deserializing xml messages
                    msg.Formatter = new System.Messaging.XmlMessageFormatter((new Type[] { typeof(Information) }));

                    //Deserializing the message
                    Information info = (Information)msg.Body;


                    //string to send to the que, which contains the needed info
                    string tosend = "FormatName:DIRECT=OS:" + info.Name + "\\Private$\\myqueue";

                    MessageQueue queue = new MessageQueue(tosend);
                    Information objMsg = new Information();
                    objMsg.Name = "sender";
                    objMsg.Message = info.Message;
                    objMsg.clientName = info.clientName;

                    Message m = new Message();

                    m.Body = objMsg;
                    //send to que, the message, and machine name, so it knows where to send the message later on
                    queue.Send(m, Environment.MachineName);
                    
                }
                //catch block, if an exception gets thrown, then we know what the issue is, and if its que or something else
                catch (MessageQueueException mqex)
                {
                    Console.WriteLine("MQ Exception: " + mqex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }

            mq.Close();

        }
    }
}
