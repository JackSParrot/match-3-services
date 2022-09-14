using UnityEngine;

namespace MVC.Model
{
    public class BoardModel
    {
        private CellModel[,] _cells;
        public int Width { get; }
        public int Height { get; }

        public int MovesRemaining;

        public int Score;
        public int TargetScore;

        public BoardModel(int width, int height, CellItem[,] initialValues = null)
        {
            Width = width;
            Height = height;
            _cells = new CellModel[width, height];
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    _cells[x, y] = new CellModel
                    {
                        Position = new Vector2Int(x, y),
                        Item = initialValues?[x, y]
                    };
                }
            }
        }

        public BoardModel(BoardModel other)
        {
            _cells = new CellModel[other.Width, other.Height];
            foreach (CellModel cell in other._cells)
            {
                _cells[cell.Position.x, cell.Position.y] = new CellModel
                {
                    Position = cell.Position,
                    Item = cell.Item
                };
            }

            Width = other.Width;
            Height = other.Height;
        }

        public CellModel this[int x, int y]
        {
            get => _cells[x, y];
        }

        public CellModel this[Vector2Int pos]
        {
            get => _cells[pos.x, pos.y];
        }

        public void Clear()
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    _cells[x, y] = new CellModel
                    {
                        Position = new Vector2Int(x, y),
                        Item = null
                    };
                }
            }
        }

        public CellModel GetCell(int x, int y) => this[x, y];

        public CellModel GetCell(Vector2Int pos) => _cells[pos.x, pos.y];
    }
}