using System.Text.Json;
using property_price_cosmos_db.Models;


namespace property_price_cosmos_db.Services;

public class JsonPlaceholderService(HttpClient client)
{
    private readonly HttpClient _client = client;

    public async Task<List<Post>> GetJsonPlaceholderPosts()
    {
        var httpResponseMessage = await _client.GetAsync(
            "posts?_start=0&_limit=2");

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream =
                await httpResponseMessage.Content.ReadAsStreamAsync();

            return (List<Post>)await JsonSerializer.DeserializeAsync
               <IEnumerable<Post>>(contentStream);
        }

        return [];
    }
}
