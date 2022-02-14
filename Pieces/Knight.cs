using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Knight : Piece
    {
        public Knight(Colour colour) : base(colour)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackKnight : Properties.Resources.WhiteKnight;
        }

        public override List<int> GetPossibleMoves(Board board, int position)
        {
            var possibleMoves = new List<int>();
            var possibleSteps = new List<int>() { -17, -15, -6, 10, 17, 15, 6, -10 };
            AddStepsToPossibleMoves(board, position, possibleMoves, possibleSteps);
            return possibleMoves;
        }
    }
}
