using System;
using System.Collections.Generic;

namespace JPlayer.Data.Dao.Model
{
    public class UsrUserDao
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public bool Deactivated { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? LastConnectionDate { get; set; }

        public virtual ICollection<UsrUserProfileDao> UserProfiles { get; set; }

        public bool ReadOnly { get; set; }
    }
}