namespace Lacuna
{
class SyncValues
{
    public string? ProbeNow { get; set; }
    public long? RoundTrip { get; set; }

    public SyncValues(string probeNow,long roundTrip)
    {
        ProbeNow = probeNow;
        RoundTrip = roundTrip;
    }

}
}