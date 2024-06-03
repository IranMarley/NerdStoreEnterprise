namespace NSE.Identity.API.Extensions;

public class AppSettings
{
    public string Key { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int Expires { get; set; }
}