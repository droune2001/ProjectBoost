using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10,10,10);

    //[Range(0, 1)][SerializeField] float movementFactor; // 0 for not moved, 1 for fully moved
    float movementFactor; // 0 for not moved, 1 for fully moved

    [SerializeField] float period = 3.0f;
    
    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon)
            return;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;

        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2.0f;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = 0.5f + 0.5f * rawSinWave;
    }
}
