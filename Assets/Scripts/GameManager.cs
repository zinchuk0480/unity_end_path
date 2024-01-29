using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using TMPro;
using UnityEngine.UIElements;


public class GameManager : MonoBehaviour
{

    // HUD
    private string pointerText = "\u2022";
    private string pointerTextOnStairs = "—\n—\n—\n—";
    public GameObject pointer;
    public TextMeshProUGUI pointerTMP;


    // GameObjects
    public GameObject generator;
    private Generator generatorScript;

    private GameObject moveAndLook;
    private Move_and_Look moveAndLookScript;



    public bool alarm = false;
    public GameObject alarmLight;

    private GameObject audioManager;
    public AudioSource audioManagerSource;


    // Start is called before the first frame update
    void Start()
    {
        pointer = GameObject.Find("Pointer");
        pointerTMP = pointer.GetComponent<TextMeshProUGUI>();

        generator = GameObject.FindGameObjectWithTag("generatorButton");
        generatorScript = generator.GetComponent<Generator>();

        moveAndLook = GameObject.FindGameObjectWithTag("Player");
        moveAndLookScript = moveAndLook.GetComponent<Move_and_Look>();


        GameObject alarmLight = GameObject.Find("alarm_lights");
        alarmLight.SetActive(false);


        audioManager = GameObject.Find("AudioManager");
        audioManagerSource = audioManager.GetComponent<AudioSource>();



    }

    // Update is called once per frame
    void Update()
    {
        PointerControl();

        if (alarm)
        {
            alarmLight.SetActive(true);
            alarmLight.GetComponent<Animator>().SetBool("alarmLigthActive", true);
            generatorScript.GeneratorOff();
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
}
