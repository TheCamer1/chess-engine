using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(Colour colour) : base(colour)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackBishop : Properties.Resources.WhiteBishop;
        }

        public override List<int> GetPossibleMoves(Board board, int position)
        {
            return GetDiagonalMoves(board, position);
        }
    }
}
