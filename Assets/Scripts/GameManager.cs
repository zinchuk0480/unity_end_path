using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine.VFX;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using static Unity.Burst.Intrinsics.X86;


public class GameManager : MonoBehaviour
{

    // HUD
    private string pointerText = "\u2022";
    private string pointerTextOnStairs = "—\n—\n—\n—";
    public GameObject pointer;
    public TextMeshProUGUI pointerTMP;

    public GameObject flashLight;
    private bool flashLightIsOn = false;

    // GameObjects
    public GameObject generator;
    private Generator generatorScript;

    public GameObject elevator;
    private Elevator elevatorScript;

    private GameObject moveAndLook;
    private Move_and_Look moveAndLookScript;

    public GameObject exitDoor;
    public bool exitDoorClosed = true;

    public GameObject caveWallLight;


    public bool alarm = false;
    public GameObject alarmLight;

    public GameObject luke;
    public bool lukeOpen = true;

    private GameObject audioManager;
    public AudioSource audioManagerSource;

    public bool insideDoorOpen = false;
    public GameObject insideDoor;

    public AudioSource insideDoorAudio;
    public GameObject afx;
    public AudioSource afxBoom;

    
    public AudioClip doorSignal;
    public AudioClip doorOpening;
    public AudioClip doorClosing;
    public AudioClip afxBoomClip;

    public VisualEffect vfxBoom;

    public Camera caveCamera;


    //GameControl
    public bool paused = false;
    private GameObject pauseMenu;

    public bool gameOver = false;
    private GameObject gameOverMenu;
    private GameObject[] gameOverTriggers;
    public GameObject gameOverDinamyc;


    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        pointer = GameObject.Find("Pointer");
        pointerTMP = pointer.GetComponent<TextMeshProUGUI>();

        generator = GameObject.FindGameObjectWithTag("generatorButton");
        generatorScript = generator.GetComponent<Generator>();

        elevator = GameObject.FindGameObjectWithTag("Elevator");
        elevatorScript = elevator.GetComponent<Elevator>();

        moveAndLook = GameObject.FindGameObjectWithTag("Player");
        moveAndLookScript = moveAndLook.GetComponent<Move_and_Look>();
        

        caveWallLight = GameObject.FindGameObjectWithTag("wallLight");
        caveWallLight.SetActive(false);

        GameObject alarmLight = GameObject.Find("alarm_lights");
        alarmLight.SetActive(false);

        exitDoor = GameObject.FindGameObjectWithTag("exitDoor");

        luke = GameObject.Find("Luke");


        audioManager = GameObject.Find("AudioManager");
        audioManagerSource = audioManager.GetComponent<AudioSource>();

        afx = GameObject.Find("afxBoom");
        afxBoom = afx.GetComponent<AudioSource>();

        insideDoor = GameObject.FindGameObjectWithTag("insideDoor");
        insideDoorAudio = insideDoor.GetComponent<AudioSource>();

        vfxBoom = GameObject.Find("vfxBoom").GetComponent<VisualEffect>();


        caveCamera = GameObject.FindGameObjectWithTag("caveCameraView").GetComponent<Camera>();



        //GameControl
        pauseMenu = GameObject.Find("Pause");
        pauseMenu.SetActive(false);
        
        gameOverMenu = GameObject.Find("Game Over");
        gameOverMenu.SetActive(false);
        gameOverTriggers = GameObject.FindGameObjectsWithTag("gameOver");


    }

    // Update is called once per frame
    void Update()
    {
        GameOverControl();
        PointerControl();
        FlashControl();
        

        if (alarm)
        {
            alarmLight.SetActive(true);
            alarmLight.GetComponent<Animator>().SetBool("alarmLigthActive", true);
            gameOverDinamyc.transform.position = new Vector3(2.2f, -0.3f, 0.7f);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseControl();
        }
    }

    void PointerControl()
    {
        if (moveAndLookScript.lookToStairs)
        {
            pointerTMP.text = pointerTextOnStairs;
            pointerTMP.lineSpacingAdjustment = -60;
        } else
        {
            pointerTMP.text = pointerText;
            pointerTMP.lineSpacingAdjustment = 0;
        }
    }

    public void PauseControl()
    {
        paused = !paused;

        if (paused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        } else
        {
            Time.timeScale = 1.0f;
            pauseMenu.SetActive(false);
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void GameOverControl()
    {
        if (gameOver)
        {
            paused = true;
            Time.timeScale = 0f;
            gameOverMenu.SetActive(true);
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
        if (!gameOver & !paused)
        {
            Time.timeScale = 1.0f;
            gameOverMenu.SetActive(false);
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ApplicationControl()
    {
        Application.Quit();
    }

    public void FlashControl()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {

            flashLightIsOn = !flashLightIsOn;
            moveAndLookScript.playerAudio.PlayOneShot(moveAndLookScript.flashSound, 0.3f);
            if (flashLightIsOn)
            {
                flashLight.SetActive(true);
            }
            if (!flashLightIsOn)
            {
                flashLight.SetActive(false);
            }
        }

    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartControl()
    {
        alarm = false;
        alarmLight.SetActive(false);
        moveAndLookScript.Restart();
        elevatorScript.Restart();
        generatorScript.Restart();
        gameOver = false;
        gameOverDinamyc.transform.position = new Vector3(2.2f, -5f, 0.7f);
        PauseControl();
    }
}
