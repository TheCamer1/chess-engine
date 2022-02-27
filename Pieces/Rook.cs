using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Rook : Piece
    {
        public Rook(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackRook : Properties.Resources.WhiteRook;
        }

        public Rook(Rook rook) : base(rook) { }

        public override List<int> GetAttackedSquares(Board board)
        {
            return ChessService.GetHorizontalMoves(board, Colour, Position, true);
        }

        public override List<int> GetPossibleMovesIgnoringCheckRules(Board board)
        {
            return ChessService.GetHorizontalMoves(board, Colour, Position);
        }

        public override object Clone()
        {
            return new Rook(this);
        }
    }
}
