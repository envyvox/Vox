using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vox.Data.Entities;

public class CreateChannel
{
    public Guid Id { get; set; }
    public long GuildId { get; set; }
    public long CategoryId { get; set; }
    public long ChannelId { get; set; }
}

public class GuildCreateChannelConfiguration : IEntityTypeConfiguration<CreateChannel>
{
    public void Configure(EntityTypeBuilder<CreateChannel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.GuildId);
        builder.HasIndex(x => new { x.GuildId, x.CategoryId }).IsUnique();

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.CategoryId).IsRequired();
        builder.Property(x => x.ChannelId).IsRequired();
    }
}
