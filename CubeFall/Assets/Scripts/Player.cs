using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody rigidBody;

    AudioSource audioSource;

    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip preDeathSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip beatLevelSound;

    [SerializeField] ParticleSystem jumpParticles;
    [SerializeField] ParticleSystem victoryParticles;
    [SerializeField] ParticleSystem deathParticles;


    float jumpSpeedOnFrame;
    private float initialJumpForce;
   [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float cubeRotation = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    // Will determine if the player can rotate left or right
    enum Rotation { Left, Right, noRotation };
    Rotation rotationState;

    enum State { Alive, Dying, lvlSwitching };
    State playerState = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        initialJumpForce = _jumpForce;
        audioSource = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();
        rotationState = Rotation.noRotation;     
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == State.Alive)
        {
            Rotate();
            RespondToJumpInput();
        }

    }

    private void Rotate()
    {
        float rotationOnFrame = cubeRotation * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) && (rotationState != Rotation.Right))
        {
            rotationState = Rotation.Left;
            transform.Rotate(Vector3.forward * rotationOnFrame, Space.World);
        }
        else if (Input.GetKey(KeyCode.D) && (rotationState != Rotation.Left))
        {
            rotationState = Rotation.Right;
            transform.Rotate(Vector3.back * rotationOnFrame, Space.World);
        }
        else
        {
            rotationState = Rotation.noRotation;
        }
    }

    private void RespondToJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidBody.velocity = new Vector3(0f, 0f, 0f);
            PlayJumpSound();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            _jumpForce -= 10f;
            Jump();
        }
        else
        {
            _jumpForce = initialJumpForce;
            jumpParticles.Stop();
        }

    }

    private void PlayJumpSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    private void Jump()
    {
        if (_jumpForce <= 0f)
        {
            _jumpForce = 0f;
        }

        jumpSpeedOnFrame = _jumpForce * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * jumpSpeedOnFrame, ForceMode.VelocityChange); // VelocityChange so the force ignores mass
       
        jumpParticles.Play();
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (playerState != State.Alive) { return; } // ignore collisons on death
 
        switch (other.gameObject.tag)
        {
            case "Friendly":
                break;

            case "Finish":
                Win();
                break;

            default:
                Die();
                break;
        }
    }

    private void Win()
    {
        playerState = State.lvlSwitching;
        audioSource.Stop();
        audioSource.PlayOneShot(beatLevelSound);
        victoryParticles.Play();
        
        Invoke("LoadNextLevel", levelLoadDelay); // Invoke is a lesser form of CoRoutines
    }

    private void Die() //TODO stop jump particle from playing after hitting walls
    {
        Debug.Log("Dead");
        playerState = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound); // TODO predeath sound needs to play before this       
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); // TODO allow for more than 2 levels
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }
}
