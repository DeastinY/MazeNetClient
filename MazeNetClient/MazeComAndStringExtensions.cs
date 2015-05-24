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

        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(MazeCom), typeof(MazeCom).Namespace);

        internal static MazeCom ConvertToMazeCom(this string mazeString)
        {
            using (var stringReader = new StringReader(mazeString))
            {
                var mazeObject = serializer.Deserialize(stringReader);
                return (MazeCom)mazeObject;
            }
        }

        internal static string ConvertToString(this MazeCom mazeObject)
        {
            using (var stringWriter = new UTF8StringWriter())
            {
                serializer.Serialize(stringWriter, mazeObject);
                var mazeString = stringWriter.ToString();
                return mazeString;
            }
        }
    }
}