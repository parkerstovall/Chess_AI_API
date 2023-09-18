namespace api.models.api
{
    public class Board
    {
        public List<BoardRow> Rows { get; set; } = new List<BoardRow>();

        public override string ToString()
        {
            string board = "";
            foreach (BoardRow row in Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    if (square.Piece == null)
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

                        board +=
                            square.Piece.Color == "white" ? type.ToUpper()[0] : type.ToLower()[0];
                    }
                    board += " ";
                }
                board += "\n";
            }
            return board;
        }
    }
}
