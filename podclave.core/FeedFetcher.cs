using System;
using System.Net.Http;

namespace Podclave.Core;

public class FeedFetcher
{
    public async Task Fetch(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try 
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(responseBody);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured try to fetch feed from {url}. Error: {e.Message}");
            }
        }
    }
}