using System.Collections.Generic;

namespace JPlayer.Lib
{
    /// <summary>
    ///     Paginated generic object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T> where T : class
    {
        /// <summary>
        ///     Total count
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        ///     Data list
        /// </summary>
        public IEnumerable<T> List { get; set; }

        /// <summary>
        ///     Items count
        /// </summary>
        public int Taked { get; set; }

        /// <summary>
        ///     Items skipped
        /// </summary>
        public int Skipped { get; set; }
    }
}