using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Infstructure.Data.Config;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyReadsApp.Infstructure.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<FriendShip> FriendShips { get; set; }
        public DbSet<FaviorateBook> FaviorateBooks { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {   
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(UserConfigration).Assembly);
        }
    }
}
