using System.Collections.Generic;
using System.Linq;

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

        public override List<Move> GetPossibleMovesIgnoringCheckRules(Board board)
        {
            var possibleMoves = new List<Move>();
            var direction = board.Perspective == Colour ? -1 : 1;

            AddPushesToPossibleMoves(board, possibleMoves, direction);

            AddDirectCapturingStep(board, possibleMoves, direction, 9);
            AddDirectCapturingStep(board, possibleMoves, direction, 7);
            AddEnPassantCapturingStep(board, possibleMoves, direction, 9, 1);
            AddEnPassantCapturingStep(board, possibleMoves, direction, 7, -1);

            return possibleMoves;
        }

        private void AddPushesToPossibleMoves(Board board, List<Move> possibleMoves, int direction)
        {
            var firstStepSquare = Position + direction * 8;
            var pieceAtFirstStep = board.GetPiece(firstStepSquare);
            if (firstStepSquare >= 0 && firstStepSquare < 64 && pieceAtFirstStep == null)
            {
                if (Position / 8 == 1 && direction < 0 || Position / 8 == 6 && direction > 0)
                {
                    AddPromotionsToPossbleMoves(board, possibleMoves, firstStepSquare);
                    return;
                }
                possibleMoves.Add(ChessService.GetMove(board, Position, firstStepSquare));
            }

            var secondStepSquare = Position + direction * 16;
            var pieceAtSecondStep = board.GetPiece(secondStepSquare);
            if (!HasMoved
                && secondStepSquare >= 0
                && secondStepSquare < 64
                && pieceAtFirstStep == null
                && pieceAtSecondStep == null)
            {
                possibleMoves.Add(ChessService.GetMove(board, Position, secondStepSquare));
            }
        }

        private void AddPromotionsToPossbleMoves(Board board, List<Move> possibleMoves, int firstStepSquare)
        {
            var pieces = new List<Piece>()
            {
                new Queen(Colour, firstStepSquare),
                new Rook(Colour, firstStepSquare),
                new Bishop(Colour, firstStepSquare),
                new Knight(Colour, firstStepSquare)
            };
            pieces.ForEach(e =>
            {
                e.HasMoved = HasMoved;
            });

            var moves = pieces
                .Select(e =>
                {
                    var move = ChessService.GetMove(board, Position, firstStepSquare);
                    move.PromotionPiece = e;
                    return move;
                });
            possibleMoves.AddRange(moves);
        }

        private void AddDirectCapturingStep(Board board, List<Move> possibleMoves, int direction, int step)
        {
            if (!ChessService.IsNextSquareValid(board, Colour, Position, Position, step * direction))
            {
                return;
            }
            var testingPosition = Position + direction * step;
            var capturablePiece = board.GetPiece(testingPosition);
            if (capturablePiece != null
                && capturablePiece.Colour != Colour)
            {
                if (Position / 8 == 1 && direction < 0 || Position / 8 == 6 && direction > 0)
                {
                    AddPromotionsToPossbleMoves(board, possibleMoves, Position + step * direction);
                    return;
                }
                possibleMoves.Add(ChessService.GetMove(board, Position, Position + direction * step));
            }
        }

        private void AddEnPassantCapturingStep(Board board, List<Move> possibleMoves, int direction, int step, int capturablePieceStep)
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
                && ((Pawn)capturablePiece).TwoStepMovePerformedOn == board.CurrentPly - 1
                && !IsPinnedFromEnPassant(board, capturablePiecePosition))
            {
                var move = ChessService.GetMove(board, Position, Position + direction * step);
                move.IsEnPassant = true;
                move.CapturedPiece = capturablePiece;
                possibleMoves.Add(move);
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
