using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

//using UnityEditor;

public class GameManager : Singleton<GameManager>
{
    [Header("Persistent")]
    public PlayerController player;
    private GameObject playerObj;
    public int sceneIndex = 0;

    [Header("Main Menu")]
    public GameObject mainMenu;
    public GameObject controlsMenu;

    [Header("Game UI")]
    public GameObject scoreboard;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI XPText;

    [Header("Ending Screen")]
    public GameObject dedScreen;
    public TextMeshProUGUI finalScoreText;

    [Header("Scoring")]
    public int distanceBase = 0;
    public int actions = 0;
    public int orbValue = 100;
    public int eatValue = 100;
    public int missValue = 200;

    // Start is called before the first frame update
    void Start()
    {
        //StartRun();
        mainMenu.SetActive(true);
        scoreboard.SetActive(false);
        dedScreen.SetActive(false);
        controlsMenu.SetActive(false);
    }

    void StartRun()
    {
        mainMenu.SetActive(false);
        dedScreen.SetActive(false);
        scoreboard.SetActive(true);

        distanceBase = 0;
        actions = 0;

        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneIndex == 1)
        {
            scoreText.text =
                "Score: "
                + (distanceBase + actions + ((int)player.transform.position.z)).ToString();
            XPText.text = "XP: " + actions.ToString();
        }
    }

    public void CollectOrb(GameObject orb)
    {
        Destroy(orb);

        actions += orbValue;
        //SetCountText()
    }

    public void GrantNearMiss()
    {
        actions += missValue;
    }

    public void EatEntity(GameObject entity)
    {
        Destroy(entity);
        actions += eatValue;
    }

    public void SetXP(int gain)
    {
        actions += gain;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (sceneIndex == 1)
        {
            StartRun();
        }
        else
        {
            Start();
        }

        Time.timeScale = 1;
    }

    public void OnPlayButtonClick()
    {
        sceneIndex = 1;
        GoToScene();
    }

    public void OnMenuButtonClick()
    {
        sceneIndex = 0;
        GoToScene();
    }

    public void OnControlsButtonClick()
    {
        controlsMenu.SetActive(!controlsMenu.activeSelf);
        mainMenu.SetActive(!mainMenu.activeSelf);
    }

    private void GoToScene()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowEndScreen()
    {
        Time.timeScale = 0;
        scoreboard.SetActive(false);
        int distanceRun = distanceBase + (int)player.transform.position.z;
        finalScoreText.text =
            $"You were farmed for {distanceRun + actions} XP!\nYou farmed {actions} XP during your run!\nYou ran {distanceRun} meters before you were caught.";
        dedScreen.SetActive(true);
    }
}
