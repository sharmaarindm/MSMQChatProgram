/*
* FILE        : MainWindow.xaml.cs*
* PROJECT     : WinProg - Assignment #4
* PROGRAMMER  : Zivojin Pecin & Arindm Sharma
* DESCRIPTION : The purpose of this file is to act as the GUI for the user of the program. In this main window, the user will be asked to input
*               the machine name for the machine that is supposed to receive a message, and the message itsself, and the server that will hold the master que between the clients
*/


using System;
using System.Windows;
using System.Messaging;
using System.Threading;

namespace chatClient
{
    
    public partial class MainWindow : Window
    {

        MessageQueue myQ;
        public MainWindow()
        {
            //check if the que exists
            if (!MessageQueue.Exists(@".\Private$\MyQueue"))
            {
                //if not create it, and attach the pc to it
                myQ = MessageQueue.Create(@".\Private$\MyQueue", false);
                MessageBox.Show("Queue named queuename is created on your machine");
            }
            else
            {
                myQ = new MessageQueue(@".\Private$\MyQueue");
            }

            InitializeComponent();
            //make new thread for new client
            Thread t = new Thread(GetMessagesb);
            t.Start();
            
        }

        /*
        * FUNCTION    : button_Click
        *
        * DESCRIPTION : Upon buttonclick this code will be executed, this is the main window where the user specifies where to send a message, and the user 
        *               also places his message in this mains windows textbox
        *
        * PARAMETERS  : object sender, RoutedEventArgs
        * RETURNS     : NA
        */

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageQueue msgQ_Send = new MessageQueue();

                if (textBox1.Text == "")
                {
                    //Select User to which you want to send message.
                    MessageBox.Show("please provide the name of the computer you wish to chat with!.");
                }
                else
                {
                 

                    string tosends = "FormatName:DIRECT=OS:"+ textBox2.Text +"\\Private$\\zyQueue";

                    
                    MessageQueue queue = new MessageQueue(tosends);
                    
                    Information objMsg = new Information();
                    //name of the machine that is supposed to get the message from user
                    objMsg.Name = textBox1.Text; 
                    //the message from the user
                    objMsg.Message = textBox.Text;
                    //client name
                    objMsg.clientName = Environment.MachineName;

                    //bind the message with message class
                    System.Messaging.Message m = new System.Messaging.Message();
                    m.Body = objMsg;
                  
                  
                    queue.Send(m, System.Environment.MachineName);

                    //the message about to be sent
                    string temp = "\r\n"+ Environment.MachineName + ": " + textBox.Text;
                    textBlock.Text += temp;

                    textBox.Text = "";

                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
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

        public void GetMessagesb()
        {
            bool finished = false;
            //always try to get message from the que
            while (!finished)
            {
                try
                {
                    string mQueueName = @".\private$\MyQueue";
                    MessageQueue msgqueue_Get = new MessageQueue();
                    msgqueue_Get.Path = mQueueName;


                    Type[] target = new Type[1];
                    target[0] = typeof(string);

                    //Time out for 2 seconds
                    Message msg = msgqueue_Get.Receive();


                    //Formatter for deserializing xml messages
                    msg.Formatter = new System.Messaging.XmlMessageFormatter((new Type[] { typeof(Information) }));

                    //Deserialize the message
                    Information info = (Information)msg.Body;
                    //check if other client is connected
                    if (info.Message == "window_closing")
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            string disc = "the client you are trying to chat with has been disconnected!"; // if the client has been diconnected.

                            textBlock.Text = disc;
                        });
                    }

                    this.Dispatcher.Invoke(() =>
                    {
                        string temp = "\r\n" + info.clientName + ": " + info.Message; // else print the message.
                        textBlock.Text += temp;
                    });
                    

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

        }

        /*
        * FUNCTION    : Window_Closing
        *
        * DESCRIPTION : This function closes the main window, that the user uses as the GUI for the program
        *
        * PARAMETERS  : object sender, System.ComponentModel.CancelEventArgs e
        * RETURNS     : NA
        */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string tosends = "FormatName:DIRECT=OS:" + textBox2.Text + "\\Private$\\zyQueue";


            MessageQueue queue = new MessageQueue(tosends); // setting up the queue with the path.

            Information objMsg = new Information(); // instantiating the serializable class.
            objMsg.Name = textBox1.Text; //name of the machine which is supposed to recieve the message.
            objMsg.Message = "window_closing";

            //Bind message with message class
            Message m = new Message();
            m.Body = objMsg;

            //send to que the message and the machine name
            queue.Send(m, System.Environment.MachineName);

            MessageQueue.Delete(@".\private$\MyQueue"); //cleaning up by deleting the message queue.
        }
    }
}
