using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Elevator : MonoBehaviour
{

    private GameObject gameManager;
    private GameManager gameManagerScript;

    private GameObject player;


    // Start is called before the first frame update
    public float elevatorSpeed = 1f;
    public float topLevel = 20f;
    public float bottomLevel = 0.3f;

    public bool elevatorActive = false;
    public bool elevatorStartMoving = false;
    public bool elevatorIsDown = false;
    public bool elevatorIsUp = true;


    public AudioClip buttonClick;
    public AudioSource elevatorAudio;
    

    public GameObject cageDoor;

    
    public GameObject generator;
    private Generator generatorScript;


    private Vector3 hit_point;


    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();

        player = GameObject.Find("PlayerContainer");
        elevatorAudio = GetComponent<AudioSource>();

        /*elevatorButton*/ /*= GameObject.FindGameObjectWithTag("elevatorButton").GetComponent<AudioSource>();*/

        cageDoor = GameObject.FindGameObjectWithTag("cageDoor");
        

        generator = GameObject.FindGameObjectWithTag("generatorButton");
        generatorScript = generator.GetComponent<Generator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ElevatorActivate();
        ElevatorMove();
        ElevatorAudioStop();
        ClickButton();
    }

    public void ClickButton()
    {
        if (elevatorActive)
        {
            gameManagerScript.audioManagerSource.PlayOneShot(buttonClick, 0.7f);
            elevatorActive = false;
        }
    }
    private void ElevatorActivate()
    {
        if (elevatorActive && generatorScript.start && !elevatorStartMoving)
        {
            gameManagerScript.audioManagerSource.PlayOneShot(buttonClick, 0.7f);
            elevatorAudio.Play();
            elevatorStartMoving = true;
            elevatorActive = false;
        }
    }
    public void ElevatorMove()
    {
        if (elevatorStartMoving && generatorScript.start && elevatorIsUp)
        {
            moveDown();
        }
        if (elevatorStartMoving && generatorScript.start && elevatorIsDown)
        {
            moveUp();
        }
    }
    public void moveDown()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, bottomLevel, transform.position.z), elevatorSpeed * Time.deltaTime);


        if (transform.position.y <= bottomLevel)
        {
            transform.position = Destination(bottomLevel);
            elevatorActive = false;
            elevatorIsUp = false;
            elevatorIsDown = true;
            elevatorStartMoving = false;
            elevatorAudio.Stop();

            cageDoor.GetComponent<Animator>().SetBool("cageDoorClose", true);
        }
    }
    public void moveUp()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, topLevel, transform.position.z), elevatorSpeed * Time.deltaTime);

        cageDoor.GetComponent<Animator>().SetBool("cageDoorClose", false);

        if (transform.position.y >= topLevel)
        {
            transform.position = Destination(topLevel);
            elevatorActive = false;
            elevatorIsUp = true;
            elevatorIsDown = false;
            elevatorStartMoving = false;
            elevatorAudio.Stop();

        }
    }

    public Vector3 Destination(float destenation)
    {
        return new Vector3(transform.position.x, destenation, transform.position.z);
    }

    private void ElevatorAudioStop()
    {
        if (generatorScript.stop)
        {
            elevatorAudio.Stop();
        }
    }

    public void Restart()
    {
        elevatorStartMoving = false;
        elevatorIsDown = false;
        elevatorIsUp = true;
        transform.position = new Vector3(transform.position.x, topLevel, transform.position.z);
    }
}
