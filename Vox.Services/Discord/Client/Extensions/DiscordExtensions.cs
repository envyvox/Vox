using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Vox.Services.Discord.Client.Extensions;

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
