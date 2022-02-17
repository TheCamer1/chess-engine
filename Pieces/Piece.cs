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

        protected readonly List<int> _diagonalVectors = new List<int> { -9, -7, 7, 9 };
        protected readonly List<int> _horizontalVectors = new List<int> { -1, 1, -8, 8 };
        protected readonly List<int> _queenVectors = new List<int> { -9, -7, 7, 9 , -1, 1, -8, 8 };
        private readonly List<int> _knightVectors = new List<int>() { -17, -15, -6, 10, 17, 15, 6, -10 };

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
            var possibleMovesIfPinned = GetPossibleMovesIfPinned(board, possibleMoves, position);
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
            return GetMovesToBlockCheck(board, possibleMovesIfPinned, kingPosition, checkingPiece);
        }

        private List<int> GetMovesToBlockCheck(Board board, List<int> possibleMovesIfPinned, int kingPosition, Piece checkingPiece)
        {
            var squaresAlongVector = GetSquaresAlongVectorUntilPiece(board, checkingPiece, kingPosition);
            return possibleMovesIfPinned
                .Where(e => squaresAlongVector.Contains(e))
                .ToList();
        }

        private int GetVector(int initialPosition, int finalPosition)
        {
            var difference = finalPosition - initialPosition;
            var direction = difference / Math.Abs(difference);
            if (difference % 9 == 0)
            {
                return direction * 9;
            }
            if (difference % 8 == 0)
            {
                return direction * 8;
            }
            if (difference % 7 == 0)
            {
                return direction * 7;
            }
            return difference;
        }

        private List<int> GetSquaresAlongVectorUntilPiece(Board board, Piece piece, int initialPosition)
        {
            var squares = new List<int>();
            var currentPosition = initialPosition;
            var vector = GetVector(initialPosition, piece.Position);
            while (board.GetPiece(currentPosition + vector) != piece)
            {
                currentPosition += vector;
                squares.Add(currentPosition);
            }
            squares.Add(currentPosition += vector);
            return squares;
        }

        protected List<int> GetPossibleMovesIfPinned(Board board, List<int> possibleMoves, int initialPosition)
        {
            foreach (var vector in _queenVectors)
            {
                if (!IsKingOnVector(board, initialPosition, vector))
                {
                    continue;
                }
                var pinningPiece = GetQueenVectoredAttackingPiece(board, initialPosition, -vector);
                if (pinningPiece == null)
                {
                    continue;
                }
                return GetPinnedPossibleMoves(possibleMoves, initialPosition, vector);
            }
            return possibleMoves;
        }

        private static List<int> GetPinnedPossibleMoves(List<int> possibleMoves, int initialPosition, int vector)
        {
            var pinnedPossibleMoves = new List<int>();
            foreach (var possibleMove in possibleMoves)
            {
                if ((possibleMove - initialPosition) % vector == 0)
                {
                    pinnedPossibleMoves.Add(possibleMove);
                }
            }
            return pinnedPossibleMoves;
        }

        private bool IsKingOnVector(Board board, int initialPosition, int vector)
        {
            var endOfVector = GetEndOfVector(board, initialPosition, vector);
            var piece = board.GetPiece(endOfVector);
            if (piece == null || piece.Colour != Colour || piece.GetType() != typeof(King))
            {
                return false;
            }
            return true;
        }

        private Piece GetQueenVectoredAttackingPiece(Board board, int initialPosition, int vector)
        {
            var endOfVector = GetEndOfVector(board, initialPosition, vector);
            var piece = board.GetPiece(endOfVector);
            if (piece == null || piece.Colour == Colour)
            {
                return null;
            }

            if (piece.GetType() != typeof(Queen)
                && (piece.GetType() != typeof(Bishop) || !_diagonalVectors.Contains(vector))
                && (piece.GetType() != typeof(Rook) || !_horizontalVectors.Contains(vector))
                && (piece.GetType() != typeof(Pawn) || !_diagonalVectors.Contains(vector)))
            {
                return null;
            }

            return piece;
        }

        protected int GetEndOfVector(Board board, int initialPosition, int vector)
        {
            var testingPosition = initialPosition;
            while (IsNextSquareValid(board, initialPosition, testingPosition, vector, true))
            {
                testingPosition += vector;
            }
            return testingPosition;
        }

        protected List<int> GetDiagonalMoves(Board board, int position, bool includeOwnPieces = false)
        {
            var possibleMoves = new List<int>();
            foreach (var vector in _diagonalVectors)
            {
                AddVectorToPossibleMoves(board, position, possibleMoves, vector, includeOwnPieces);
            }
            return possibleMoves;
        }

        protected List<int> GetHorizontalMoves(Board board, int position, bool includeOwnPieces = false)
        {
            var possibleMoves = new List<int>();
            foreach (var vector in _horizontalVectors)
            {
                AddVectorToPossibleMoves(board, position, possibleMoves, vector, includeOwnPieces);
            }
            return possibleMoves;
        }

        protected void AddVectorToPossibleMoves(Board board, int initialPosition, List<int> possibleMoves, int vector, bool includeOwnPieces = false)
        {
            var testingPosition = initialPosition;
            while (IsNextSquareValid(board, initialPosition, testingPosition, vector, includeOwnPieces))
            {
                testingPosition += vector;
                possibleMoves.Add(testingPosition);
            }
        }

        protected void AddStepsToPossibleMoves(Board board, int initialPosition, List<int> possibleMoves, List<int> steps, bool includeOwnPieces = false)
        {
            foreach (var step in steps)
            {
                if (IsNextSquareValid(board, initialPosition, initialPosition, step, includeOwnPieces))
                {
                    possibleMoves.Add(initialPosition + step);
                }
            }
        }

        protected bool IsNextSquareValid(Board board, int initialPosition, int testingPosition, int vector, bool includeOwnPieces = false)
        {
            var newSquare = testingPosition + vector;
            var testingPiece = board.GetPiece(testingPosition);
            var newPiece = board.GetPiece(newSquare);
            return newSquare >= 0
                && newSquare < 64
                && Math.Abs(newSquare % 8 - testingPosition % 8) <= 2
                && (includeOwnPieces || newPiece == null || newPiece.Colour != Colour)
                && (testingPiece == null 
                    || testingPosition == initialPosition 
                    || (includeOwnPieces && testingPiece is King && testingPiece.Colour != Colour));
        }
    }
}
