namespace JPlayer.Lib.Contract
{
    /// <summary>
    ///     Base class for search
    /// </summary>
    public abstract class ApiCriteria
    {
        protected ApiCriteria(int limit, int skip, SortDir sortDir, string sortField)
        {
            this.Limit = limit;
            this.Skip = skip;
            this.SortDir = sortDir;
            this.SortField = sortField;
        }

        /// <summary>
        ///     Max result to return
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        ///     Result to skip
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        ///     Sorting direction
        /// </summary>
        public SortDir SortDir { get; set; }

        /// <summary>
        ///     Field to sort
        /// </summary>
        public string SortField { get; set; }
    }

    /// <summary>
    ///     Sort direction
    /// </summary>
    public enum SortDir
    {
        Asc = 0,
        Desc = 1
    }
}