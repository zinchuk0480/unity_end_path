using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86;
using static UnityEngine.GraphicsBuffer;

public class Move_and_Look : MonoBehaviour
{
    private CharacterController controller;
    private float playerHeight;
    private float playerHeightStay = 1.7f;
    private float playerHeightSit = 0.7f;
    private float playerRadius = 0.5f;
    private float playerCenterY = 0.5f;
    private Vector3 playerVelocity;
    private float gravityValue = -9.81f;

    public float speed;
    public float speedWalk = 3.0f;
    public float speedSit = 2.0f;
    public float speedRun = 5.0f;
    public float speedAir = 1.0f;
    public bool stateSit = false;
    public float mouseSens = 2.0f;
    public float jumpForce = 1.5f;
    public float rayDistance = 1.5f;

    private GameObject playerContainer;
    public Rigidbody player;
    private Vector3 playerStartPosition = new Vector3(-9.5f, 0f, -8.08f);
    private Quaternion playerStartRotate = Quaternion.Euler(0f, 24f, 0f);


    private GameObject gameManager;
    private GameManager gameManagerScript;




    public bool onGround = true;
    public float rayToGroundDistance = 0.37f;

    public float verticalRotation = 0;
    public float horizontalRotation = 0;

    public LayerMask cave;
    public LayerMask used;
    public LayerMask floor;

    public GameObject elevator;
    private Elevator elevatorScript;

    public GameObject generator;
    private Generator generatorScript;

    public bool inElevator = false;

    private GameObject stairs;
    private GameObject current_stairs;
    public bool lookToStairs = false;
    public bool onStairs = false;

    public AudioSource playerAudio;
    public AudioClip flashSound;


    // Start is called before the first frame update
    void Start()
    {
        /*controller = gameObject.AddComponent<CharacterController>();*/
        controller = GetComponent<CharacterController>();
        playerHeight = playerHeightStay;
        controller.height = playerHeight;
        controller.radius = playerRadius;
        controller.center = new Vector3(0, playerCenterY, 0);

        speed = speedWalk;
        

        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();

        playerContainer = GameObject.Find("PlayerContainer");
        player = GetComponent<Rigidbody>();
        cave = 1 << 6;
        used = 1 << 8;
        floor = 1 << 11;

        elevator = GameObject.Find("Elevator");
        elevatorScript = elevator.GetComponent<Elevator>();
        generatorScript = generator.GetComponent<Generator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        PlayerInElevatorMove();
        if (!gameManagerScript.paused & !gameManagerScript.gameOver)
        {
            Look();
        }
        Move();

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            stateSit = true;
            playerHeight = playerHeightSit;
            speed = speedSit;
        }
        else
        {
            playerHeight = playerHeightStay;
        }
        controller.height = playerHeight;
        StairsControl();
        lookToStairs = false;

