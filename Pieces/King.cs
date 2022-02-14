using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class King : Piece
    {
        public King(Colour colour) : base(colour)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackKing : Properties.Resources.WhiteKing;
        }

        public override List<int> GetPossibleMoves(Board board, int position)
        {
            var possibleMoves = new List<int>();
            var possibleSteps = new List<int>() { 1, -1, 8, -8, -9, 9, -7, 7 };
            AddStepsToPossibleMoves(board, position, possibleMoves, possibleSteps);
            return possibleMoves;
        }
    }
}
