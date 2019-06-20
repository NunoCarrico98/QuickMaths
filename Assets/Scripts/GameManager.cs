using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Four answers always. First one is the correct one.")]
    [SerializeField] private List<Question> questions;
    [Header("Car Spawn/not used atm")]
    [SerializeField] private int spawnZoneLimiter;
    [Header("Gold phase")]
    [SerializeField] private float goldPhaseTime;
    [SerializeField] private int coinsGrantedPerCar;

    private List<GameObject> carsList;
    private Question actualQuestion;
    private GameObject floor;
    private Bounds spawnableFloorArea;
    private Canvas canvas;

    private bool isGameSet;
    private int rightAnswerIndex;

    public static GameManager Instance { get; private set; }
    public int Coins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (isGameSet)
            if (Input.GetAxis("Car Movement") > 0 || Input.GetKey(KeyCode.Return))
            {
                if (canvas != null)
                    canvas.transform.GetChild(0).gameObject.SetActive(false);
                isGameSet = false;
            }
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameLevel());
    }

    private IEnumerator LoadGameLevel()
    {
        AsyncOperation asyncLoadLevel;
        asyncLoadLevel = SceneManager.LoadSceneAsync("Game");
        while (!asyncLoadLevel.isDone)
        {
            print("Loading the Scene");
            yield return null;
        }

        SetupGame();
        isGameSet = true;
    }

    private void SetupGame()
    {
        carsList = GameObject.FindGameObjectsWithTag("Car").Where(c => c.GetComponent<Car>() is Enemy).ToList();

        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        /*
        floor = GameObject.FindGameObjectWithTag("Floor");
        Collider col = floor.GetComponent<Collider>();
        spawnableFloorArea = new Bounds(col.bounds.center, new Vector3
            (col.bounds.extents.x * 2 - spawnZoneLimiter, 0, col.bounds.extents.z * 2 - spawnZoneLimiter)); */
        canvas.gameObject.SetActive(true);

        int randomQuestion = Random.Range(0, questions.Count);
        actualQuestion = questions[randomQuestion];

        FillQuestionBoard();
    }

    private void FillQuestionBoard()
    {
        List<int> answerNumbers = new List<int>() { 0, 1, 2, 3 };
        Transform panel = canvas.transform.GetChild(0).GetChild(0);

        for (int i = 0; i < 4; i++)
        {
            int n = answerNumbers[Random.Range(0, answerNumbers.Count)];
            string txt = panel.GetChild(i).GetComponent<TextMeshProUGUI>().text;
            panel.GetChild(i).GetComponent<TextMeshProUGUI>().text = $"{txt} {actualQuestion.Answers[n]}";

            if (n == 0)
            {
                rightAnswerIndex = i;
                Debug.Log($"Right answer is {rightAnswerIndex}");
            }
            answerNumbers.Remove(n);
        }
        panel.GetChild(4).GetComponent<TextMeshProUGUI>().text = actualQuestion.QuestionText;
    }

    private void OpenShop()
    {
        SceneManager.LoadScene("Shop");
    }

    private void ShowCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    private void Exit()
    {
        Application.Quit();
    }

    public void CheckWin(Enemy enemy)
    {
        if (enemy.Type == (EnemyType)rightAnswerIndex)
        {
            SetGoldState();
            StartCoroutine(GoldenPhaseCountdown());
        }
        else
        {
            if (enemy.Type != EnemyType.Regular)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    public void SetGoldState()
    {
        foreach (GameObject car in carsList)
        {
            car.GetComponent<Enemy>().GoldState = true;
        }
    }

    public void GrantCoins(Enemy enemy)
    {
        if (enemy.HasGrantedCoins)
            return;
        else
        {
            Coins += Random.Range(1, coinsGrantedPerCar + 1);
            enemy.HasGrantedCoins = true;
        }
    }

    private IEnumerator GoldenPhaseCountdown()
    {
        for (int i = 0; i < goldPhaseTime; i++)
        {
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene("MainMenu");
    }
}
