using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackBishop : Properties.Resources.WhiteBishop;
        }

        public override List<int> GetAttackedSquares(Board board, int position)
        {
            return ChessService.GetDiagonalMoves(board, Colour, position, true);
        }

        public override List<int> GetPossibleMovesIgnoringCheckRules(Board board, int position)
        {
            return ChessService.GetDiagonalMoves(board, Colour, position);
        }
    }
}
