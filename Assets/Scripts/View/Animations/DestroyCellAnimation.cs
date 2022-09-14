using System.Collections;
using UnityEngine;

namespace MVC.View
{
    public class DestroyCellAnimation : IViewAnimation
    {
        private CellView _cell;

        public DestroyCellAnimation(CellView cell)
        {
            _cell = cell;
        }

        public Coroutine PlayAnimation(BoardView board)
        {
            return board.StartCoroutine(AnimationCoroutine(board));
        }

        private IEnumerator AnimationCoroutine(BoardView board)
        {
            board.RemoveCellView(_cell);
            yield return _cell.DestroyCell();
        }
    }
}