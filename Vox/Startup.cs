using System;
using System.Reflection;
using Autofac;
using Dapper;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Vox.Data;
using Vox.Framework.Autofac;
using Vox.Framework.Database;
using Vox.Services.Discord.Client;
using Vox.Services.Discord.Client.Extensions;

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
        services.Configure<ConnectionOptions>(x => x.ConnectionString = _config.GetConnectionString("main"));
        services.Configure<DiscordClientOptions>(x => _config.GetSection("DiscordOptions").Bind(x));

        services
            .AddHealthChecks()
            .AddNpgSql(_config.GetConnectionString("main"))
            .AddDbContextCheck<AppDbContext>();

        services.AddDbContextPool<DbContext, AppDbContext>(o =>
        {
            o.UseNpgsql(_config.GetConnectionString("main"),
                s => { s.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name); });
        });

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddHangfireServer();
        services.AddHangfire(config =>
        {
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            config.UsePostgreSqlStorage(_config.GetConnectionString("main"));
        });

        services.AddMemoryCache();
        services.AddControllers();
        services.AddOpenApiDocument();

        services.AddSingleton(_ =>
            TimeZoneInfo.FindSystemTimeZoneById(_config.GetValue<string>("CronTimezoneId")));

        services.AddSingleton<CommandHandler>();
        services.AddSingleton<IDiscordClientService, DiscordClientService>();
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

    public void ConfigureContainer(ContainerBuilder builder)
    {
        var servicesAssembly = typeof(IDiscordClientService).Assembly;

        builder.RegisterAssemblyTypes(servicesAssembly)
            .Where(x => x.IsDefined(typeof(InjectableServiceAttribute), false) &&
                        x.GetCustomAttribute<InjectableServiceAttribute>().IsSingletone)
            .As(x => x.GetInterfaces()[0])
            .SingleInstance();

        builder.RegisterAssemblyTypes(servicesAssembly)
            .Where(x => x.IsDefined(typeof(InjectableServiceAttribute), false) &&
                        !x.GetCustomAttribute<InjectableServiceAttribute>().IsSingletone)
            .As(x => x.GetInterfaces()[0])
            .InstancePerLifetimeScope();
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
