using MyReadsApp.Core.Enums;

namespace MyReadsApp.Core.DTOs.FriendShip.Response
{
    public class FriendShipResponse : BaseFriendShipResponse
    {
        public string SendUserName { get; set; }
        public string ReceivedUserName { get; set; }
    }
    public class BaseFriendShipResponse
    {
        public DateTime CreatedAt { get; set; }
        public FriendShipStatus Status { get; set; }
    }
}
