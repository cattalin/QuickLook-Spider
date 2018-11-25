using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawDotNetSpider
{
    public interface IOutputManager
    {
        void OutputEntry(WebsiteInfo retrievedInfo);
        Task OutputEntryAsync(WebsiteInfo retrievedInfo);
    }
}
