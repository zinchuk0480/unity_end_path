using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using TMPro;
using UnityEngine.UIElements;


public class GameManager : MonoBehaviour
{

    private string pointerText = "\u2022";
    public GameObject pointer;
    public TextMeshProUGUI pointerTMP;


    public GameObject generator;
    private Generator generatorScript;

    public bool alarm = false;
    public GameObject alarmLight;

    private GameObject audioManager;
    public AudioSource audioManagerSource;


    // Start is called before the first frame update
    void Start()
    {
        pointer = GameObject.Find("Pointer");
        pointerTMP = pointer.GetComponent<TextMeshProUGUI>();
        pointerTMP.text = pointerText;


        generator = GameObject.FindGameObjectWithTag("generatorButton");
        generatorScript = generator.GetComponent<Generator>();

        GameObject alarmLight = GameObject.Find("alarm_lights");
        alarmLight.SetActive(false);


        audioManager = GameObject.Find("AudioManager");
        audioManagerSource = audioManager.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (alarm)
        {
            alarmLight.SetActive(true);
            alarmLight.GetComponent<Animator>().SetBool("alarmLigthActive", true);
            generatorScript.GeneratorOff();
        }
    }
}
