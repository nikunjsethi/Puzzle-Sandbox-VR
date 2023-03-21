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


    //Failure Amount For Cues
    public int failAmount = 0;


    // Start is called before the first frame update
    void Start()
    {
        //Find Laser Controller
        laserControl = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LaserControl>();

        //Find Colors
        Transform[] children = new Transform[2];
        children = GetComponentsInChildren<Transform>();

        if (!children[0].CompareTag("Symbol0"))
            System.Array.Reverse(children);

        colors.Add(children[0].GetComponent<Renderer>().material.color);
        colors.Add(children[1].GetComponent<Renderer>().material.color);

        renderers.Add(children[0].GetComponent<Renderer>());
        renderers.Add(children[1].GetComponent<Renderer>());
    }

    private void OnTriggerEnter(Collider other)
    {
        //Tag For hands?
        if (other.CompareTag("Player") && !rotating)
        {
            targetAngle += 180;
            rotating = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        laserState = laserControl.status;
        modulo = Mathf.Abs(Mathf.Sin(Time.time) * 10);

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
        }

        //Rotation
        if (rotating)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 0, angle);

            if (angle >= targetAngle)
            {
                if (targetAngle == 360f)
                    targetAngle = 0;

                angle = targetAngle;
                //Add bit of delay???
                rotating = false;
            }
        }
        else
        {
            if (targetAngle == 360f)
            {
                targetAngle = 0;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        

    }
}
