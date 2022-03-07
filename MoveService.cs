using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterface.Pieces;

namespace UserInterface
{
    public static class MoveService
    {
        public static void MakeMove(Board board, Move move)
        {
            if (move.IsEnPassant)
            {
                board.PiecePositions.Remove(move.CapturedPiece.Position);
            }
            PerformCastlingRookMove(board, move.Piece.Position, move.FinalPosition, move.Piece);
            SetPawnTwoStepMovePerformedOn(board, move.Piece.Position, move.FinalPosition, move.Piece);

            board.PiecePositions.Remove(move.Piece.Position);
            board.PiecePositions[move.FinalPosition] = move.Piece;
            Promotion(board, move);

            if (move.Piece is King)
            {
                board.KingPositions[move.Piece.Colour] = move.FinalPosition;
            }

            UpdatePieceAfterMoving(board, move.PromotionPiece ?? move.Piece, move.FinalPosition);

            board.CurrentPly += 1;
            board.CurrentColour = UtilityService.GetOppositeColour(board.CurrentColour);
        }

        public static void UpdatePieceAfterMoving(Board board, Piece piece, int finalPosition)
        {
            piece.HasMoved = true;
            piece.Position = finalPosition;
            SetPieceAndLongRangePieceAttackedSquares(board, piece);
        }

        public static void UnmakeMove(Board board, Move move)
        {
            board.CurrentPly -= 1;
            board.CurrentColour = UtilityService.GetOppositeColour(board.CurrentColour);

            if (move.IsEnPassant || move.CapturedPiece == null)
            {
                board.PiecePositions.Remove(move.FinalPosition);
            }
            UnperformCastlingRookMove(board, move.InitialPosition, move.FinalPosition, move.Piece, move);
            UnsetPawnTwoStepMovePerformedOn(board, move.InitialPosition, move.FinalPosition, move.Piece);

            if (move.CapturedPiece != null)
            {
                board.PiecePositions[move.CapturedPiece.Position] = move.CapturedPiece;
            }
            board.PiecePositions[move.InitialPosition] = move.Piece;

            if (move.Piece is King)
            {
                board.KingPositions[move.Piece.Colour] = move.InitialPosition;
            }

            UpdatePieceAfterMoving(board, move.Piece, move.InitialPosition);
            if (move.IsFirstMove)
            {
                move.Piece.HasMoved = false;
            }
        }

        private static void SetPieceAndLongRangePieceAttackedSquares(Board board, Piece selectedPiece)
        {
            var selectedPieceType = selectedPiece.GetType();
            if (selectedPieceType != typeof(Queen)
                && selectedPieceType != typeof(Rook)
                && selectedPieceType != typeof(Bishop))
            {
                selectedPiece.SetAttackedSquares(board);
            }
            var longRangePieces = board.PiecePositions
                .Select(e => e.Value)
                .Where(e => e is Queen || e is Rook || e is Bishop);
            foreach (var longRangePiece in longRangePieces)
            {
                longRangePiece.SetAttackedSquares(board);
            }
        }

        private static void SetPawnTwoStepMovePerformedOn(Board board, int initialPosition, int finalPosition, Piece selectedPiece)
        {
            if (selectedPiece.GetType() != typeof(Pawn) || Math.Abs(initialPosition - finalPosition) != 16)
            {
                return;
            }
            ((Pawn)selectedPiece).TwoStepMovePerformedOn = board.CurrentPly;
        }

        private static void PerformCastlingRookMove(Board board, int initialPosition, int finalPosition, Piece selectedPiece)
        {
            if (selectedPiece.GetType() != typeof(King) || Math.Abs(initialPosition - finalPosition) != 2)
            {
                return;
            }
            if (initialPosition - finalPosition > 0)
            {
                board.PiecePositions[initialPosition - 1] = board.PiecePositions[initialPosition - 4];
                board.PiecePositions.Remove(initialPosition - 4);
                UpdatePieceAfterMoving(board, board.PiecePositions[initialPosition - 1], initialPosition - 1);
            }
            else
            {
                board.PiecePositions[initialPosition + 1] = board.PiecePositions[initialPosition + 3];
                board.PiecePositions.Remove(initialPosition + 3);
                UpdatePieceAfterMoving(board, board.PiecePositions[initialPosition + 1], initialPosition + 1);
            }
        }

        private static void UnsetPawnTwoStepMovePerformedOn(Board board, int initialPosition, int finalPosition, Piece selectedPiece)
        {
            if (selectedPiece.GetType() != typeof(Pawn) || Math.Abs(initialPosition - finalPosition) != 16)
            {
                return;
            }
            ((Pawn)selectedPiece).TwoStepMovePerformedOn = null;
        }

        private static void UnperformCastlingRookMove(Board board, int initialPosition, int finalPosition, Piece selectedPiece, Move move)
        {
            if (selectedPiece.GetType() != typeof(King) || Math.Abs(initialPosition - finalPosition) != 2)
            {
                return;
            }
            if (initialPosition - finalPosition > 0)
            {
                board.PiecePositions[initialPosition - 4] = board.PiecePositions[initialPosition - 1];
                board.PiecePositions.Remove(initialPosition - 1);
                UpdatePieceAfterMoving(board, board.PiecePositions[initialPosition - 4], initialPosition - 4);
                if (move.IsFirstMove)
                {
                    board.PiecePositions[initialPosition - 4].HasMoved = false;
                }
            }
            else
            {
                board.PiecePositions[initialPosition + 3] = board.PiecePositions[initialPosition + 1];
                board.PiecePositions.Remove(initialPosition + 1);
                UpdatePieceAfterMoving(board, board.PiecePositions[initialPosition + 3], initialPosition + 3);
                if (move.IsFirstMove)
                {
                    board.PiecePositions[initialPosition + 3].HasMoved = false;
                }
            }
        }

        private static void Promotion(Board board, Move move)
        {
            if (move.PromotionPiece == null)
            {
                return;
            }

            board.PiecePositions[move.FinalPosition] = move.PromotionPiece;
        }
    }
}
