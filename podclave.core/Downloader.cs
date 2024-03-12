
using Microsoft.Extensions.Logging;

namespace Podclave.Core;

public interface IDownloader
{
    Task<string> Download(string feedUrl);
}

public class Downloader: IDownloader
{
    private readonly ILogger<Downloader> _logger;

    public Downloader(ILogger<Downloader> logger)
    {
        _logger = logger;
    }

    public async Task<string> Download(string feedUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            try 
            {
                HttpResponseMessage response = await client.GetAsync(feedUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    return responseBody;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occured try to fetch feed from {feedUrl}. Error: {e.Message}");
                throw;
            }
        } 
        return string.Empty;      
    }
}