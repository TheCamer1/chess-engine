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
            PieceType = PieceType.King;
        }

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

        public override List<int> GetPossibleMoves(Board board)
        {
            var possibleMoves = GetPossibleCastlingMoves(board);
            foreach (var step in _possibleSteps)
            {
                var newPosition = Position + step;
                if (ChessService.IsNextSquareValid(board, Colour, Position, Position, step)
                    && !board.IsSquareAttacked(ChessService.GetOppositeColour(Colour), newPosition))
                {
                    possibleMoves.Add(newPosition);
                }
            }
            return possibleMoves;
        }

        private List<int> GetPossibleCastlingMoves(Board board)
        {
            var castlingMoves = new List<int>();
            if (MovedOn != null)
            {
                return castlingMoves;
            }
            var kingsideCastlingSquares = new HashSet<int>()
            {
                Position + 1,
                Position + 2
            };
            var queensideEmptySquares = new HashSet<int>()
            {
                Position - 1,
                Position - 2,
                Position - 3
            };
            var queensideNonAttackedSquares = new HashSet<int>()
            {
                Position - 1,
                Position - 2
            };
            var oppositeColour = ChessService.GetOppositeColour(Colour);
            AddCastlingMove(board, Position + 2, Position + 3, castlingMoves, kingsideCastlingSquares, kingsideCastlingSquares, oppositeColour);
            AddCastlingMove(board, Position - 2, Position - 4, castlingMoves, queensideEmptySquares, queensideNonAttackedSquares, oppositeColour);
            return castlingMoves;
        }

        private void AddCastlingMove(Board board, int castlingPosition, int rookPosition, List<int> castlingMoves, HashSet<int> emptySquares, HashSet<int> nonAttackedSquares, Colour oppositeColour)
        {
            var rook = board.GetPiece(rookPosition);
            if (rook != null 
                && rook.MovedOn == null
                && !board.IsKingInCheck(Colour)
                && nonAttackedSquares.All(e => !board.IsSquareAttacked(oppositeColour, e))
                && emptySquares.All(e => board.GetPiece(e) == null))
            {
                castlingMoves.Add(castlingPosition);
            }
        }
    }
}
