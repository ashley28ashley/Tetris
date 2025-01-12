using System;
using System.Drawing;

namespace TetrisGame
{
    public class Grid
    {
        private readonly Color[,] cells;
        public const int Rows = 20;
        public const int Columns = 10;

        public event EventHandler<int> LinesCleared = delegate { };

        public Grid()
        {
            cells = new Color[Rows, Columns];
        }

        public Color GetCell(int row, int col) => cells[row, col];

        public bool IsValidMove(Tetromino tetromino)
        {
            var shape = tetromino.GetCurrentShape();
            for (int i = 0; i < tetromino.Size; i++)
            {
                for (int j = 0; j < tetromino.Size; j++)
                {
                    if (!shape[i, j]) continue;

                    int newRow = tetromino.Position.Y + i;
                    int newCol = tetromino.Position.X + j;

                    if (newRow < 0 || newRow >= Rows || newCol < 0 || newCol >= Columns ||
                        (newRow >= 0 && cells[newRow, newCol] != Color.Empty))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void PlaceTetromino(Tetromino tetromino)
        {
            var shape = tetromino.GetCurrentShape();
            for (int i = 0; i < tetromino.Size; i++)
            {
                for (int j = 0; j < tetromino.Size; j++)
                {
                    if (shape[i, j])
                    {
                        int row = tetromino.Position.Y + i;
                        int col = tetromino.Position.X + j;
                        if (row >= 0)
                        {
                            cells[row, col] = tetromino.Color;
                        }
                    }
                }
            }
            CheckLines();
        }

        private void CheckLines()
        {
            int linesCleared = 0;
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (IsLineFull(row))
                {
                    ClearLine(row);
                    ShiftLinesDown(row);
                    linesCleared++;
                    row++;
                }
            }

            if (linesCleared > 0)
            {
                LinesCleared?.Invoke(this, linesCleared);
            }
        }

        private bool IsLineFull(int row)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (cells[row, col] == Color.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        private void ClearLine(int row)
        {
            for (int col = 0; col < Columns; col++)
            {
                cells[row, col] = Color.Empty;
            }
        }

        private void ShiftLinesDown(int startRow)
        {
            for (int row = startRow - 1; row >= 0; row--)
            {
                for (int col = 0; col < Columns; col++)
                {
                    cells[row + 1, col] = cells[row, col];
                    cells[row, col] = Color.Empty;
                }
            }
        }

        public bool IsGameOver()
        {
            for (int col = 0; col < Columns; col++)
            {
                if (cells[0, col] != Color.Empty)
                {
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    cells[row, col] = Color.Empty;
                }
            }
        }
    }
}
