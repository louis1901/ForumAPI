namespace ForumApiTest.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using Microsoft.AspNet.Identity;

    public class ApplicationUser : IdentityUser
    {
        public DateTime RegisterTime { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationUserInfo
    {        
        public string id { get; set; }
        public string Nickname { get; set; }
        public string Signature { get; set; }
        public string Avatar { get; set; }
        public int ArticleCount { get; set; }
        public int FellowCount { get; set; }
        public DateTime UpdateTime { get; set; }
        [ForeignKey("id")]
        public ApplicationUser ApplicationUser { get; set; }
    }

    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUserInfo> AspNetUserInfo { get; set; }
        public ApplicationDbContext()
            : base("name=DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //}
    }
}
