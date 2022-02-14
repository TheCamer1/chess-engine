using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Queen : Piece
    {
        public Queen(Colour colour) : base(colour)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackQueen : Properties.Resources.WhiteQueen;
        }

        public override List<int> GetPossibleMoves(Board board, int position)
        {
            var queenMoves = GetDiagonalMoves(board, position);
            queenMoves.AddRange(GetHorizontalMoves(board, position));
            return queenMoves;
        }
    }
}
