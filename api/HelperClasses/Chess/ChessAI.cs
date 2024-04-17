using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Serialization;
using api.models.api;
using api.models.db;
using api.pieces;
using api.pieces.interfaces;

namespace api.helperclasses.chess
{
    public class ChessAI
    {
        private readonly int Max_Depth;
        private readonly string max_color = "black";
        private readonly string min_color = "white";

        public ChessAI(bool isBlack = true, int Max_Depth = 3)
        {
            this.Max_Depth = Max_Depth;
            if (!isBlack)
            {
                max_color = "white";
                min_color = "black";
            }
        }

        public void GetMove(ref Game game, out Move? foundMove)
        {
            int score = int.MinValue;
            int[] move = new int[4];

            foreach (BoardRow row in game.Board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    if (square.Piece != null && square.Piece.Color == max_color)
                    {
                        foreach (
                            int[] localMove in MoveHelper.GetMovesFromPiece(
                                game.Board,
                                square.Coords,
                                game.CheckedColor
                            )
                        )
                        {
                            Game newGame = CopyGame(game);

                            MoveHelper.MovePiece(
                                [square.Coords[0], square.Coords[1]],
                                [localMove[0], localMove[1]],
                                ref newGame
                            );

                            int tempScore = MinMax(newGame, false, 0, int.MinValue, int.MaxValue);

                            if (tempScore > score)
                            {
                                score = tempScore;
                                move =
                                [
                                    square.Coords[0],
                                    square.Coords[1],
                                    localMove[0],
                                    localMove[1]
                                ];
                            }
                        }
                    }
                }
            }

            MoveHelper.MovePiece([move[0], move[1]], [move[2], move[3]], ref game);

            foundMove = null;
            IPiece? piece = game.Board.Rows[move[2]].Squares[move[3]].Piece;
            if (piece != null)
            {
                foundMove = new()
                {
                    GameID = game.GameID,
                    From = [move[0], move[1]],
                    To = [move[2], move[3]],
                    PieceColor = piece.Color,
                    PieceType = piece.GetType().Name
                };
            }
        }

        private int MinMax(Game game, bool max, int depth, double alpha, double beta)
        {
            string color = max ? max_color : min_color;

            if (depth == Max_Depth)
            {
                return GetBoardScore(game.Board, color);
            }

            List<BoardSquare> moveSquares = new();

            foreach (BoardRow row in game.Board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    if (square.Piece != null && square.Piece.Color == color)
                    {
                        if (square.Piece is King king && king.InCheckMate)
                        {
                            return color == max_color ? int.MinValue : int.MaxValue;
                        }

                        moveSquares.Add(square);
                    }
                }
            }

            List<int[]> moves = [];
            foreach (BoardSquare square in moveSquares)
            {
                if (square.Piece == null)
                {
                    continue;
                }

                List<int[]> localMoves = MoveHelper.GetMovesFromPiece(
                    game.Board,
                    square.Coords,
                    game.CheckedColor
                );

                foreach (int[] localMove in localMoves)
                {
                    moves.Add([square.Coords[0], square.Coords[1], localMove[0], localMove[1]]);
                }
            }

            int score = max ? int.MinValue : int.MaxValue;

            if (moves.Count == 0)
            {
                return 0;
            }

            foreach (int[] move in moves)
            {
                Game newGame = CopyGame(game);

                MoveHelper.MovePiece([move[0], move[1]], [move[2], move[3]], ref newGame);

                int tempScore = MinMax(newGame, !max, depth + 1, alpha, beta);

                if ((tempScore > score && max) || (tempScore < score && !max))
                {
                    score = tempScore;
                }

                if (max)
                {
                    alpha = Math.Max(score, alpha);
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
                else
                {
                    beta = Math.Min(score, beta);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }

            return score;
        }

        public int GetBoardScore(Board board, string color)
        {
            int boardScore = 0;

            foreach (BoardRow row in board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    IPiece? piece = square.Piece;
                    if (piece != null)
                    {
                        int[,] modifier =
                            piece.Color == "white" ? piece.WhiteValues : piece.BlackValues;
                        int pieceValue = piece.Value + modifier[square.Coords[0], square.Coords[1]];

                        if (piece.Color == color)
                        {
                            boardScore += pieceValue;
                        }
                        else
                        {
                            boardScore -= pieceValue;
                        }
                    }
                }
            }

            return boardScore;
        }

        public Game CopyGame(Game game)
        {
            Game newGame = new() { Board = new(), CheckedColor = game.CheckedColor };

            int i = 0;
            foreach (BoardRow row in game.Board.Rows)
            {
                newGame.Board.Rows.Add(new());
                foreach (BoardSquare square in row.Squares)
                {
                    newGame.Board.Rows[i].Squares.Add(square.Copy());
                }
                i++;
            }

            return newGame;
        }
    }
}
