using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject introPage;
    public GameObject howToPlay;
    public GameObject playerWin;
    public GameObject playerLose;
    private bool startGame = false;
    private bool showingHowToPlay = false;
    private ClawController clawController;
    public GameObject clawControlScript;

    public RawImage button1;
    public RawImage restart1;
    public RawImage restart2;
    public AudioSource buttonSound;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonSound = GetComponent<AudioSource>();
        clawController = ClawController.Instance;
        //Start game paused
        Time.timeScale = 0;
        introPage.SetActive(true);
        howToPlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Time.timeScale);
        if (!startGame)
        {
            if (Input.GetKey(KeyCode.O) || Input.GetKey(KeyCode.P))
            {
                button1.color = Color.gray;
                buttonSound.Play();
            }
            if (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.P))
            {
                introPage.SetActive(false);
                howToPlay.SetActive(true);
                startGame = true;
                showingHowToPlay = true;
            }
        }
        else if (showingHowToPlay)
        {
            
            if (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.P))
            {
                Time.timeScale = 1;
                howToPlay.SetActive(false);
                StartCoroutine(PlayNow());
                //howToPlay.SetActive(false);
                
            }
        }
        if (clawController.numberOfTries <= 0)
        {
            Time.timeScale = 0;
            Debug.Log("Game Over");
            playerWin.SetActive(true);
            if (Input.GetKey(KeyCode.O) || Input.GetKey(KeyCode.P))
            {
                restart1.color = Color.gray;
                buttonSound.Play();
            }
            if (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.P))
            {
                
                RestartGame();
            }
            

        }
        else if (clawController.playerIsCaught)
        {
            Time.timeScale = 0;
            Debug.Log("Player Caught");
            playerLose.SetActive(true); 
            if (Input.GetKey(KeyCode.O) || Input.GetKey(KeyCode.P))
            {
                restart2.color = Color.gray;
                buttonSound.Play();
            }
            if (Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.P))
            {
                RestartGame();
            }
        }
    }

    public void EndGame()
    {
        
        //show ui of who won
        //prompt players to restart
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    IEnumerator PlayNow()
    {
        
       // yield return new WaitForSeconds(0.5f);
        
        yield return new WaitForSeconds(3f);
        clawControlScript.GetComponent<ClawController>().enabled = true;

    }
}
