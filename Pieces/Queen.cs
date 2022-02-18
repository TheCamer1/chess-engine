using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Queen : Piece
    {
        public Queen(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackQueen : Properties.Resources.WhiteQueen;
            PieceType = PieceType.Queen;
        }

        public override List<int> GetAttackedSquares(Board board)
        {
            var queenMoves = ChessService.GetDiagonalMoves(board, Colour, Position, true, true);
            queenMoves.AddRange(ChessService.GetHorizontalMoves(board, Colour, Position, true, true));
            return queenMoves;
        }

        public override List<int> GetPossibleMovesIgnoringCheckRules(Board board)
        {
            var queenMoves = ChessService.GetDiagonalMoves(board, Colour, Position);
            queenMoves.AddRange(ChessService.GetHorizontalMoves(board, Colour, Position));
            return queenMoves;
        }
    }
}
