using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TrainingWeb.ChessFiles
{
    public class BoardSquare
    {
        public Piece piece = null;
        public int[] coords;
        public int[] enPassantVictim;
        public string enPassantColor = "";
        public string blockCheckColor = "";
        public string blackPressureSource = "";
        public string whitePressureSource = "";
        public string pinDir = "";
        public int blackPressure;
        public int whitePressure;
        public bool isSelected = false;
        public bool isHighlighted = false;
        public bool hasMoved = false;
        public bool moved = false;
        public bool inCheck = false;
        public bool checkmate = false;

        public BoardSquare(BoardArgs args)
        {
            this.isSelected = args.isSelected;
            this.isHighlighted = args.isHighlighted;
            this.piece = args.piece;
            this.coords = args.coords;
            if (this.piece != null)
            {
                this.piece.coords = this.coords;
            }
            this.blackPressure = args.blackPressure;
            this.whitePressure = args.whitePressure;
            this.hasMoved = args.hasMoved;
            this.enPassantColor = args.enPassantColor;
            this.pinDir = args.pinDir;
            this.inCheck = args.inCheck;
            this.blockCheckColor = args.blockCheckColor;
        }

        public void SetPiece(Piece piece)
        {
            this.piece = piece;
            if (this.piece != null)
            {
                this.piece.coords = this.coords;
            }
        }

        public List<int[]> GetPaths(BoardSquare[,] board, string checkedColor)
        {
            List<int[]> moves = new List<int[]>();

            if (this.piece != null && !this.isSelected)
            {
                this.Select();
                moves = this.piece.GetPaths(board, this.piece.color == checkedColor);
            }
            else if (this.isSelected)
            {
                this.Clear();
            }

            return moves;
        }

        public List<int[]> GetPressure(BoardSquare[,] board)
        {
            return piece.GetPressure(board);
        }

        public void Clear()
        {
            this.isSelected = false;
            this.isHighlighted = false;
        }

        public void Hightlight()
        {
            this.isHighlighted = true;
        }

        public void Select()
        {
            this.isSelected = true;
        }

        public BoardArgs GetBoardArgs()
        {
            return new BoardArgs(this);
        }
    }
}
