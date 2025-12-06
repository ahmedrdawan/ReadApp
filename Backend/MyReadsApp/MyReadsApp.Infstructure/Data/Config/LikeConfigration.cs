using MyReadsApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyReadsApp.Infstructure.Data.Config
{
    public class LikeConfigration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(l=>new {l.UserId, l.PostId});

            builder.HasOne(l=>l.User)
                .WithMany(u=>u.Likes)
                .HasForeignKey(l=>l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
