using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using UserInterface.Pieces;

namespace UserInterface
{
    public partial class ChessBoard : Form
    {
        private const int _panelSize = 100;
        private const int _gridSize = 8;

        private Board _board;
        private List<int> _potentialMoves = new List<int>();
        private int? _selectedPiecePosition;

        private Panel[,] _panels = new Panel[_gridSize, _gridSize];

        public ChessBoard()
        {
            InitializeComponent();
        }

        private void LoadChessBoard(object sender, EventArgs e)
        {
            _board = new Board(Colour.White);
            SetUpPanels();
            DrawBoard(_board);
        }

        private void PlayEveryPosition(int numberOfMoves)
        {
            for (var i = 0; i < numberOfMoves; i++)
            {
                MovePieces(board);
            }
        }

        private void MovePieces(Board board)
        {
            foreach (var piece in board.PiecePositions)
            {
                var possibleMoves = piece.Value.GetPossibleMoves(board);
                foreach (var move in possibleMoves)
                {
                    board.MovePiece(piece.Value, move);
                }
            }
        }

        private void SetUpPanels()
        {
            for (int row = 0; row < _gridSize; row++)
            {
                for (int column = 0; column < _gridSize; column++)
                {
                    var newPanel = new Panel
                    {
                        Size = new Size(_panelSize, _panelSize),
                        Location = new Point(_panelSize * column, _panelSize * row),
                        BackgroundImageLayout = ImageLayout.Stretch
                    };

                    Controls.Add(newPanel);

                    _panels[column, row] = newPanel;
                }
            }

            foreach (Control control in Controls)
            {
                if (control is Panel)
                {
                    control.Click += OnPanelClick;
                }
            }
        }

        private void DrawBoard(Board board)
        {
            for (int row = 0; row < _gridSize; row++)
            {
                for (int column = 0; column < _gridSize; column++)
                {
                    var isLightSquare = (row + column) % 2 != 0;
                    _panels[column, row].BackColor = isLightSquare ? Color.FromArgb(255, 206, 158) : Color.FromArgb(209, 139, 71);
                }
            }

            for (var i = 0; i < 64; i++)
            {
                var point = ChessService.GetPointFromPosition(i);

                //if (board.IsSquareAttacked(Colour.White, i))
                //{
                //    _panels[point.X, point.Y].BackColor = Color.AliceBlue;
                //}

                if (board.GetPiece(i) == null)
                {
                    _panels[point.X, point.Y].BackgroundImage = null;
                    continue;
                }
                _panels[point.X, point.Y].BackgroundImage = board.GetPiece(i).Image;
            }
        }

