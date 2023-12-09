﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TournamentScheduler.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231209132248_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Competition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("DefeatPoints")
                        .HasColumnType("real");

                    b.Property<float>("DrawPoints")
                        .HasColumnType("real");

                    b.Property<string>("Teams")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<float>("VictoryPoints")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Competitions");
                });

            modelBuilder.Entity("Match", b =>
                {
                    b.Property<int>("MatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MatchId"));

                    b.Property<int>("CompetitionId")
                        .HasColumnType("integer");

                    b.Property<string>("Outcome")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Team1")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Team2")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MatchId");

                    b.HasIndex("CompetitionId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("Match", b =>
                {
                    b.HasOne("Competition", "Competition")
                        .WithMany()
                        .HasForeignKey("CompetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Competition");
                });
#pragma warning restore 612, 618
        }
    }
}
