using MazeNetClient.XSDGenerated;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MazeNetClient
{
    static class MazeComAndStringExtensions
    {
        /// <summary>
        /// This class is a StringWriter with an UTF8-Encoding.
        /// </summary>
        class UTF8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        internal static MazeCom ConvertToMazeCom(this string mazeString)
        {
            using (var stringReader = new StringReader(mazeString))
            {
                var serializer = new XmlSerializer(typeof(MazeCom));
                var mazeObject = serializer.Deserialize(stringReader);
                return (MazeCom)mazeObject;
            }
        }

        internal static string ConvertToString(this MazeCom mazeObject)
        {
            using (var stringWriter = new UTF8StringWriter())
            {
                var serializer = new XmlSerializer(typeof(MazeCom));
                serializer.Serialize(stringWriter, mazeObject);
                var mazeString = stringWriter.ToString();
                return mazeString;
            }
        }
    }
}