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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

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

            modelBuilder.Entity("DAMBackend.Models.MetadataTagModel", b =>
                {
                    b.Property<Guid>("FileId")
                        .HasColumnType("uniqueidentifier");

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

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("DAMBackend.Models.ProjectTagModel", b =>
                {
                    b.Property<int>("ProjectId")
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

            modelBuilder.Entity("DAMBackend.Models.UserFavouriteProject", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<bool>("IsFavourite")
                        .HasColumnType("bit");

                    b.HasKey("UserId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("UserFavouriteProjects");
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

            modelBuilder.Entity("FileTag", b =>
                {
                    b.Property<Guid>("FileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TagId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("FileId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("FileTag");
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

            modelBuilder.Entity("DAMBackend.Models.MetadataTagModel", b =>
                {
                    b.HasOne("DAMBackend.Models.FileModel", "File")
                        .WithMany("mTags")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
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

            modelBuilder.Entity("DAMBackend.Models.UserFavouriteProject", b =>
                {
                    b.HasOne("DAMBackend.Models.ProjectModel", "Project")
                        .WithMany("UserAccess")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAMBackend.Models.UserModel", "User")
                        .WithMany("UserFavouriteProjects")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAMBackend.Models.UserModel", b =>
                {
                    b.HasOne("DAMBackend.Models.ProjectModel", null)
                        .WithMany("Users")
                        .HasForeignKey("ProjectModelId");
                });

            modelBuilder.Entity("FileTag", b =>
                {
                    b.HasOne("DAMBackend.Models.FileModel", null)
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAMBackend.Models.TagBasicModel", null)
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DAMBackend.Models.FileModel", b =>
                {
                    b.Navigation("mTags");
                });

            modelBuilder.Entity("DAMBackend.Models.ProjectModel", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("Tags");

                    b.Navigation("UserAccess");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("DAMBackend.Models.UserModel", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("UserFavouriteProjects");
                });
#pragma warning restore 612, 618
        }
    }
}
