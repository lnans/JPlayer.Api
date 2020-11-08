using System.Collections.Generic;

namespace JPlayer.Data.Dao.Model
{
    public class UsrProfileDao
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<UsrUserProfileDao> UserProfiles { get; set; }

        public virtual ICollection<UsrProfileFunctionDao> ProfileFunctions { get; set; }

        public bool ReadOnly { get; set; }
    }
}