        CreateRay();
    }

    void Look()
    {
        horizontalRotation = Input.GetAxis("Mouse X") * mouseSens;
        transform.Rotate(0, horizontalRotation, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSens;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        gameManagerScript.flashLight.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void Move()
    {
        
        speed = speedWalk;
        if (stateSit)
        {
            speed = speedSit;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = speedRun;
        }

        float input_forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float input_horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        if (onStairs)
        {
            player.isKinematic = true;

            Vector3 stairsTop = current_stairs.transform.position + Vector3.up * (current_stairs.GetComponent<Renderer>().bounds.size.y / 2);
            Vector3 stairsBottom = current_stairs.transform.position - Vector3.up * (current_stairs.GetComponent<Renderer>().bounds.size.y / 2);

            if (input_forward > 0 && player.transform.position.y < stairsTop.y)
            {
                transform.Translate(new Vector3(0, input_forward * 0.5f, 0));
            }
            if (input_forward < 0 && player.transform.position.y > stairsBottom.y)
            {
                transform.Translate(new Vector3(0, input_forward, 0));
            }
        }
        else
        {
            onGround = controller.isGrounded;
            if (onGround && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            Vector3 move = new Vector3(input_horizontal, 0, input_forward);
            move = transform.TransformDirection(move);
            controller.Move(move);

            if (Input.GetKeyDown(KeyCode.Space) && onGround)
            {
                playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }


    void CreateRay()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, UnityEngine.Color.yellow);

        RaycastHit hit;
        int layer_mask = LayerMask.GetMask("used", "used_2");

        gameManagerScript.pointerTMP.color = new UnityEngine.Color(0.3f, 0.3f, 0.3f, 0.5f);

        if (Physics.Raycast(ray, out hit, rayDistance, used))
        {
            gameManagerScript.pointerTMP.color = new UnityEngine.Color(1, 0, 0, 0.7f);
            ClickOnHandler(hit);
        }
    }

    void ClickOnHandler(RaycastHit hit)
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E) && hit.rigidbody.CompareTag("elevatorButton"))
        {
            gameManagerScript.audioManagerSource.transform.position = hit.point;
            elevatorScript.elevatorActive = true;
        }
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E) && hit.rigidbody.CompareTag("generatorButton"))
        {
            gameManagerScript.audioManagerSource.transform.position = hit.point;
            generatorScript.generatorActive = true;
        }
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E) && hit.rigidbody.CompareTag("exitDoor"))
        {
            gameManagerScript.audioManagerSource.transform.position = hit.point;
            gameManagerScript.exitDoorClosed = !gameManagerScript.exitDoorClosed;
            if (gameManagerScript.exitDoorClosed)
            {
                gameManagerScript.exitDoor.GetComponent<Animator>().SetBool("doorIsClosed", true);
            } else
            {
                gameManagerScript.exitDoor.GetComponent<Animator>().SetBool("doorIsClosed", false);
                gameManagerScript.exitDoor.GetComponent<AudioSource>().PlayOneShot(gameManagerScript.doorSignal, 0.7f);
            }
        }
        if (hit.rigidbody.CompareTag("stairs"))
        {
            lookToStairs = true;
            stairs = hit.collider.gameObject;
        }
    }

    public void StairsControl()
    {
        if (onStairs && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
        {
            onStairs = false;
            player.isKinematic = false;
            transform.localPosition = new Vector3(0f, transform.position.y + 0.5f, 2f);
        }
        if (!onStairs && lookToStairs && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)))
        {
            current_stairs = stairs;
            onStairs = true;
            player.isKinematic = true;
            Vector3 newPlayerPosition = new Vector3(current_stairs.GetComponent<Rigidbody>().transform.position.x - 0.5f, transform.position.y, current_stairs.GetComponent<Rigidbody>().transform.position.z - 0.1f);
            transform.position = newPlayerPosition;
        }
    }

    public void PlayerInElevatorMove()
    {
        if (inElevator)
        {
            transform.SetParent(elevator.transform);
            transform.position = new Vector3(transform.position.x, elevator.transform.position.y + 1.4f, transform.position.z);
            player.isKinematic = true;
        } else
        {
            transform.SetParent(playerContainer.transform);
            player.isKinematic = false;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Elevator") && !onStairs)
        {
            inElevator = true;
        }

        if (other.CompareTag("alarmTrigger") && !generatorScript.generatorBroke)
        {
            gameManagerScript.alarm = true;
            gameManagerScript.vfxBoom.Play();

            gameManagerScript.afxBoom.transform.position = generatorScript.transform.position;
            gameManagerScript.afxBoom.PlayOneShot(gameManagerScript.afxBoomClip, 0.2f);
        }
        if (other.CompareTag("insideDoorTrigger") && !gameManagerScript.insideDoorOpen)
        {
            gameManagerScript.insideDoorOpen = true;
            gameManagerScript.insideDoor.GetComponent<Animator>().SetBool("doorOpening", true);
            gameManagerScript.insideDoorAudio.Stop();
            gameManagerScript.insideDoorAudio.PlayOneShot(gameManagerScript.doorOpening);
        }
        if (other.CompareTag("gameOver"))
        {
            gameManagerScript.gameOver = true;
        }
        if (other.CompareTag("lukeTrigger") & gameManagerScript.lukeOpen)
        {
            gameManagerScript.lukeOpen = false;
            gameManagerScript.luke.transform.Translate(-99.9f, -11.2f, 13);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            inElevator = false;
        }
        if (other.CompareTag("insideDoorTrigger") && gameManagerScript.insideDoorOpen)
        {
            gameManagerScript.insideDoorOpen = false;
            gameManagerScript.insideDoor.GetComponent<Animator>().SetBool("doorOpening", false);
            gameManagerScript.insideDoorAudio.Stop();
            gameManagerScript.insideDoorAudio.PlayOneShot(gameManagerScript.doorClosing);
        }
    }

    public void Restart()
    {
        player.velocity = Vector3.zero;
        controller.enabled = false;
        inElevator = false;
        PlayerInElevatorMove();
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotate;
        controller.enabled = true;
    }
}
