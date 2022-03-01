using System;
using System.Collections.Generic;
using System.Linq;
using UserInterface.Pieces;

namespace UserInterface
{
    public class Board
    {
        public Dictionary<int, Piece> PiecePositions { get; set; } = new Dictionary<int, Piece>();
        public Colour Perspective { get; }
        public int CurrentPly { get; set; } = 1;
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

        public bool IsCheckmate(Colour colour)
        {
            if (!IsKingInCheck(colour))
            {
                return false;
            }
            if (PiecePositions[KingPositions[colour]].GetPossibleMoves(this).Any())
            {
                return false;
            }
            return !PiecePositions
                .Values
                .Where(e => e.Colour == colour)
                .Any(e => e.GetPossibleMoves(this).Any());
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

        public void MakeMove(Move move)
        {
            if (move.IsEnPassant)
            {
                PiecePositions.Remove(move.CapturedPiece.Position);
            }
            PerformCastlingRookMove(move.Piece.Position, move.FinalPosition, move.Piece);
            SetPawnTwoStepMovePerformedOn(move.Piece.Position, move.FinalPosition, move.Piece);

            PiecePositions.Remove(move.Piece.Position);
            PiecePositions[move.FinalPosition] = move.Piece;
            Promotion(move.FinalPosition, move.Piece);

            if (move.Piece is King)
            {
                KingPositions[move.Piece.Colour] = move.FinalPosition;
            }

            UpdatePieceAfterMoving(move.Piece, move.FinalPosition);

            CurrentPly += 1;
            CurrentColour = ChessService.GetOppositeColour(CurrentColour);
        }

        public void UpdatePieceAfterMoving(Piece piece, int finalPosition)
        {
            piece.MovedOn = CurrentPly;
            piece.Position = finalPosition;
            SetPieceAndLongRangePieceAttackedSquares(piece);
        }

        public void UnmakeMove(Move move)
        {
            CurrentPly -= 1;
            CurrentColour = ChessService.GetOppositeColour(CurrentColour);

            if (move.IsEnPassant || move.CapturedPiece == null)
            {
                PiecePositions.Remove(move.FinalPosition);
            }
            UnperformCastlingRookMove(move.InitialPosition, move.FinalPosition, move.Piece, move);
            UnsetPawnTwoStepMovePerformedOn(move.InitialPosition, move.FinalPosition, move.Piece);

            if (move.CapturedPiece != null)
            {
                PiecePositions[move.CapturedPiece.Position] = move.CapturedPiece;
            }
            PiecePositions[move.InitialPosition] = move.Piece;

            if (move.Piece is King)
            {
                KingPositions[move.Piece.Colour] = move.InitialPosition;
            }

            UpdatePieceAfterMoving(move.Piece, move.InitialPosition);
            if (move.IsFirstMove)
            {
                move.Piece.MovedOn = null;
            }
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

        private void SetPawnTwoStepMovePerformedOn(int initialPosition, int finalPosition, Piece selectedPiece)
        {
            if (selectedPiece.GetType() != typeof(Pawn) || Math.Abs(initialPosition - finalPosition) != 16)
            {
                return;
            }
            ((Pawn)selectedPiece).TwoStepMovePerformedOn = CurrentPly;
        }

        private void PerformCastlingRookMove(int initialPosition, int finalPosition, Piece selectedPiece)
        {
            if (selectedPiece.GetType() != typeof(King) || Math.Abs(initialPosition - finalPosition) != 2)
            {
                return;
            }
            if (initialPosition - finalPosition > 0)
            {
                PiecePositions[initialPosition - 1] = PiecePositions[initialPosition - 4];
                PiecePositions.Remove(initialPosition - 4);
                UpdatePieceAfterMoving(PiecePositions[initialPosition - 1], initialPosition - 1);
            }
            else
            {
                PiecePositions[initialPosition + 1] = PiecePositions[initialPosition + 3];
                PiecePositions.Remove(initialPosition + 3);
                UpdatePieceAfterMoving(PiecePositions[initialPosition + 1], initialPosition + 1);
            }
        }

        private void UnsetPawnTwoStepMovePerformedOn(int initialPosition, int finalPosition, Piece selectedPiece)
        {
            if (selectedPiece.GetType() != typeof(Pawn) || Math.Abs(initialPosition - finalPosition) != 16)
            {
                return;
            }
            ((Pawn)selectedPiece).TwoStepMovePerformedOn = null;
        }

        private void UnperformCastlingRookMove(int initialPosition, int finalPosition, Piece selectedPiece, Move move)
        {
            if (selectedPiece.GetType() != typeof(King) || Math.Abs(initialPosition - finalPosition) != 2)
            {
                return;
            }
            if (initialPosition - finalPosition > 0)
            {
                PiecePositions[initialPosition - 4] = PiecePositions[initialPosition - 1];
                PiecePositions.Remove(initialPosition - 1);
                UpdatePieceAfterMoving(PiecePositions[initialPosition - 4], initialPosition - 4);
                if (move.IsFirstMove)
                {
                    PiecePositions[initialPosition - 4].MovedOn = null;
                }
            }
            else
            {
                PiecePositions[initialPosition + 3] = PiecePositions[initialPosition + 1];
                PiecePositions.Remove(initialPosition + 1);
                UpdatePieceAfterMoving(PiecePositions[initialPosition + 1], initialPosition + 1);
                if (move.IsFirstMove)
                {
                    PiecePositions[initialPosition + 1].MovedOn = null;
                }
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

        private void SetUpBoard(Colour perspective)
        {
            var bottomColour = perspective;
            var topColour = ChessService.GetOppositeColour(perspective);
            PiecePositions[0] = new Rook(topColour, 0);
            PiecePositions[1] = new Knight(topColour, 1);
            PiecePositions[2] = new Bishop(topColour, 2);
            PiecePositions[3] = new Queen(topColour, 3);
            PiecePositions[4] = new King(topColour, 4);
            PiecePositions[5] = new Bishop(topColour, 5);
            PiecePositions[6] = new Knight(topColour, 6);
            PiecePositions[7] = new Rook(topColour, 7);
            PiecePositions[8] = new Pawn(topColour, 8);
            PiecePositions[9] = new Pawn(topColour, 9);
            PiecePositions[10] = new Pawn(topColour, 10);
            PiecePositions[11] = new Pawn(topColour, 11);
            PiecePositions[12] = new Pawn(topColour, 12);
            PiecePositions[13] = new Pawn(topColour, 13);
            PiecePositions[14] = new Pawn(topColour, 14);
            PiecePositions[15] = new Pawn(topColour, 15);

            PiecePositions[48] = new Pawn(bottomColour, 48);
            PiecePositions[49] = new Pawn(bottomColour, 49);
            PiecePositions[50] = new Pawn(bottomColour, 50);
            PiecePositions[51] = new Pawn(bottomColour, 51);
            PiecePositions[52] = new Pawn(bottomColour, 52);
            PiecePositions[53] = new Pawn(bottomColour, 53);
            PiecePositions[54] = new Pawn(bottomColour, 54);
            PiecePositions[55] = new Pawn(bottomColour, 55);
            PiecePositions[56] = new Rook(bottomColour, 56);
            PiecePositions[57] = new Knight(bottomColour, 57);
            PiecePositions[58] = new Bishop(bottomColour, 58);
            PiecePositions[59] = new Queen(bottomColour, 59);
            PiecePositions[60] = new King(bottomColour, 60);
            PiecePositions[61] = new Bishop(bottomColour, 61);
            PiecePositions[62] = new Knight(bottomColour, 62);
            PiecePositions[63] = new Rook(bottomColour, 63);

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
