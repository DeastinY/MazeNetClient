using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace MazeNetClient.Network
{
    static class StreamExtensions
    {
        /// <summary>
        /// Before you can send a message, you need to send the length of that message.
        /// This constant contains the size in bytes, that the length needs to have.
        /// </summary>
        const int MESSAGE_LENGTH_SIZE = 4;

        /// <summary>
        /// This constant represents an integer, with the first byte being filled up with 1.
        /// </summary>
        const int LOWEST_BYTE = 0xff;

        /// <summary>
        /// Describes eight bits for shifting operations.
        /// </summary>
        const int EIGHT_BITS = 8;

        static byte[] StringToBytes(string value)
        {
            byte[] byteString = Encoding.UTF8.GetBytes(value);
            Debug.Assert(value.SequenceEqual(Encoding.UTF8.GetString(byteString)));
            return byteString;
        }

        static string BytesToString(byte[] value)
        {
            string s = Encoding.UTF8.GetString(value);
            Debug.Assert(value.SequenceEqual(Encoding.UTF8.GetBytes(s)));
            return s;
        }

        static byte[] IntToBytes(int value)
        {
            byte[] byteInt = new byte[MESSAGE_LENGTH_SIZE];
            for (int i = MESSAGE_LENGTH_SIZE - 1; i >= 0; --i)
            {
                byteInt[i] = (byte)(value & LOWEST_BYTE);
                value >>= EIGHT_BITS;
            }
            return byteInt;
        }

        static int BytesToInt(byte[] byteInt)
        {
            int value = 0;
            for (int i = 0; i < MESSAGE_LENGTH_SIZE; ++i)
            {
                value <<= EIGHT_BITS;
                value |= (byteInt[i] & LOWEST_BYTE);
            }
            return value;
        }

        static byte[] ReadWithLength(NetworkStream stream, int receiveLength)
        {
            byte[] buffer = new byte[receiveLength];
            int totalNumberOfReadBytes = 0;
            try
            {
                if (receiveLength != MESSAGE_LENGTH_SIZE)
                {
                    //We often come to the problem that we try to read data from the stream, but then get an ArrayOutOfBoundsException.
                    //Perhaps this happens because the data did not arrive yet. So we wait for a second to let the data arrive first.
                    System.Threading.Thread.Sleep(1000);
                    //TODO: When we come to the point that performance becomes important, we can test whether we could reduce the sleep-time.
                }

                while (totalNumberOfReadBytes != receiveLength)
                {
                    int numberOfReadBytes = stream.Read(buffer, totalNumberOfReadBytes, receiveLength);
                    totalNumberOfReadBytes += numberOfReadBytes;
                }
            }
            catch (System.Exception ex)
            {
                Logger.ShowException(ex);
            }
            return buffer;
        }

        internal static void WriteUTF8(this NetworkStream stream, string message)
        {
            byte[] messageBuffer = StringToBytes(message);
            int messageLength = messageBuffer.Length;
            byte[] lengthBuffer = IntToBytes(messageLength);

            stream.Write(lengthBuffer, 0, lengthBuffer.Length);
            stream.Write(messageBuffer, 0, messageLength);
            stream.Flush();
        }

        internal static string ReadUTF8(this NetworkStream stream)
        {
            byte[] lengthBuffer = ReadWithLength(stream, MESSAGE_LENGTH_SIZE);
            int messageLength = BytesToInt(lengthBuffer);
            byte[] messageBuffer = ReadWithLength(stream, messageLength);

            string message = BytesToString(messageBuffer);
            return message;
        }
    }
}