using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mandel2D : MonoBehaviour
{
    public Squares2D squares;
    public int maxIterations = 50; //for mandelbrot
    public Renderer renderer;
    Texture2D tex;

    private Vector2 pos;
    private float scale;
    
    void Awake()
    {
        tex = new Texture2D(Screen.width, Screen.height);
        tex.filterMode = FilterMode.Point;
        renderer.material.mainTexture = tex;
        Setup();
    }

    // Start is called before the first frame update
    void Start()
    {
        pos = Vector2.zero;
        scale = 0.8f;
        Redraw();
    }

    // Update is called once per frame
    void Update()
    {
        bool change = false;
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            scale *= 0.8f;
            change = true;
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            scale /= 0.8f;
            change = true;
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h !=0 || v != 0)
        {
            pos += new Vector2(h * scale, v * scale);
            change = true;   
        }

        if (Input.GetMouseButtonDown(0))
        {
            scale *= 0.8f;
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) * scale;
            change = true;
        }

        if (change)
        {
            Redraw();
        }
    }

    void Redraw()
    {
        DrawMandel(pos, scale);
    }

    void Setup()
    {
        Camera cam = Camera.main;
        Mesh mesh = renderer.GetComponent<MeshFilter>().mesh;
        Vector3 v0 = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 v1 = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)); 
        Vector3 v2 = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        Vector3 v3 = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(v0.x, v0.y, 0),
            new Vector3(v1.x, v1.y, 0),
            new Vector3(v2.x, v2.y, 0),
            new Vector3(v3.x, v3.y, 0)
             
        };
        mesh.vertices = vertices;
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;
    }


    void DrawMandel(Vector2 pos, float scale)
    {
        
        Vector2 screenBL = Camera.main.ScreenToWorldPoint(Vector2.zero); //bottom left.
        Vector2 screenTR = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height) );
        
        Vector2 posstart = screenBL * scale + pos;
        Vector2 posend = screenTR * scale + pos;
        
        //Debug.Log("From " + posstart + " to " + posend + " with increments of (" + dx + ", " + dy + 
        //") results in iterations: " + iterx + ", " + itery);
        float dx = (posend.x - posstart.x)/Screen.width;
        float dy = (posend.y - posstart.y)/Screen.height;
        
        for (int i = 0; i < Screen.width; i+=1)
        {
            for (int j = 0; j < Screen.height; j+=1)
            {
                Vector2 currentPos = new Vector2(posstart.x + (dx*i), posstart.y + (dy*j) );
                //Vector2 currentPos = Camera.main.ScreenToWorldPoint(new Vector2(i, j)) * 0.2f;
                
                int iters = CountSquares(currentPos);
                if (iters == 0)
                {
                    tex.SetPixel(i, j, Color.blue);
                }
                else
                {
                    float c = iters * 1.0f / maxIterations;
                    tex.SetPixel(i, j, new Color(c*currentPos.x, c*currentPos.y, c*scale) );
                }
            }
        }
        tex.Apply();
        Debug.Log("Done " + Time.time );
    }

    //return 0 if you were stable.
    //otherwise, return the number of iters before you did.
    int CountSquares(Vector2 c)
    {
        int count = 0;
        Vector2 s = Vector2.zero;
        while (count < maxIterations && s.magnitude <= squares.radius)
        {
            count += 1;
            //points.Add(Instantiate(pointPrefab, s, Quaternion.identity));
            s = squares.Square(s) + c;
        }

        if (count == maxIterations)
            count = 0;
        return count;    
    }

    
}
