using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDisplays : MonoBehaviour
{
    public LaserControl laserControl;

    public float T = 0;
    public float clampVal;
    public float speed = 0.4f;
    public bool changing;
    public bool reducing;
    public int listIndex;

    [SerializeField]
    MeshRenderer[] renderers = new MeshRenderer[2];
    public Color defColor = Color.red;
    public Color color2 = Color.green;
    private Color rendCol;

    private void Start()
    {
        changing = false;
        reducing = false;

        clampVal = 0;

        laserControl = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LaserControl>();

        rendCol = defColor;
    }
    // Update is called once per frame
    void Update()
    {
        if (changing)
        {
            if (!reducing)
            {
                if (listIndex == 0 || laserControl.displays[listIndex - 1].T > 0.6f)
                    T += speed * Time.deltaTime;
            }
            else
            {
                if (listIndex + 1 == laserControl.displays.Count || laserControl.displays[listIndex + 1].T < 0.75f)
                {
                    T -= speed * 2 * Time.deltaTime;
                }
            }
        }

        if (laserControl.status == 3)
            changing = true;

        if (laserControl.status == 2 && T <= 0.1f)
        {
            changing = false;
            reducing = false;
            clampVal = 0;
            T = 0;
        }


        T = Mathf.Clamp(T, 0, clampVal);
        //Change the color

        rendCol = Color.Lerp(defColor, color2, T);

        renderers[0].material.SetColor("_Color", rendCol);
        renderers[1].material.SetColor("_Color", rendCol);
        renderers[0].material.SetColor("_EmissionColor", rendCol);
        renderers[1].material.SetColor("_EmissionColor", rendCol);
    }
}
