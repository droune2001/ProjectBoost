using UnityEngine;

[DisallowMultipleComponent]
public class Rotator : MonoBehaviour
{
    enum PlaneOfRotation { XY, YZ, ZX };
    [SerializeField] PlaneOfRotation plane = PlaneOfRotation.YZ;
    [SerializeField] float period = 3.0f;
    [SerializeField] float radius = 3.0f;
    [SerializeField] float offsetPercent = 0.0f;

    Vector3 startingPos;
    Vector3 startingEuler;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        startingEuler = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon)
            return;

        switch(plane)
        {
            case PlaneOfRotation.XY:
            {
                float globalOffset = startingEuler.z / 360.0f;
                float cycles = globalOffset + offsetPercent + (Time.time / period);
                const float tau = Mathf.PI * 2.0f;
                float s = Mathf.Sin(cycles * tau);
                float c = Mathf.Cos(cycles * tau);
                Vector3 p = new Vector3(c, s, 0);
                    transform.position = startingPos + radius * p;
                    transform.rotation = Quaternion.Euler(0, 0, cycles * 360.0f);
                    break;
            }

            case PlaneOfRotation.YZ:
            {
                float globalOffset = startingEuler.x / 360.0f;
                float cycles = globalOffset + offsetPercent + (Time.time / period);
                const float tau = Mathf.PI * 2.0f;
                float s = Mathf.Sin(cycles * tau);
                float c = Mathf.Cos(cycles * tau);
                Vector3 p = new Vector3(0, s, c);
                transform.position = startingPos + radius * p;
                transform.rotation = Quaternion.Euler(-cycles * 360.0f, 0, 0);
                break;
            }

            case PlaneOfRotation.ZX:
            {
                float globalOffset = startingEuler.y / 360.0f;
                float cycles = globalOffset + offsetPercent + (Time.time / period);
                const float tau = Mathf.PI * 2.0f;
                float s = Mathf.Sin(cycles * tau);
                float c = Mathf.Cos(cycles * tau);
                Vector3 p = new Vector3(c, 0, s);
                transform.position = startingPos + radius * p;
                transform.rotation = Quaternion.Euler(0, -cycles * 360.0f, 0);
                break;
            }
        }

        
        
    }
}
