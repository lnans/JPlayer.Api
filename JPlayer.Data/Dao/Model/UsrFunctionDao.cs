using System.Collections.Generic;

namespace JPlayer.Data.Dao.Model
{
    public class UsrFunctionDao
    {
        public int Id { get; set; }

        public string FunctionCode { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public virtual ICollection<UsrProfileFunctionDao> ProfileFunctions { get; set; }
    }
}