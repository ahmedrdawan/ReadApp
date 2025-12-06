using MyReadsApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyReadsApp.Infstructure.Data.Config
{
    public class FaviorateBookConfigration : IEntityTypeConfiguration<FaviorateBook>
    {
        public void Configure(EntityTypeBuilder<FaviorateBook> builder)
        {
            builder.HasKey(fb => new { fb.UserId, fb.BookId });

            builder.HasOne(fb => fb.User)
                .WithMany(u => u.FaviorateBooks)
                .HasForeignKey(fb => fb.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(fb => fb.Book)
                .WithMany(b => b.FaviorateBooks)
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
