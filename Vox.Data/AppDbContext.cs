using System;
using Microsoft.EntityFrameworkCore;
using Vox.Data.Converters;
using Vox.Data.Entities;
using Vox.Data.Extensions;

namespace Vox.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.UseEntityTypeConfiguration<AppDbContext>();
        modelBuilder.UseSnakeCaseNamingConvention();
        modelBuilder.UseValueConverterForType<DateTime>(new DateTimeUtcKindConverter());
    }

    public DbSet<Guild> Guilds { get; set; }
    public DbSet<GuildCreateChannel> GuildCreateChannels { get; set; }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<PollAnswer> PollAnswers { get; set; }
    public DbSet<UserChannel> UserChannels { get; set; }
    public DbSet<UserPollAnswer> UserPollAnswers { get; set; }
}
