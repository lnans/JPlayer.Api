using System;

namespace JPlayer.Data.Dao.Model.User
{
    public class UserDao
    {
        public long Id { get; set; }

        public string Login { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastConnectionDate { get; set; }
    }
}