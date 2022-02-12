namespace PhotoAPI.DTO_s
{
    public class PhotoResponseDTO
    {

        public Guid Id { get; set; }
        public int UserId { get; set; }
        public int MarkerId { get; set; }
        //Metadata related properties.
        public string? ImageType { get; set; }
        public string? ImageHeight { get; set; }
        public string? ImageWidth { get; set; }
        public string? Base64 { get; set; }
    }
}
