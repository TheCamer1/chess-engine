using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UserInterface.Pieces;

namespace UserInterface
{
    public partial class ChessBoard : Form
    {
        private const int _panelSize = 100;
        private const int _gridSize = 8;

        private Board _board = new Board(Colour.White);
        private Colour _currentColour = Colour.White;
        private List<int> _potentialMoves = new List<int>();
        private int? _selectedPiecePosition;

        private Panel[,] _panels = new Panel[_gridSize, _gridSize];

        public ChessBoard()
        {
            InitializeComponent();
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
                if (_board.GetPiece(i) == null)
                {
                    continue;
                }
                var point = _board.GetPointFromPosition(i);
                _panels[point.X, point.Y].BackgroundImage = _board.GetPiece(i).Image;
            }
        }

        private void OnPanelClick(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;

            int selectedPosition = GetPanelPosition(panel);

            //check if square is highlighted and if is, moves selected piece
            if (_potentialMoves.Contains(selectedPosition))
            {
                MovePiece(selectedPosition);
                _potentialMoves.Clear();
                RedrawBoard();
                return;
            }

            //check if has unselected piece
            var chessPiece = _board.GetPiece(selectedPosition);
            if (chessPiece == null || chessPiece.Colour != _currentColour)
            {
                _selectedPiecePosition = null;
                _potentialMoves.Clear();
                return;
            }
            _selectedPiecePosition = selectedPosition;

            var panelCoordinates = _board.GetPointFromPosition(selectedPosition);
            _panels[panelCoordinates.X, panelCoordinates.Y].BackColor = Color.Blue;
            _potentialMoves = chessPiece.GetPossibleMoves(_board, selectedPosition);

            // colors in potential panels
            foreach (var potentialMove in _potentialMoves)
            {
                var potentialMoveCoordinates = _board.GetPointFromPosition(potentialMove);
                _panels[potentialMoveCoordinates.X, potentialMoveCoordinates.Y].BackColor = Color.LightBlue;
            }
        }

        private void MovePiece(int selectedPosition)
        {
            if (_selectedPiecePosition == null)
            {
                throw new Exception("Piece is not selected");
            }
            var selectedPieceCoordinates = _board.GetPointFromPosition(_selectedPiecePosition.Value);
            var selectedPositionCoordinates = _board.GetPointFromPosition(selectedPosition);
            _panels[selectedPieceCoordinates.X, selectedPieceCoordinates.Y].BackgroundImage = null;

            var selectedPiece = _board.GetPiece(_selectedPiecePosition.Value);

            _panels[selectedPositionCoordinates.X, selectedPositionCoordinates.Y].BackgroundImage = selectedPiece.Image;
            CheckForEndOfGame(selectedPosition);
            _board.Pieces[selectedPosition] = selectedPiece;
            _board.Pieces[_selectedPiecePosition.Value] = null;

            selectedPiece.MovedOn = _board.CurrentPlay;

            _board.CurrentPlay += 1;
            _currentColour = _currentColour == Colour.Black ? Colour.White : Colour.Black;
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
                        return _board.GetPositionFromPoint(row, column);
                    }
                }
            }
            throw new Exception("Panel was not found");
        }

    }
}
