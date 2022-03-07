using System.Collections.Generic;
using System.Linq;

namespace UserInterface.Pieces
{
    public class Rook : Piece
    {
        public Rook(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackRook : Properties.Resources.WhiteRook;
        }

        public override List<int> GetAttackedSquares(Board board)
        {
            return PieceMoveService.GetHorizontalMoves(board, Colour, Position, true).Select(e => e.FinalPosition).ToList();
        }

        public override List<Move> GetPossibleMovesIgnoringCheckRules(Board board)
        {
            return PieceMoveService.GetHorizontalMoves(board, Colour, Position);
        }
    }
}
