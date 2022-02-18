using System.Collections.Generic;

namespace UserInterface.Pieces
{
    public class Pawn : Piece
    {
        public int? TwoStepMovePerformedOn { get; set; }
        public Pawn(Colour colour, int position) : base(colour, position)
        {
            Image = colour == Colour.Black ? Properties.Resources.BlackPawn : Properties.Resources.WhitePawn;
            PieceType = PieceType.Pawn;
        }

        public override List<int> GetAttackedSquares(Board board)
        {
            var direction = board.Perspective == Colour ? -1 : 1;
            var possibleChecks = new List<int>();
            if (ChessService.IsNextSquareValid(board, Colour, Position, Position, direction * 9, true, true))
            {
                possibleChecks.Add(Position + direction * 9);
            }
            if (ChessService.IsNextSquareValid(board, Colour, Position, Position, direction * 7, true, true))
            {
                possibleChecks.Add(Position + direction * 7);
            }
            return possibleChecks;
        }

        public override List<int> GetPossibleMovesIgnoringCheckRules(Board board)
        {
            var possibleMoves = new List<int>();
            var direction = board.Perspective == Colour ? -1 : 1;

            AddPushesToPossibleMoves(board, possibleMoves, direction);

            var possibleCaptures = new List<int>();
            AddDirectCapturingStep(board, possibleCaptures, direction, 9);
            AddDirectCapturingStep(board, possibleCaptures, direction, 7);
            AddEnPassantCapturingStep(board, possibleCaptures, direction, 9, 1);
            AddEnPassantCapturingStep(board, possibleCaptures, direction, 7, -1);

            ChessService.AddStepsToPossibleMoves(board, Colour, Position, possibleMoves, possibleCaptures);
            return possibleMoves;
        }

        private void AddPushesToPossibleMoves(Board board, List<int> possibleMoves, int direction)
        {
            var firstStepSquare = Position + direction * 8;
            var pieceAtFirstStep = board.GetPiece(firstStepSquare);
            if (firstStepSquare >= 0 && firstStepSquare < 64 && pieceAtFirstStep == null)
            {
                possibleMoves.Add(firstStepSquare);
            }

            var secondStepSquare = Position + direction * 16;
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

        private void AddDirectCapturingStep(Board board, List<int> possibleSteps, int direction, int step)
        {
            if (!ChessService.IsNextSquareValid(board, Colour, Position, Position, step * direction))
            {
                return;
            }
            var testingPosition = Position + direction * step;
            var capturablePiece = board.GetPiece(testingPosition);
            if (capturablePiece != null && capturablePiece.Colour != Colour)
            {
                possibleSteps.Add(direction * step);
            }
        }

        private void AddEnPassantCapturingStep(Board board, List<int> possibleSteps, int direction, int step, int capturablePieceStep)
        {
            if (!ChessService.IsNextSquareValid(board, Colour, Position, Position, step * direction))
            {
                return;
            }
            var capturablePiecePosition = Position + capturablePieceStep * direction;
            var capturablePiece = board.GetPiece(capturablePiecePosition);
            if (capturablePiece != null
                && capturablePiece.Colour != Colour
                && capturablePiece is Pawn
                && ((Pawn)capturablePiece).TwoStepMovePerformedOn == board.CurrentPlay - 1
                && !IsPinnedFromEnPassant(board, capturablePiecePosition))
            {
                possibleSteps.Add(direction * step);
            }
        }

        private bool IsPinnedFromEnPassant(Board board, int capturablePosition)
        {
            var vectors = new List<int> { -1, 1 };
            foreach (var vector in vectors)
            {
                if (!ChessService.IsKingOnVector(board, Colour, Position, vector) && !ChessService.IsKingOnVector(board, Colour, capturablePosition, vector))
                {
                    continue;
                }
                var pinningPiece = ChessService.GetQueenVectoredAttackingPiece(board, Colour, Position, -vector) 
                    ?? ChessService.GetQueenVectoredAttackingPiece(board, Colour, capturablePosition, -vector);
                if (pinningPiece == null)
                {
                    continue;
                }
                return true;
            }
            return false;
        }
    }
}
