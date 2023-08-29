using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingWeb.ChessFiles
{
    public class Piece
    {
        public string color = "";
        public string type = "";
        public int[] coords = { -1, -1 };
        public Piece()
        {

        }

        public virtual string ToString(bool pipeSeparated) {
            return "";
        }

        public virtual List<int[]> GetPaths(BoardSquare[,] board, bool check)
        {
            return new List<int[]>();
        }

        public virtual List<int[]> GetPressure(BoardSquare[,] board)
        {
            return this.GetPaths(board, false);
        }

        public Piece GetPiece(string[] args, int[] coords, bool hasMoved)
        {
            string color = args[0];
            string type = args[1];
            switch (type)
            {
                case "Pawn":
                    return new Pawn(color, coords, hasMoved);
                case "King":
                    return new King(color, coords, hasMoved);
                case "Bishop":
                    return new Bishop(color, coords);
                case "Queen":
                    return new Queen(color, coords);
                case "Rook":
                    return new Rook(color, coords);
                case "Knight":
                    return new Knight(color, coords);
                default:
                    return null;
            }
        }
    }
}