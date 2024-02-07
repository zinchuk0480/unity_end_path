using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Generator : MonoBehaviour
{
    private GameObject gameManager;
    private GameManager gameManagerScript;

    public bool generatorActive = false;
    public bool start = false;
    public bool stop = true;

    public GameObject generator;
    public AudioSource generatorAudio;
    public VisualEffect generatorVFX;
    public VisualEffect generatorRotateVFX;
    public GameObject containerParticleGenerator;
    private ParticleSystem particleGeneratorStart;

    public AudioClip buttonClick;


    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();

        generatorAudio = generator.GetComponent<AudioSource>();
        generatorVFX = generator.GetComponent<VisualEffect>();
        generatorRotateVFX = GameObject.Find("VFX_Rotate").GetComponent<VisualEffect>();

        generatorVFX.Stop();
        generatorRotateVFX.Stop();

        containerParticleGenerator = GameObject.Find("ParticleGeneratorStart");
        particleGeneratorStart = containerParticleGenerator.GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!gameManagerScript.generatorBroke && gameManagerScript.alarm)
        {
            GeneratorOff();
            gameManagerScript.generatorBroke = true;
        }
        if (!gameManagerScript.generatorBroke && start && generatorActive)
        {
            GeneratorOff();
        }
        if (!gameManagerScript.generatorBroke && stop && generatorActive)
        {
            GeneratorOn();
        }
        if (generatorActive && gameManagerScript.alarm)
        {
            generatorAudio.PlayOneShot(buttonClick, 0.5f);
            generatorActive = false;
        }
    }

    public void GeneratorOn()
    {
        generatorActive = false;
        stop = false;
        start = true;
        generatorAudio.Play();
        generatorVFX.Play();
        generatorRotateVFX.Play();
        particleGeneratorStart.Play();
        generatorAudio.PlayOneShot(buttonClick, 0.5f);
    }
    public void GeneratorOff()
    {
        generatorActive = false;
        stop = true;
        start = false;
        generatorAudio.Stop();
        generatorVFX.Stop();
        generatorRotateVFX.Stop();
        particleGeneratorStart.Play();
        generatorAudio.PlayOneShot(buttonClick, 0.5f);
    }
}
