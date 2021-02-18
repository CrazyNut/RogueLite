using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class tmpScript : MonoBehaviour
{
    public MapGenerator generator;
    public Vector2 startPoint;
    public Vector2 center;
    public bool draw;
    public float angle;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (draw)
        {
            angle = (angle) * (Mathf.PI / 180);
            float s = Mathf.Sin(angle);
            float c = Mathf.Cos(angle);

            // translate point back to origin:
            startPoint.x -= center.x;
            startPoint.y -= center.y;

            // rotate point
            float xnew = startPoint.x * c + startPoint.y * s;
            float ynew = -startPoint.x * s + startPoint.y * c;

            // translate point back:
            startPoint.x = xnew + center.x;
            startPoint.y = ynew + center.y;
            draw = false;
        }
    }
}
