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
    private bool elevatorIsDown = false;
    private bool elevatorIsUp = true;


    // Update is called once per frame
    void Update()
    {
        if (elevatorActive && elevatorIsUp)
        {
            moveDown();

        }
        if (elevatorActive && elevatorIsDown)
        {
            moveUp();
        }
    }

    public void moveDown()
    {
        Debug.Log("MoveDown");
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, bottomLevel, transform.position.z), elevatorSpeed * Time.deltaTime);
        if (transform.position.y <= bottomLevel)
        {
            transform.position = EndPath(bottomLevel);
            elevatorActive = false;
            elevatorIsUp = false;
            elevatorIsDown = true;
        }
    }

    public void moveUp()
    {
        Debug.Log("MoveUp");
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, topLevel, transform.position.z), elevatorSpeed * Time.deltaTime);
        /*transform.position = new Vector3(transform.position.x, transform.position.y * 0.001f * Time.deltaTime, transform.position.z);*/
        if (transform.position.y >= topLevel)
        {
            transform.position = EndPath(topLevel);
            elevatorActive = false;
            elevatorIsDown = false;
            elevatorIsUp = true;
        }
    }

    public Vector3 EndPath(float distenation)
    {
        return new Vector3(transform.position.x, distenation, transform.position.z);
    }
}
