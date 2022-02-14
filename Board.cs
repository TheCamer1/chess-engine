using System.Drawing;
using UserInterface.Pieces;

namespace UserInterface
{
    public class Board
    {
        public Piece[] Pieces { get; set; }
        public Colour Perspective { get; set; }
        public int CurrentPlay { get; set; } = 0;

        public Board(Colour perspective)
        {
            SetUpBoard(perspective);
            Perspective = perspective;
        }

        private void SetUpBoard(Colour perspective)
        {
            var bottomColour = perspective;
            var topColour = perspective == Colour.White ? Colour.Black : Colour.White;
            Pieces = new Piece[64];
            Pieces[0] = new Rook(topColour);
            Pieces[1] = new Knight(topColour);
            Pieces[2] = new Bishop(topColour);
            Pieces[3] = new Queen(topColour);
            Pieces[4] = new King(topColour);
            Pieces[5] = new Bishop(topColour);
            Pieces[6] = new Knight(topColour);
            Pieces[7] = new Rook(topColour);
            for (var i = 8; i < 16; i++)
            {
                Pieces[i] = new Pawn(topColour);
            }
            for (var i = 48; i < 56; i++)
            {
                Pieces[i] = new Pawn(bottomColour);
            }
            Pieces[56] = new Rook(bottomColour);
            Pieces[57] = new Knight(bottomColour);
            Pieces[58] = new Bishop(bottomColour);
            Pieces[59] = new Queen(bottomColour);
            Pieces[60] = new King(bottomColour);
            Pieces[61] = new Bishop(bottomColour);
            Pieces[62] = new Knight(bottomColour);
            Pieces[63] = new Rook(bottomColour);
        }

        public Point GetPointFromPosition(int position)
        {
            var x = position % 8;
            var y = position / 8;
            return new Point(x, y);
        }

        public int GetPositionFromPoint(int x, int y)
        {
            if (x > 7 || y > 7)
            {
                throw new System.Exception("Invalid coordinates: " + x + ", " + y);
            }
            return x + 8 * y;
        }

        public Piece GetPiece(int x, int y)
        {
            return Pieces[GetPositionFromPoint(x, y)];
        }

        public Piece GetPiece(int position)
        {
            if (position < 0 || position > 63)
            {
                return null;
            }
            return Pieces[position];
        }
    }
}
