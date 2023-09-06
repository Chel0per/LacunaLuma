using System.Text;

namespace Lacuna
{
class StartingPost
{
    public string Username
    {
        get { return _username; }
        set { _username = value; }
    }
    private string _username;

    public string Email
    {
        get { return _email; }
        set { _email = value; }
    }
    private string _email;

    public StartingPost()
    {
        _username="Unknown";
        _email="Unknown";
    }
    public StartingPost(string username,string email)
    {
        _username=username;
        _email=email;
    }

    public async Task GetAcessToken( )
    {
        using HttpClient httpClient = new HttpClient();
        try
        {
            string requestBody = "{\"username\":\"" + _username + "\",\"email\":\"" + _email + "\"}";
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("https://luma.lacuna.cc/api/start", content);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {responseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
}
