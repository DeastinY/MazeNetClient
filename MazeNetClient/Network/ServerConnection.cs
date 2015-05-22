using System;
using System.Net.Sockets;

namespace MazeNetClient.Network
{
    class ServerConnection : IDisposable
    {
        private TcpClient m_client;

        internal ServerConnection(string hostname, int port)
        {
            m_client = new TcpClient(hostname, port);
        }

        internal void SendMessage(string message)
        {
            m_client.GetStream().WriteUTF8(message);
        }

        internal string ReceiveMessage()
        {
            return m_client.GetStream().ReadUTF8();
        }

        public void Dispose()
        {
            m_client.Close();
            m_client = null;
        }

        ~ServerConnection()
        {
            if (m_client != null)
            {
                Logger.WriteLine("Did not dispose Connection::m_client!", true);
            }
        }
    }
}