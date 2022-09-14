using System.Collections;
using UnityEngine;

namespace MVC.View
{
    public class MoveCellAnimation : IViewAnimation
    {
        private Vector2Int _from;
        private Vector2Int _to;

        public MoveCellAnimation(Vector2Int from, Vector2Int to)
        {
            _from = from;
            _to = to;
        }

        public Coroutine PlayAnimation(BoardView board)
        {
            return board.StartCoroutine(AnimationCoroutine(board));
        }

        private IEnumerator AnimationCoroutine(BoardView board)
        {
            var cell = board.GetCellViewAt(_from);
            if (cell == null)
                yield break;
            yield return cell.MoveTo(_to);
        }
    }
}