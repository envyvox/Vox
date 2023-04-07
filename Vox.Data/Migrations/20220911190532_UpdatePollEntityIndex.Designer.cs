﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Vox.Data;

#nullable disable

namespace Vox.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220911190532_UpdatePollEntityIndex")]
    partial class UpdatePollEntityIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Vox.Data.Entities.Guild", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<int>("CreateRoomLimit")
                        .HasColumnType("integer")
                        .HasColumnName("create_room_limit");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

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

                    b.Property<Guid?>("PollId1")
                        .HasColumnType("uuid")
                        .HasColumnName("poll_id1");

                    b.HasKey("Id")
                        .HasName("pk_poll_answers");

                    b.HasIndex("PollId1")
                        .HasDatabaseName("ix_poll_answers_poll_id1");

                    b.HasIndex("PollId", "Answer")
                        .IsUnique()
                        .HasDatabaseName("ix_poll_answers_poll_id_answer");

                    b.ToTable("poll_answers");
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

                    b.HasOne("Vox.Data.Entities.Poll", null)
                        .WithMany("Answers")
                        .HasForeignKey("PollId1")
                        .HasConstraintName("fk_poll_answers_polls_poll_id1");

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

            modelBuilder.Entity("Vox.Data.Entities.Poll", b =>
                {
                    b.Navigation("Answers");
                });
#pragma warning restore 612, 618
        }
    }
}
