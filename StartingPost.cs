using System.Text;
using System.Text.Json;

namespace Lacuna
{
class StartingPost
{
    private string? _username;
    private string? _email;

    public StartingPost(string username,string email)
    {
        _username=username;
        _email=email;
    }

    public async Task<string> GetAcessToken( )
    {
        using HttpClient httpClient = new HttpClient();
        try
        {
            string requestBody = "{\"username\":\"" + _username + "\",\"email\":\"" + _email + "\"}";
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("https://luma.lacuna.cc/api/start", content);
            string responseContent = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
            Response? startingResponse = JsonSerializer.Deserialize<Response>(responseContent,options);
                
            if (startingResponse != null)
            {
                return startingResponse.AccessToken ?? "AccessToken is null";
            }
            else
            {
                return "Response could not be deserialized";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
}
