using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    //state �������� Tile �ĵ�ǰ״̬��������ɫ���ı���ɫ
    public TileState state { get; private set; }
    // cell �������� Tile λ���ĸ� TileCell ֮��
    public TileCell cell { get; private set; }
    //number �������� Tile ��ʾ����
    public int number { get; private set; }

    //Tile ��һ�� Image �� һ�� TextMeshProUGUI�������壩���
    private Image background;
    private TextMeshProUGUI text;

    public bool locked { get;  set; }  //��֤ÿ��ֻ�ܺϲ�һ��

    private void Awake()
    {
        background= GetComponent<Image>();
        text=GetComponentInChildren<TextMeshProUGUI>();
    }

    //���� Tile ��״̬��������ɫ��������ɫ������ֵ
    public void SetState(TileState state ,int number)
    {
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor;
        text.color=state.textColor;
        text.text = number.ToString();
    }

    //�� Tile ͣ���� TileCell ��
    public void Spawn(TileCell cell)
    {
        if(this.cell != null)  
        {
            //��֤ cell ����ռ�ã������ռ�ã��ͽ� tile ����Ϊ null
            this.cell.tile = null;
        }

        this.cell = cell;  
        //�� cell �� Tile ���ó� ��ǰTile
        this.cell.tile = this;

        //���ĵ�ǰ Tile ��λ��Ϊ cell ��λ��
        transform.position = cell.transform.position;
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            //��֤ cell ����ռ�ã������ռ�ã��ͽ� tile ����Ϊ null
            this.cell.tile = null;
        }

        this.cell = null;
        cell.tile.locked = true;

    
        StartCoroutine(Animation(cell.transform.position, true));
    }

    //�� Tile �ƶ��� TileCell ��
    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            //��֤ cell ����ռ�ã������ռ�ã��ͽ� tile ����Ϊ null
            this.cell.tile = null;
        }

        this.cell = cell;
        //�� cell �� Tile ���ó� ��ǰTile
        this.cell.tile = this;

        //���ĵ�ǰ Tile ��λ��Ϊ cell ��λ��
        StartCoroutine(Animation(cell.transform.position,false));
    }

    /// <summary>
    /// �ƶ��Ķ���
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
