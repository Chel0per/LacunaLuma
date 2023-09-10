using System.Text.Json;

namespace Lacuna
{
class Probe
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Encoding { get; set; }
    public SyncValues? SyncValues { get; set; }
    public async Task SyncProbe(string accessToken)
    {
        using HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        long timeOffset = 0;
        long newTimeOffset;
        long roundTrip = 0;
        long t0;
        long t3;

        do 
        {
            t0 = DateTime.UtcNow.Ticks + timeOffset;
            HttpResponseMessage response = await httpClient.PostAsync($"https://luma.lacuna.cc/api/probe/{Id}/sync",null);
            t3 = DateTime.UtcNow.Ticks + timeOffset;
            string responseContent = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Response? syncResponse = JsonSerializer.Deserialize<Response>(responseContent, options);
            Decode(syncResponse);
            long t1 = long.Parse(syncResponse.T1);
            long t2 = long.Parse(syncResponse.T2);

            newTimeOffset = (t1 - t0 + (t2 - t3))/2;  
            timeOffset += newTimeOffset;
            roundTrip = t3 - t0 - (t2 - t1); 

        }
        while(newTimeOffset >= 50000 || newTimeOffset <= -50000);

        SyncValues syncValues = new SyncValues(timeOffset,roundTrip);

        SyncValues = syncValues;
    }

    private void Decode(Response syncResponse){
        switch(Encoding)
        {
            case "TicksBinary":
                byte[] bytes = Convert.FromBase64String(syncResponse.T1);
                syncResponse.T1 = BitConverter.ToInt64(bytes, 0).ToString();

                byte[] bytes2 = Convert.FromBase64String(syncResponse.T2);
                syncResponse.T2 = BitConverter.ToInt64(bytes2, 0).ToString();
                break;
            case "TicksBinaryBigEndian":
                byte[] bytesbe = Convert.FromBase64String(syncResponse.T1);
                Array.Reverse(bytesbe); 
                syncResponse.T1 = BitConverter.ToInt64(bytesbe, 0).ToString();

                byte[] bytesbe2 = Convert.FromBase64String(syncResponse.T2);
                Array.Reverse(bytesbe2); 
                syncResponse.T2 = BitConverter.ToInt64(bytesbe2, 0).ToString();
                break;
            case "Iso8601":
                DateTime dateTime = DateTime.Parse(syncResponse.T1);
                syncResponse.T1 = (dateTime.Ticks + 108000000000).ToString();

                DateTime dateTime2 = DateTime.Parse(syncResponse.T2);
                syncResponse.T2 = (dateTime2.Ticks + 108000000000).ToString();
                break;          
        }
    }
}
}