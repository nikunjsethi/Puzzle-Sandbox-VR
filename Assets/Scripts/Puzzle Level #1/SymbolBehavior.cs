using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolBehavior : MonoBehaviour
{
    private LaserControl laserControl;
    private int laserState;
    //Colors
    private Color oColorDef;
    private Color oColor;
    private Color xColorDef;
    private Color xColor;
    private List<Color> colors = new List<Color>();
    private List<Renderer> renderers = new List<Renderer>();
    private float modulo;
    private float modulo2;

    //Turning
    private bool rotating = false;
    public float rotSpeed = 30f;
    private float targetAngle = 0;
    public float rotationSpeed = 100f;  // Rotation speed in degrees per second
    public float accelerationTime = 1f; // Time to accelerate and decelerate in seconds

    private Quaternion startRotation;   // Starting rotation of the object
    private Quaternion targetRotation;  // Target rotation of the object
    private float currentRotationTime;  // Current rotation time in seconds
    private float rotationProgress;     // Rotation progress as a fraction (0 to 1)
    private float resetTime = 1f;
    private float currentResetTime = 0;


    //Failure Amount For Cues
    public int failAmount = 0;


    // Start is called before the first frame update
    void Start()
    {
        //Find Laser Controller
        laserControl = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LaserControl>();

        //Find Colors
        Transform[] children = GetComponentsInChildren<Transform>();

        if (!children[0].CompareTag("Symbol0"))
            System.Array.Reverse(children);

        colors.Add(children[0].GetComponent<Renderer>().material.color);
        colors.Add(children[1].GetComponent<Renderer>().material.color);

        renderers.Add(children[0].GetComponent<Renderer>());
        renderers.Add(children[1].GetComponent<Renderer>());

        // Store the starting rotation of the object
        startRotation = transform.localRotation;

        Vector3 currentEuler = startRotation.eulerAngles;
        currentEuler.x += 180f;
        targetRotation = Quaternion.Euler(currentEuler);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Tag For hands?
        if (other.CompareTag("Player") && !rotating)
        {
            //targetRotation.x += 180;
            rotating = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        laserState = laserControl.status;
        modulo = Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * Time.time) * 10);
        /*
        //Setting the color variation cues
        //If the symbol number matches the laserstate, I am modulating the emission intensity with the sin modulo
        foreach (Renderer renderer in renderers)
        {
            int indexOfRend = renderers.IndexOf(renderer);

            switch (laserState)
            {
                case < 2 when renderer.CompareTag("Symbol" + laserState) && failAmount < 2:
                    renderer.material.SetColor("_EmissionColor", colors[laserState] * modulo);
                    break;
                case < 2 when renderer.CompareTag("Symbol" + laserState) && failAmount >= 2:
                    renderer.material.SetColor("_EmissionColor", colors[laserState] * modulo * 4);
                    break;
                case < 2 when !renderer.CompareTag("Symbol" + laserState) && failAmount < 2:
                    renderer.material.SetColor("_EmissionColor", colors[Mathf.Abs(laserState - 1)] * 0.5f);
                    break;
                case < 2 when !renderer.CompareTag("Symbol" + laserState) && failAmount >= 2:
                    renderer.material.SetColor("_EmissionColor", colors[Mathf.Abs(laserState - 1)] * modulo * 2);
                    break;
                case >= 2:
                    renderer.material.SetColor("_EmissionColor", colors[Mathf.Abs(indexOfRend)] * 0);
                    break;
            }
        }*/

        //Rotation
        if (rotating)
        {
            // Increment the current rotation time
            currentRotationTime += Time.deltaTime;

            // Calculate the rotation progress as a fraction (0 to 1)
            rotationProgress = Mathf.Clamp01(currentRotationTime / (2f * accelerationTime));

            // Use a Lerp function to smoothly interpolate between the start and target rotations
            Quaternion newRotation = Quaternion.Lerp(startRotation, targetRotation, rotationProgress);

            // Apply the new rotation to the object's transform
            transform.localRotation = newRotation;

            if (newRotation == targetRotation)
            {
                    startRotation = targetRotation;
                    rotationProgress = 0;
                    Vector3 currentEuler = targetRotation.eulerAngles;
                    currentEuler.x += 180f;
                    targetRotation = Quaternion.Euler(currentEuler);
                    rotating = false;
                    
            }
        }
        

    }
}
