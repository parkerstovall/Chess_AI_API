using System.ComponentModel.DataAnnotations;

namespace ChessApi.Models.API
{
    public class OpenGame
    {
        [Required]
        public Guid GameID { get; set; }

        [Required]
        public DateTime LastPing { get; set; }
    }
}
