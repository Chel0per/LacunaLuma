using System.Text.Json;

namespace Lacuna
{
class Program
{
    static async Task Main()
    {
        StartingPost startingPost = new StartingPost("chel0per","marcelo.smarques7@gmail.com");
        string accessToken = await startingPost.GetAcessToken();
        
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://luma.lacuna.cc/api/probe");
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
                Response? probesResponse = JsonSerializer.Deserialize<Response>(responseContent,options);

                if (probesResponse.Probes != null)
                {
                    foreach (Probe probe in probesResponse.Probes)
                    {
                        long T0 = DateTime.UtcNow.Ticks;
                        Response syncResponse = await probe.SyncProbe(accessToken);
                        long T3 = DateTime.UtcNow.Ticks;

                        Console.WriteLine(T0);
                        Console.WriteLine(syncResponse.T1);
                        Console.WriteLine(syncResponse.T2);
                        Console.WriteLine(T3);
                        Console.WriteLine("");
                    }
                }
                else
                {
                    Console.WriteLine("Probes list is null.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

}