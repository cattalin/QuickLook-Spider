using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace ElasticsearchService.OutputManagers
{
    public interface IOutputManager
    {
        void OutputEntry(WebsiteInfo retrievedInfo);
        Task OutputEntryAsync(WebsiteInfo retrievedInfo);
        Task UpdateEntryAsync(WebsiteInfo retrievedInfo, string Id);
    }
}
