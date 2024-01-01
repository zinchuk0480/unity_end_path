using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Generator : MonoBehaviour
{
    public bool generatorActive = false;
    public bool start = false;
    public bool stop = true;

    public GameObject generator;
    public VisualEffect generatorVFX;

    private void Start()
    {
        generatorVFX = generator.GetComponent<VisualEffect>();
        generatorVFX.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (stop && generatorActive)
        {
            GeneratorOn();
        }
        if (start && generatorActive)
        {
            GeneratorOff();
        }
    }

    public void GeneratorOn()
    {
        generatorActive = false;
        stop = false;
        start = true;
        generator.GetComponent<AudioSource>().Play();
        generatorVFX.Play();
    }
    public void GeneratorOff()
    {
        generatorActive = false;
        stop = true;
        start = false;
        generator.GetComponent<AudioSource>().Stop();
        generatorVFX.Stop();
    }
}
