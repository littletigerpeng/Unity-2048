using UnityEngine;

public class TileRow : MonoBehaviour
{
    //每个 Row 跟踪，该行上的所有 TileCell
    public TileCell[] cells { get;private set; }

    private void Awake()
    {
        cells = GetComponentsInChildren<TileCell>();
    }
}
