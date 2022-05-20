using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vox.Data.Util;

namespace Vox.Data.Entities;

public class GuildCreateChannel : IUniqueIdentifiedEntity, ICreatedEntity
{
    public Guid Id { get; set; }
    public long GuildId { get; set; }
    public long CategoryId { get; set; }
    public long ChannelId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Guild Guild { get; set; }
}

public class GuildCreateChannelConfiguration : IEntityTypeConfiguration<GuildCreateChannel>
{
    public void Configure(EntityTypeBuilder<GuildCreateChannel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.GuildId);
        builder.HasIndex(x => new {x.GuildId, x.CategoryId}).IsUnique();

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.CategoryId).IsRequired();
        builder.Property(x => x.ChannelId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder
            .HasOne(x => x.Guild)
            .WithMany()
            .HasForeignKey(x => x.GuildId);
    }
}