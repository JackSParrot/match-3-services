using UnityEngine;

namespace MVC.Model
{
    public class CellModel
    {
        public Vector2Int Position;
        public CellItem   Item;
        public bool IsEmpty() => Item == null;
    }
}