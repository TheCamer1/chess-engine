using System.Collections.Generic;
using System.Linq;

namespace UserInterface.Pieces
{
    public class King : Piece
    {
        private List<int> _possibleSteps { get => new List<int>() { 1, -1, 8, -8, -9, 9, -7, 7 }; }

        public King(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackKing : Properties.Resources.WhiteKing;
        }

        public override List<int> GetAttackedSquares(Board board, int position)
        {
            var possibleMoves = new List<int>();
            AddStepsToPossibleMoves(board, position, possibleMoves, _possibleSteps, true);
            return possibleMoves;
        }

        public override List<int> GetPossibleMovesIgnoringCheckRules(Board board, int position)
        {
            var possibleMoves = new List<int>();
            AddStepsToPossibleMoves(board, position, possibleMoves, _possibleSteps);
            return possibleMoves;
        }

        private List<int> GetPossibleCastlingMoves(Board board, int position)
        {
            var castlingMoves = new List<int>();
            if (MovedOn != null)
            {
                return castlingMoves;
            }
            var kingsideCastlingSquares = new HashSet<int>()
            {
                position + 1,
                position + 2
            };
            var queensideEmptySquares = new HashSet<int>()
            {
                position - 1,
                position - 2,
                position - 3
            };
            var queensideNonAttackedSquares = new HashSet<int>()
            {
                position - 1,
                position - 2
            };
            var oppositeColour = ChessService.GetOppositeColour(Colour);
            AddCastlingMove(board, position + 2, position + 3, castlingMoves, kingsideCastlingSquares, kingsideCastlingSquares, oppositeColour);
            AddCastlingMove(board, position - 2, position - 4, castlingMoves, queensideEmptySquares, queensideNonAttackedSquares, oppositeColour);
            return castlingMoves;
        }

        private void AddCastlingMove(Board board, int castlingPosition, int rookPosition, List<int> castlingMoves, HashSet<int> emptySquares, HashSet<int> nonAttackedSquares, Colour oppositeColour)
        {
            if (board.GetPiece(rookPosition).MovedOn == null
                && !board.IsKingInCheck(Colour)
                && nonAttackedSquares.All(e => !board.IsSquareAttacked(oppositeColour, e))
                && emptySquares.All(e => board.GetPiece(e) == null))
            {
                castlingMoves.Add(castlingPosition);
            }
        }

        public override List<int> GetPossibleMoves(Board board, int position)
        {
            var possibleMoves = GetPossibleCastlingMoves(board, position);
            foreach (var step in _possibleSteps)
            {
                var newPosition = position + step;
                if (IsNextSquareValid(board, position, position, step) 
                    && !board.IsSquareAttacked(ChessService.GetOppositeColour(Colour), newPosition))
                {
                    possibleMoves.Add(newPosition);
                }
            }
            return possibleMoves;
        }
    }
}
