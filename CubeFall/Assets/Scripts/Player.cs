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



    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float cubeRotation = 100f;

    // Will determine if the player can rotate left or right
    enum Rotation { Left, Right, noRotation };
    Rotation rotationState;

    enum State { Alive, Dying, lvlSwitching };
    State playerState = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
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
            Jumping();
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

    private void Jumping() // TODO make better Jump - longer spacebar held the higher the jump
    {
        float jumpSpeedOnFrame = _jumpForce * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidBody.velocity = new Vector3(0f, 0f, 0f);          
            audioSource.PlayOneShot(jumpSound);
            rigidBody.AddRelativeForce(Vector3.up * jumpSpeedOnFrame, ForceMode.VelocityChange); // VelocityChange so the force ignores mass
        }
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
        Invoke("LoadNextLevel", 3f); // Invoke is a lesser form of CoRoutines
    }

    private void Die()
    {
        Debug.Log("Dead"); // TODO kill player
        playerState = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound); // TODO predeath sound needs to play before this       
        Invoke("LoadFirstLevel", 2f);
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
