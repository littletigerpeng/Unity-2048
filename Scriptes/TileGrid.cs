using UnityEngine;

public class TileGrid : MonoBehaviour
{
    //��¼���е� TileRow��TileCell
    public TileRow[] rows { get; private set; }  //����ĳһ��ʱ���ܷ���
    public TileCell[] cells { get; private set; }  //����������ʱ���ܷ���

    //ӵ�е��� cells ����Ŀ��Ϊ�˱���Ӳ���룩��Ĭ��Ϊ 16
    public int size => cells.Length;

    //cells �ᡢ�ݵ�����
    public int width => size/height;
    public int height => rows.Length;

    private void Awake()
    {
        rows=GetComponentsInChildren<TileRow>();    //���ҵ� 4 �� TileRow
        cells=GetComponentsInChildren<TileCell>();  //������ҵ� 16 �� TileCell
    }

    private void Start()
    {
        for(int y=0; y<rows.Length; y++)
        {
            for(int x = 0; x < rows[y].cells.Length; x++)
            {
                //Ϊ TileCell �е� coordinates ��������ֵ
                //��ע1������� cells ���� TileRow �ﶨ�壬һ�� 4 ��
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
            /*�����ܵ� coordinates ����ʵ�����������ģ�
             * (0,0) (1,0) (2,0) (3,0)
             * (0,1) (1,1) (2,1) (3,1)
             * (0,2) (1,2) (2,2) (3,2)
             * (0,3) (1,3) (2,3) (3,3)
             */
        }
    }

    //���� x��y ��ֵ��� tileCell
    public TileCell GetCell(int x,int y)
    {
        if ((x >= 0 && x < width) && (y >= 0 && y < height))
            return rows[y].cells[x];
        else
            return null;
    }

    //���أ�����һ�� Vector2Int ��� TileCell
    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x,coordinates.y);
    }

    /// <summary>
    /// ����� TileCell �� diection ���������ڵ� TileCell
    /// </summary>
    /// <param name="cell">TileCell</param>
    /// <param name="direction">Vector2Int</param>
    /// <returns>TileCell</returns>
    public TileCell GetAdjancentCell(TileCell cell,Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        //��Ϊ���Ͻ��� (0,0)�����½��ǣ�3,3��
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }

    /// <summary>
    /// ���һ������Ŀ� TileCell��ͨ������ cells ����ʵ��
    /// </summary>
    /// <returns>TileCell</returns>
    public TileCell GetRandomEmptyCell()
    {
        int index =Random.Range(0,cells.Length);
        int startIndex = index;

        while (cells[index].occupied)
        {
            index++;

            if (index >= cells.Length) //��֤�������£������ȫ���� cells
                index = 0;

            if (index == startIndex)  //��֤�������£�ֻѭ��һ��
                return null;
        }

        //����������淵�أ����� while ѭ�������ֻ���� int ���͵����ݣ�index��
        return cells[index];
    }
}
