using System.Collections.Generic;
using System.Linq;

namespace UserInterface.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackBishop : Properties.Resources.WhiteBishop;
        }

        public override List<int> GetAttackedSquares(Board board)
        {
            return PieceMoveService.GetDiagonalMoves(board, Colour, Position, true).Select(e => e.FinalPosition).ToList();
        }

        public override List<Move> GetPossibleMovesIgnoringCheckRules(Board board)
        {
            return PieceMoveService.GetDiagonalMoves(board, Colour, Position);
        }
    }
}
