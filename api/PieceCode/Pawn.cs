using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.pieces
{
    public class Pawn : Piece
    {
        private bool hasMoved = false;

        public Pawn(string color, int[] coords, bool hasMoved)
        {
            this.color = color;
            this.type = "Pawn";
            this.coords = coords;
            this.hasMoved = hasMoved;
        }

        public override List<int[]> GetPaths(BoardSquare[,] board, bool check)
        {
            List<int[]> moves = new List<int[]>();
            int dir = (this.color == "black") ? 1 : -1;

            int col = coords[0] + dir;
            int row = coords[1];

            if (col > -1 && col < board.GetLength(0))
            {
                //Moving
                if (
                    board[col, row].piece == null
                    && (board[coords[0], row].pinDir == "" || board[coords[0], row].pinDir == "tb")
                )
                {
                    if (check)
                    {
                        if (board[col, row].blockCheckColor == this.color)
                        {
                            moves.Add(new int[] { col, row });
                        }
                    }
                    else
                    {
                        moves.Add(new int[] { col, row });
                    }

                    col += dir;

                    if (
                        col > -1
                        && col < board.GetLength(0)
                        && !this.hasMoved
                        && board[col, row].piece == null
                    )
                    {
                        if (check)
                        {
                            if (board[col, row].blockCheckColor == this.color)
                            {
                                moves.Add(new int[] { col, row });
                            }
                        }
                        else
                        {
                            moves.Add(new int[] { col, row });
                        }
                    }
                }

                col = coords[0] + dir;
                row -= 1;

                //Attacking
                if (row > -1)
                {
                    if (
                        this.color == "black"
                            ? (
                                board[coords[0], row].pinDir == ""
                                || board[coords[0], row].pinDir == "bltr"
                            )
                            : (
                                board[coords[0], row].pinDir == ""
                                || board[coords[0], row].pinDir == "tlbr"
                            )
                    )
                    {
                        BoardSquare left = board[col, row];

                        if (
                            (left.piece != null && left.piece.color != this.color)
                            || (left.enPassantColor != "" && left.enPassantColor != this.color)
                        )
                        {
                            if (check)
                            {
                                if (left.blockCheckColor == this.color)
                                {
                                    moves.Add(new int[] { col, row });
                                }
                            }
                            else
                            {
                                moves.Add(new int[] { col, row });
                            }
                        }
                    }
                }

                row += 2;

                if (row < board.GetLength(1))
                {
                    if (
                        this.color == "white"
                            ? (
                                board[coords[0], row].pinDir == ""
                                || board[coords[0], row].pinDir == "bltr"
                            )
                            : (
                                board[coords[0] - dir, row].pinDir == ""
                                || board[coords[0], row].pinDir == "tlbr"
                            )
                    )
                    {
                        BoardSquare right = board[col, row];

                        if (
                            (right.piece != null && right.piece.color != this.color)
                            || (right.enPassantColor != "" && right.enPassantColor != this.color)
                        )
                        {
                            if (check)
                            {
                                if (right.blockCheckColor == this.color)
                                {
                                    moves.Add(new int[] { col, row });
                                }
                            }
                            else
                            {
                                moves.Add(new int[] { col, row });
                            }
                        }
                    }
                }
            }

            return moves;
        }

        public override List<int[]> GetPressure(BoardSquare[,] board)
        {
            List<int[]> moves = new List<int[]>();
            int dir = (this.color == "black") ? 1 : -1;

            int i = coords[0] + dir;
            int j = coords[1] - 1;

            if (i > -1 && i < board.GetLength(0))
            {
                //Attacking
                if (j > -1)
                {
                    moves.Add(board[i, j].coords);
                }

                j += 2;

                if (j < board.GetLength(1))
                {
                    moves.Add(board[i, j].coords);
                }
            }

            return moves;
        }

        public override string ToString(bool pipeSeparated)
        {
            if (pipeSeparated)
            {
                return color + "|Pawn";
            }

            return color + "Pawn";
        }
    }
}
