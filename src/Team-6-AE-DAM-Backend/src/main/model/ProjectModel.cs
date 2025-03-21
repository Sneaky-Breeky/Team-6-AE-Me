using System.Data.SqlTypes;

namespace DAMBackend.Models


{
    public enum AccessLevel {
        Admin,
        Everyone,
        Selected_Users
    }

    public class ProjectModel
    {
        
        public int Id { get; set; }

        public required string Description {get; set;}

        public required string Name { get; set; }

        public string Status { get; set; }

        public string? Location { get; set; }

        public string? ImagePath {get; set; }

        public AccessLevel AccessLevel {get; set;}

        public string Phase { get; set;}

        public DateTime LastUpdate { get; set; }
        // change in ER diagram

        public ICollection<FileModel> Files { get; set;} = new HashSet<FileModel>();

        public ICollection<UserModel> Users { get; set;} = new HashSet<UserModel>();

        public ICollection<ProjectTagModel> Tags { get; set; } = new HashSet<ProjectTagModel>();
        public ICollection<UserProjectRelation> UserProjectRelations { get; set; } = new HashSet<UserProjectRelation>();
    }
}