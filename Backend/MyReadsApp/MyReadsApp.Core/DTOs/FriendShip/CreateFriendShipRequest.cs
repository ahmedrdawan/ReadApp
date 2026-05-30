using System;

namespace MyReadsApp.Core.DTOs.FriendShip
{
    /// <summary>
    /// Request DTO for creating a friendship relationship.
    /// </summary>
    public record CreateFriendShipRequest(
        /// <summary>
        /// Identifier of the user who initiated the friendship.
        /// </summary>
        Guid UserId,

        /// <summary>
        /// Identifier of the user to be friended.
        /// </summary>
        Guid FriendId,

        /// <summary>
        /// Friendship status value (e.g., pending, accepted, blocked).
        /// </summary>
        int Status
    );
}