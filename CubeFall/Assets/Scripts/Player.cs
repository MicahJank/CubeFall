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

    private Vector2 touchOrigin = -Vector2.one; // For touch controls

    // Will determine if the player can rotate left or right
    enum Rotation { Left, Right, noRotation };
    Rotation rotationState;

    bool isTransitioning = false;

    private bool _debugMode = false;

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
        if (!isTransitioning)
        {
            Rotate();
            RespondToJumpInput();
        }
        if (Debug.isDebugBuild) // checks if the final build of the game is a development build or not
        {
            RunDebugMode();
        }
    }

    private void RunDebugMode()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        } else if (Input.GetKeyDown(KeyCode.C))
        {
            _debugMode = !_debugMode; // toggle the boolean
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
        if (isTransitioning || _debugMode == true) { return; } // ignore collisons on death
 
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
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(beatLevelSound);
        victoryParticles.Play();
        
        Invoke("LoadNextLevel", levelLoadDelay); // Invoke is a lesser form of CoRoutines
    }

    private void Die()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        BeginDeathParticles();
        Invoke("ReloadCurrentLevel", levelLoadDelay);
    }

    private void BeginDeathParticles()
    {
        jumpParticles.Stop();
        deathParticles.Play();
    }

    private void LoadNextLevel()
    {
        int totalSceneIndex = SceneManager.sceneCountInBuildSettings; // total scene index will be equal to last level in game
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex == totalSceneIndex)
        {
            nextSceneIndex = 0; // loop back to level 1 when you reach the end.
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void ReloadCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
