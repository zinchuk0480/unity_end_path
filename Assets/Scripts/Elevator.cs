using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Elevator : MonoBehaviour
{
    // Start is called before the first frame update
    public float elevatorSpeed = 1f;
    public float topLevel = 20f;
    public float bottomLevel = 0.3f;

    public bool elevatorActive = false;
    private bool elevatorStartMoving = false;
    private bool elevatorIsDown = false;
    private bool elevatorIsUp = true;


    public AudioClip buttonClick;
    private AudioSource elevatorAudio;
    public AudioSource elevatorButton;

    public GameObject cageDoor;
    private bool cageOpen = false;

    public GameObject generator;
    private Generator generatorScript;

    private void Start()
    {
        elevatorAudio = GetComponent<AudioSource>();
        elevatorButton = GameObject.FindGameObjectWithTag("elevatorButton").GetComponent<AudioSource>();

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

    private void ClickButton()
    {
        if (elevatorActive)
        {
            elevatorButton.PlayOneShot(buttonClick, 0.7f);
            elevatorActive = false;
        }
    }
    private void ElevatorActivate()
    {
        if (elevatorActive && generatorScript.start && !elevatorStartMoving)
        {
            elevatorButton.PlayOneShot(buttonClick, 0.7f);
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

            cageOpen = true;
            cageDoor.GetComponent<Animator>().SetBool("cageDoorClose", true);
        }
    }
    public void moveUp()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, topLevel, transform.position.z), elevatorSpeed * Time.deltaTime);

        cageOpen = false;
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
}
