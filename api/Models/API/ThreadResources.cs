using System.ComponentModel.DataAnnotations;

namespace ChessApi.Models.API
{
    public class ThreadResources
    {
        public int Alpha { get; set; } = int.MinValue;
        public PossibleMove? Move { get; set; }
    }
}
