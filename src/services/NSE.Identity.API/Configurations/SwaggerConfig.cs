using Microsoft.OpenApi.Models;

namespace NSE.Identity.API.Configurations;

public static class SwaggerConfig
{
    public static WebApplicationBuilder AddSwaggerConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "NerdStore Enterprise Identity API",
                Description = "",
                Contact = new OpenApiContact
                {
                    Name = "Iran Marley",
                    Email = "iran_marley@live.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
        });

        return builder;
    }

    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(config => { config.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });
        }

        return app;
    }
}