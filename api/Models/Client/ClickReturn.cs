using System.ComponentModel.DataAnnotations;

namespace api.models.client
{
    public class ClickReturn
    {
        [Required]
        public BoardDisplay Board { get; set; } = new BoardDisplay();

        [Required]
        public bool Moved { get; set; } = false;
    }
}
