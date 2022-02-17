using System.Collections.Generic;

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
            return ChessService.GetDiagonalMoves(board, Colour, Position, true);
        }

        public override List<int> GetPossibleMovesIgnoringCheckRules(Board board)
        {
            return ChessService.GetDiagonalMoves(board, Colour, Position);
        }
    }
}
