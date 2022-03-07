using System;
using System.Drawing;
using System.Windows.Forms;
using UserInterface.Pieces;

namespace UserInterface
{
    public static class UtilityService
    {
        public static Point GetPointFromPosition(int position)
        {
            var x = position % 8;
            var y = position / 8;
            return new Point(x, y);
        }

        public static int GetPositionFromPoint(int x, int y)
        {
            if (x > 7 || y > 7)
            {
                throw new Exception("Invalid coordinates: " + x + ", " + y);
            }
            return x + 8 * y;
        }

        public static Colour GetOppositeColour(Colour colour)
        {
            return colour == Colour.White ? Colour.Black : Colour.White;
        }

        public static string GetPgn(int startPosition, int endPosition, Piece promotionPiece)
        {
            var promotion = promotionPiece != null ? GetPromotionString(promotionPiece) : "";
            return GetPgn(startPosition) + GetPgn(endPosition) + promotion;
        }

        private static string GetPromotionString(Piece promotionPiece)
        {
            var promotionString = char.ToLower(promotionPiece.GetType().Name[0]).ToString();
            return promotionString == "k" ? "n" : promotionString;
        }

        public static string GetPgn(int position)
        {
            var first = (char)((position % 8) + 97);
            var second = (8 - (position / 8)).ToString();
            return first + second;
        }

        public static int GetPositionFromPgn(string pgn)
        {
            var column = (int)char.GetNumericValue(pgn[0]) - 97;
            var row = int.Parse(pgn[1].ToString());
            return column + 8 * row;
        }

        public static void Wait(int milliseconds)
        {
            if (milliseconds == 0 || milliseconds < 0)
            {
                return;
            }

            var timer = new Timer
            {
                Interval = milliseconds,
                Enabled = true
            };
            timer.Start();

            timer.Tick += (s, e) =>
            {
                timer.Enabled = false;
                timer.Stop();
            };

            while (timer.Enabled)
            {
                Application.DoEvents();
            }
        }
    }
}
