﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Vox.Data;

#nullable disable

namespace Vox.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Vox.Data.Entities.Guild", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<bool>("Removed")
                        .HasColumnType("boolean")
                        .HasColumnName("removed");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_guilds");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_guilds_id");

                    b.ToTable("guilds");
                });

            modelBuilder.Entity("Vox.Data.Entities.GuildCreateChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("CategoryId")
                        .HasColumnType("bigint")
                        .HasColumnName("category_id");

                    b.Property<long>("ChannelId")
                        .HasColumnType("bigint")
                        .HasColumnName("channel_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long>("GuildId")
                        .HasColumnType("bigint")
                        .HasColumnName("guild_id");

                    b.HasKey("Id")
                        .HasName("pk_guild_create_channels");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_guild_create_channels_guild_id");

                    b.HasIndex("GuildId", "CategoryId")
                        .IsUnique()
                        .HasDatabaseName("ix_guild_create_channels_guild_id_category_id");

                    b.ToTable("guild_create_channels");
                });

            modelBuilder.Entity("Vox.Data.Entities.Poll", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<int>("MaxAnswers")
                        .HasColumnType("integer")
                        .HasColumnName("max_answers");

                    b.HasKey("Id")
                        .HasName("pk_polls");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_polls_id");

                    b.ToTable("polls");
                });

            modelBuilder.Entity("Vox.Data.Entities.PollAnswer", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Answer")
                        .HasColumnType("text")
                        .HasColumnName("answer");

                    b.Property<Guid>("PollId")
                        .HasColumnType("uuid")
                        .HasColumnName("poll_id");

                    b.HasKey("Id")
                        .HasName("pk_poll_answers");

                    b.HasIndex("PollId", "Answer")
                        .IsUnique()
                        .HasDatabaseName("ix_poll_answers_poll_id_answer");

                    b.ToTable("poll_answers");
                });

            modelBuilder.Entity("Vox.Data.Entities.UserChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("ChannelLimit")
                        .HasColumnType("integer")
                        .HasColumnName("channel_limit");

                    b.Property<string>("ChannelName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("channel_name");

                    b.Property<string>("OverwritesData")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("overwrites_data");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_channels");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_channels_user_id");

                    b.ToTable("user_channels");
                });

            modelBuilder.Entity("Vox.Data.Entities.UserPollAnswer", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("AnswerId")
                        .HasColumnType("uuid")
                        .HasColumnName("answer_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("PollId")
                        .HasColumnType("uuid")
                        .HasColumnName("poll_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_poll_answers");

                    b.HasIndex("AnswerId")
                        .HasDatabaseName("ix_user_poll_answers_answer_id");

                    b.HasIndex("PollId")
                        .HasDatabaseName("ix_user_poll_answers_poll_id");

                    b.HasIndex("UserId", "PollId", "AnswerId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_poll_answers_user_id_poll_id_answer_id");

                    b.ToTable("user_poll_answers");
                });

            modelBuilder.Entity("Vox.Data.Entities.GuildCreateChannel", b =>
                {
                    b.HasOne("Vox.Data.Entities.Guild", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_guild_create_channels_guilds_guild_id");

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("Vox.Data.Entities.PollAnswer", b =>
                {
                    b.HasOne("Vox.Data.Entities.Poll", "Poll")
                        .WithMany()
                        .HasForeignKey("PollId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_poll_answers_polls_poll_id");

                    b.Navigation("Poll");
                });

            modelBuilder.Entity("Vox.Data.Entities.UserPollAnswer", b =>
                {
                    b.HasOne("Vox.Data.Entities.PollAnswer", "Answer")
                        .WithMany()
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_poll_answers_poll_answers_answer_id");

                    b.HasOne("Vox.Data.Entities.Poll", "Poll")
                        .WithMany()
                        .HasForeignKey("PollId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_poll_answers_polls_poll_id");

                    b.Navigation("Answer");

                    b.Navigation("Poll");
                });
#pragma warning restore 612, 618
        }
    }
}
