﻿using api.helperclasses;
using api.models.api;
using api.models.db;
using api.pieces.interfaces;

namespace api.pieces
{
    public class Bishop : IPieceCanPin
    {
        public string Color { get; set; }
        public Direction PinnedDir { get; set; } = Direction.None;
        public int[,] WhiteValues { get; } =
            new int[,]
            {
                { -20, -10, -10, -10, -10, -10, -10, -20 },
                { -10, 0, 0, 0, 0, 0, 0, -10 },
                { -10, 0, 5, 10, 10, 5, 0, -10 },
                { -10, 5, 5, 10, 10, 5, 5, -10 },
                { -10, 0, 10, 10, 10, 10, 0, -10 },
                { -10, 10, 10, 10, 10, 10, 10, -10 },
                { -10, 5, 0, 0, 0, 0, 5, -10 },
                { -20, -10, -10, -10, -10, -10, -10, -20 }
            };

        public int[,] BlackValues { get; } =
            new int[,]
            {
                { -20, -10, -10, -10, -10, -10, -10, -20 },
                { -10, 5, 0, 0, 0, 0, 5, -10 },
                { -10, 10, 10, 10, 10, 10, 10, -10 },
                { -10, 0, 10, 10, 10, 10, 0, -10 },
                { -10, 5, 5, 10, 10, 5, 5, -10 },
                { -10, 0, 5, 10, 10, 5, 0, -10 },
                { -10, 0, 0, 0, 0, 0, 0, -10 },
                { -20, -10, -10, -10, -10, -10, -10, -20 }
            };

        public int Value { get; } = 330;

        public Bishop(string Color)
        {
            this.Color = Color;
        }

        public List<int[]> GetPaths(Board board, int[] coords, bool check)
        {
            List<int[]> moves = new();

            int col = coords[0];
            int row = coords[1];

            Tuple<int[], int[]> increments = PieceHelper.GetIncrements(
                PinnedDir,
                diag: true,
                straight: false
            );
            int[] colInc = increments.Item1;
            int[] rowInc = increments.Item2;

            for (int i = 0; i < colInc.Length; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                while (PieceHelper.IsInBoard(col, row))
                {
                    BoardSquare square = board.Rows[col].Squares[row];
                    if (square.Piece == null || square.Piece.Color != this.Color)
                    {
                        if (check)
                        {
                            if (square.CheckBlockingColor == this.Color)
                            {
                                moves.Add([col, row]);
                            }
                        }
                        else
                        {
                            moves.Add([col, row]);
                        }

                        if (square.Piece != null)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                    col += colInc[i];
                    row += rowInc[i];
                }
            }

            return moves;
        }

        public List<int[]> GetPressure(Board board, int[] coords)
        {
            List<int[]> moves = new();

            int col = coords[0];
            int row = coords[1];
            int[] colInc = { -1, -1, 1, 1 };
            int[] rowInc = { 1, -1, -1, 1 };

            for (int i = 0; i < 4; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                while (PieceHelper.IsInBoard(col, row))
                {
                    moves.Add(new int[] { col, row });

                    if (board.Rows[col].Squares[row].Piece != null)
                    {
                        if (board.Rows[col].Squares[row].Piece is King)
                        {
                            col += colInc[i];
                            row += rowInc[i];
                            if (PieceHelper.IsInBoard(col, row))
                            {
                                moves.Add(new int[] { col, row });
                            }
                        }

                        break;
                    }

                    col += colInc[i];
                    row += rowInc[i];
                }
            }

            return moves;
        }

        public bool CanPin(Board board, int[] start, int[] dest)
        {
            Direction dir = PieceHelper.GetDirection(start, dest);

            return GoodDir(dir);
        }

        public void CheckPins(int[] start, int[] dest, ref Game game)
        {
            Direction dir = PieceHelper.GetDirection(start, dest);

            if (!GoodDir(dir))
            {
                return;
            }

            int[] inc = PieceHelper.GetSingleIncrement(dir);

            PieceHelper.SetPins(start, inc, dir, this.Color, ref game);
        }

        public bool GoodDir(Direction dir)
        {
            return dir == Direction.FromTopLeftToBottomRight
                || dir == Direction.FromBottomRightToTopLeft
                || dir == Direction.FromTopRightToBottomLeft
                || dir == Direction.FromBottomLeftToTopRight;
        }

        public bool HasSavingSquares(int[] start, int[] dest, ref Game game)
        {
            Direction dir = PieceHelper.GetDirection(start, dest);

            if (!GoodDir(dir))
            {
                return false;
            }

            int[] inc = PieceHelper.GetSingleIncrement(dir);

            return PieceHelper.SetSavingSquares(start, inc, this.Color, ref game);
        }

        public IPiece Copy()
        {
            Bishop newPiece = new(this.Color) { PinnedDir = this.PinnedDir };
            return newPiece;
        }

        public override string ToString()
        {
            return Color + "Bishop";
        }
    }
}
