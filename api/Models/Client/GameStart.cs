using System.ComponentModel.DataAnnotations;

namespace api.models.client
{
    public class GameStart
    {
        [Required]
        public BoardDisplay Board { get; set; } = new BoardDisplay();

        [Required]
        public string? GameID { get; set; }
    }
}
