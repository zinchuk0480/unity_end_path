using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Move_and_Look : MonoBehaviour
{

    private Rigidbody player;

    public float speed = 5.0f;
    public float mouseSens = 2.0f;
    public float jumpForce = 5.0f;

    public bool onGround = true;

    public float verticalRotation = 0;
    public float horizontalRotation = 0;

    public LayerMask cave;

    public GameObject Pointer;

    private Renderer pointerRender;

    public GameObject elevator;
    private Elevator elevatorScript;
    
    public GameObject generator;
    private Generator generatorScript;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody>();
        cave = 1 << 6;

        elevatorScript = elevator.GetComponent<Elevator>();
        generatorScript = generator.GetComponent<Generator>();
        pointerRender = Pointer.GetComponent<Renderer>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        Look();
        Move();
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

        transform.Translate(new Vector3(input_horizontal, 0, input_forward));

        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            player.AddForce(transform.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
            onGround = false;
        }
    }

    void CreateRay()
    {
        // центр экрана
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);


        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * 100f, UnityEngine.Color.yellow);

        RaycastHit hit;
        int layer_mask = LayerMask.GetMask("used", "used_2");

        pointerRender.material.color = new UnityEngine.Color(0.3f, 0.3f, 0.3f, 0.2f);

        if (Physics.Raycast(ray, out hit, 50, ~cave))
        {
            pointerRender.material.color = new UnityEngine.Color(1, 0, 0, 0.7f);

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
}
