namespace JPlayer.Data.Dao.Model
{
    public class UsrProfileFunctionDao
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public int FunctionId { get; set; }

        public virtual UsrProfileDao Profile { get; set; }

        public virtual UsrFunctionDao Function { get; set; }
    }
}