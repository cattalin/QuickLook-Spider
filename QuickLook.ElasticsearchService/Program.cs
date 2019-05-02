using System;
using System.Collections.Generic;
using ElasticsearchService.OutputManagers;

namespace ElasticsearchService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WilcardSearch for a content: ");
            string searchedContent = Console.ReadLine();

            ESOutputManager client = new ESOutputManager();
            var results = client.WilcardSearch(searchedContent);

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
