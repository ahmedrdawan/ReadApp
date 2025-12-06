using MyReadsApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyReadsApp.Infstructure.Data.Config
{
    public class FriendShipConfigration : IEntityTypeConfiguration<FriendShip>
    {
        public void Configure(EntityTypeBuilder<FriendShip> builder)
        {

            builder.HasOne(fs => fs.User)
                .WithMany(u => u.SentFriendShips)
                .HasForeignKey(fs => fs.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(fs => fs.FriendUser)
                .WithMany(u => u.ReceivedFriendShips)
                .HasForeignKey(fs => fs.FriendId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
