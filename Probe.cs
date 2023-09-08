using System.Text.Json;

namespace Lacuna
{
class Probe
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Encoding { get; set; }

    public async Task<Response> SyncProbe(string accessToken)
    {
        using HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        HttpResponseMessage response = await httpClient.PostAsync($"https://luma.lacuna.cc/api/probe/{Id}/sync",null);
        string responseContent = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        Response? syncResponse = JsonSerializer.Deserialize<Response>(responseContent, options);
        Decode(syncResponse);

        return syncResponse;
    }

    private void Decode(Response syncResponse){
        switch(Encoding)
        {
            case "TicksBinary":
                break;
            case "TicksBinaryBigEndian":
                break;
            case "Iso8601":
                DateTime dateTime = DateTime.Parse(syncResponse.T1);
                syncResponse.T1 = dateTime.Ticks.ToString();
                DateTime dateTime2 = DateTime.Parse(syncResponse.T2);
                syncResponse.T2 = dateTime2.Ticks.ToString();
                break;
        }
    }
}
}