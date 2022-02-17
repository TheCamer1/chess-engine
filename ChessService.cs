using System.Drawing;

namespace UserInterface
{
    public static class ChessService
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
                throw new System.Exception("Invalid coordinates: " + x + ", " + y);
            }
            return x + 8 * y;
        }

        public static Colour GetOppositeColour(Colour colour)
        {
            return colour == Colour.White ? Colour.Black : Colour.White;
        }
    }
}
