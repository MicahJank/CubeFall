using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(7f, 0f, 0f);
    [SerializeField] float period = 2f; // Higher the number, slower the oscillation

    Vector3 startingPos;
    Vector3 offset;

    float movementFactor; // 0 for not moved, 1 for fully moved

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period > 0)
        {
            Oscillate();
        }
        else
        {
            //Debug.LogError("PERIOD CANNOT BE EQUAL TO ZERO.");
            throw new System.Exception("PERIOD CANNOT BE EQUAL TO ZERO");
        }
    }

    private void Oscillate()
    {
        float cycles = Time.time / period; // grows continually from 0

        const float tau = Mathf.PI * 2f; // just a number, about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to 1
        
        movementFactor = rawSinWave;
        offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
