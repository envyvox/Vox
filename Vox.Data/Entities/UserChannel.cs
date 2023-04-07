using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vox.Data.Entities;

public class UserChannel
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public string ChannelName { get; set; }
    public int ChannelLimit { get; set; }
    public string OverwritesData { get; set; }
}

public class UserChannelConfiguration : IEntityTypeConfiguration<UserChannel>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserChannel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId).IsUnique();

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.ChannelName).IsRequired();
        builder.Property(x => x.ChannelLimit).IsRequired();
        builder.Property(x => x.OverwritesData).IsRequired();
    }
}
