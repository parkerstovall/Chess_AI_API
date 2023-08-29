using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingWeb.ChessFiles
{
    [Serializable]
    public class BoardArgs
    {
        public bool isSelected = false;
        public bool isHighlighted = false;
        public bool hasMoved = false;
        public bool enPassant = false;
        public bool checkmate = false;
        public bool inCheck = false;
        public int blackPressure = 0;
        public int whitePressure = 0;
        public int[] coords = { 0, 0 };
        public int[] enPassantVictim = { -1, -1 };
        public string enPassantColor = "";
        public string blockCheckColor = "";
        public string whitePressureSource = "";
        public string blackPressureSource = "";
        public string pinDir = "";
        public Piece piece = null;

        public BoardArgs(BoardSquare square)
        {
            this.isSelected = square.isSelected;
            this.isHighlighted = square.isHighlighted;
            this.blackPressure = square.blackPressure;
            this.whitePressure = square.whitePressure;
            this.coords = square.coords;
            this.hasMoved = square.hasMoved;
            this.enPassantColor = square.enPassantColor;
            this.blackPressureSource = square.blackPressureSource;
            this.whitePressureSource = square.whitePressureSource;
            this.blockCheckColor = square.blockCheckColor;
            this.piece = square.piece;
            this.enPassantVictim = square.enPassantVictim;
            this.pinDir = square.pinDir;
            this.inCheck = square.inCheck;
            this.checkmate = square.checkmate;
        }
        public BoardArgs(int[] coords)
        {
            this.coords = coords;
        }

        public BoardArgs(dynamic args)
        {
            bool isSelected = false;
            bool isHighlighted = false;
            bool hasMoved = false;
            bool checkmate = false;
            bool inCheck = false;
            int blackPressure = 0;
            int whitePressure = 0;
            int row = 0;
            int col = 0;

            if (args.ContainsKey("isSelected") && !string.IsNullOrEmpty(args["isSelected"].ToString()) && Boolean.TryParse(args["isSelected"].ToString(), out isSelected))
            {
                this.isSelected = isSelected;
            }

            if (args.ContainsKey("isHighlighted") && !string.IsNullOrEmpty(args["isHighlighted"].ToString()) && Boolean.TryParse(args["isHighlighted"].ToString(), out isHighlighted))
            {
                this.isHighlighted = isHighlighted;
            }

            if (args.ContainsKey("hasMoved") && !string.IsNullOrEmpty(args["hasMoved"].ToString()) && Boolean.TryParse(args["hasMoved"].ToString(), out hasMoved))
            {
                this.hasMoved = hasMoved;
            }

            if (args.ContainsKey("inCheck") && !string.IsNullOrEmpty(args["inCheck"].ToString()) && Boolean.TryParse(args["inCheck"].ToString(), out inCheck))
            {
                this.inCheck = inCheck;
            }

            if (args.ContainsKey("checkmate") && !string.IsNullOrEmpty(args["checkmate"].ToString()) && Boolean.TryParse(args["checkmate"].ToString(), out checkmate))
            {
                this.checkmate = checkmate;
            }

            if (args.ContainsKey("blackPressure") && !string.IsNullOrEmpty(args["blackPressure"].ToString()) && Int32.TryParse(args["blackPressure"].ToString(), out blackPressure))
            {
                this.blackPressure = blackPressure;
            }

            if (args.ContainsKey("whitePressure") && !string.IsNullOrEmpty(args["whitePressure"].ToString()) && Int32.TryParse(args["whitePressure"].ToString(), out whitePressure))
            {
                this.whitePressure = whitePressure;
            }

            if (args.ContainsKey("col") && !string.IsNullOrEmpty(args["col"].ToString()))
            {
                Int32.TryParse(args["col"].ToString(), out col);
            }

            if (args.ContainsKey("row") && !string.IsNullOrEmpty(args["row"].ToString()))
            {
                Int32.TryParse(args["row"].ToString(), out row);
            }

            this.coords = new int[] { col, row };

            if (args.ContainsKey("enPassantColor") && !string.IsNullOrEmpty(args["enPassantColor"].ToString()))
            {
                this.enPassantColor = args["enPassantColor"].ToString();

                int enPassantCol = 0;
                int enPassantRow = 0;

                if (args.ContainsKey("enPassantVictim") && !string.IsNullOrEmpty(args["enPassantVictim"][0].ToString()))
                {
                    Int32.TryParse(args["enPassantVictim"][0].ToString(), out enPassantCol);
                }

                if (args.ContainsKey("enPassantVictim") && !string.IsNullOrEmpty(args["enPassantVictim"][1].ToString()))
                {
                    Int32.TryParse(args["enPassantVictim"][1].ToString(), out enPassantRow);
                }

                this.enPassantVictim = new int[] { enPassantCol, enPassantRow };
            }

            if (args.ContainsKey("blockCheckColor") && !string.IsNullOrEmpty(args["blockCheckColor"].ToString()))
            {
                this.blockCheckColor = args["blockCheckColor"].ToString();
            }

            if (args.ContainsKey("blackPressureSource") && !string.IsNullOrEmpty(args["blackPressureSource"].ToString()))
            {
                this.blackPressureSource = args["blackPressureSource"].ToString();
            }

            if (args.ContainsKey("whitePressureSource") && !string.IsNullOrEmpty(args["whitePressureSource"].ToString()))
            {
                this.whitePressureSource = args["whitePressureSource"].ToString();
            }

            if (args.ContainsKey("pinDir") && !string.IsNullOrEmpty(args["pinDir"].ToString()))
            {
                this.pinDir = args["pinDir"].ToString();
            }
        }
    }
}