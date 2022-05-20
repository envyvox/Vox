using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vox.Data.Util;

namespace Vox.Data.Entities;

public class PollAnswer : IUniqueIdentifiedEntity, ICreatedEntity
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public Guid PollId { get; set; }
    public string Answer { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Poll Poll { get; set; }
}

public class PollAnswerConfiguration : IEntityTypeConfiguration<PollAnswer>
{
    public void Configure(EntityTypeBuilder<PollAnswer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new {x.UserId, x.PollId, x.Answer}).IsUnique();

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder
            .HasOne(x => x.Poll)
            .WithMany()
            .HasForeignKey(x => x.PollId);
    }
}