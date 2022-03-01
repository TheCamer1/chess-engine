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

        private Board _board = new Board(Colour.White);
        private List<Move> _potentialMoves = new List<Move>();
        private int? _selectedPiecePosition;
        private Stack<Move> _moves = new Stack<Move>();

        private Panel[,] _panels = new Panel[_gridSize, _gridSize];

        public ChessBoard()
        {
            InitializeComponent();
            KeyDown += ChessBoard_KeyDown;
            KeyPreview = true;
        }

        private void ChessBoard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.M)
            {
                RunNumberOfPositions(sender, e);
            }
            if (e.KeyCode == Keys.Left)
            {
                if (!_moves.Any())
                {
                    return;
                }
                var move = _moves.Pop();
                _board.UnmakeMove(move);
                _potentialMoves.Clear();
                RedrawBoard();
            }
        }

        private void LoadChessBoard(object sender, EventArgs e)
        {
            SetUpPanels();
            RedrawBoard();
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

        private void RedrawBoard()
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

                //if (_board.IsSquareAttacked(Colour.White, i))
                //{
                //    _panels[point.X, point.Y].BackColor = Color.AliceBlue;
                //}

                if (_board.GetPiece(i) == null)
                {
                    _panels[point.X, point.Y].BackgroundImage = null;
                    continue;
                }
                _panels[point.X, point.Y].BackgroundImage = _board.GetPiece(i).Image;
            }
        }

        private void PlayRandomly()
        {
            while (!IsGameOver())
            {
                var random = new Random();
                var pieces = _board.PiecePositions.Where(e => e.Value.Colour == _board.CurrentColour).ToList();
                int pieceNumber = random.Next(0, pieces.Count());
                var piece = pieces[pieceNumber];
                var possibleMoves = piece.Value.GetPossibleMoves(_board);
                if (!possibleMoves.Any())
                {
                    continue;
                }
                Wait(100);
                int moveNumber = random.Next(0, possibleMoves.Count() - 1);
                var move = possibleMoves[moveNumber];
                _board.MakeMove(move);
                RedrawBoard();
            }
        }

        public void Wait(int milliseconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            // Console.WriteLine("start wait timer");
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                // Console.WriteLine("stop wait timer");
            };

            while (timer1.Enabled)
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

        private void RunNumberOfPositions(object sender, EventArgs e)
        {
            var sequence = new List<string>
            {
                "a4a5",
                "b7b6"
            };
            var number = GetNumberOfPositions(_board, 3, new Stack<string>(), null, 3);
            Console.WriteLine(number);
        }

        private int GetNumberOfPositions(Board board, int depth, Stack<string> moveHistory, List<string> desiredMoves = null, int? depthToPrintPositions = null)
        {
            if (depth == 0)
            {
                return 1;
            }
            var numberOfMoves = 0;
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
                    board.MakeMove(move);
                    var position = GetPgn(move.InitialPosition, move.FinalPosition);
                    moveHistory.Push(position);
                    var moveCount = GetNumberOfPositions(board, depth - 1, moveHistory, desiredMoves, depthToPrintPositions);
                    numberOfMoves += moveCount;
                    moveHistory.Pop();
                    if (desiredMoves != null && moveHistory.SequenceEqual(desiredMoves) || depthToPrintPositions == depth)
                    {
                        Console.WriteLine(position + ": " + moveCount);
                    }
                    board.UnmakeMove(move);
                }
            }
            return numberOfMoves;
        }

        private string GetPgn(int startPosition, int endPosition)
        {
            return GetPgn(startPosition) + GetPgn(endPosition);
        }

        private string GetPgn(int position)
        {
            var first = (char)((position % 8) + 97);
            var second = (8 - (position / 8)).ToString();
            return first + second;
        }

        private void OnPanelClick(object sender, EventArgs ev)
        {
            Panel panel = sender as Panel;
            RedrawBoard();

            int selectedPosition = GetPanelPosition(panel);

            //check if square is highlighted and if is, moves selected piece
            if (_potentialMoves.Select(e => e.FinalPosition).Contains(selectedPosition))
            {
                if (_selectedPiecePosition == null)
                {
                    throw new Exception("Piece is not selected");
                }
                CheckForEndOfGame(selectedPosition);
                var move = _potentialMoves.Where(e => e.FinalPosition == selectedPosition).First();
                _board.MakeMove(move);
                _potentialMoves.Clear();
                _moves.Push(move);

                RedrawBoard();
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
                var potentialMoveCoordinates = ChessService.GetPointFromPosition(potentialMove.FinalPosition);
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
