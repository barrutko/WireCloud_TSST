using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WireCloud
{
    class LogWriter
    {
        static Mutex logMutex = new Mutex();
        static bool backgroundSwitch = true;
        public static void connectionsDown(List<string> callSigns, bool addCurrentTime)
        {
            logMutex.WaitOne();
            if (callSigns.Count > 1 && callSigns.Count % 2 == 0)
            { 
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("Connection");
                if (callSigns.Count > 2)
                    Console.Write("s");
                Console.Write(" down: ");
                Console.ResetColor();
                bool separatorFlag = true;
                foreach (string callSign in callSigns)
                {
                    Console.Write(callSign);
                    if (separatorFlag)
                    {
                        Console.Write("-");
                    }
                    else
                    {
                        Console.Write("; ");
                    }
                    separatorFlag = !separatorFlag;
                }
                addTimestamp(addCurrentTime);
            }
            logMutex.ReleaseMutex();
        }

        public static void connectionsUp(List<string> callSigns, bool addCurrentTime)
        {
            logMutex.WaitOne();
            if (callSigns.Count > 1 && callSigns.Count % 2 == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Connection");
                if(callSigns.Count>2)
                Console.Write("s");
                Console.Write(" up: ");
                Console.ResetColor();
                bool separatorFlag = true;
                foreach (string callSign in callSigns)
                {
                    Console.Write(callSign);
                    if (separatorFlag)
                    {
                        Console.Write("-");
                    }
                    else
                    {
                        Console.Write("; ");
                    }
                    separatorFlag = !separatorFlag;
                }
                addTimestamp(addCurrentTime);
            }
            logMutex.ReleaseMutex();
        }

        public static void startingMessage()
        {
            logMutex.WaitOne();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("WireCloud started!");
            Console.ResetColor();
            logMutex.ReleaseMutex();
        }

        public static void notificationMessage(string text, bool addCurrentTime)
        {
            logMutex.WaitOne();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(text);
            addTimestamp(addCurrentTime);
            logMutex.ReleaseMutex();
        }
        public static void generalMessage(string text, bool addCurrentTime)
        {
            logMutex.WaitOne();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(text);
            addTimestamp(addCurrentTime);
            logMutex.ReleaseMutex();
        }

        public static void receivedMessage(byte[] data, string whereFrom, bool addCurrentTime)
        {
            try
            {
                logMutex.WaitOne();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("Cloud received data batch from " + whereFrom + ":" + data[0]);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" (ID: \"" + data[1] + "\"; dbSize: " + (data.Length - 2) + " bytes;)");
                addTimestamp(addCurrentTime);
                logMutex.ReleaseMutex();
            }
            catch(Exception ex)
            {

            }
        }

        public static void sendMessage(byte[] data, string whereTo, bool addCurrentTime)
        {

            logMutex.WaitOne();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("Cloud submitted data batch to " + whereTo + ":" + data[0]);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" (ID: \"" + data[1] + "\"; dbSize: " + (data.Length - 2) + " bytes;)");
            addTimestamp(addCurrentTime);
            logMutex.ReleaseMutex();
        }
        public static void dataLossMessage(byte[] data, bool addCurrentTime)
        {

            logMutex.WaitOne();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Data batch lost");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" (ID: \"" + data[1] + "\"; dbSize: " + (data.Length - 2) + " bytes;)");
            addTimestamp(addCurrentTime);
            logMutex.ReleaseMutex();
        }

        private static void addTimestamp(bool flag)
        {
            if (flag)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(" <" + DateTime.Now.ToString("hh:mm:ss:fff") + ">");
                
            }
            else
            {
                Console.WriteLine("");
            }
            Console.ResetColor();
        }
        
        public static void deviceDown(string callSign, bool addCurrentTime)
        {
            logMutex.WaitOne();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("<" + callSign + "> is down");
            addTimestamp(addCurrentTime);
            logMutex.ReleaseMutex();
        }
        public static void deviceRevived(string callSign, bool addCurrentTime)
        {
            logMutex.WaitOne();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("<"+callSign + "> revived");
            addTimestamp(addCurrentTime);
            logMutex.ReleaseMutex();
        }
        public static void newOrderFromManager()
        {
            logMutex.WaitOne();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write("Received new order from Manager");
            Console.ResetColor();
            Console.Write(" ");
            logMutex.ReleaseMutex();
        }
    }
}
