using ICSharpCode.SharpZipLib.Zip;
using Plugin;

namespace ZipPlugin
{
    public class Zip : IPlugin
    {
        public string Name => "Zip";

        public string[] Extensions =>  new string[] { ".zip" };

        public Dictionary<string, Stream> Preprocess(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            Dictionary<string, Stream> streams = new Dictionary<string, Stream>();
            using (ZipFile zipFile =new ZipFile(stream))
            {
                foreach (ZipEntry entry in zipFile)
                {
                    MemoryStream entryStream = new MemoryStream();
                    zipFile.GetInputStream(entry).CopyTo(entryStream);
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
            
            using (ZipOutputStream zipStream = new ZipOutputStream(output))
            {
                zipStream.IsStreamOwner = false;
                foreach(var (name, stream) in entries)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    ZipEntry entry = new ZipEntry(name);
                    zipStream.PutNextEntry(entry);
                    stream.CopyTo(zipStream);
                    zipStream.CloseEntry();
                }
                zipStream.Finish();
            }
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }
    }
}