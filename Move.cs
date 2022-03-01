using UserInterface.Pieces;

namespace UserInterface
{
    public class Move
    {
        public Move(Piece piece, int finalPosition)
        {
            Piece = piece;
            InitialPosition = piece.Position;
            FinalPosition = finalPosition;
            IsFirstMove = piece.MovedOn == null;
        }

        public int InitialPosition { get; set; }
        public int FinalPosition { get; set; }
        public Piece Piece { get; set; }
        public Piece CapturedPiece { get; set; }
        public bool IsCastling { get; set; }
        public bool IsEnPassant { get; set; }
        public bool IsFirstMove { get; set; }
    }
}
