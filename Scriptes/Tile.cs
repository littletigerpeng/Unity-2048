using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    //state 用于描述 Tile 的当前状态：背景颜色、文本颜色
    public TileState state { get; private set; }
    // cell 用于描述 Tile 位于哪个 TileCell 之上
    public TileCell cell { get; private set; }
    //number 用于描述 Tile 显示数字
    public int number { get; private set; }

    //Tile 由一个 Image 和 一个 TextMeshProUGUI（子物体）组成
    private Image background;
    private TextMeshProUGUI text;

    public bool locked { get;  set; }  //保证每次只能合并一次

    private void Awake()
    {
        background= GetComponent<Image>();
        text=GetComponentInChildren<TextMeshProUGUI>();
    }

    //设置 Tile 的状态：背景颜色、文字颜色、数字值
    public void SetState(TileState state ,int number)
    {
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor;
        text.color=state.textColor;
        text.text = number.ToString();
    }

    //将 Tile 停靠在 TileCell 上
    public void Spawn(TileCell cell)
    {
        if(this.cell != null)  
        {
            //保证 cell 不被占用，如果被占用，就将 tile 设置为 null
            this.cell.tile = null;
        }

        this.cell = cell;  
        //将 cell 的 Tile 设置成 当前Tile
        this.cell.tile = this;

        //更改当前 Tile 的位置为 cell 的位置
        transform.position = cell.transform.position;
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            //保证 cell 不被占用，如果被占用，就将 tile 设置为 null
            this.cell.tile = null;
        }

        this.cell = null;
        cell.tile.locked = true;

    
        StartCoroutine(Animation(cell.transform.position, true));
    }

    //将 Tile 移动到 TileCell 上
    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            //保证 cell 不被占用，如果被占用，就将 tile 设置为 null
            this.cell.tile = null;
        }

        this.cell = cell;
        //将 cell 的 Tile 设置成 当前Tile
        this.cell.tile = this;

        //更改当前 Tile 的位置为 cell 的位置
        StartCoroutine(Animation(cell.transform.position,false));
    }

    /// <summary>
    /// 移动的动画
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    private IEnumerator Animation(Vector2 to , bool Merge)
    {
        float elapsed = 0.0f;
        float duration = 0.1f;

        Vector2 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector2.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (Merge)
            Destroy(gameObject);
    }

    
}
