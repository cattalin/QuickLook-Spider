using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Interfaces;
using Shared.Models;

namespace ElasticsearchService.OutputManagers
{
    public class FileWriteManager : IOutputManager<WebsiteInfo>, IDisposable
    {
        private readonly StreamWriter _streamWriter;
        private readonly JsonSerializer _serializer;

        public FileWriteManager(string filePath)
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

        public async Task UpdateEntryAsync(WebsiteInfo retrievedInfo, string Id)
        {

            Console.WriteLine("Website --> " + retrievedInfo.Url);
            Console.WriteLine("Title   --> " + retrievedInfo.Title);
            Console.WriteLine("Desc    --> " + retrievedInfo.DescriptionMeta);


            await Task.Run(() => _serializer.Serialize(_streamWriter, retrievedInfo));
        }
        public Task BulkOutputAsync(List<PendingWebsite> items)
        {
            throw new NotImplementedException();
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
