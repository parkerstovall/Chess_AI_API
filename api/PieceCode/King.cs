using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.pieces
{
    public class King : Piece
    {
        bool hasMoved = false;

        public King(string color, int[] coords, bool hasMoved)
        {
            this.color = color;
            this.type = "King";
            this.coords = coords;
            this.hasMoved = hasMoved;
        }

        public override List<int[]> GetPaths(BoardSquare[,] board, bool check)
        {
            List<int[]> moves = new List<int[]>();

            int col = coords[0];
            int row = coords[1];
            int[] colInc = { 0, 0, 1, -1, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 0, -0, 1, -1, -1, 1 };

            for (int i = 0; i < 8; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                if (col >= 0 && row >= 0 && col < board.GetLength(0) && row < board.GetLength(1))
                {
                    if (board[col, row].piece == null || board[col, row].piece.color != this.color)
                    {
                        if (
                            (this.color == "white" && board[col, row].blackPressure == 0)
                            || (this.color == "black" && board[col, row].whitePressure == 0)
                        )
                            moves.Add(new int[] { col, row });
                    }
                }
            }

            col = coords[0];
            row = coords[1];
            if (!this.hasMoved)
            {
                if (
                    board[col, row - 1].piece == null
                    && board[col, row - 2].piece == null
                    && board[col, row - 3].piece == null
                    && board[col, row - 4].piece != null
                    && board[col, row - 4].piece.type == "Rook"
                    && !board[col, row - 4].hasMoved
                )
                {
                    if (this.color == "white")
                    {
                        if (
                            board[col, row].blackPressure == 0
                            && board[col, row - 1].blackPressure == 0
                            && board[col, row - 2].blackPressure == 0
                            && board[col, row - 3].blackPressure == 0
                        )
                        {
                            moves.Add(new int[] { col, row - 2 });
                        }
                    }
                    else
                    {
                        if (
                            board[col, row].whitePressure == 0
                            && board[col, row - 1].whitePressure == 0
                            && board[col, row - 2].whitePressure == 0
                            && board[col, row - 3].whitePressure == 0
                        )
                        {
                            moves.Add(new int[] { col, row - 2 });
                        }
                    }
                }

                if (
                    board[col, row + 1].piece == null
                    && board[col, row + 2].piece == null
                    && board[col, row + 3].piece != null
                    && board[col, row + 3].piece.type == "Rook"
                    && !board[col, row + 3].hasMoved
                )
                {
                    if (this.color == "white")
                    {
                        if (
                            board[col, row].blackPressure == 0
                            && board[col, row + 1].blackPressure == 0
                            && board[col, row + 2].blackPressure == 0
                        )
                        {
                            moves.Add(new int[] { col, row + 2 });
                        }
                    }
                    else
                    {
                        if (
                            board[col, row].whitePressure == 0
                            && board[col, row + 1].whitePressure == 0
                            && board[col, row + 2].whitePressure == 0
                        )
                        {
                            moves.Add(new int[] { col, row + 2 });
                        }
                    }
                }
            }

            return moves;
        }

        public override List<int[]> GetPressure(BoardSquare[,] board)
        {
            List<int[]> moves = new List<int[]>();
            int col = coords[0];
            int row = coords[1];
            int[] colInc = { 0, 0, 1, -1, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 0, -0, 1, -1, -1, 1 };

            for (int i = 0; i < 8; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                if (col >= 0 && row >= 0 && col < board.GetLength(0) && row < board.GetLength(1))
                {
                    moves.Add(new int[] { col, row });
                }
            }

            return moves;
        }

        public override string ToString(bool pipeSeparated)
        {
            if (pipeSeparated)
            {
                return color + "|King";
            }

            return color + "King";
        }
    }
}
