using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using TMPro;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine.VFX;
using UnityEngine.Rendering.Universal;
using UnityEditor.PackageManager;

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


    public bool alarm = false;
    public GameObject alarmLight;

    private GameObject audioManager;
    public AudioSource audioManagerSource;

    public bool insideDoorOpen = false;
    public GameObject insideDoor;
    public AudioSource insideDoorAudio;

    public AudioClip doorSignal;
    public AudioClip doorOpening;
    public AudioClip doorClosing;

    public VisualEffect boom;

    //GameControl
    public bool paused = false;
    private GameObject pauseMenu;


    // Start is called before the first frame update
    void Start()
    {
        pointer = GameObject.Find("Pointer");
        pointerTMP = pointer.GetComponent<TextMeshProUGUI>();

        generator = GameObject.FindGameObjectWithTag("generatorButton");
        generatorScript = generator.GetComponent<Generator>();

        elevator = GameObject.FindGameObjectWithTag("Elevator");
        elevatorScript = elevator.GetComponent<Elevator>();

        moveAndLook = GameObject.FindGameObjectWithTag("Player");
        moveAndLookScript = moveAndLook.GetComponent<Move_and_Look>();


        GameObject alarmLight = GameObject.Find("alarm_lights");
        alarmLight.SetActive(false);

        exitDoor = GameObject.FindGameObjectWithTag("exitDoor");



        audioManager = GameObject.Find("AudioManager");
        audioManagerSource = audioManager.GetComponent<AudioSource>();

        insideDoor = GameObject.FindGameObjectWithTag("insideDoor");
        insideDoorAudio = insideDoor.GetComponent<AudioSource>();

        boom = GameObject.Find("Boom").GetComponent<VisualEffect>();



        //GameControl
        pauseMenu = GameObject.Find("Pause");
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PointerControl();
        FlashControl();
        

        if (alarm)
        {
            alarmLight.SetActive(true);
            alarmLight.GetComponent<Animator>().SetBool("alarmLigthActive", true);
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
        } else
        {
            Time.timeScale = 1.0f;
            pauseMenu.SetActive(false);
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


    public void RestartControl()
    {
        alarm = false;
        moveAndLookScript.Restart();
        elevatorScript.Restart();
        generatorScript.Restart();
        PauseControl();
    }
}
