/*
* FILE        : information.cs
* PROJECT     : WinProg - Assignment #4
* PROGRAMMER  : Zivojin Pecin & Arindm Sharma
* DESCRIPTION :  Purpose of this file is to store the information needed for the program. Which is, the message that goes on the que,
*                the computer name amd the client name.
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatClient
{
    [Serializable]
    public class Information
    {
        public Information()
        {

        }
        //store name of machine to send the messgae to
        public string Name;
        //store the message 
        public string Message;
        //store name of client
        public string clientName;
    }
}
