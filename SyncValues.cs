namespace Lacuna
{
class SyncValues
{
    public long Offset { get; set; }
    public long RoundTrip { get; set; }

    public SyncValues(long offset,long roundTrip)
    {
        Offset = offset;
        RoundTrip = roundTrip;
    }

}
}