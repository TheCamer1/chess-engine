﻿using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(Colour colour) : base(colour)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackPawn : Properties.Resources.WhitePawn;
        }

        public override List<int> GetPossibleMoves(Board board, int position)
        {
            var possibleMoves = new List<int>();
            var direction = board.Perspective == Colour ? -1 : 1;

            AddPushesToPossibleMoves(board, possibleMoves, position, direction);

            var possibleCaptures = new List<int>();
            AddDirectCapturingStep(board, possibleCaptures, position, direction, 9);
            AddDirectCapturingStep(board, possibleCaptures, position, direction, 7);
            AddEnPassantCapturingStep(board, possibleCaptures, position, direction, 9, -1);
            AddEnPassantCapturingStep(board, possibleCaptures, position, direction, 7, 1);

            AddStepsToPossibleMoves(board, position, possibleMoves, possibleCaptures);
            return possibleMoves;
        }

        private void AddPushesToPossibleMoves(Board board, List<int> possibleMoves, int position, int direction)
        {
            var firstStepSquare = position + direction * 8;
            var pieceAtFirstStep = board.GetPiece(firstStepSquare);
            if (firstStepSquare >= 0 && firstStepSquare < 64 && pieceAtFirstStep == null)
            {
                possibleMoves.Add(firstStepSquare);
            }

            var secondStepSquare = position + direction * 16;
            var pieceAtSecondStep = board.GetPiece(secondStepSquare);
            if (MovedOn == null 
                && secondStepSquare >= 0 
                && secondStepSquare < 64 
                && pieceAtFirstStep == null 
                && pieceAtSecondStep == null)
            {
                possibleMoves.Add(secondStepSquare);
            }
        }

        private void AddDirectCapturingStep(Board board, List<int> possibleSteps, int position, int direction, int step)
        {
            var testingPosition = position + direction * step;
            var capturablePiece = board.GetPiece(testingPosition);
            if (capturablePiece != null && capturablePiece.Colour != Colour)
            {
                possibleSteps.Add(direction * step);
            }
        }

        private void AddEnPassantCapturingStep(Board board, List<int> possibleSteps, int position, int direction, int step, int capturablePieceStep)
        {
            var newPosition = position + direction * step;
            var capturablePiecePosition = position + capturablePieceStep;
            var capturablePiece = board.GetPiece(capturablePiecePosition);
            if (capturablePiece != null
                && capturablePiece.Colour != Colour
                && capturablePiece is Pawn
                && capturablePiece.MovedOn == board.CurrentPlay - 1
                && (capturablePiecePosition / 8 == 3 || capturablePiecePosition / 8 == 4))
            {
                possibleSteps.Add(direction * step);
            }
        }
    }
}
