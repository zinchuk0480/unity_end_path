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

    public GameObject generator;
    private Generator generatorScript;

    private void Start()
    {
        elevatorAudio = GetComponent<AudioSource>();
        elevatorButton = GameObject.FindGameObjectWithTag("elevatorButton").GetComponent<AudioSource>();
        generator = GameObject.FindGameObjectWithTag("generatorButton");
        generatorScript = generator.GetComponent<Generator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!elevatorStartMoving && elevatorActive && generatorScript.start)
        {
            elevatorButton.PlayOneShot(buttonClick, 0.7f);
            elevatorAudio.Play();
            elevatorStartMoving = true;
            elevatorActive = false;
        }
        

        if (elevatorStartMoving && elevatorIsUp && generatorScript.start)
        {
            moveDown();
        }
        if (elevatorStartMoving && elevatorIsDown && generatorScript.start)
        {
            moveUp();
        }

        if (elevatorActive && generatorScript.stop)
        {
            elevatorButton.PlayOneShot(buttonClick, 0.7f);
            elevatorAudio.Stop();
            elevatorActive = false;
        }                
        if (elevatorActive)
        {
            elevatorButton.PlayOneShot(buttonClick, 0.7f);
            elevatorActive = false;
        }
        if (generatorScript.stop)
        {
            elevatorAudio.Stop();
        }        
    }
    public void moveDown()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, bottomLevel, transform.position.z), elevatorSpeed * Time.deltaTime);
        
        if (transform.position.y <= bottomLevel)
        {
            transform.position = EndPath(bottomLevel);
            elevatorActive = false;
            elevatorIsUp = false;
            elevatorIsDown = true;
            elevatorStartMoving = false;
            elevatorAudio.Stop();
        }
    }

    public void moveUp()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, topLevel, transform.position.z), elevatorSpeed * Time.deltaTime);

        if (transform.position.y >= topLevel)
        {
            transform.position = EndPath(topLevel);
            elevatorActive = false;
            elevatorIsDown = false;
            elevatorIsUp = true;
            elevatorStartMoving = false;
            elevatorAudio.Stop();
        }
    }

    public Vector3 EndPath(float distenation)
    {
        return new Vector3(transform.position.x, distenation, transform.position.z);
    }
}
