using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UserInterface.Pieces;

namespace UserInterface
{
    public class Board
    {
        public Dictionary<int, Piece> PiecePositions { get; set; } = new Dictionary<int, Piece>();
        public Colour Perspective { get; }
        public int CurrentPlay { get; set; } = 0;
        public Colour CurrentColour { get; set; } = Colour.White;
        public Dictionary<Colour, int> KingPositions { get; set; } = new Dictionary<Colour, int>();

        public Board(Colour perspective)
        {
            Perspective = perspective;
            SetUpBoard(perspective);
            foreach (var piece in PiecePositions)
            {
                piece.Value.SetAttackedSquares(this);
            }
        }

        public bool IsKingInCheck(Colour colour)
        {
            var kingPosition = KingPositions[colour];
            return IsSquareAttacked(ChessService.GetOppositeColour(colour), kingPosition);
        }

        public bool IsSquareAttacked(Colour colour, int square)
        {
            return PiecePositions.Any(e => e.Value.Colour == colour && e.Value.AttackedSquares.Contains(square));
        }

        public List<Piece> GetAttackingPieces(Colour colour, int square)
        {
            return PiecePositions
                .Select(e => e.Value)
                .Where(e => e.Colour == colour && e.AttackedSquares.Contains(square))
                .ToList();
        }

        public void MovePiece(Piece piece, int selectedPosition)
        {
            var selectedPiecePosition = piece.Position;
            PerformEnPassantCapture(selectedPiecePosition, selectedPosition, piece);
            PerformCastlingRookMove(selectedPiecePosition, selectedPosition, piece);
            SetPawnTwoStepMovePerformedOn(selectedPiecePosition, selectedPosition, piece);

            MovePiece(selectedPiecePosition, selectedPosition, piece);

            if (piece is King)
            {
                KingPositions[piece.Colour] = selectedPosition;
            }

            CurrentPlay += 1;
            CurrentColour = ChessService.GetOppositeColour(CurrentColour);
        }

        private void MovePiece(int selectedPiecePosition, int selectedPosition, Piece selectedPiece)
        {
            PiecePositions.Remove(selectedPiecePosition);
            PiecePositions.Remove(selectedPosition);
            PiecePositions[selectedPosition] = selectedPiece;
            Promotion(selectedPosition, selectedPiece);

            selectedPiece.MovedOn = CurrentPlay;
            selectedPiece.Position = selectedPosition;
            SetPieceAndLongRangePieceAttackedSquares(selectedPiece);
        }

        private void SetPieceAndLongRangePieceAttackedSquares(Piece selectedPiece)
        {
            var selectedPieceType = selectedPiece.GetType();
            if (selectedPieceType != typeof(Queen) 
                && selectedPieceType != typeof(Rook) 
                && selectedPieceType != typeof(Bishop))
            {
                selectedPiece.SetAttackedSquares(this);
            }
            var longRangePieces = PiecePositions
                .Select(e => e.Value)
                .Where(e => e is Queen || e is Rook || e is Bishop);
            foreach (var longRangePiece in longRangePieces)
            {
                longRangePiece.SetAttackedSquares(this);
            }
        }

        private void SetPawnTwoStepMovePerformedOn(int selectedPiecePosition, int selectedPosition, Piece selectedPiece)
        {
            if (selectedPiece.GetType() != typeof(Pawn) || (selectedPiecePosition - selectedPosition) % 8 != 0)
            {
                return;
            }
            ((Pawn)selectedPiece).TwoStepMovePerformedOn = CurrentPlay;
        }

        private void PerformCastlingRookMove(int selectedPiecePosition, int selectedPosition, Piece selectedPiece)
        {
            if (selectedPiece.GetType() != typeof(King) || Math.Abs(selectedPiecePosition - selectedPosition) != 2)
            {
                return;
            }
            if (selectedPiecePosition - selectedPosition > 0)
            {
                MovePiece(selectedPiecePosition - 4, selectedPiecePosition - 1, PiecePositions[selectedPiecePosition - 4]);
            }
            else
            {
                MovePiece(selectedPiecePosition + 3, selectedPiecePosition + 1, PiecePositions[selectedPiecePosition + 3]);
            }
        }

        private void Promotion(int selectedPosition, Piece selectedPiece)
        {
            if (selectedPiece is Pawn && (selectedPosition / 8 == 0 || selectedPosition / 8 == 7))
            {
                var newQueen = new Queen(selectedPiece.Colour, selectedPosition)
                {
                    MovedOn = selectedPiece.MovedOn
                };
                PiecePositions[selectedPosition] = newQueen;
            }
        }

        private void PerformEnPassantCapture(int selectedPiecePosition, int selectedPosition, Piece selectedPiece)
        {
            if (selectedPiece is Pawn
                && (selectedPiecePosition - selectedPosition) % 8 != 0
                && GetPiece(selectedPosition) == null)
            {
                var vector = selectedPiecePosition - selectedPosition;
                int capturedPawnPosition;
                if (Math.Abs(vector) == 9)
                {
                    capturedPawnPosition = selectedPiecePosition - (vector / Math.Abs(vector));
                }
                else
                {
                    capturedPawnPosition = selectedPiecePosition + (vector / Math.Abs(vector));
                }
                PiecePositions.Remove(capturedPawnPosition);
            }
        }

        private void SetUpBoard(Colour perspective)
        {
            var bottomColour = perspective;
            var topColour = ChessService.GetOppositeColour(perspective);
            PlacePieces(topColour, 0);
            for (var i = 8; i < 16; i++)
            {
                PiecePositions[i] = new Pawn(topColour, i);
            }
            for (var i = 48; i < 56; i++)
            {
                PiecePositions[i] = new Pawn(bottomColour, i);
            }
            PlacePieces(bottomColour, 56);

            KingPositions[bottomColour] = 60;
            KingPositions[topColour] = 4;
        }

        private void PlacePieces(Colour colour, int startingPosition)
        {
            PiecePositions[startingPosition] = new Rook(colour, startingPosition);
            PiecePositions[startingPosition + 1] = new Knight(colour, startingPosition + 1);
            PiecePositions[startingPosition + 2] = new Bishop(colour, startingPosition + 2);
            PiecePositions[startingPosition + 3] = new Queen(colour, startingPosition + 3);
            PiecePositions[startingPosition + 4] = new King(colour, startingPosition + 4);
            PiecePositions[startingPosition + 5] = new Bishop(colour, startingPosition + 5);
            PiecePositions[startingPosition + 6] = new Knight(colour, startingPosition + 6);
            PiecePositions[startingPosition + 7] = new Rook(colour, startingPosition + 7);
        }

        public Piece GetPiece(int x, int y)
        {
            return GetPiece(ChessService.GetPositionFromPoint(x, y));
        }

        public Piece GetPiece(int position)
        {
            if (position < 0 || position > 63 || !PiecePositions.ContainsKey(position))
            {
                return null;
            }
            return PiecePositions[position];
        }
    }
}
