using ICSharpCode.SharpZipLib.Tar;
using Plugin;

namespace TarPlugin
{
    public class Tar : IPlugin
    {
        public string Name => "Tar";

        public string[] Extensions => new string[] { ".tar" };

        public Dictionary<string, Stream> Preprocess(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            Dictionary<string, Stream> streams = new Dictionary<string, Stream>();
            
            using (TarInputStream tarStream = new TarInputStream(stream, System.Text.Encoding.UTF8))
            {
                tarStream.IsStreamOwner = false;
                TarEntry entry;
                while ((entry = tarStream.GetNextEntry()) != null)
                {
                    MemoryStream entryStream = new MemoryStream();
                    tarStream.CopyEntryContents(entryStream);
                    entryStream.Seek(0, SeekOrigin.Begin);
                    streams.Add(entry.Name, entryStream);
                }
                stream.Seek(0, SeekOrigin.Begin);
            }
            return streams;
        }


        public Stream Postprocess(Dictionary<string, Stream> entries)
        {
            MemoryStream output = new MemoryStream();
            using (TarOutputStream tarStream = new TarOutputStream(output, System.Text.Encoding.UTF8))
            {
                tarStream.IsStreamOwner = false;
                foreach (var (name, stream) in entries)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    TarEntry entry = new TarEntry(new TarHeader() { Name = name, Size = stream.Length });
                    tarStream.PutNextEntry(entry);
                    stream.CopyTo(tarStream);
                    tarStream.CloseEntry();
                }
                tarStream.Flush();  
            }
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }
    }
}