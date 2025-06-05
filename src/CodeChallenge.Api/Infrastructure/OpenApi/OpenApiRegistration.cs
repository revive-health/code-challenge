using Microsoft.OpenApi.Models;

namespace CodeChallenge.Api.Infrastructure.OpenApi;

public static class OpenApiRegistrations
{
    public static void AddOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "Member Management API";
                document.Info.Contact = new OpenApiContact
                {
                    Name = "ReviveHealth - Code Challenge",
                };
                return Task.CompletedTask;
            });
        });
    }
}

