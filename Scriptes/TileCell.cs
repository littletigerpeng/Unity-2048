using UnityEngine;

public class TileCell : MonoBehaviour
{
    //��¼ TileCell���̶����䣩 ��λ�� (0,0) -> (3,3)
    public Vector2Int coordinates { get; set; }

    //��¼��ǰ TileCell �ϵ� Tile
    public Tile tile { get; set; }

    //TileCell ���Ƿ��� Tile
    public bool empty => !tile;
    public bool occupied => tile != null;
}
