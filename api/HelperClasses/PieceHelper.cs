namespace api.helperclasses
{
    public static class PieceHelper
    {
        public static bool IsInBoard(int col, int row)
        {
            return col > -1 && col < 8 && row > -1 && row < 8;
        }
    }
}
