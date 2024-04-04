using Microsoft.Identity.Client;

namespace ListeningCompanionDataService.Models.View
{
    public class UserDetails
    {
        public int UserID { get; set; }
        public List<UserShowDetails> UserShowDetails{ get; set; }
        public List<UserSongDetails> UserSongDetails { get; set; }
    }
}
