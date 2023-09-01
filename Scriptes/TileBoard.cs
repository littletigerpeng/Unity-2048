using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;

/// <summary>
/// ��Ϸ�߼�
/// </summary>
public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;

    public Tile tilePrefab;         //Tile �ѱ������� Prefab
    public TileState[] tileStates;  //�������е� TileState�������½���ͬ Tile ʱʹ��

    private TileGrid grid;
    //�����������е� Tile
    private List<Tile> tiles;

    private bool waiting;  

    private void Awake()
    {
        grid=GetComponentInChildren<TileGrid>();
        //��Ϊ List �ڳ���ԭ����С��ʱ�����ܼ�����ӣ����Ը� List һ�� 16 ����Ӳ����
        tiles = new List<Tile>(16);   
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
            cell.tile = null;

        foreach (var tile in tiles)
            Destroy(tile.gameObject);

        tiles.Clear();
    }

    //�����λ�ã�����һ�� Tile
    public void CreateTile()
    {
        //�ȵõ� Tile(Prefab����һ������
        Tile title = Instantiate(tilePrefab,grid.transform);

        //���� tile �� state������λ�ã�TileCell�������� number
        title.SetState(tileStates[0], 2);
        title.Spawn(grid.GetRandomEmptyCell());
        
        //�������� Tile����ӵ� tile �б�
        tiles.Add(title);
    }

    private void Update()
    {
        if (!waiting)  //���ȴ���ʱ������ƶ�
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);   //�����ƶ�����һ�Ų�����û������
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0 , 1, grid.height-2, -1);  //�����ƶ������һ�Ų���
            }else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left,1,1,0,1);    //�����ƶ�����һ�в���
            }else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right,grid.width - 2, -1, 0 , 1);  //�����ƶ������һ�в���
            }
        }
    }

    //�Ӹ��������꿪ʼ�����μ�� TileCell ���Ƿ�ͣ���� Till������У�������ƶ�
    private void MoveTiles(Vector2Int direction,int startX,int incrementX,int startY,int incrementY)
    {
        bool change = false;
        for(int x=startX; x>=0 && x < grid.width; x += incrementX)
        {
            for(int y=startY; y>=0 && y < grid.height; y += incrementY)
            {
                //��õ�ǰ TillCell
                TileCell cell =grid.GetCell(x,y);

                if (cell.occupied)   //˵�� TileCell ���� Tile ͣ���������ƶ�
                    change |= MoveTile(cell.tile, direction);  // |= ��֤��һ��Ϊ�棬��ԶΪ��
            }
        }

        if (change) StartCoroutine(WaitingForChanges());
    }

    //�ƶ������� TileCell
    private bool MoveTile(Tile tile,Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjancentCell(tile.cell, direction);

        while(adjacent !=null)  //ֻҪ�����ڵ� TileCell
        {
            if(adjacent.occupied)
            {
                //TODO: �ϲ�
                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            //���浱ǰ TileCell���ٻ�ȡ��һ�����ڵ� TileCell
            newCell = adjacent; 
            adjacent=grid.GetAdjancentCell(adjacent,direction);
        }

        if(newCell != null)
        {
            tile.MoveTo(newCell);  //ʵ���ƶ�
            //StartCoroutine(WaitingForChanges());  //���Է����⣬�������еĴ���̫���ˣ���������
            return true;   //�÷���ֵ����
        }

        return false;
    }

    private void Merge(Tile a,Tile b)
    {
        tiles.Remove(a);   //��Ҫɾ���� tile �Ƴ� tiles �б�
        a.Merge(b.cell);

        int stateIndex = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length-1);
        b.SetState(tileStates[stateIndex], b.number * 2);

        gameManager.IncrementScore(b.number);
    }

    private int IndexOf(TileState state)
    {
        for(int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
                return i;
        }

        return -1;   //û�ҵ������ظ�ֵ
    }
    private bool CanMerge(Tile a,Tile b)
    {
        return a.number == b.number && !b.locked;
    }

    /// <summary>
    /// �����������룬��ֹ����û��Ͷ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitingForChanges()  
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);  //ʵ���ϣ��ȴ���������ƶ����������ʱ��

        waiting = false;

        foreach(var item in tiles)
            item.locked = false;

        if (tiles.Count <= grid.size)  //�ƶ�һ�Σ����һ�� tile
            CreateTile();

        if (CheckGameOver()) gameManager.GameOver();  //��Ϸ����
    }

    private bool CheckGameOver()
    {
        if (tiles.Count != grid.size) return false;
        
        for(int i = 0; i < tiles.Count; i++)
        {
            TileCell up = grid.GetAdjancentCell(tiles[i].cell, Vector2Int.up);
            TileCell down = grid.GetAdjancentCell(tiles[i].cell, Vector2Int.down);
            TileCell left = grid.GetAdjancentCell(tiles[i].cell, Vector2Int.left);
            TileCell right = grid.GetAdjancentCell(tiles[i].cell, Vector2Int.right);

            if (up != null && CanMerge(tiles[i], up.tile)) return false;
            if (down != null && CanMerge(tiles[i], down.tile)) return false;
            if (left != null && CanMerge(tiles[i], left.tile)) return false;
            if (right != null && CanMerge(tiles[i], right.tile)) return false;

            /* ������д��GetAdjancenetCell() ���ܻ��Ҳ������ڵ� cell�������ڱ߽��ϣ���ʱ���� .tile������������õĴ��� 
             * Tile up = grid.GetAdjancentCell(tiles[i].cell, Vector2Int.up).tile;
            Tile down = grid.GetAdjancentCell(tiles[i].cell, Vector2Int.down).tile;
            Tile left = grid.GetAdjancentCell(tiles[i].cell, Vector2Int.left).tile;
            Tile right = grid.GetAdjancentCell(tiles[i].cell, Vector2Int.right).tile;

            if (up != null && CanMerge(tiles[i], up)) return false;
            if (down != null && CanMerge(tiles[i], down)) return false;
            if (left != null && CanMerge(tiles[i], left)) return false;
            if (right != null && CanMerge(tiles[i], right)) return false;
            
             */
        }

        return true;
    }
}
