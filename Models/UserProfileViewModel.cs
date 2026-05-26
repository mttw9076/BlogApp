namespace WebApplication1.Models
{
    public class UserProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public int PostsCount { get; set; }
        public int FriendsCount { get; set; }
        public int SubscriptionsCount { get; set; }
    }
}
