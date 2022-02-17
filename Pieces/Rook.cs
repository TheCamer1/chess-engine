using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Rook : Piece
    {
        public Rook(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackRook : Properties.Resources.WhiteRook;
        }

        public override List<int> GetAttackedSquares(Board board, int position)
        {
            return GetHorizontalMoves(board, position, true);
        }

        public override List<int> GetPossibleMovesIgnoringCheckRules(Board board, int position)
        {
            return GetHorizontalMoves(board, position);
        }
    }
}
