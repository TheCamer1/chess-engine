using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UserInterface.Pieces
{
    public abstract class Piece : ICloneable
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

        public Piece(Piece piece)
        {
            Colour = piece.Colour;
            Image = piece.Image;
            MovedOn = piece.MovedOn;
            AttackedSquares = new HashSet<int>(piece.AttackedSquares);
            Position = piece.Position;
        }

        public virtual void SetAttackedSquares(Board board)
        {
            AttackedSquares = new HashSet<int>(GetAttackedSquares(board));
        }

        public abstract List<int> GetAttackedSquares(Board board);
        public abstract List<int> GetPossibleMovesIgnoringCheckRules(Board board);

        public virtual List<int> GetPossibleMoves(Board board)
        {
            var possibleMoves = GetPossibleMovesIgnoringCheckRules(board);
            var possibleMovesIfPinned = ChessService.GetPossibleMovesIfPinned(board, Colour, possibleMoves, Position);
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

        public abstract object Clone();
    }
}
