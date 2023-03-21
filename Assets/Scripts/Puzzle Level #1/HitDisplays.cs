using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDisplays : MonoBehaviour
{
    public LaserControl laserControl;

    public float T = 0;
    public float speed = 0.3f;
    public bool changing = false;
    public int listIndex;

    [SerializeField]
    MeshRenderer[] renderers = new MeshRenderer[2];
    public Color defColor = Color.red;
    public Color color2 = Color.green;

    private void Start()
    {
        laserControl = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LaserControl>();

        //Fill up the array
        MeshRenderer ren1 = GetComponent<MeshRenderer>();
        MeshRenderer ren2 = GetComponentInChildren<MeshRenderer>();
        renderers[0] = ren1;
        renderers[1] = ren2;

        //Find which index of the master list this display is
        listIndex = laserControl.displays.IndexOf(this);
    }
    // Update is called once per frame
    void Update()
    {
        if (changing && laserControl.status != 2)
        {
            if ((listIndex != 0 && laserControl.displays[listIndex - 1].T >= 0.5f) || listIndex == 0)
            {
               T += speed * Time.deltaTime;
            }
        }
        else if (changing && laserControl.status == 2)
        {
            if((listIndex != laserControl.displays.Count - 1 && laserControl.displays[listIndex + 1].T <= 0.5f) || listIndex == laserControl.displays.Count - 1)
                T -= speed * Time.deltaTime;
        }

        T = Mathf.Clamp01(T);
        //Change the color
        foreach(MeshRenderer renderer in renderers)
            renderer.material.color = Color.Lerp(defColor, color2, T);
    }
}
