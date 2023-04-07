using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vox.Data.Util;

namespace Vox.Data.Entities;

public class UserPollAnswer : IUniqueIdentifiedEntity, ICreatedEntity
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public Guid PollId { get; set; }
    public Guid AnswerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Poll Poll { get; set; }
    public PollAnswer Answer { get; set; }
}

public class UserPollAnswerConfiguration : IEntityTypeConfiguration<UserPollAnswer>
{
    public void Configure(EntityTypeBuilder<UserPollAnswer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new {x.UserId, x.PollId, x.AnswerId}).IsUnique();

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder
            .HasOne(x => x.Poll)
            .WithMany()
            .HasForeignKey(x => x.PollId);
    }
}