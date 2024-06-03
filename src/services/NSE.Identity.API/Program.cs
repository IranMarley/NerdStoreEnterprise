using NSE.Identity.API.Configurations;

public class Program
{
    private static WebApplicationBuilder _builder;
    private static WebApplication _app;
    
    public static void Main(string[] args)
    {
        _builder = WebApplication.CreateBuilder(args);

        ConfigureServices();

        _app = _builder.Build();

        ConfigureRequestsPipeline();

        _app.Run();
    }
    
    private static void ConfigureServices()
    {
        _builder.AddIdentityConfiguration();
        _builder.AddJwtConfiguration();
        _builder.AddApiConfiguration();
        _builder.AddSwaggerConfiguration();
    }
    
    private static void ConfigureRequestsPipeline()
    {
        _app.UseApiConfiguration();
        _app.UseSwaggerConfiguration();
    }
}