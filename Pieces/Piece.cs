using System;
using System.Collections.Generic;
using System.Drawing;

namespace UserInterface.Pieces
{
    public abstract class Piece
    {
        public Colour Colour { get; set; }
        public Image Image { get; set; }
        public int? MovedOn { get; set; }

        public Piece(Colour colour)
        {
            Colour = colour;
        }

        public abstract List<int> GetPossibleMoves(Board board, int position);

        protected List<int> GetDiagonalMoves(Board board, int position)
        {
            var possibleMoves = new List<int>();
            var leftUp = -9;
            var rightUp = -7;
            var leftDown = 7;
            var rightDown = 9;
            AddVectorToPossibleMoves(board, position, possibleMoves, leftUp);
            AddVectorToPossibleMoves(board, position, possibleMoves, rightUp);
            AddVectorToPossibleMoves(board, position, possibleMoves, leftDown);
            AddVectorToPossibleMoves(board, position, possibleMoves, rightDown);
            return possibleMoves;
        }

        protected List<int> GetHorizontalMoves(Board board, int position)
        {
            var possibleMoves = new List<int>();
            var left = -1;
            var right = 1;
            var up = -8;
            var down = 8;
            AddVectorToPossibleMoves(board, position, possibleMoves, left);
            AddVectorToPossibleMoves(board, position, possibleMoves, right);
            AddVectorToPossibleMoves(board, position, possibleMoves, up);
            AddVectorToPossibleMoves(board, position, possibleMoves, down);
            return possibleMoves;
        }

        protected void AddVectorToPossibleMoves(Board board, int initialPosition, List<int> possibleMoves, int vector)
        {
            var testingPosition = initialPosition;
            while (IsNextSquareValid(board, initialPosition, testingPosition, vector))
            {
                testingPosition += vector;
                possibleMoves.Add(testingPosition);
            }
        }

        protected void AddStepsToPossibleMoves(Board board, int initialPosition, List<int> possibleMoves, List<int> steps)
        {
            foreach (var step in steps)
            {
                if (IsNextSquareValid(board, initialPosition, initialPosition, step))
                {
                    possibleMoves.Add(initialPosition + step);
                }
            }
        }

        protected bool IsNextSquareValid(Board board, int initialPosition, int testingPosition, int vector)
        {
            var newSquare = testingPosition + vector;
            var testingPiece = board.GetPiece(testingPosition);
            var newPiece = board.GetPiece(newSquare);
            return newSquare >= 0
                && newSquare < 64
                && Math.Abs(newSquare % 8 - testingPosition % 8) <= 2
                && (newPiece == null || newPiece.Colour != Colour)
                && (testingPiece == null || testingPosition == initialPosition);
        }
    }
}
