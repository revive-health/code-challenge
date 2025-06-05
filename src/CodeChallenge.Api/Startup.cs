using Scalar.AspNetCore;

namespace CodeChallenge.Api;

public class Startup
{
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddOpenApi();
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapOpenApi().CacheOutput();
                endpoints.MapScalarApiReference("api-doc");
            });
        }
}