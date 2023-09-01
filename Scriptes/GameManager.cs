using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOver;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private int score;

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);   //初始化 score 分数
        highScoreText.text = LoadHighScore().ToString();  //加载最高分数

        //隐藏游戏结束界面
        gameOver.alpha = 0;
        gameOver.interactable= false;

        board.ClearBoard();    //清空界面上的 tile
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;

        StartCoroutine(Fad(1, 1));
    }


    //分数系统
    public void IncrementScore(int number)
    {
        SetScore(score + number);
    }

    //设置分数
    private void SetScore(int score)
    {
        this.score= score;
        scoreText.text = score.ToString();

        SaveHighScore(score);
    }

    /// <summary>
    /// 保存最高分数
    /// </summary>
    /// <param name="score"></param>
    private void SaveHighScore(int score)
    {
        int highScore = LoadHighScore();
        if(highScore < score)
        {
            PlayerPrefs.SetInt("highScore", score);
        }
    }

    /// <summary>
    /// 加载最高分数
    /// </summary>
    /// <returns></returns>
    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt("highScore", 0);
    }

    /// <summary>
    /// 结束界面动画
    /// </summary>
    /// <param name="to"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator Fad(float to , float delay)
    {
        yield return new WaitForSeconds(delay);

        float elasped = 0.0f;
        float duration = 1.0f;
        float from = gameOver.alpha;

        while(elasped< duration)
        {
            gameOver.alpha = Mathf.Lerp(from, to, elasped / duration);
            elasped += Time.deltaTime;

            yield return null;
        }

        gameOver.alpha = to;
    }
}
