using System;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Vox.Data;
using Vox.Services.Discord.Client;
using Vox.Services.Discord.Extensions;
using Vox.Services.Hangfire.CompletePoll;

namespace Vox;

public class Startup
{
    private readonly IConfiguration _config;

    public Startup(IConfiguration config)
    {
        _config = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<DiscordClientOptions>(x => _config.GetSection("DiscordOptions").Bind(x));

        services.AddDbContextPool<AppDbContext>(o =>
        {
            o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            o.UseNpgsql(_config.GetConnectionString("main"),
                s => { s.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name); });
        });

        services.AddHangfireServer();
        services.AddHangfire(config =>
        {
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            config.UsePostgreSqlStorage(_config.GetConnectionString("main"));
        });

        services.AddAutoMapper(typeof(IDiscordClientService).Assembly);
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(IDiscordClientService).Assembly));
        services.AddMemoryCache();

        services
            .AddControllers()
            .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            );

        services.AddOpenApiDocument();

        services.AddSingleton(_ =>
            TimeZoneInfo.FindSystemTimeZoneById(_config.GetValue<string>("CronTimezoneId")));

        services.AddSingleton<CommandHandler>();
        services.AddSingleton<IDiscordClientService, DiscordClientService>();

        services.AddScoped<ICompletePollJob, CompletePollJob>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        MigrateDb(app.ApplicationServices);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new AllowAllAuthorizationFilter() }
        });

        app.UseSerilogRequestLogging();

        app.UseRouting();

        app.UseCors(options => options
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .Build());

        app.UseOpenApi();
        app.UseSwaggerUi3();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.StartDiscord();
    }

    private static void MigrateDb(IServiceProvider serviceProvider)
    {
        using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }

    private class AllowAllAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
