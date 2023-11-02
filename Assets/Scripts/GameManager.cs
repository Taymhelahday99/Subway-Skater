using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    public static GameManager Instance {set; get;}

    public bool IsDead {set; get;}
    private bool isGameStarted = false;
    private PlayerController motor;

    //UI and UI fields
    public Animator gameCanvas, menuAnim;
    public TextMeshProUGUI scoreText, coinText, modifierText, highScore;
    public float score, coinScore, modifierScore;
    private int lastScore;

    //Death Menu
    public Animator deathMenuAnim;
    public TextMeshProUGUI finalScore, finalCoin;
    private void Awake()
    {
        Instance = this;
        modifierScore = 1;
        motor =GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        modifierText.text ="x " + modifierScore.ToString("0.0");
        scoreText.text =score.ToString("0");
        coinText.text = coinScore.ToString("0");
       highScore.text = PlayerPrefs.GetInt("HighScore").ToString();
    } 

    private void Update()
    {
        if(MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            motor.StartRunning();
            FindObjectOfType<GlacierSpawner>().IsScrolling = true;
            FindObjectOfType<Cameracontroller>().IsMoving =true;
            gameCanvas.SetTrigger("Show"); 
            menuAnim.SetTrigger("Hide"); 
        }
        if (isGameStarted && !IsDead)
        {
            lastScore = (int)score;
            score += (Time.deltaTime * modifierScore);
            if(lastScore != (int)score)
            {
                lastScore = (int)score;
                scoreText.text =score.ToString("0");
            }
           
        }
    }

    // private void UpdateScore()
    // {
    //     scoreText.text =score.ToString();
    //     coinText.text = coinScore.ToString();
        

    // }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text ="x " + modifierScore.ToString("0.0");
    }

    public void GetCoin()
    {
        coinScore ++;
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE_AMOUNT; 
        scoreText.text =  scoreText.text =score.ToString();
    }

    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game"); 
    }

    public void OnDeath()
    {
        IsDead = true;
        finalScore.text = score.ToString("0");
        finalCoin.text = coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        gameCanvas.SetTrigger("Hide"); 
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;

        if(score > PlayerPrefs.GetInt("HighScore"))
        {
            
            float s = score;
            if(s% 1 == 0)
                s += 1;
            PlayerPrefs.SetInt("HighScore", (int)score);

        }
    }
}
