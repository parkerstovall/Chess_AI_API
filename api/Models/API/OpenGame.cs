using System.ComponentModel.DataAnnotations;

namespace api.models.api
{
    public class OpenGame
    {
        [Required]
        public Guid GameID { get; set; }

        [Required]
        public DateTime LastPing { get; set; }
    }
}
