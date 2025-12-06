using MyReadsApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyReadsApp.Infstructure.Data.Config
{
    public class PostConfigration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasOne(p=>p.User)
                .WithMany(u=>u.Posts)
                .HasForeignKey(p=>p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Book)
                .WithMany(b => b.Posts)
                .HasForeignKey(p => p.BookId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
