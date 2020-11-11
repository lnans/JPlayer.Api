using JPlayer.Lib.Contract;

namespace JPlayer.Data.Dto.Function
{
    /// <summary>
    ///     Search filter for functions
    /// </summary>
    public class FunctionCriteria : ApiCriteria
    {
        public FunctionCriteria() : base(int.MaxValue - 1, 0, SortDir.Asc, nameof(FunctionCode))
        {
        }

        /// <summary>
        ///     Search by code
        /// </summary>
        public string FunctionCode { get; set; }
    }
}