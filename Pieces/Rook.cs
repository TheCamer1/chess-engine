using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Rook : Piece
    {
        public Rook(Colour colour) : base(colour)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackRook : Properties.Resources.WhiteRook;
        }

        public override List<int> GetPossibleMoves(Board board, int position)
        {
            return GetHorizontalMoves(board, position);
        }
    }
}
