using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UserInterface.Pieces
{
    public abstract class Piece
    {
        public Colour Colour { get; set; }
        public Image Image { get; set; }
        public int? MovedOn { get; set; }
        public HashSet<int> AttackedSquares { get; set; }
        public int Position { get; set; }

        public Piece(Colour colour, int position)
        {
            Colour = colour;
            Position = position;
        }

        public virtual void SetAttackedSquares(Board board)
        {
            AttackedSquares = new HashSet<int>(GetAttackedSquares(board, Position));
        }

        public abstract List<int> GetAttackedSquares(Board board, int position);
        public abstract List<int> GetPossibleMovesIgnoringCheckRules(Board board, int position);

        public virtual List<int> GetPossibleMoves(Board board, int position)
        {
            var possibleMoves = GetPossibleMovesIgnoringCheckRules(board, position);
            var possibleMovesIfPinned = ChessService.GetPossibleMovesIfPinned(board, Colour, possibleMoves, position);
            if (!board.IsKingInCheck(Colour))
            {
                return possibleMovesIfPinned;
            }
            var kingPosition = board.KingPositions[Colour];
            var checkingPieces = board.GetAttackingPieces(ChessService.GetOppositeColour(Colour), kingPosition);
            if (checkingPieces.Count > 1)
            {
                return new List<int>();
            }
            var checkingPiece = checkingPieces.First();
            return ChessService.GetMovesToBlockCheck(board, possibleMovesIfPinned, kingPosition, checkingPiece);
        }
    }
}
