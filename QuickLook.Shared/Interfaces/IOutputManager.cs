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
        Task IndexEntryAsync(T retrievedInfo);
        Task BulkIndexAsync(List<T> items);
        Task UpdateEntryAsync(T retrievedInfo, string Id);
    }
}
