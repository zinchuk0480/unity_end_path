using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Move_and_Look : MonoBehaviour
{
    private GameObject gameManager;
    private GameManager gameManagerScript;

    private GameObject playerContainer;
    private Rigidbody player;

    public float speed = 5.0f;
    public float mouseSens = 2.0f;
    public float jumpForce = 20.0f;
    public float rayDistance = 1.5f;

    public bool onGround = true;

    public float verticalRotation = 0;
    public float horizontalRotation = 0;

    public LayerMask cave;
    public LayerMask used;

    public GameObject elevator;
    private Elevator elevatorScript;
    
    public GameObject generator;
    private Generator generatorScript;

    public bool inElevator = false;

    private GameObject stairs;
    private GameObject current_stairs;
    public bool lookToStairs = false;
    public bool onStairs = false;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();

        playerContainer = GameObject.Find("PlayerContainer");
        player = GetComponent<Rigidbody>();
        cave = 1 << 6;
        used = 1 << 8;

        elevator = GameObject.Find("Elevator");
        elevatorScript = elevator.GetComponent<Elevator>();
        generatorScript = generator.GetComponent<Generator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        PlayerInElevatorMove();
        Look();
        Move();

        StairsControl();
        lookToStairs = false;

        CreateRay();
    }

    void Look()
    {
        horizontalRotation = Input.GetAxis("Mouse X") * mouseSens;
        transform.Rotate(0, horizontalRotation, 0);

        // Вращение камеры по вертикали
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSens;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90); // Ограничение угла обзора по вертикали
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

    }

    void Move()
    {
        float input_forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float input_horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        if (onStairs)
        {
            player.isKinematic = true;

            Vector3 stairsTop = current_stairs.transform.position + Vector3.up * (current_stairs.GetComponent<Renderer>().bounds.size.y / 2);
            Vector3 stairsBottom = current_stairs.transform.position - Vector3.up * (current_stairs.GetComponent<Renderer>().bounds.size.y / 2);

            if (input_forward > 0 && player.transform.position.y < stairsTop.y)
            {
                transform.Translate(new Vector3(0, input_forward, 0));
            }
            if (input_forward < 0 && player.transform.position.y > stairsBottom.y)
            {
                transform.Translate(new Vector3(0, input_forward, 0));
            }
        }
        else
        {
            transform.Translate(new Vector3(input_horizontal, 0, input_forward));

            if (Input.GetKeyDown(KeyCode.Space) && onGround)
            {
                player.AddForce(Vector3.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
                onGround = false;
            }
        }
    }

    void CreateRay()
    {
        // центр экрана
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, UnityEngine.Color.yellow);

        RaycastHit hit;
        int layer_mask = LayerMask.GetMask("used", "used_2");

        gameManagerScript.pointerTMP.color = new UnityEngine.Color(0.3f, 0.3f, 0.3f, 0.5f);

        if (Physics.Raycast(ray, out hit, rayDistance, used))
        {
            
            gameManagerScript.pointerTMP.color = new UnityEngine.Color(1, 0, 0, 0.7f);

            Debug.Log(hit.rigidbody.name);

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
        if (hit.rigidbody.CompareTag("stairs"))
        {
            lookToStairs = true;
            stairs = hit.collider.gameObject;
        }
    }

    public void StairsControl()
    {
        if (onStairs && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)))
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
            /*transform.position = transform.position;*/
            /*transform.parent.position = player.transform.position;*/
            player.isKinematic = false;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Elevator") && !onStairs)
        {
            inElevator = true;
        }

        if (other.CompareTag("alarmTrigger"))
        {
            Debug.Log("Generator!!!!");
            gameManagerScript.alarm = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            inElevator = false;
        }
    }
}
