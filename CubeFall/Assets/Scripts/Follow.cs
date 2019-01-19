using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject objectToFollow;
    private Player _player;

    public float speed = 2.0f;
    private bool _followPlayer = false;

    private void Start()
    {
        _player = GameObject.Find("PlayerCube").GetComponent<Player>();

        StartCoroutine(BeginLevelCoroutine());
    }

    void Update()
    {
        if (_followPlayer)
        {
            CameraFollow();
        }
    }

    private IEnumerator BeginLevelCoroutine()
    {
        yield return new WaitForSeconds(2f);
        _followPlayer = true;

        Invoke("EnablePlayerScript", 2f);
    }

    private void EnablePlayerScript()
    {
        ToggleScript.EnableScript(_player); // enables player controls after camera has moved to position
    }

    private void CameraFollow()
    {
        float interpolation = speed * Time.deltaTime;
        float offset = 5f;

        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, objectToFollow.transform.position.y + offset, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, interpolation);

        this.transform.position = position;
    }
}
