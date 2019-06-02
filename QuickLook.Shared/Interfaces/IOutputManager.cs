using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Interfaces
{
    public interface IOutputManager<T>
    {
        void OutputEntry(T retrievedInfo);
        Task OutputEntryAsync(T retrievedInfo);
        Task BulkOutputAsync(List<PendingWebsite> items);
        Task UpdateEntryAsync(T retrievedInfo, string Id);
    }
}
