using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour
{

    public Transform CameraTransform;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0.7f);
        Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);

        transform.position = worldCenter;
    }
}