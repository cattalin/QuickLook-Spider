using System;
using System.Collections.Generic;
using ElasticsearchService.OutputManagers;

namespace ElasticsearchService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Search for a content: ");
            string searchedContent = Console.ReadLine();

            NestClient client = new NestClient();
            var results = client.Search(searchedContent);

            if(results.Count == 0)
                Console.WriteLine("No results found");

            foreach (var response in results)
            {
                Console.WriteLine(response.Url);
            }

            Console.ReadLine();
        }
    }
}
