using MyReadsApp.Core.Enums;

namespace MyReadsApp.API.DTOs.FriendShip
{
    /// <summary>
    /// Request DTO for creating a friendship containing friendship status.
    /// </summary>
    public class CreateFriendShipRequest
    {
        /// <summary>
        /// Gets or sets the status of the friendship.
        /// </summary>
        public FriendShipStatus Status { get; set; }
    }
}
