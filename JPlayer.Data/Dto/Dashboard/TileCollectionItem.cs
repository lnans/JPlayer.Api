namespace JPlayer.Data.Dto.Dashboard
{
    /// <summary>
    ///     Dashboard tile information
    /// </summary>
    public class TileCollectionItem
    {
        /// <summary>
        ///     Tile label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     Tile sub label
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///     Tile link
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        ///     Tile info count
        /// </summary>
        public int Count { get; set; }
    }
}