﻿// <auto-generated />
using System;
using DAMBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAMBackend.Migrations
{
    [DbContext(typeof(SQLDbContext))]
    partial class SQLDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DAMBackend.Models.FileModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float?>("Aperture")
                        .HasColumnType("real");

                    b.Property<string>("Copyright")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateTimeOriginal")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("FocalLength")
                        .HasColumnType("int");

                    b.Property<decimal?>("GPSAlt")
                        .HasPrecision(10, 3)
                        .HasColumnType("decimal(10,3)");

                    b.Property<decimal?>("GPSLat")
                        .HasPrecision(10, 7)
                        .HasColumnType("decimal(10,7)");

                    b.Property<decimal?>("GPSLon")
                        .HasPrecision(10, 7)
                        .HasColumnType("decimal(10,7)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Make")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Model")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OriginalPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Palette")
                        .HasColumnType("bit");

                    b.Property<int>("PixelHeight")
                        .HasColumnType("int");

                    b.Property<int>("PixelWidth")
                        .HasColumnType("int");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("Resolution")
                        .HasColumnType("int");

                    b.Property<string>("ThumbnailPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("ViewPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("DAMBackend.Models.FileTag", b =>
                {
                    b.Property<int>("FileId")
                        .HasColumnType("int");

                    b.Property<string>("TagId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("FileId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("FileTag", (string)null);
                });

            modelBuilder.Entity("DAMBackend.Models.LogImage", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogId"));

                    b.Property<int>("FileId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LogDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<string>("TypeOfLog")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LogId");

                    b.HasIndex("FileId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("LogImage");
                });

            modelBuilder.Entity("DAMBackend.Models.MetadataTagModel", b =>
                {
                    b.Property<int>("FileId")
                        .HasColumnType("int");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("iValue")
                        .HasColumnType("int");

                    b.Property<string>("sValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("type")
                        .HasColumnType("int");

                    b.HasKey("FileId", "Key");

                    b.ToTable("MetadataTags");
                });

            modelBuilder.Entity("DAMBackend.Models.ProjectBasicTag", b =>
                {
                    b.Property<string>("BasicTagValue")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.HasKey("BasicTagValue", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectBasicTag");
                });

            modelBuilder.Entity("DAMBackend.Models.ProjectModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessLevel")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isArchived")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("DAMBackend.Models.ProjectTagModel", b =>
                {
                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("iValue")
                        .HasColumnType("int");

                    b.Property<string>("sValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("type")
                        .HasColumnType("int");

                    b.HasKey("ProjectId", "Key");

                    b.ToTable("ProjectTags");
                });

            modelBuilder.Entity("DAMBackend.Models.TagBasicModel", b =>
                {
                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Value");

                    b.ToTable("BasicTags");
                });

            modelBuilder.Entity("DAMBackend.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProjectModelId")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ProjectModelId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DAMBackend.Models.UserProjectRelation", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<bool>("IsFavourite")
                        .HasColumnType("bit");

                    b.Property<bool>("WorkingOn")
                        .HasColumnType("bit");

                    b.HasKey("UserId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("UserProjectRelations");
                });

            modelBuilder.Entity("DAMBackend.Models.FileModel", b =>
                {
                    b.HasOne("DAMBackend.Models.ProjectModel", "Project")
                        .WithMany("Files")
                        .HasForeignKey("ProjectId");

                    b.HasOne("DAMBackend.Models.UserModel", "User")
                        .WithMany("Files")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAMBackend.Models.FileTag", b =>
                {
                    b.HasOne("DAMBackend.Models.FileModel", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DAMBackend.Models.TagBasicModel", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("File");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("DAMBackend.Models.LogImage", b =>
                {
                    b.HasOne("DAMBackend.Models.FileModel", "File")
                        .WithMany("Logs")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAMBackend.Models.ProjectModel", "Project")
                        .WithMany("Logs")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAMBackend.Models.UserModel", "User")
                        .WithMany("Logs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");

                    b.Navigation("Project");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAMBackend.Models.MetadataTagModel", b =>
                {
                    b.HasOne("DAMBackend.Models.FileModel", "File")
                        .WithMany("mTags")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("DAMBackend.Models.ProjectBasicTag", b =>
                {
                    b.HasOne("DAMBackend.Models.TagBasicModel", "BasicTag")
                        .WithMany()
                        .HasForeignKey("BasicTagValue")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAMBackend.Models.ProjectModel", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BasicTag");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("DAMBackend.Models.ProjectTagModel", b =>
                {
                    b.HasOne("DAMBackend.Models.ProjectModel", "Project")
                        .WithMany("Tags")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("DAMBackend.Models.UserModel", b =>
                {
                    b.HasOne("DAMBackend.Models.ProjectModel", null)
                        .WithMany("Users")
                        .HasForeignKey("ProjectModelId");
                });

            modelBuilder.Entity("DAMBackend.Models.UserProjectRelation", b =>
                {
                    b.HasOne("DAMBackend.Models.ProjectModel", "Project")
                        .WithMany("UserProjectRelations")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAMBackend.Models.UserModel", "User")
                        .WithMany("UserProjectRelations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAMBackend.Models.FileModel", b =>
                {
                    b.Navigation("Logs");

                    b.Navigation("mTags");
                });

            modelBuilder.Entity("DAMBackend.Models.ProjectModel", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("Logs");

                    b.Navigation("Tags");

                    b.Navigation("UserProjectRelations");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("DAMBackend.Models.UserModel", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("Logs");

                    b.Navigation("UserProjectRelations");
                });
#pragma warning restore 612, 618
        }
    }
}
