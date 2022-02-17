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

        private void OnPanelClick(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            RedrawBoard();

            int selectedPosition = GetPanelPosition(panel);

            //check if square is highlighted and if is, moves selected piece
            if (_potentialMoves.Contains(selectedPosition))
            {
                if (_selectedPiecePosition == null)
                {
                    throw new Exception("Piece is not selected");
                }
                CheckForEndOfGame(selectedPosition);
                _board.MovePiece(_selectedPiecePosition.Value, selectedPosition);
                _potentialMoves.Clear();
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
            _potentialMoves = chessPiece.GetPossibleMoves(_board, selectedPosition);

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
