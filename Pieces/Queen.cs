using System.Collections.Generic;
using System.Linq;

namespace UserInterface.Pieces
{
    public class Queen : Piece
    {
        public Queen(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackQueen : Properties.Resources.WhiteQueen;
        }

        public override List<int> GetAttackedSquares(Board board)
        {
            var queenMoves = ChessService.GetDiagonalMoves(board, Colour, Position, true);
            queenMoves.AddRange(ChessService.GetHorizontalMoves(board, Colour, Position, true));
            return queenMoves.Select(e => e.FinalPosition).ToList();
        }

        public override List<Move> GetPossibleMovesIgnoringCheckRules(Board board)
        {
            var queenMoves = ChessService.GetDiagonalMoves(board, Colour, Position);
            queenMoves.AddRange(ChessService.GetHorizontalMoves(board, Colour, Position));
            return queenMoves;
        }
    }
}
