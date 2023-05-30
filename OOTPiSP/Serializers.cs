using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data;
using OOTPiSP.TextFormat;

namespace Serializers
{
    interface ISerializer
    {
        string Name { get; }
        string[] Extensions { get; }
        void Serialize(object value, Stream stream);
        T Deserialize<T>(Stream stream);
    }

    public class BinarySerializer : ISerializer
    {
        public string Name => "Binary";
        public string[] Extensions => new string[] { ".bin" };
        [Obsolete("Binary serialization is not safe and is not recommended for data processing.")]
        public void Serialize(object value, Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, value);
        }

        [Obsolete("Binary serialization is not safe and is not recommended for data processing.")]
        public T Deserialize<T>(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (T)bf.Deserialize(stream);
        }
    }

    public class JsonMySerializer : ISerializer
    {
        public string Name => "JSON";
        public string[] Extensions => new string[] { ".json" };
        private static readonly Encoding utf8 = Encoding.UTF8;
        public void Serialize(object value, Stream stream)
        {
            string json = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            stream.Write(utf8.GetBytes(json));

        }

        public T Deserialize<T>(Stream stream)
        {
            byte[] buf = new byte[stream.Length];
            stream.Read(buf);
            return JsonConvert.DeserializeObject<T>(utf8.GetString(buf), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }

    public class TxtSerializer : ISerializer
    {
        public string Name => "Text-format";
        public string[] Extensions => new string[] { ".txt" };
        private static readonly Encoding utf8 = Encoding.UTF8;
        private const string indentString = "\u0020\u0020";
        private string SerializeObject(object value, string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            if (value == null)
                return "null";
            Type type = value.GetType();
            if (type.IsEnum)
            {

                sb.Append(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())).ToString());
                //sb.Append("\"").Append(Uri.EscapeDataString(value.ToString())).Append("\"");
            }
            else if (type == typeof(string))
            {
                sb.Append("\"").Append(Uri.EscapeDataString(value.ToString())).Append("\"");
            }
            else if (type.GetInterface(nameof(IEnumerable)) != null)
            {
                IEnumerable<object> array = ((IEnumerable)value).Cast<object>();
                sb.Append("[");
                string innerIndent = indent + indentString;
                string separatorString = ";\n" + innerIndent;
                IEnumerable<string> elemStrings = array.
                    Select(
                        (object obj) => SerializeObject(obj, innerIndent)
                    );
                string body = string.Join(separatorString, elemStrings);
                if (body.Length > 0)
                {
                    sb.Append("\n").Append(innerIndent).Append(body).Append("\n").Append(indent);
                }
                sb.Append("]");
            }
            else if (type.IsClass)
            {
                sb.Append("(").Append(type.FullName).Append(")");
                sb.Append("{");
                string innerIndent = indent + indentString;
                string separatorString = ";\n" + innerIndent;
                IEnumerable<string> propStrings = type.GetProperties().
                    Select(
                        (PropertyInfo prop) => $"\"{Uri.EscapeDataString(prop.Name)}\" : {SerializeObject(prop.GetValue(value), innerIndent)}"
                    );
                string body = string.Join(separatorString, propStrings);
                if (body.Length > 0)
                {
                    sb.Append("\n").Append(innerIndent).Append(body).Append("\n").Append(indent);
                }
                sb.Append("}");
            }
            else if (type.IsValueType)
            {
                sb.Append(value.ToString());
            }
            return sb.ToString();
        }
        public void Serialize(object value, Stream stream)
        {
            string serialized = SerializeObject(value);
            stream.Write(utf8.GetBytes(serialized));
        }

        public T Deserialize<T>(Stream stream)
        {
            byte[] buf = new byte[stream.Length];
            stream.Read(buf);
            TxtReader reader = new TxtReader(utf8.GetString(buf));
            ObjectParser bm = new ObjectParser(reader, typeof(T));
            return (T)bm.Parse();
        }
    }
}