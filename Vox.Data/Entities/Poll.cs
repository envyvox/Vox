using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vox.Data.Util;

namespace Vox.Data.Entities;

public class Poll : IUniqueIdentifiedEntity, ICreatedEntity
{
    public Guid Id { get; set; }
    public int MaxAnswers { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.MaxAnswers).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}