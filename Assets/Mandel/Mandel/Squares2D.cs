using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squares2D : MonoBehaviour
{
    public int maxCount = 20; //for the drawing of 'points'
    public float radius = 2.0f; //anything that gets outside 2 is too big.
    public GameObject pointPrefab;
    List<GameObject> points; //for points drawing.
    Vector2 previousMousePosition;
    

    // Start is called before the first frame update
    void Start()
    {
        points = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 m = Input.mousePosition;
        if (m!= previousMousePosition)
        {
            previousMousePosition = m;
            Vector3 p = Camera.main.ScreenToWorldPoint(m);
            Debug.Log(p);
            DrawSquares(p);
        }
    }

    public Vector2 Square(Vector2 num)
    {
        float xx = num.x * num.x;
        float xy = num.x * num.y;
        float yx = num.y * num.x;
        float yy = num.y * num.y;

        return new Vector2(xx - yy, xy + yx);
    }

    void ClearPoints()
    {
        foreach (GameObject g in points)
        {
            Destroy(g);
        }
        points.Clear();
    }

    void DrawSquares(Vector2 s)
    {
        int count = 0;
        
        ClearPoints();
        while (count < maxCount && s.magnitude <= radius)
        {
            count += 1;
            points.Add(Instantiate(pointPrefab, s, Quaternion.identity));
            s = Square(s);
        }    
    }
}
