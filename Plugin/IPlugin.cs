namespace Plugin
{
    public interface IPlugin
    {
        string Name { get; }
        string[] Extensions { get; }
        Dictionary<string, Stream> Preprocess(Stream stream);
        Stream Postprocess(Dictionary<string, Stream> entries);
    }
}