using System;
using System.Drawing;

namespace TetrisGame
{
    public class Tetromino
    {
        private readonly int[,] shape;
        public Color Color { get; private set; }
        public Point Position { get; set; }
        public int Size => shape.GetLength(0);

        private static readonly int[][,] Shapes = {
            // I
            new int[,] {
                {0, 0, 0, 0},
                {1, 1, 1, 1},
                {0, 0, 0, 0},
                {0, 0, 0, 0}
            },
            // O
            new int[,] {
                {1, 1},
                {1, 1}
            },
            // T
            new int[,] {
                {0, 1, 0},
                {1, 1, 1},
                {0, 0, 0}
            },
            // L
            new int[,] {
                {0, 0, 1},
                {1, 1, 1},
                {0, 0, 0}
            },
            // J
            new int[,] {
                {1, 0, 0},
                {1, 1, 1},
                {0, 0, 0}
            },
            // S
            new int[,] {
                {0, 1, 1},
                {1, 1, 0},
                {0, 0, 0}
            },
            // Z
            new int[,] {
                {1, 1, 0},
                {0, 1, 1},
                {0, 0, 0}
            }
        };

        private static readonly Color[] Colors = {
            Color.Cyan,    // I
            Color.Yellow,  // O
            Color.Purple,  // T
            Color.Orange,  // L
            Color.Blue,    // J
            Color.Green,   // S
            Color.Red      // Z
        };

        public Tetromino(int index)
        {
            shape = (int[,])Shapes[index].Clone();
            Color = Colors[index];
            Position = new Point(3, 0);
        }

        public static Tetromino CreateRandom()
        {
            var random = new Random();
            return new Tetromino(random.Next(Shapes.Length));
        }

        public bool[,] GetCurrentShape()
        {
            var result = new bool[Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    result[i, j] = shape[i, j] == 1;
            return result;
        }

        public void Rotate()
        {
            int[,] temp = new int[Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    temp[j, Size - 1 - i] = shape[i, j];

            Array.Copy(temp, shape, temp.Length);
        }

        public int[,] GetShape() => shape;
    }
}
