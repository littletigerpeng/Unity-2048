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
        SetScore(0);   //��ʼ�� score ����
        highScoreText.text = LoadHighScore().ToString();  //������߷���

        //������Ϸ��������
        gameOver.alpha = 0;
        gameOver.interactable= false;

        board.ClearBoard();    //��ս����ϵ� tile
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


    //����ϵͳ
    public void IncrementScore(int number)
    {
        SetScore(score + number);
    }

    //���÷���
    private void SetScore(int score)
    {
        this.score= score;
        scoreText.text = score.ToString();

        SaveHighScore(score);
    }

    /// <summary>
    /// ������߷���
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
    /// ������߷���
    /// </summary>
    /// <returns></returns>
    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt("highScore", 0);
    }

    /// <summary>
    /// �������涯��
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
