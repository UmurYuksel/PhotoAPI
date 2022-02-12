using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoAPI.Models
{
    /// <summary>
    /// Main Database operation Class.
    /// </summary>
    public class Photo
    {

        public Guid Id { get; set; }
        public int UserId { get; set; }
        //I have watched the FotoQuest app demo on youtube and came up with this idea, I am assuming that every photo belongs to some unique marker id.
        public int MarkerId { get; set; }
        public string? ImagePath { get; set; }
        public string? ImageType { get; set; }
        public string? ImageHeight { get; set; }
        public string? ImageWidth { get; set; }

        [NotMapped]
        public string? Base64 { get; set; }

    }
}
