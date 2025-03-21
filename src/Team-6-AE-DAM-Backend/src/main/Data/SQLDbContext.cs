using Microsoft.EntityFrameworkCore;

namespace DAMBackend.Models

{
    public class SQLDbContext : DbContext
    {
        public SQLDbContext(DbContextOptions<SQLDbContext> options) : base(options) { }

        public DbSet<ProjectModel> Projects { get; set; }

        public DbSet<FileModel> Files { get; set; }
        
        public DbSet<UserModel> Users { get; set; }

        public DbSet<MetadataTagModel> MetadataTags { get; set; }

        public DbSet<TagBasicModel> BasicTags { get; set; }
        
        public DbSet<ProjectTagModel> ProjectTags { get; set; }
        
        public DbSet<UserProjectRelation> UserProjectRelations { get; set; }
        
        

        protected override void OnModelCreating(ModelBuilder modelBuilder) 

        {
            modelBuilder.Entity<UserProjectRelation>()
                .HasOne(ufp => ufp.User)
                .WithMany(u => u.UserProjectRelations)
                .HasForeignKey(ufp => ufp.UserId)
                .OnDelete(DeleteBehavior.Cascade);



            // Key for basic data tag model
            modelBuilder.Entity<TagBasicModel>()
                .HasKey(m => new { m.Value });
            


            // Key for metadata tag model
            modelBuilder.Entity<MetadataTagModel>()
                .HasKey(m => new { m.FileId, m.Key });
            
            // Key for project tag model
            modelBuilder.Entity<ProjectTagModel>()
                .HasKey(t => new { t.ProjectId, t.Key });
            
            // Delete tags with Project
            modelBuilder.Entity<ProjectTagModel>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tags)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            
                
            // One to many betwen file and metadatatag model
            modelBuilder.Entity<FileModel>()
                .Ignore(f => f.bTags)
                .HasMany(f => f.mTags)
                .WithOne(t => t.File)
                .HasForeignKey(t => t.FileId)
                .IsRequired();
            // Many to many betwen file and basictag model
            modelBuilder.Entity<FileModel>()
                .HasMany(f => f.bTags)
                .WithMany(t => t.Files) 
                .UsingEntity<Dictionary<string, object>>(
                    "FileTag", 
                    j => j.HasOne<TagBasicModel>().WithMany().HasForeignKey("TagId"),
                    j => j.HasOne<FileModel>().WithMany().HasForeignKey("FileId")
                );
            modelBuilder.Entity<FileModel>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();



            // Configuring the many-to-many relationship with User and Project in the join table
            modelBuilder.Entity<UserProjectRelation>()
                .HasKey(ufp => new { ufp.UserId, ufp.ProjectId });
            
