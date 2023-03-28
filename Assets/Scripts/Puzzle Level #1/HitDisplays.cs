using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDisplays : MonoBehaviour
{
    //public LaserControl laserControl;

    public float T = 0;
    public float speed = 0.3f;
    public bool changing = false;
    public int listIndex;

    [SerializeField]
    MeshRenderer[] renderers = new MeshRenderer[2];
    public Color defColor = Color.red;
    public Color color2 = Color.green;
    private Color rendCol;

    private void Start()
    {
        //laserControl = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LaserControl>();

        //Find which index of the master list this display is
        //listIndex = laserControl.displays.IndexOf(this);

        rendCol = defColor;
    }
    // Update is called once per frame
    void Update()
    {
        if (changing)
        {
            rendCol = color2;
        }
        else
        {
            rendCol = defColor;
        }
            

        //T = Mathf.Clamp01(T);
        //Change the color

        //Color color = Color.Lerp(defColor, color2, T);

        renderers[0].material.SetColor("_Color", rendCol);
        renderers[1].material.SetColor("_Color", rendCol);
    }
}
