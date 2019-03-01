using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody),typeof(AudioSource))]
public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody = null;
    AudioSource audioSource = null;

    [SerializeField] float booster = 1000.0f;
    [SerializeField] float thrust = 100.0f;
    [SerializeField] float levelLoadDelay = 2.0f;
    [SerializeField] AudioClip mainEngineSound = null;
    [SerializeField] AudioClip victorySound = null;
    [SerializeField] AudioClip deathSound = null;

    [SerializeField] ParticleSystem mainEngineParticles = null;
    [SerializeField] ParticleSystem victoryParticles = null;
    [SerializeField] ParticleSystem deathParticles = null;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // ignore collisions when dead.
        if (state != State.Alive || collisionsDisabled)
            return;

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;

            case "Fuel":
                break;

            case "Finish":
                StartVictorySequence();
                break;

            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        StopSound();
        PlayDeathSound();
        StopMainEngineParticles();
        deathParticles.Play();
        Invoke("ReloadLevel", levelLoadDelay);
    }

    private void StopMainEngineParticles()
    {
        if (mainEngineParticles.IsAlive())
            mainEngineParticles.Stop();
    }

    private void StartVictorySequence()
    {
        state = State.Transcending;
        StopSound();
        PlayVictorySound();
        victoryParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay); // co-routine
    }

    private void ReloadLevel()
    {
        state = State.Alive;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextLevel()
    {
        state = State.Alive;
        var nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ApplyThrust();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayThrustSound();
            mainEngineParticles.Play();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            StopSound();
            StopMainEngineParticles();
        }
    }

    private void StopSound()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    private void PlayThrustSound()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(mainEngineSound);
    }

    private void PlayVictorySound()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(victorySound);
    }

    private void PlayDeathSound()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(deathSound);
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * booster * Time.deltaTime);
    }

    private void RespondToRotateInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(thrust);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-thrust);
        }
    }

    private void RotateManually(float amountOfThrust)
    {
        rigidBody.freezeRotation = true; // take manual control of the rotation
        transform.Rotate(Vector3.forward * amountOfThrust * Time.deltaTime);
        rigidBody.freezeRotation = false; // give back control. This way we avoid spin with force greater than wee can control.
    }
}
