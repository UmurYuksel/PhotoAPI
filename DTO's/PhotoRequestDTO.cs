namespace PhotoAPI.DTO_s
{
    /// <summary>
    ///  Api Request Model
    /// </summary>
    public class PhotoRequestDTO
    {
        /// <summary>
        ///  =>**Can be send as empty, just uncheck the send empty value
        /// </summary>
        public Guid Id { get; set; }
        public int UserId { get; set; }

        //I have watched the FotoQuest app demo on youtube and came up with this idea, I am assuming that every photo belongs to some unique marker id.
        public int MarkerId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
