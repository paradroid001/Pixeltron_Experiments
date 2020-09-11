using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Blob : MonoBehaviour
{
    public GameObject blobPrefab;
    public int maxIterations = 255;
    public float maxRadius = 2.0f;
    public Gradient blobColours;

    Texture2D texPosScale;
    Texture2D texColour;
    VisualEffect visualEffect;
    uint resolution = 4096; //res of the textures
    public float particleSize = 0.1f;
    bool toUpdate = false;
    uint particleCount = 0; //set by code



    // Start is called before the first frame update
    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
        Cursor.lockState = CursorLockMode.Locked;      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int res = 100; //100=1million 200 = 8million 500=125 million
            Debug.Log("Calculating..." + res*res*res);
            float range = 2.0f;
            particleSize = 2*range/res;
            Draw( Vector3.zero, new Vector3(range, range, range), res, res, res);
        }

        if (toUpdate)
        {
            toUpdate = false;
            visualEffect.Reinit(); //clear and start again
            visualEffect.SetUInt(Shader.PropertyToID("ParticleCount"), particleCount);
            visualEffect.SetTexture(Shader.PropertyToID("TexColour"), texColour);
            visualEffect.SetTexture(Shader.PropertyToID("TexPosScale"), texPosScale);
            visualEffect.SetUInt(Shader.PropertyToID("Resolution"), resolution);


        }
    }

    public void SetParticles(Vector3[] positions, Color[] colours)
    {
        int texWidth = positions.Length > (int)resolution ? (int)resolution : positions.Length;
        int texHeight = Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution);
        texColour = new Texture2D(texWidth, texHeight, TextureFormat.RGBAFloat, false);
        texPosScale = new Texture2D(texWidth, texHeight, TextureFormat.RGBAFloat, false);
        
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int index = x + y * texWidth;
                texColour.SetPixel(x, y, colours[index]);
                Color data = new Color(positions[index].x, positions[index].y, positions[index].z, particleSize);
                texPosScale.SetPixel(x, y, data);
            }
        }
        texColour.Apply();
        texPosScale.Apply();

        particleCount = (uint)positions.Length;
        toUpdate = true;
    }

    Vector3 Square3D(Vector3 prev)
    {
        float xx = prev.x * prev.x;
        float xy = prev.x * prev.y;
        float xz = prev.x * prev.z;
        float yy = prev.y * prev.y;
        float yz = prev.y * prev.z;
        float zz = prev.z * prev.z;

        return new Vector3(xx + yy, 2*xy + zz, 2*xz + 2*yz);
    }

    int CountSquares3D(Vector3 point)
    {
        int count = 0;
        Vector3 s = Vector3.zero;
        while (count < maxIterations && s.magnitude <= maxRadius)
        {
            count += 1;
            s = Square3D(s) + point;
            //Debug.Log(s);
        }
        if (count >= maxIterations)
        {
            count = 0; //0 means it's in the set, should be blank.
        }
        return count;
    }

    void Draw(Vector3 centre, Vector3 range, int width, int height, int depth)
    {
        Vector3[] positions = new Vector3[width*height*depth];
        Color[] colours = new Color[width*height*depth];
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                for (int d = 0; d < depth; d++)
                {
                    Vector3 offset = new Vector3(w * range.x/width,
                                                 h * range.y/height,
                                                 d * range.z/depth);
                    Vector3 testpoint = centre + 2*offset - range;

                    int iters = CountSquares3D(testpoint);
                    if (iters != 0) //0 means in the set.
                    {
                        //Debug.Log(iters);
                        //mark points not in set.
                        //GameObject blob = Instantiate(blobPrefab, testpoint, Quaternion.identity);
                        //float c = iters * 1.0f/maxIterations;
                        //blob.GetComponent<Renderer>().material.color = blobColours.Evaluate( (iters % 50 ) / 50.0f );
                        int index = w * height * depth + h * depth + d;
                        positions[index] = testpoint;
                        colours[index] = blobColours.Evaluate( (iters % 50 ) / 50.0f );
                    }
                }
            }
        }
        SetParticles(positions, colours);
    }

}
