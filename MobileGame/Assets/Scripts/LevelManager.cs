using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;

    public static LevelManager Instance { get { return _instance; } }

    [Header("UI")]
    public GameObject gameOverScreen;
    public GameObject winScreen;
    public GameObject pauseButton;
    public TextMeshProUGUI hitsTakenText;

    [Header("GamePlay")]
    public Transform ball;

    public int maxNumShots = 3;

    public int CurrentShot { get; private set; }

    public bool Loading { get; private set; }

    private float levelBaseY;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
        Loading = true;
        StartCoroutine(Load());
    }

    // Start is called before the first frame update
    void Start()
    {
        levelBaseY = transform.position.y;
        CurrentShot = 0;
        IncrementShot();
    }

    // Update is called once per frame
    void Update()
    {
        bool gameOverConditions = (CurrentShot > maxNumShots ||
                                   ball.position.y < levelBaseY);

        if(gameOverConditions)
        {
            GameOver();
        }
    }

    public void IncrementShot()
    {
        CurrentShot++;
        hitsTakenText.text = "Hit: " + CurrentShot + " / " + maxNumShots;
    }

    public void WinLevel()
    {
        winScreen.SetActive(true);
        ball.gameObject.SetActive(false);
        pauseButton.SetActive(false);
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        CurrentShot = maxNumShots;
        ball.gameObject.SetActive(false);
        hitsTakenText.gameObject.SetActive(false);
        pauseButton.SetActive(false);
    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(0.3f);

        Loading = false;
    }
}
