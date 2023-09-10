using System.Text;
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

                foreach (Probe probe in probesResponse.Probes)
                {
                    Console.WriteLine(probe.Name);
                    await probe.SyncProbe(accessToken);
                    Console.WriteLine("Probe Synched");
                }

                string checkCode = "";

                while(checkCode != "Done")
                {
                    HttpResponseMessage response2 = await httpClient.PostAsync("https://luma.lacuna.cc/api/job/take",null);
                    string responseContent2 = await response2.Content.ReadAsStringAsync();
                    Response? jobResponse = JsonSerializer.Deserialize<Response>(responseContent2,options);                    
                    Console.WriteLine($"{responseContent2}");
                    if (jobResponse.Job == null)
                    {
                        Console.WriteLine($"Job is null handle it!");
                    }
                    

                    Probe foundProbe = probesResponse.Probes.Find(probe => probe.Name == jobResponse.Job.ProbeName);
                    long probeNow = DateTime.UtcNow.Ticks + foundProbe.SyncValues.Offset;
                    string probeNowEncoded = Encode(probeNow,foundProbe.Encoding);
                    string requestBody = "{\"probeNow\":\"" + probeNowEncoded + "\",\"roundTrip\":" + foundProbe.SyncValues.RoundTrip.ToString() + "}";
                    HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response3 = await httpClient.PostAsync($"https://luma.lacuna.cc/api/job/{jobResponse.Job.Id}/check", content);
                    string responseContent3 = await response3.Content.ReadAsStringAsync();
                    Response? checkResponse = JsonSerializer.Deserialize<Response>(responseContent3,options);

                    checkCode = checkResponse.Code;
                    Console.WriteLine(responseContent3);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catch got me");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public static string Encode(long probeNow ,string encoding)
    {
        switch (encoding)
        {
            case "TicksBinary":
                return Convert.ToBase64String(BitConverter.GetBytes(probeNow));
            case "TicksBinaryBigEndian":
                byte[] bytes1BigEndian = BitConverter.GetBytes(probeNow);
                Array.Reverse(bytes1BigEndian);
                return Convert.ToBase64String(bytes1BigEndian);
            case "Iso8601":
                long ticks1Iso8601 = probeNow;
                DateTime dateTime1 = new DateTime(ticks1Iso8601, DateTimeKind.Utc);
                return dateTime1.ToString("o");
            case "Ticks":
                return probeNow.ToString();
            default:
                return "Why do i have to use default?";
        }
    }
}

}