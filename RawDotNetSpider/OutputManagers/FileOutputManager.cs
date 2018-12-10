using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Spider
{
    public class FileOutputManager : IDisposable, IOutputManager
    {
        private readonly StreamWriter _streamWriter;
        private readonly JsonSerializer _serializer;

        public FileOutputManager(string filePath)
        {
            this._streamWriter = new StreamWriter(filePath, true); ;
            this._serializer = new JsonSerializer();
        }

        public void OutputEntry(WebsiteInfo retrievedInfo)
        {

            Console.WriteLine("Website --> " + retrievedInfo.Url);
            Console.WriteLine("Title   --> " + retrievedInfo.Title);
            Console.WriteLine("Desc    --> " + retrievedInfo.DescriptionMeta);


            _serializer.Serialize(_streamWriter, retrievedInfo);
        }

        public async Task OutputEntryAsync(WebsiteInfo retrievedInfo)
        {

            Console.WriteLine("Website --> " + retrievedInfo.Url);
            Console.WriteLine("Title   --> " + retrievedInfo.Title);
            Console.WriteLine("Desc    --> " + retrievedInfo.DescriptionMeta);


            await Task.Run(() => _serializer.Serialize(_streamWriter, retrievedInfo));
        }

        public bool isDisposed = false;

        public void Dispose()
        {
            if (isDisposed == false)
            {
                isDisposed = true;
                _streamWriter.Close();
                this.Dispose();
            }
        }
    }
}
