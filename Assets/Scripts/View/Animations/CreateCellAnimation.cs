using System.Collections;
using MVC.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MVC.View
{
    public class CreateCellAnimation : IViewAnimation
    {
        private Vector2Int _position;
        private CellItem   _item;

        public CreateCellAnimation(Vector2Int position, CellItem item)
        {
            _position = position;
            _item = item;
        }

        public Coroutine PlayAnimation(BoardView board)
        {
            return board.StartCoroutine(AnimationCoroutine(board));
        }

        private IEnumerator AnimationCoroutine(BoardView board)
        {
            AsyncOperationHandle<GameObject> handler =
                Addressables.InstantiateAsync($"Cell_{_item.CellColor}",
                    new Vector3(_position.x, _position.y, 0f),
                    Quaternion.identity,
                    board.transform);
            while (!handler.IsDone)
            {
                yield return null;
            }

            CellView view = handler.Result.GetComponent<CellView>();
            board.AddCellView(view);
            yield return view.Appear(_position);
        }
    }
}