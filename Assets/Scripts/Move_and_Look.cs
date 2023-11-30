using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_and_Look : MonoBehaviour
{
    public float speed = 5.0f;
    public float mouseSens = 2.0f;

    public float verticalRotation = 0;
    public float horizontalRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Look();
        Move();
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
    }
}
