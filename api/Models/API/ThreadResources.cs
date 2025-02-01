using System.ComponentModel.DataAnnotations;

namespace ChessApi.Models.API
{
    public class ThreadResources
    {
        public int alpha { get; set; } = int.MinValue;
        public PossibleMove move { get; set; } = new();
    }
}
