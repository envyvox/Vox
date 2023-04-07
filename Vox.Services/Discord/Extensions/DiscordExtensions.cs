using Microsoft.Extensions.DependencyInjection;
using Vox.Services.Discord.Client;
using Microsoft.AspNetCore.Builder;

namespace Vox.Services.Discord.Extensions;

public static class DiscordExtensions
{
    public static IApplicationBuilder StartDiscord(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var service = serviceScope.ServiceProvider.GetRequiredService<IDiscordClientService>();
        service.Start().Wait();

        return app;
    }
}