﻿using System;
using System.IO;
using System.Xml.Serialization;

namespace MazeNetClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.WriteLine("Starting MazeNetClient");

            const int PORT = 5123;
            var ipAddress = ReadIPAddress();
            if (String.IsNullOrWhiteSpace(ipAddress))
            {
                Logger.WriteLine("Invalid IP-Address!");
            }
            else
            {
                Logger.WriteLine("IP-Address: \"" + ipAddress + "\"");
                Logger.WriteLine("Port: \"" + PORT + "\"");
                EnterMessageLoop(ipAddress, PORT);
            }

            Logger.WriteLine("Closing MazeNetClient");
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.Read();
            }
        }

        static string ReadIPAddress()
        {
            const string ipAddressFilePath = @".\ipaddress.txt";

            try
            {
                var ipAddress = File.ReadAllText(ipAddressFilePath);
                return ipAddress;
            }
            catch (Exception ex)
            {
                Logger.ShowException(ex);
            }

            Logger.WriteLine("Could not get IP-Address from file: \"" + ipAddressFilePath + "\". Using localhost as fallback value.");
            return "localhost";
        }

        static void EnterMessageLoop(string ipAddress, int port)
        {
            while (true)
            {
                Logger.Write("Join a new game? (J/N):");
                var userInput = Console.ReadLine();
                bool isYes = StringEquals(userInput, "j");
                if (isYes)
                    JoinGame(ipAddress, port);
                else
                {
                    bool isNo = StringEquals(userInput, "n");
                    if (isNo)
                        break;
                    else
                        Logger.WriteLine("Invalid input: " + userInput);
                }
            }
        }

        static bool StringEquals(string first, string second)
        {
            return String.Equals(first, second, StringComparison.CurrentCultureIgnoreCase);
        }

        static void JoinGame(string ipAddress, int port)
        {
            Logger.WriteLine();
            Logger.WriteLine("Joining the game");
            try
            {
                using (var connection = new Network.ServerConnection(ipAddress, port))
                {
                    connection.SendMessage(MazeComMessageFactory.CreateLoginMessage("hi").ConvertToString());
                    string s = connection.ReceiveMessage();
                    var a = s.ConvertToMazeCom();
                    int id = a.id;

                    string ataiwmove = connection.ReceiveMessage();
                    var mc = ataiwmove.ConvertToMazeCom();

                }
            }
            catch (Exception ex)
            {
                Logger.ShowException(ex);
            }
            Logger.WriteLine("Finished the game");
            Logger.WriteLine();
        }
    }
}