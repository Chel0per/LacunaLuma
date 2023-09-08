
namespace Lacuna
{
class Response
{
    public string? AccessToken { get; set; }
    public string? Code { get; set; }
    public string? Message { get; set; }
    public List<Probe>? Probes { get; set; }
    public string? T1 { get; set; }
    public string? T2 { get; set; }
}
}