using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace MVC.View
{
    public class CellView : MonoBehaviour
    {
        public Vector2Int Position;

        public virtual Coroutine Appear(Vector2Int newPosition)
        {
            Position = newPosition;
            return StartCoroutine(AppearCoroutine(newPosition));
        }

        private IEnumerator AppearCoroutine(Vector2Int newPosition)
        {
            yield return new WaitForSeconds(0.01f);
        }

        public virtual Coroutine MoveTo(Vector2Int newPosition)
        {
            return StartCoroutine(MoveToCoroutine(newPosition));
        }

        private IEnumerator MoveToCoroutine(Vector2Int newPosition)
        {
            Position = newPosition;
            transform.DOMove(new Vector3(newPosition.x, newPosition.y), 0.05f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.05f);
        }

        public virtual Coroutine DestroyCell()
        {
            return StartCoroutine(DestroyCellCoroutine());
        }

        private IEnumerator DestroyCellCoroutine()
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0f, 0.1f).SetEase(Ease.InOutBounce);
            yield return new WaitForSeconds(0.1f);
            transform.SetParent(null);
            Destroy(gameObject);
        }
    }
}