using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UserInterface.Pieces;

namespace UserInterface
{
    public static class ChessService
    {
        public static readonly List<int> _diagonalVectors = new List<int> { -9, -7, 7, 9 };
        public static readonly List<int> _horizontalVectors = new List<int> { -1, 1, -8, 8 };
        public static readonly List<int> _queenVectors = new List<int> { -9, -7, 7, 9, -1, 1, -8, 8 };
        public static readonly List<int> _knightVectors = new List<int>() { -17, -15, -6, 10, 17, 15, 6, -10 };

        public static List<int> GetMovesToBlockCheck(Board board, List<int> possibleMovesIfPinned, int kingPosition, Piece checkingPiece)
        {
            var squaresAlongVector = GetSquaresAlongVectorUntilPiece(board, checkingPiece, kingPosition);
            return possibleMovesIfPinned
                .Where(e => squaresAlongVector.Contains(e))
                .ToList();
        }

        public static int GetVector(int initialPosition, int finalPosition)
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

        public static List<int> GetSquaresAlongVectorUntilPiece(Board board, Piece piece, int initialPosition)
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

        public static List<int> GetPossibleMovesIfPinned(Board board, Colour colour, List<int> possibleMoves, int initialPosition)
        {
            foreach (var vector in _queenVectors)
            {
                if (!IsKingOnVector(board, colour, initialPosition, vector))
                {
                    continue;
                }
                var pinningPiece = GetQueenVectoredAttackingPiece(board, colour, initialPosition, -vector, true);
                if (pinningPiece == null)
                {
                    continue;
                }
                return GetPinnedPossibleMoves(possibleMoves, initialPosition, vector);
            }
            return possibleMoves;
        }

        public static List<int> GetPinnedPossibleMoves(List<int> possibleMoves, int initialPosition, int vector)
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

        public static bool IsKingOnVector(Board board, Colour colour, int initialPosition, int vector)
        {
            var endOfVector = GetEndOfVector(board, colour, initialPosition, vector, true);
            var piece = board.GetPiece(endOfVector);
            if (piece == null || piece.Colour != colour || piece.GetType() != typeof(King))
            {
                return false;
            }
            return true;
        }

        public static Piece GetQueenVectoredAttackingPiece(Board board, Colour colour, int initialPosition, int vector, bool longRangeOnly = false)
        {
            var endOfVector = GetEndOfVector(board, colour, initialPosition, vector, true, true);
            var piece = board.GetPiece(endOfVector);
            if (piece == null || piece.Colour == colour)
            {
                return null;
            }

            if (piece.GetType() != typeof(Queen)
                && (piece.GetType() != typeof(Bishop) || !_diagonalVectors.Contains(vector))
                && (piece.GetType() != typeof(Rook) || !_horizontalVectors.Contains(vector))
                && (longRangeOnly || piece.GetType() != typeof(Pawn) || !_diagonalVectors.Contains(vector)))
            {
                return null;
            }

            return piece;
        }

        public static int GetEndOfVector(Board board, Colour colour, int initialPosition, int vector, bool includeOwnPieces = false, bool excludeKings = false)
        {
            var testingPosition = initialPosition;
            while (IsNextSquareValid(board, colour, initialPosition, testingPosition, vector, includeOwnPieces, excludeKings))
            {
                testingPosition += vector;
            }
            return testingPosition;
        }

        public static List<int> GetDiagonalMoves(Board board, Colour colour, int position, bool includeOwnPieces = false)
        {
            var possibleMoves = new List<int>();
            foreach (var vector in _diagonalVectors)
            {
                AddVectorToPossibleMoves(board, colour, position, possibleMoves, vector, includeOwnPieces);
            }
            return possibleMoves;
        }

        public static List<int> GetHorizontalMoves(Board board, Colour colour, int position, bool includeOwnPieces = false)
        {
            var possibleMoves = new List<int>();
            foreach (var vector in _horizontalVectors)
            {
                AddVectorToPossibleMoves(board, colour, position, possibleMoves, vector, includeOwnPieces);
            }
            return possibleMoves;
        }

        public static void AddVectorToPossibleMoves(Board board, Colour colour, int initialPosition, List<int> possibleMoves, int vector, bool includeOwnPieces = false)
        {
            var testingPosition = initialPosition;
            while (IsNextSquareValid(board, colour, initialPosition, testingPosition, vector, includeOwnPieces))
            {
                testingPosition += vector;
                possibleMoves.Add(testingPosition);
            }
        }

        public static void AddStepsToPossibleMoves(Board board, Colour colour, int initialPosition, List<int> possibleMoves, List<int> steps, bool includeOwnPieces = false)
        {
            foreach (var step in steps)
            {
                if (IsNextSquareValid(board, colour, initialPosition, initialPosition, step, includeOwnPieces))
                {
                    possibleMoves.Add(initialPosition + step);
                }
            }
        }

        public static bool IsNextSquareValid(Board board, Colour colour, int initialPosition, int testingPosition, int vector, bool includeOwnPieces = false, bool excludeKings = false)
        {
            var newSquare = testingPosition + vector;
            var testingPiece = board.GetPiece(testingPosition);
            var newPiece = board.GetPiece(newSquare);
            return newSquare >= 0
                && newSquare < 64
                && Math.Abs(newSquare % 8 - testingPosition % 8) <= 2
                && (includeOwnPieces || newPiece == null || newPiece.Colour != colour)
                && (testingPiece == null 
                    || testingPosition == initialPosition 
                    || (excludeKings && testingPiece is King && testingPiece.Colour != colour));
        }

        public static Point GetPointFromPosition(int position)
        {
            var x = position % 8;
            var y = position / 8;
            return new Point(x, y);
        }

        public static int GetPositionFromPoint(int x, int y)
        {
            if (x > 7 || y > 7)
            {
                throw new System.Exception("Invalid coordinates: " + x + ", " + y);
            }
            return x + 8 * y;
        }

        public static Colour GetOppositeColour(Colour colour)
        {
            return colour == Colour.White ? Colour.Black : Colour.White;
        }
    }
}
