using UnityEngine;

public class TileCell : MonoBehaviour
{
    //记录 TileCell（固定不变） 的位置 (0,0) -> (3,3)
    public Vector2Int coordinates { get; set; }

    //记录当前 TileCell 上的 Tile
    public Tile tile { get; set; }

    //TileCell 上是否有 Tile
    public bool empty => !tile;
    public bool occupied => tile != null;
}
