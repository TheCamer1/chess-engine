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
        public bool HasMoved { get; set; }
        public List<int> AttackedSquares { get; set; }
        public int Position { get; set; }
        public PieceType PieceType { get; set; }

        public Piece(Colour colour, int position)
        {
            Colour = colour;
            Position = position;
        }

        public virtual void SetAttackedSquares(Board board)
        {
            AttackedSquares = new List<int>(GetAttackedSquares(board));
        }

        public abstract List<int> GetAttackedSquares(Board board);
        public abstract List<Move> GetPossibleMovesIgnoringCheckRules(Board board);

        public virtual List<Move> GetPossibleMoves(Board board)
        {
            var possibleMoves = GetPossibleMovesIgnoringCheckRules(board);
            var possibleMovesIfPinned = PieceMoveService.GetPossibleMovesIfPinned(board, Colour, possibleMoves, Position);
            if (!board.IsKingInCheck(Colour))
            {
                return possibleMovesIfPinned;
            }
            var kingPosition = board.KingPositions[Colour];
            var checkingPieces = board.GetAttackingPieces(UtilityService.GetOppositeColour(Colour), kingPosition);
            if (checkingPieces.Count > 1)
            {
                return new List<Move>();
            }
            var checkingPiece = checkingPieces.First();
            return PieceMoveService.GetMovesToBlockCheck(board, possibleMovesIfPinned, kingPosition, checkingPiece);
        }
    }
}
