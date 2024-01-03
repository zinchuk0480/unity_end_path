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
    public AudioSource generatorAudio;
    public VisualEffect generatorVFX;


    public AudioClip buttonClick;


    private void Start()
    {
        generatorAudio = generator.GetComponent<AudioSource>();
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
            generatorAudio.PlayOneShot(buttonClick, 0.5f);
        }
    }

    public void GeneratorOn()
    {
        generatorActive = false;
        stop = false;
        start = true;
        generatorAudio.PlayOneShot(buttonClick, 0.5f);
        generatorAudio.Play();
        generatorVFX.Play();
    }
    public void GeneratorOff()
    {
        generatorActive = false;
        stop = true;
        start = false;
        generatorAudio.Stop();
        generatorVFX.Stop();
    }
}
