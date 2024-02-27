﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace PII.Migrations
{
    [DbContext(typeof(PlanItContext))]
    partial class PlanItContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.0");

            modelBuilder.Entity("Disponibilite", b =>
                {
                    b.Property<int>("DisponibiliteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("NbHeure")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UtilisateurId")
                        .HasColumnType("INTEGER");

                    b.HasKey("DisponibiliteId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Disponibilite");
                });

            modelBuilder.Entity("Tache", b =>
                {
                    b.Property<int>("TacheId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("Deadline")
                        .HasColumnType("TEXT");

                    b.Property<int>("Duree")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UtilisateurId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TacheId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Taches");
                });

            modelBuilder.Entity("Todo", b =>
                {
                    b.Property<int>("TodoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("Duree")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Rates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Realisation")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TacheId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UtilisateurId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TodoId");

                    b.HasIndex("TacheId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Todos");
                });

            modelBuilder.Entity("TodoGET", b =>
                {
                    b.Property<int>("TodoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("Duree")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Rates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Realisation")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("UtilisateurId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TodoId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("TodoGET");
                });

            modelBuilder.Entity("Utilisateur", b =>
                {
                    b.Property<int>("UtilisateurId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Note")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Prenom")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UtilisateurId");

                    b.ToTable("Utilisateurs");
                });

            modelBuilder.Entity("Disponibilite", b =>
                {
                    b.HasOne("Utilisateur", "Utilisateur")
                        .WithMany("Disponibilites")
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("Tache", b =>
                {
                    b.HasOne("Utilisateur", "Utilisateur")
                        .WithMany("Taches")
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("Todo", b =>
                {
                    b.HasOne("Tache", "Tache")
                        .WithMany()
                        .HasForeignKey("TacheId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Utilisateur", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tache");

                    b.Navigation("Utilisateur");
                });

            modelBuilder.Entity("TodoGET", b =>
                {
                    b.HasOne("Utilisateur", null)
                        .WithMany("Todos")
                        .HasForeignKey("UtilisateurId");
                });

            modelBuilder.Entity("Utilisateur", b =>
                {
                    b.Navigation("Disponibilites");

                    b.Navigation("Taches");

                    b.Navigation("Todos");
                });
#pragma warning restore 612, 618
        }
    }
}
