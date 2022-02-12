namespace PhotoAPI.DTO_s
{
    /// <summary>
    /// Pagination Model
    /// </summary>
    public class PhotoQueryResponseDTO
    {
        public List<PhotoResponseDTO> Photos { get; set; }
        public int Pages { get; set; }

        public int CurrentPage { get; set; }
    }
}
