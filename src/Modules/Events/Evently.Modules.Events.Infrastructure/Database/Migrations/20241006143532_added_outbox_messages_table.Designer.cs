﻿// <auto-generated />
using System;
using Evently.Modules.Events.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Evently.Modules.Events.Infrastructure.Database.Migrations
{
    [DbContext(typeof(EventsDbContext))]
    [Migration("20241006143532_added_outbox_messages_table")]
    partial class added_outbox_messages_table
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("events")
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Evently.Common.Infrastructure.Outbox.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("jsonb")
                        .HasColumnName("content");

                    b.Property<string>("Error")
                        .HasColumnType("text")
                        .HasColumnName("error");

                    b.Property<DateTime>("OccuredOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("occured_on_utc");

                    b.Property<DateTime?>("ProcessedOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("processed_on_utc");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_outbox_message");

                    b.ToTable("outbox_message", "events");
                });

            modelBuilder.Entity("Evently.Modules.Events.Domain.Categories.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("boolean")
                        .HasColumnName("is_archived");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_categories");

                    b.ToTable("categories", "events");
                });

            modelBuilder.Entity("Evently.Modules.Events.Domain.Events.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("category_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<DateTime?>("EndsAtUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("ends_at_utc");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("location");

                    b.Property<DateTime>("StartsAtUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("starts_at_utc");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_events");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_events_category_id");

                    b.ToTable("events", "events");
                });

            modelBuilder.Entity("Evently.Modules.Events.Domain.TicketTypes.TicketType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("currency");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("price");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric")
                        .HasColumnName("quantity");

                    b.HasKey("Id")
                        .HasName("pk_ticket_types");

                    b.HasIndex("EventId")
                        .HasDatabaseName("ix_ticket_types_event_id");

                    b.ToTable("ticket_types", "events");
                });

            modelBuilder.Entity("Evently.Modules.Events.Domain.Events.Event", b =>
                {
                    b.HasOne("Evently.Modules.Events.Domain.Categories.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_events_categories_category_id");
                });

            modelBuilder.Entity("Evently.Modules.Events.Domain.TicketTypes.TicketType", b =>
                {
                    b.HasOne("Evently.Modules.Events.Domain.Events.Event", null)
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_ticket_types_events_event_id");
                });
#pragma warning restore 612, 618
        }
    }
}
