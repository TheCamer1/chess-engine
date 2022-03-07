using System;
using System.Collections.Generic;
using UserInterface.Pieces;

namespace UserInterface
{
    public static class BoardSetupService
    {
        public static void SetUpBoardFromFen(Board board, string fen)
        {
            var fenFields = fen.Split(' ');

            SetBoardPositions(board, fenFields);

            board.CurrentColour = fenFields[1] == "w" ? Colour.White : Colour.Black;

            ManageCastling(board, fenFields);

            SetEnPassantPosition(board, fenFields);
        }

        private static void SetEnPassantPosition(Board board, string[] fenFields)
        {
            if (fenFields[3] == "-")
            {
                return;
            }
            var enPassantPosition = UtilityService.GetPositionFromPgn(fenFields[3]);
            var colour = enPassantPosition / 8 == 2 ? Colour.Black : Colour.White;
            var position = enPassantPosition + (colour == Colour.White ? 1 : -1);
            var piece = (Pawn)board.GetPiece(position);
            piece.TwoStepMovePerformedOn = board.CurrentPly - 1;
        }

        private static void SetBoardPositions(Board board, string[] fenFields)
        {
            var position = 0;
            var boardCharacters = fenFields[0];
            foreach (var character in boardCharacters)
            {
                if (character == '/')
                {
                    continue;
                }
                if (char.IsDigit(character))
                {
                    position += int.Parse(character.ToString());
                    continue;
                }
                var piece = GetPieceFromCharacter(character, position);
                board.PiecePositions[position] = piece;
                position += 1;
                if (char.ToLower(character) == 'k')
                {
                    var colour = GetColourFromCharacter(character);
                    board.KingPositions[colour] = position;
                }
            }
        }

        private static void ManageCastling(Board board, string[] fenFields)
        {
            var castlingCharacters = fenFields[2];
            var rookPositions = new List<int>() { 0, 7, 56, 63 };
            foreach (var castlingCharacter in castlingCharacters)
            {
                var colour = GetColourFromCharacter(castlingCharacter);
                var rookRow = colour == Colour.White ? 7 : 0;
                var rookPosition = char.ToLower(castlingCharacter) == 'k' ? rookRow * 8 + 0 : rookRow * 8 + 7;
                rookPositions.Remove(rookPosition);
            }
            foreach (var rookPosition in rookPositions)
            {
                var piece = board.GetPiece(rookPosition);
                if (piece == null)
                {
                    continue;
                }
                piece.HasMoved = true;
            }
        }

        private static Colour GetColourFromCharacter(char character)
        {
            return char.IsLower(character) ? Colour.Black : Colour.White;
        }

        private static Piece GetPieceFromCharacter(char character, int position)
        {
            var colour = GetColourFromCharacter(character);
            var lowerChar = char.ToLower(character);
            Piece piece;
            switch (lowerChar)
            {
                case 'k':
                    piece = new King(colour, position);
                    if (position % 8 != 4 || position / 8 != 0 && colour == Colour.Black || position / 8 != 7 && colour == Colour.White)
                    {
                        piece.HasMoved = true;
                    }
                    break;
                case 'q':
                    piece = new Queen(colour, position);
                    break;
                case 'r':
                    piece = new Rook(colour, position);
                    break;
                case 'b':
                    piece = new Bishop(colour, position);
                    break;
                case 'n':
                    piece = new Knight(colour, position);
                    break;
                case 'p':
                    piece = new Pawn(colour, position);
                    if (position / 8 != 1 && colour == Colour.Black || position / 8 != 6 && colour == Colour.White)
                    {
                        piece.HasMoved = true;
                    }
                    break;
                default:
                    throw new Exception("Unhandled character: " + lowerChar);
            }
            return piece;
        }
    }
}
