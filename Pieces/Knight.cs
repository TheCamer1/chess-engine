using System.Collections.Generic;
using System.Linq;

namespace UserInterface.Pieces
{
    public class Knight : Piece
    {
        private List<int> _possibleSteps { get => new List<int>() { -17, -15, -6, 10, 17, 15, 6, -10 }; }

        public Knight(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackKnight : Properties.Resources.WhiteKnight;
            PieceType = PieceType.Knight;
        }

        public override List<int> GetAttackedSquares(Board board)
        {
            var possibleMoves = new List<Move>();
            PieceMoveService.AddStepsToPossibleMoves(board, Colour, Position, possibleMoves, _possibleSteps, true);
            return possibleMoves.Select(e => e.FinalPosition).ToList();
        }

        public override List<Move> GetPossibleMovesIgnoringCheckRules(Board board)
        {
            var possibleMoves = new List<Move>();
            PieceMoveService.AddStepsToPossibleMoves(board, Colour, Position, possibleMoves, _possibleSteps);
            return possibleMoves;
        }
    }
}
