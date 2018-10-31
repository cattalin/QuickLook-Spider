using System;
using System.IO;
using Newtonsoft.Json;

namespace RawDotNetSpider
{
    public class FileOutputManager : IDisposable, IOutputManager
    {
        private StreamWriter streamWriter;
        private JsonSerializer serializer;

        public FileOutputManager(string filePath)
        {
            this.streamWriter = new StreamWriter(filePath, true); ;
            this.serializer = new JsonSerializer();
        }

        public void AddEntry(WebsiteInfo retrievedInfo)
        {

            Console.WriteLine("Website --> " + retrievedInfo.Url);
            Console.WriteLine("Title   --> " + retrievedInfo.Title);
            Console.WriteLine("Desc    --> " + retrievedInfo.DescriptionMeta);


            serializer.Serialize(streamWriter, retrievedInfo);
        }

        public bool isDisposed = false;

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                streamWriter.Close();
                this.Dispose();
            }
        }
    }
}
