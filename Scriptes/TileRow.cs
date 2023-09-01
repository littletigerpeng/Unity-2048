using UnityEngine;

public class TileRow : MonoBehaviour
{
    //ÿ�� Row ���٣������ϵ����� TileCell
    public TileCell[] cells { get;private set; }

    private void Awake()
    {
        cells = GetComponentsInChildren<TileCell>();
    }
}
