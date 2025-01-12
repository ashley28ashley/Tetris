using System;
using System.Drawing;

namespace TetrisGame
{
    public class GameState
    {
        private Color[,] grid; // Utilisation de Coleur pour la grille
        public Tetromino CurrentPiece { get; private set; }
        public Tetromino NextPiece { get; private set; }
        public Point CurrentPosition { get; private set; }
        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }

        public event EventHandler<int> ScoreChanged;
        public event EventHandler GameOver;
        public event EventHandler LineCleared;

        public GameState()
        {
            grid = new Color[20, 10];
            // Initialisation de la grille avec des cellules vides
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    grid[y, x] = Color.Empty;
                }
            }

            NextPiece = Tetromino.CreateRandom();
            SpawnNewPiece();
        }

        public bool MoveLeft()
        {
            var newPosition = new Point(CurrentPosition.X - 1, CurrentPosition.Y);
            if (IsValidPosition(newPosition, CurrentPiece.GetShape()))
            {
                CurrentPosition = newPosition;
                return true;
            }
            return false;
        }

        public bool MoveRight()
        {
            var newPosition = new Point(CurrentPosition.X + 1, CurrentPosition.Y);
            if (IsValidPosition(newPosition, CurrentPiece.GetShape()))
            {
                CurrentPosition = newPosition;
                return true;
            }
            return false;
        }

        public bool MoveDown()
        {
            var newPosition = new Point(CurrentPosition.X, CurrentPosition.Y + 1);
            if (IsValidPosition(newPosition, CurrentPiece.GetShape()))
            {
                CurrentPosition = newPosition;
                return true;
            }
            return false;
        }

        public void Rotate()
        {
            CurrentPiece.Rotate();
            if (!IsValidPosition(CurrentPosition, CurrentPiece.GetShape()))
            {
                // Rotation vers l’arrière si elle n’est pas valide
                CurrentPiece.Rotate();
                CurrentPiece.Rotate();
                CurrentPiece.Rotate();
            }
        }

        public void Drop()
        {
            while (MoveDown()) { }
            LockPiece();
        }

        public void LockPiece()
        {
            PlacePiece();
            ClearLines();
            SpawnNewPiece();
        }

        private void PlacePiece()
        {
            foreach (var cell in GetPieceCells(CurrentPiece.GetShape(), CurrentPosition))
            {
                grid[cell.Y, cell.X] = CurrentPiece.Color; // Stocke la couleur de la pièce
            }
        }

        public void ClearLines()
        {
            bool linesCleared = false;
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                bool isFull = true;
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x] == Color.Empty)
                    {
                        isFull = false;
                        break;
                    }
                }

                if (isFull)
                {
                    RemoveLine(y);
                    Score += 100;
                    ScoreChanged?.Invoke(this, Score);
                    linesCleared = true;
                }
            }

            if (linesCleared)
            {
                LineCleared?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RemoveLine(int row)
        {
            for (int y = row; y > 0; y--)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    grid[y, x] = grid[y - 1, x];
                }
            }

            for (int x = 0; x < grid.GetLength(1); x++)
            {
                grid[0, x] = Color.Empty;
            }
        }

        private void SpawnNewPiece()
        {
            CurrentPiece = NextPiece;
            NextPiece = Tetromino.CreateRandom();
            CurrentPosition = new Point(3, 0);

            if (!IsValidPosition(CurrentPosition, CurrentPiece.GetShape()))
            {
                IsGameOver = true;
                GameOver?.Invoke(this, EventArgs.Empty);
            }
        }

        public void DrawNextPiece(Graphics g, int cellSize, Point location)
        {
            var shape = NextPiece.GetShape();
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    if (shape[y, x] == 1)
                    {
                        g.FillRectangle(
                            new SolidBrush(NextPiece.Color),
                            location.X + (x * cellSize),
                            location.Y + (y * cellSize),
                            cellSize,
                            cellSize
                        );
                    }
                }
            }
        }

        private bool IsValidPosition(Point position, int[,] pieceShape)
        {
            foreach (var cell in GetPieceCells(pieceShape, position))
            {
                if (cell.X < 0 || cell.X >= grid.GetLength(1) ||
                    cell.Y < 0 || cell.Y >= grid.GetLength(0) ||
                    grid[cell.Y, cell.X] != Color.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        private Point[] GetPieceCells(int[,] pieceShape, Point position)
        {
            var cells = new Point[pieceShape.GetLength(0) * pieceShape.GetLength(1)];
            int index = 0;

            for (int y = 0; y < pieceShape.GetLength(0); y++)
            {
                for (int x = 0; x < pieceShape.GetLength(1); x++)
                {
                    if (pieceShape[y, x] == 1)
                    {
                        cells[index++] = new Point(position.X + x, position.Y + y);
                    }
                }
            }

            Array.Resize(ref cells, index);
            return cells;
        }

        public void Draw(Graphics g, int cellSize)
        {
            // Dessiner les cellules verrouillées
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x] != Color.Empty)
                    {
                        using (Brush brush = new SolidBrush(grid[y, x]))
                        {
                            g.FillRectangle(brush, x * cellSize, y * cellSize, cellSize, cellSize);
                        }
                    }
                }
            }

            // Dessiner la pièce en mouvement
            foreach (var cell in GetPieceCells(CurrentPiece.GetShape(), CurrentPosition))
            {
                g.FillRectangle(new SolidBrush(CurrentPiece.Color), cell.X * cellSize, cell.Y * cellSize, cellSize, cellSize);
            }
        }
    }
}
