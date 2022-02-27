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
        }

        public Knight(Knight knight) : base(knight) { }

        public override List<int> GetAttackedSquares(Board board)
        {
            var possibleMoves = new List<int>();
            ChessService.AddStepsToPossibleMoves(board, Colour, Position, possibleMoves, _possibleSteps, true);
            return possibleMoves;
        }

        public override List<int> GetPossibleMovesIgnoringCheckRules(Board board)
        {
            var possibleMoves = new List<int>();
            ChessService.AddStepsToPossibleMoves(board, Colour, Position, possibleMoves, _possibleSteps);
            return possibleMoves;
        }

        public override object Clone()
        {
            return new Knight(this);
        }
    }
}
