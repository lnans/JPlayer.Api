namespace JPlayer.Data.Dto.Profile
{
    /// <summary>
    ///     Profile list item
    /// </summary>
    public class ProfileCollectionItem
    {
        /// <summary>
        ///     Profile id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Profile name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Determinate if the profile is in read only mode
        /// </summary>
        public bool ReadOnly { get; set; }
    }
}