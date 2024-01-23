using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Move_and_Look : MonoBehaviour
{
    private GameObject gameManager;
    private GameManager gameManagerScript;

    private Rigidbody player;

    public float speed = 5.0f;
    public float mouseSens = 2.0f;
    public float jumpForce = 5.0f;
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

    private bool inElevator = false;



    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();

        player = GetComponent<Rigidbody>();
        cave = 1 << 6;
        used = 1 << 8;

        elevatorScript = elevator.GetComponent<Elevator>();
        generatorScript = generator.GetComponent<Generator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        PlayerInElevatorMove();
        Look();
        Move();
        CreateRay();
    }

    void Look()
    {
        horizontalRotation = Input.GetAxis("Mouse X") * mouseSens;
        transform.Rotate(0, horizontalRotation, 0);

        // �������� ������ �� ���������
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSens;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90); // ����������� ���� ������ �� ���������
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

    }

    void Move()
    {
        float input_forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float input_horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.Translate(new Vector3(input_horizontal, 0, input_forward));

        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            player.AddForce(transform.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
            onGround = false;
        }
    }

    void CreateRay()
    {
        // ����� ������
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);


        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, UnityEngine.Color.yellow);

        RaycastHit hit;
        int layer_mask = LayerMask.GetMask("used", "used_2");

        gameManagerScript.pointerTMP.color = new UnityEngine.Color(0.3f, 0.3f, 0.3f, 0.5f);

        if (Physics.Raycast(ray, out hit, rayDistance, used))
        {
            gameManagerScript.pointerTMP.color = new UnityEngine.Color(1, 0, 0, 0.7f);

            if (Input.GetMouseButtonDown(0) && hit.rigidbody.CompareTag("elevatorButton"))
            {
                elevatorScript.elevatorActive = true;
            }
            if (Input.GetMouseButtonDown(0) && hit.rigidbody.CompareTag("generatorButton"))
            {
                generatorScript.generatorActive = true;
            }

        }
    }

    public void PlayerInElevatorMove()
    {
        if (inElevator)
        {
            transform.position = new Vector3(transform.position.x, elevator.transform.position.y + 1.8f, transform.position.z);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            /*player.isKinematic = true;*/
            inElevator = true;
        }

        if (other.CompareTag("alarmTrigger"))
        {
            generatorScript.generatorActive = false;
            gameManagerScript.alarm = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            /*player.isKinematic = false;*/
            inElevator = false;
        }
    }
/*    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.contacts.Length);
        if (collision.contacts.Length <= 0)
        {
            player.isKinematic = false;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log(collision.contacts.Length);
        if (collision.contacts.Length >= 0 & collision.gameObject.layer != cave)
        {
            player.isKinematic = true;
        }
    }*/
}
