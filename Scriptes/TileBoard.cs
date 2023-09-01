using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;

/// <summary>
/// 游戏逻辑
/// </summary>
public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;

    public Tile tilePrefab;         //Tile 已被制作成 Prefab
    public TileState[] tileStates;  //管理所有的 TileState，方便新建不同 Tile 时使用

    private TileGrid grid;
    //管理所有已有的 Tile
    private List<Tile> tiles;

    private bool waiting;  

    private void Awake()
    {
        grid=GetComponentInChildren<TileGrid>();
        //因为 List 在超出原定大小的时候，仍能继续添加，所以给 List 一个 16 不算硬编码
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

    //在随机位置，创建一个 Tile
    public void CreateTile()
    {
        //先得到 Tile(Prefab）的一个副本
        Tile title = Instantiate(tilePrefab,grid.transform);

        //设置 tile 的 state，放置位置（TileCell），设置 number
        title.SetState(tileStates[0], 2);
        title.Spawn(grid.GetRandomEmptyCell());
        
        //将创建的 Tile，添加到 tile 列表
        tiles.Add(title);
    }

    private void Update()
    {
        if (!waiting)  //不等待的时候才能移动
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);   //向上移动：第一排不动（没法动）
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0 , 1, grid.height-2, -1);  //向下移动：最后一排不动
            }else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left,1,1,0,1);    //向左移动：第一列不动
            }else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right,grid.width - 2, -1, 0 , 1);  //向右移动：最后一列不动
            }
        }
    }

    //从给定的坐标开始，依次检查 TileCell 上是否停靠了 Till，如果有，则可以移动
    private void MoveTiles(Vector2Int direction,int startX,int incrementX,int startY,int incrementY)
    {
        bool change = false;
        for(int x=startX; x>=0 && x < grid.width; x += incrementX)
        {
            for(int y=startY; y>=0 && y < grid.height; y += incrementY)
            {
                //获得当前 TillCell
                TileCell cell =grid.GetCell(x,y);

                if (cell.occupied)   //说明 TileCell 上有 Tile 停靠，可以移动
                    change |= MoveTile(cell.tile, direction);  // |= 保证，一次为真，永远为真
            }
        }

        if (change) StartCoroutine(WaitingForChanges());
    }

    //移动给定的 TileCell
    private bool MoveTile(Tile tile,Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjancentCell(tile.cell, direction);

        while(adjacent !=null)  //只要有相邻的 TileCell
        {
            if(adjacent.occupied)
            {
                //TODO: 合并
                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            //保存当前 TileCell，再获取下一个相邻的 TileCell
            newCell = adjacent; 
            adjacent=grid.GetAdjancentCell(adjacent,direction);
        }

        if(newCell != null)
        {
            tile.MoveTo(newCell);  //实际移动
            //StartCoroutine(WaitingForChanges());  //可以放在这，但是运行的次数太多了，消耗性能
            return true;   //用返回值代替
        }

        return false;
    }

    private void Merge(Tile a,Tile b)
    {
        tiles.Remove(a);   //将要删除的 tile 移出 tiles 列表
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

        return -1;   //没找到，返回负值
    }
    private bool CanMerge(Tile a,Tile b)
    {
        return a.number == b.number && !b.locked;
    }

    /// <summary>
    /// 过滤垃圾输入，防止动画没完就动
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitingForChanges()  
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);  //实际上，等待的是完成移动动画所需的时间

        waiting = false;

        foreach(var item in tiles)
            item.locked = false;

        if (tiles.Count <= grid.size)  //移动一次，添加一个 tile
            CreateTile();

        if (CheckGameOver()) gameManager.GameOver();  //游戏结束
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

            /* 这样子写，GetAdjancenetCell() 可能会找不到相邻的 cell，比如在边界上，此时访问 .tile，会产生空引用的错误 
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
