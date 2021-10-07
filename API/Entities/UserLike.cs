namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUserRef{get;set;}
        public int     SourceUserId{get;set;}

        public AppUser LikedUserRef{get;set;}
        public int     LikedUserId{get;set;}
    }
}