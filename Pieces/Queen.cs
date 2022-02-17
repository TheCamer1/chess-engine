using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Queen : Piece
    {
        public Queen(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackQueen : Properties.Resources.WhiteQueen;
        }

        public override List<int> GetAttackedSquares(Board board, int position)
        {
            var queenMoves = GetDiagonalMoves(board, position, true);
            queenMoves.AddRange(GetHorizontalMoves(board, position, true));
            return queenMoves;
        }

        public override List<int> GetPossibleMovesIgnoringCheckRules(Board board, int position)
        {
            var queenMoves = GetDiagonalMoves(board, position);
            queenMoves.AddRange(GetHorizontalMoves(board, position));
            return queenMoves;
        }
    }
}