            // Configure the relationship between User and UserProjectRelation
            modelBuilder.Entity<UserProjectRelation>()
                .HasOne(ufp => ufp.User)  
                .WithMany(u => u.UserProjectRelations) 
                .HasForeignKey(ufp => ufp.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User

            // Configure the relationship between Project and UserProjectRelation
            modelBuilder.Entity<UserProjectRelation>()
                .HasOne(ufp => ufp.Project) 
                .WithMany(p => p.UserProjectRelations) 
                .HasForeignKey(ufp => ufp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // one to many between user and files
            modelBuilder.Entity<FileModel>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .IsRequired();
            
            // one to many between project and project tags
            modelBuilder.Entity<ProjectModel>()
                .HasMany(p => p.Tags)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .IsRequired();
            modelBuilder.Entity<ProjectModel>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            // One to many from projects to files
            modelBuilder.Entity<ProjectModel>()
                .HasMany(p => p.Files)
                .WithOne(f => f.Project)
                .HasForeignKey(f => f.ProjectId);


            // generate int for userid
            modelBuilder.Entity<UserModel>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
        }
    }
}











// using Microsoft.EntityFrameworkCore;

// namespace DAMBackend.Models
// {
//     public class SQLDbContext : DbContext
//     {
//         public SQLDbContext(DbContextOptions<SQLDbContext> options) : base(options) { }

//         public DbSet<ProjectModel> Projects { get; set; }
//         public DbSet<FileModel> Files { get; set; }
//         public DbSet<UserModel> Users { get; set; }
//         public DbSet<MetadataTagModel> MetadataTags { get; set; }
//         public DbSet<TagBasicModel> BasicTags { get; set; }
//         public DbSet<UserFavouriteProject> UserFavouriteProjects { get; set; }

//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             // ✅ Define composite primary key for UserFavouriteProject
//             modelBuilder.Entity<UserFavouriteProject>()
//                 .HasKey(ufp => new { ufp.UserId, ufp.ProjectId });

//             modelBuilder.Entity<UserFavouriteProject>()
//                 .HasOne(ufp => ufp.User)
//                 .WithMany(u => u.FavoritedProjects)
//                 .HasForeignKey(ufp => ufp.UserId);

//             modelBuilder.Entity<UserFavouriteProject>()
//                 .HasOne(ufp => ufp.Project)
//                 .WithMany(p => p.FavoritedByUsers)
//                 .HasForeignKey(ufp => ufp.ProjectId);

//             // ✅ Key for basic data tag model
//             modelBuilder.Entity<TagBasicModel>()
//                 .HasKey(m => new { m.Value });

//             // ✅ Key for metadata tag model
//             modelBuilder.Entity<MetadataTagModel>()
//                 .HasKey(m => new { m.FileId, m.Key });

//             // ✅ One-to-Many between FileModel and MetadataTagModel
//             modelBuilder.Entity<FileModel>()
//                 .Ignore(f => f.bTags)
//                 .HasMany(f => f.mTags)
//                 .WithOne(t => t.File)
//                 .HasForeignKey(t => t.FileId)
//                 .IsRequired();

//             // ✅ Many-to-Many between FileModel and TagBasicModel
//             modelBuilder.Entity<FileModel>()
//                 .HasMany(f => f.bTags)
//                 .WithMany(t => t.Files)
//                 .UsingEntity<Dictionary<string, object>>(
//                     "FileTag",
//                     j => j.HasOne<TagBasicModel>().WithMany().HasForeignKey("TagId"),
//                     j => j.HasOne<FileModel>().WithMany().HasForeignKey("FileId")
//                 );

//             // ✅ One-to-Many between ProjectModel and FileModel
//             modelBuilder.Entity<ProjectModel>()
//                 .HasMany(p => p.Files)
//                 .WithOne(f => f.Project)
//                 .HasForeignKey(f => f.ProjectId);

//             // ✅ Many-to-Many between UserModel and ProjectModel (via UserFavouriteProject)
//             modelBuilder.Entity<UserModel>()
//                 .HasMany(u => u.Projects)
//                 .WithMany(p => p.Users);

//             // ✅ One-to-Many between UserModel and FileModel
//             modelBuilder.Entity<FileModel>()
//                 .HasOne(f => f.User)
//                 .WithMany(u => u.Files)
//                 .HasForeignKey(f => f.UserId)
//                 .IsRequired();

//             // ✅ Configure auto-incrementing primary keys
//             modelBuilder.Entity<UserModel>()
//                 .Property(u => u.Id)
//                 .ValueGeneratedOnAdd();

//             modelBuilder.Entity<FileModel>()
//                 .Property(f => f.Id)
//                 .ValueGeneratedOnAdd();

//             modelBuilder.Entity<ProjectModel>()
//                 .Property(p => p.Id)
//                 .ValueGeneratedOnAdd();

//             // ✅ Define precision for decimal fields in FileModel
//             modelBuilder.Entity<FileModel>()
//                 .Property(f => f.GPSLat)
//                 .HasPrecision(10, 7);

//             modelBuilder.Entity<FileModel>()
//                 .Property(f => f.GPSLon)
//                 .HasPrecision(10, 7);

//             modelBuilder.Entity<FileModel>()
//                 .Property(f => f.GPSAlt)
//                 .HasPrecision(10, 3);
//         }
//     }
// }
