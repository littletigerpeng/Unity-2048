using UnityEngine;

public class TileGrid : MonoBehaviour
{
    //记录所有的 TileRow、TileCell
    public TileRow[] rows { get; private set; }  //遍历某一行时，很方便
    public TileCell[] cells { get; private set; }  //遍历所有项时，很方便

    //拥有的总 cells 的数目（为了避免硬编码），默认为 16
    public int size => cells.Length;

    //cells 横、纵的数量
    public int width => size/height;
    public int height => rows.Length;

    private void Awake()
    {
        rows=GetComponentsInChildren<TileRow>();    //会找到 4 个 TileRow
        cells=GetComponentsInChildren<TileCell>();  //这里会找到 16 个 TileCell
    }

    private void Start()
    {
        for(int y=0; y<rows.Length; y++)
        {
            for(int x = 0; x < rows[y].cells.Length; x++)
            {
                //为 TileCell 中的 coordinates 属性设置值
                //【注1】这里的 cells 是在 TileRow 里定义，一共 4 个
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
            /*这里总的 coordinates 矩阵，实际上是这样的：
             * (0,0) (1,0) (2,0) (3,0)
             * (0,1) (1,1) (2,1) (3,1)
             * (0,2) (1,2) (2,2) (3,2)
             * (0,3) (1,3) (2,3) (3,3)
             */
        }
    }

    //根据 x，y 的值获得 tileCell
    public TileCell GetCell(int x,int y)
    {
        if ((x >= 0 && x < width) && (y >= 0 && y < height))
            return rows[y].cells[x];
        else
            return null;
    }

    //重载：根据一个 Vector2Int 获得 TileCell
    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x,coordinates.y);
    }

    /// <summary>
    /// 获得与 TileCell 在 diection 方向上相邻的 TileCell
    /// </summary>
    /// <param name="cell">TileCell</param>
    /// <param name="direction">Vector2Int</param>
    /// <returns>TileCell</returns>
    public TileCell GetAdjancentCell(TileCell cell,Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        //因为左上角是 (0,0)，右下角是（3,3）
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }

    /// <summary>
    /// 获得一个任意的空 TileCell，通过遍历 cells 数组实现
    /// </summary>
    /// <returns>TileCell</returns>
    public TileCell GetRandomEmptyCell()
    {
        int index =Random.Range(0,cells.Length);
        int startIndex = index;

        while (cells[index].occupied)
        {
            index++;

            if (index >= cells.Length) //保证最差情况下，会访问全部的 cells
                index = 0;

            if (index == startIndex)  //保证最差情况下，只循环一遍
                return null;
        }

        //最好是在外面返回，而且 while 循环里最好只处理 int 类型的数据（index）
        return cells[index];
    }
}
