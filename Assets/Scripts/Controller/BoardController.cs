using System;
using System.Collections.Generic;
using Game.Services;
using MVC.Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MVC.Controller
{
    public class BoardController
    {
        public BoardModel Model;

        public event Action<CellModel> OnCellCreated = delegate(CellModel model) { };
        public event Action<CellModel> OnCellDestroyed = delegate(CellModel model) { };
        public event Action<CellModel, CellModel> OnCellMoved = delegate(CellModel from, CellModel to) { };
        public event Action OnLoseGame = delegate() { };
        public event Action OnWinGame = delegate() { };

        private GameProgressionService _gameProgression = null;
        private GameConfigService      _gameConfig      = null;

        public BoardController(int moves, int targetScore, int width = 9, int height = 9,
            CellItem[,] initialValues = null)
        {
            Model = new BoardModel(width, height, initialValues);
            Model.MovesRemaining = moves;
            Model.TargetScore = targetScore;
            _gameProgression = ServiceLocator.GetService<GameProgressionService>();
            _gameConfig = ServiceLocator.GetService<GameConfigService>();
        }

        public void UseContinue()
        {
            Model.MovesRemaining = 2;
        }

        public void UseBooster()
        {
            Model.MovesRemaining += 2;
        }

        public void ProcessInput(Vector2Int touchedPos)
        {
            if (Model.MovesRemaining < 1)
                return;

            if (touchedPos.x >= 0 && touchedPos.y >= 0 && touchedPos.y < Model.Height && touchedPos.x < Model.Width)
            {
                ProcessMatches(touchedPos);
                Model.MovesRemaining--;
                if (Model.Score >= Model.TargetScore)
                {
                    _gameProgression.CurrentLevel++;
                    _gameProgression.UpdateGold(_gameConfig.GoldPerWin);
                    OnWinGame();
                }
                else if (Model.MovesRemaining < 1)
                {
                    _gameProgression.UpdateGold(_gameConfig.GoldPerWin / 2);
                    OnLoseGame();
                }
            }

            ProcessCollapse();
        }

        private void ProcessMatches(Vector2Int touchedPos)
        {
            CellModel touchedCell = Model[touchedPos];
            if (touchedCell.Item == null)
                return;

            if (touchedCell.Item.CellColor >= 0 && touchedCell.Item.CellColor <= 5)
            {
                ProcessColorMatch(touchedCell);
                return;
            }

            if (TryFireRocketBomb(touchedCell))
            {
                Model.Score += 5;
                return;
            }

            if (touchedCell.Item.CellColor == 8)
            {
                FireBomb(touchedCell);
                Model.Score += 3;
                return;
            }

            if (touchedCell.Item.CellColor == 6 || touchedCell.Item.CellColor == 7)
            {
                FireRocket(touchedCell);
                Model.Score += 2;
                return;
            }
        }

        private void ProcessCollapse()
        {
            for (int y = 0; y < Model.Height; ++y)
            {
                for (int x = 0; x < Model.Width; ++x)
                {
                    if (!Model[x, y].IsEmpty())
                        continue;

                    int nextY = y;
                    while (nextY < Model.Height)
                    {
                        nextY++;
                        if (nextY == Model.Height)
                        {
                            Model[x, nextY - 1].Item = new CellItem()
                            {
                                CellColor = Random.Range(0, 4)
                            };
                            OnCellCreated(Model[x, nextY - 1]);
                            if (y < nextY - 1)
                            {
                                Model[x, y].Item = Model[x, nextY - 1].Item;
                                Model[x, nextY - 1].Item = null;
                                OnCellMoved(Model[x, nextY - 1], Model[x, y]);
                            }

                            break;
                        }

                        if (!Model[x, nextY].IsEmpty())
                        {
                            Model[x, y].Item = Model[x, nextY].Item;
                            Model[x, nextY].Item = null;
                            OnCellMoved(Model[x, nextY], Model[x, y]);
                            break;
                        }
                    }
                }
            }
        }

        private List<CellModel> GetMatchedCells(CellModel touchedCell, List<int> extraAllowedMatches)
        {
            List<CellModel> closed = new List<CellModel>();
            if (touchedCell.IsEmpty())
                return closed;

            List<CellModel> open = new List<CellModel>();
            open.Add(touchedCell);
            while (open.Count > 0)
            {
                CellModel cellModel = open[0];
                open.RemoveAt(0);
                closed.Add(cellModel);

                if (cellModel.Position.x > 0)
                {
                    CellModel neighbour = Model[cellModel.Position.x - 1, cellModel.Position.y];
                    ProcessNeighbour(touchedCell, neighbour, extraAllowedMatches, open, closed);
                }

                if (cellModel.Position.x < Model.Width - 1)
                {
                    CellModel neighbour = Model[cellModel.Position.x + 1, cellModel.Position.y];
                    ProcessNeighbour(touchedCell, neighbour, extraAllowedMatches, open, closed);
                }

                if (cellModel.Position.y > 0)
                {
                    CellModel neighbour = Model[cellModel.Position.x, cellModel.Position.y - 1];
                    ProcessNeighbour(touchedCell, neighbour, extraAllowedMatches, open, closed);
                }

                if (cellModel.Position.y < Model.Height - 1)
                {
                    CellModel neighbour = Model[cellModel.Position.x, cellModel.Position.y + 1];
                    ProcessNeighbour(touchedCell, neighbour, extraAllowedMatches, open, closed);
                }
            }

            return closed;
        }

        private static void ProcessNeighbour(CellModel touchedCell, CellModel neighbour, List<int> extraAllowedMatches,
            List<CellModel> open, List<CellModel> closed)
        {
            if (!neighbour.IsEmpty() &&
                (touchedCell.Item.CellColor == neighbour.Item.CellColor ||
                 extraAllowedMatches.Contains(neighbour.Item.CellColor)) &&
                !open.Contains(neighbour) &&
                !closed.Contains(neighbour))
            {
                open.Add(neighbour);
            }
        }

        private void ProcessColorMatch(CellModel touchedCell)
        {
            List<CellModel> matchedCells = GetMatchedCells(touchedCell, new List<int>());
            if (matchedCells.Count > 2) //3 or more
            {
                foreach (CellModel cell in matchedCells)
                {
                    OnCellDestroyed(cell);
                    Model[cell.Position].Item = null;
                    Model.Score++;
                }

                if (matchedCells.Count > 6) //7 or more
                {
                    Model[touchedCell.Position].Item = new CellItem { CellColor = 8 };
                    OnCellCreated(Model[touchedCell.Position]);
                }
                else if (matchedCells.Count > 4) //5 or 6 rockets
                {
                    Model[touchedCell.Position].Item = new CellItem
                    {
                        CellColor = Random.Range(0, 100) < 50 ? 6 : 7
                    };
                    OnCellCreated(Model[touchedCell.Position]);
                }
            }
        }

        private void FireRocket(CellModel touchedCell)
        {
            List<CellModel> matchedCells = GetMatchedCells(touchedCell, new List<int> { 6, 7 });

            if (touchedCell.Item.CellColor == 6)
            {
                FireHorizontalRocket(touchedCell);
                if (matchedCells.Count > 1)
                {
                    FireVerticalRocket(touchedCell);
                }
            }
            else if (touchedCell.Item.CellColor == 7)
            {
                FireVerticalRocket(touchedCell);
                if (matchedCells.Count > 1)
                {
                    FireHorizontalRocket(touchedCell);
                }
            }
        }

        private void FireHorizontalRocket(CellModel touchedCell)
        {
            for (int x = 0; x < Model.Width; ++x)
            {
                DestroyCellAt(x, touchedCell.Position.y);
            }
        }

        private void FireVerticalRocket(CellModel touchedCell)
        {
            for (int y = 0; y < Model.Height; ++y)
            {
                DestroyCellAt(touchedCell.Position.x, y);
            }
        }

        private void FireBomb(CellModel touchedCell)
        {
            List<CellModel> matchedCells = GetMatchedCells(touchedCell, new List<int>());
            int bombRange = matchedCells.Count > 1 ? 2 : 1;

            for (int y = -bombRange; y <= bombRange; ++y)
            {
                for (int x = -bombRange; x <= bombRange; ++x)
                {
                    DestroyCellAt(touchedCell.Position.x + x, touchedCell.Position.y + y);
                }
            }
        }

        private bool TryFireRocketBomb(CellModel touchedCell)
        {
            List<CellModel> matchedCells = GetMatchedCells(touchedCell, new List<int> { 6, 7, 8 });
            if (matchedCells.Count < 2)
                return false;

            bool hasRocket = false;
            bool hasBomb = false;
            foreach (CellModel cell in matchedCells)
            {
                hasRocket |= cell.Item.CellColor == 6 || cell.Item.CellColor == 7;
                hasBomb |= cell.Item.CellColor == 8;
            }

            if (!hasRocket || !hasBomb)
            {
                return false;
            }

            FireHorizontalRocketBomb(touchedCell);
            FireVerticalRocketBomb(touchedCell);
            return true;
        }

        private void FireHorizontalRocketBomb(CellModel touchedCell)
        {
            for (int x = 0; x < Model.Width; ++x)
            {
                DestroyCellAt(x, touchedCell.Position.y - 1);
                DestroyCellAt(x, touchedCell.Position.y);
                DestroyCellAt(x, touchedCell.Position.y + 1);
            }
        }

        private void FireVerticalRocketBomb(CellModel touchedCell)
        {
            for (int y = 0; y < Model.Height; ++y)
            {
                DestroyCellAt(touchedCell.Position.x - 1, y);
                DestroyCellAt(touchedCell.Position.x, y);
                DestroyCellAt(touchedCell.Position.x + 1, y);
            }
        }

        private void DestroyCellAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Model.Width || y >= Model.Height)
                return;

            CellModel cell = Model[x, y];
            if (!cell.IsEmpty())
            {
                OnCellDestroyed(cell);
                Model[x, y].Item = null;
            }
        }
    }
}