using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vox.Data.Util;

namespace Vox.Data.Entities;

public class Poll : IUniqueIdentifiedEntity, ICreatedEntity
{
    public Guid Id { get; set; }
    public long GuildId { get; set; }
    public long ChannelId { get; set; }
    public long MessageId { get; set; }
    public int MaxAnswers { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Guild Guild { get; set; }
}

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new {x.GuildId, x.ChannelId, x.MessageId}).IsUnique();

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.MaxAnswers).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder
            .HasOne(x => x.Guild)
            .WithMany()
            .HasForeignKey(x => x.GuildId);
    }
}