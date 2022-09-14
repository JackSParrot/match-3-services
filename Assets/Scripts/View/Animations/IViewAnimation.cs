using UnityEngine;

namespace MVC.View
{
    public interface IViewAnimation
    {
        Coroutine PlayAnimation(BoardView board);
    }
}