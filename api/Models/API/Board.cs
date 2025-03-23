namespace ChessApi.Models.API
{
    public class Board
    {
        public List<BoardRow> Rows { get; set; } = [];

        public override string ToString()
        {
            string board = "";
            foreach (BoardRow row in Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    if (square.Piece is null)
                    {
                        board += "0";
                    }
                    else
                    {
                        string type = square.Piece.GetType().Name;
                        if (type == "King")
                        {
                            type = "n";
                        }

                        board += !square.Piece.Color ? type.ToUpper()[0] : type.ToLower()[0];
                    }
                    board += " ";
                }
                board += "\n";
            }
            return board;
        }
    }
}
