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

        public Board(Colour perspective, string fen = null)
        {
            Perspective = perspective;
            SetUpBoard(perspective, fen);
            foreach (var piece in PiecePositions)
            {
                piece.Value.SetAttackedSquares(this);
            }
        }

        public bool IsKingInCheck(Colour colour)
        {
            var kingPosition = KingPositions[colour];
            return IsSquareAttacked(UtilityService.GetOppositeColour(colour), kingPosition);
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

        private void SetUpBoard(Colour perspective, string fen)
        {
            BoardSetupService.SetUpBoardFromFen(this, fen ?? "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        public Piece GetPiece(int x, int y)
        {
            return GetPiece(UtilityService.GetPositionFromPoint(x, y));
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
