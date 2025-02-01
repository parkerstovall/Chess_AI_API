using System.ComponentModel.DataAnnotations;
using ChessApi.Models.DB;

namespace ChessApi.Models.API
{
    public class AiMoveResponse
    {
        public int alpha { get; set; } = int.MinValue;
        public Move? foundMove { get; set; } = null;
    }
}
