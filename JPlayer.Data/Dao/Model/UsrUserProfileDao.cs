namespace JPlayer.Data.Dao.Model
{
    public class UsrUserProfileDao
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProfileId { get; set; }

        public virtual UsrUserDao User { get; set; }

        public virtual UsrProfileDao Profile { get; set; }
    }
}