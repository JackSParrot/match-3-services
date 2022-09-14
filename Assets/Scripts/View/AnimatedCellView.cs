using System.Collections;
using UnityEngine;

namespace MVC.View
{
    public class AnimatedCellView : CellView
    {
        [SerializeField]
        private Animator _animation;

        private bool _animating = false;

        public override Coroutine DestroyCell()
        {
            return StartCoroutine(DestroyCellCoroutine());
        }

        public void OnAnimationFinished()
        {
            _animating = false;
        }

        private IEnumerator DestroyCellCoroutine()
        {
            _animating = true;
            _animation.SetTrigger("kill");
            yield return new WaitUntil(() => _animating == false);
            transform.SetParent(null);
            Destroy(gameObject);
        }
    }
}