        private void PlayRandomly()
        {
            var pieceMoves = "";
            var random = new Random();
            while (!IsGameOver())
            {
                var pieceCount = _board
                    .PiecePositions
                    .Where(e => e.Value.Colour == _board.CurrentColour)
                    .Count();
                int pieceNumber = random.Next(0, pieceCount);
                var piece = _board
                    .PiecePositions
                    .Where(e => e.Value.Colour == _board.CurrentColour)
                    .ElementAt(random.Next(0, pieceCount));
                var possibleMoves = piece.Value.GetPossibleMoves(_board);
                if (!possibleMoves.Any())
                {
                    continue;
                }
                int moveNumber = random.Next(0, possibleMoves.Count());
                var move = possibleMoves[moveNumber];
                //var pieceMove = $"{piece.Key},{move} ";
                var pieceMove = $"{_board.CurrentColour} {piece.Value.PieceType}: {piece.Key}, {move}\n";
                pieceMoves += pieceMove;
                try
                {
                    _board.MovePiece(piece.Value, move);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                DrawBoard(_board);
                Wait(5);
            }
        }

        public void Wait(int milliseconds)
        {
            var timer = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            timer.Interval = milliseconds;
            timer.Enabled = true;
            timer.Start();

            timer.Tick += (s, e) =>
            {
                timer.Enabled = false;
                timer.Stop();
            };

            while (timer.Enabled)
            {
                Application.DoEvents();
            }
        }

        private bool IsGameOver()
        {
            return !_board
                .PiecePositions
                .Where(e => e.Value.Colour == _board.CurrentColour)
                .SelectMany(e => e.Value.GetPossibleMoves(_board))
                .Any();
        }

        private void PlayGame()
        {
            var moveString = "51,43 6,23 50,34 8,24 59,51 7,6 55,39 0,16 60,59 12,28 53,45 16,22 52,36 6,7 48,32 3,39 45,37 1,18 56,48 28,37 54,38 39,31 34,26 23,29 57,40 5,19 48,56 22,20 43,35 7,6 63,47 9,17 51,24 37,45 40,50 15,23 24,8 4,12 47,45 18,28 35,28 17,26 49,41 6,5 50,35 12,3 36,29 2,9 32,24 10,18 35,50 5,4 56,57 9,0 41,33 26,33 29,21 19,10 62,52 3,2 45,37 31,28 8,62 10,24 21,14 0,9 57,49 13,21 38,30 24,10 14,6 2,1 49,57 9,0 57,49 11,19 59,51 18,26 51,60 10,3 6,20 3,24 20,47 1,8 49,57 0,18 30,21 24,17 47,2 19,27 61,54 28,21 58,49 23,31 50,44 31,39 54,61 18,25 61,54 33,41 62,55 26,34 2,18 4,44 57,56 17,24 56,24 8,1 24,32 21,19 55,63 27,35 60,61 19,21 61,53 44,4 63,39 21,23 52,42 23,37 54,45 4,12 39,31 12,52 53,62 37,5 18,34 5,23 42,52 25,4 32,40 4,11 31,4 11,4 40,32 35,43 34,39 43,52 49,56 23,31 56,35 31,29 45,0 1,10 62,55 52,60 39,12 10,1 32,40 41,49 55,54 4,25 35,14 29,28 0,36 28,19 36,18 1,2 40,45 60,63 54,63 19,23 12,39 23,37 18,0 2,10 45,40 10,17 40,56 37,39 63,54 25,34 0,36 49,56 36,9 56,14 54,62 39,37 62,63 17,24 9,36 14,38 36,43 34,41 43,52 41,34 52,34 38,52 34,6 52,12 6,15 24,25 15,22 25,16 22,43 37,34 63,54 34,25 54,62 12,10 43,15 10,50 15,6 16,17 62,63 50,41 6,13 41,40 13,41 17,26 41,48 26,18 48,34 25,28 63,62 28,4 62,55 40,33 34,16 33,35 55,47 18,27 16,52 27,26 52,25 4,2 47,55 2,7 55,46 35,21 46,54 21,17 54,45 26,19 25,52 17,26 52,34 7,5 45,54 19,12 54,63 12,21 34,52 5,13 52,16 13,31 63,54 26,29 16,9 31,4 54,55 21,13 55,46 29,21 9,63 4,11 63,36 13,4 36,43 11,14 43,22 14,13 46,54 21,56 54,47 56,16 22,31 4,5 47,39 5,14 31,45 13,8 45,27 8,26 39,47 16,18 47,38 18,10 38,45 14,15 45,54 15,7 27,41 26,31 41,34 31,45 54,45 10,55 45,38 55,15 34,25 7,6 25,32 15,9 38,29 6,15 29,28 15,23 28,19 9,10 19,28 10,58 32,25 58,51 25,32 51,54 28,21 23,15 21,29 15,23 29,37 54,51 37,46 51,59 32,25 59,52 25,32 52,55 46,55 23,30 55,63 30,38 32,4 38,30 4,31 30,31 63,62 31,39 62,54 39,30 54,47 30,37 47,55 37,45 55,47 45,37 47,54 37,38 54,55 38,37 55,47 37,30 47,54 30,37 54,53 37,29 53,44 29,20 44,51 20,21 51,50 21,29 50,59 29,22 59,52 22,29 52,60 29,21 60,52 ";
            var moves = moveString.Split(' ').Where(e => !string.IsNullOrWhiteSpace(e)).Select(e =>
            {
                var plays = e.Split(',');
                return new
                {
                    Start = int.Parse(plays[0]),
                    End = int.Parse(plays[1])
                };
            }).ToList();
            for (var i = 0; i < moves.Count(); i++)
            {
                var move = moves[i];
                if (i > 297)
                {
                    Wait(500);
                }
                Wait(10);
                _board.MovePiece(_board.GetPiece(move.Start), move.End);
                DrawBoard(_board);
            }
        }

        private void OnPanelClick2(object sender, EventArgs e)
        {
            _board = new Board(Colour.White);
            DrawBoard(_board);
            //PlayRandomly();
            //PlayGame();
            PlayEveryPosition(1);
        }

        private void OnPanelClick(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            DrawBoard(_board);

            int selectedPosition = GetPanelPosition(panel);

            //check if square is highlighted and if is, moves selected piece
            if (_potentialMoves.Contains(selectedPosition))
            {
                if (_selectedPiecePosition == null)
                {
                    throw new Exception("Piece is not selected");
                }
                CheckForEndOfGame(selectedPosition);
                var selectedPiece = _board.GetPiece(_selectedPiecePosition.Value);
                _board.MovePiece(selectedPiece, selectedPosition);
                _potentialMoves.Clear();
                DrawBoard(_board);
                return;
            }

            //check if has unselected piece
            var chessPiece = _board.GetPiece(selectedPosition);
            if (chessPiece == null || chessPiece.Colour != _board.CurrentColour)
            {
                _selectedPiecePosition = null;
                _potentialMoves.Clear();
                return;
            }
            _selectedPiecePosition = selectedPosition;

            var panelCoordinates = ChessService.GetPointFromPosition(selectedPosition);
            _panels[panelCoordinates.X, panelCoordinates.Y].BackColor = Color.Blue;
            _potentialMoves = chessPiece.GetPossibleMoves(_board);

            // colors in potential panels
            foreach (var potentialMove in _potentialMoves)
            {
                var potentialMoveCoordinates = ChessService.GetPointFromPosition(potentialMove);
                _panels[potentialMoveCoordinates.X, potentialMoveCoordinates.Y].BackColor = Color.LightBlue;
            }
        }

        private void CheckForEndOfGame(int selectedPosition)
        {
            if (_board.GetPiece(selectedPosition) is King)
            {
                if (_board.GetPiece(selectedPosition).Colour == Colour.White)
                {
                    BlackWins.Visible = true;
                }
                else
                {
                    WhiteWins.Visible = true;
                }
            }
        }

        private int GetPanelPosition(Panel panel)
        {
            for (int row = 0; row < _gridSize; row++)
            {
                for (int column = 0; column < _gridSize; column++)
                {
                    if (_panels[row, column] == panel)
                    {
                        return ChessService.GetPositionFromPoint(row, column);
                    }
                }
            }
            throw new Exception("Panel was not found");
        }
    }
}
