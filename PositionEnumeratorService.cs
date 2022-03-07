using System;
using System.Collections.Generic;
using System.Linq;

namespace UserInterface
{
    public static class PositionEnumeratorService
    {
        public static Tuple<int, string> GetNumberOfPositions(Board board, int depth)
        {
            var result = GetNumberOfPositions(board, depth, new Stack<string>(), null, depth);
            var sortedPositions = result
                .Item2
                .Substring(0, result.Item2.Length - 1)
                .Split(',')
                .OrderBy(e => e);
            return new Tuple<int, string>(result.Item1, string.Join(",", sortedPositions));
        }

        private static Tuple<int, string> GetNumberOfPositions(Board board, int depth, Stack<string> moveHistory, List<string> desiredMoves = null, int? depthToPrintPositions = null)
        {
            if (depth == 0)
            {
                return new Tuple<int, string>(1, "");
            }
            var numberOfMoves = 0;
            var positionCounts = "";
            var pieces = board
                .PiecePositions
                .Values
                .Where(e => e.Colour == board.CurrentColour)
                .ToList();
            for (var i = 0; i < pieces.Count(); i++)
            {
                var piece = pieces[i];
                var moves = piece.GetPossibleMoves(board);
                foreach (var move in moves)
                {
                    MoveService.MakeMove(board, move);
                    var position = UtilityService.GetPgn(move.InitialPosition, move.FinalPosition, move.PromotionPiece);
                    moveHistory.Push(position);
                    var moveCount = GetNumberOfPositions(board, depth - 1, moveHistory, desiredMoves, depthToPrintPositions);
                    numberOfMoves += moveCount.Item1;
                    moveHistory.Pop();
                    if (desiredMoves != null && moveHistory.SequenceEqual(desiredMoves) || depthToPrintPositions == depth)
                    {
                        positionCounts += position + ": " + moveCount.Item1 + ",";
                    }
                    MoveService.UnmakeMove(board, move);
                }
            }
            return new Tuple<int, string>(numberOfMoves, positionCounts);
        }
    }
}
