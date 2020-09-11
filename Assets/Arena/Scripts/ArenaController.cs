using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
    public int entityCount = 30;
    public int resourceCount = 30;
    public Vector2 arenaSize;
    public ArenaEntity entityPrefab;
    public ArenaResource resourcePrefab;
    public static ArenaController controller;

    ArenaEntity[] entities; 

    bool pauseSim = false;

    void Awake()
    {
        CreateEntities();
        controller = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            pauseSim = !pauseSim;
        }
        
        foreach (ArenaEntity entity in entities)
        {
            entity.EnableUI(false);
            if (!pauseSim)
                entity.UpdateEntity();
        }
        Collider2D[] colliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        foreach (Collider2D c in colliders)
        {
            ArenaEntity entity = c.GetComponent<ArenaEntity>();
            entity.EnableUI(true);
        }
    }

    public Vector2 RandomPoint()
    {
        float x = (arenaSize.x * Random.value) - arenaSize.x / 2;
        float y = (arenaSize.y * Random.value) - arenaSize.y / 2;
        return new Vector2(x, y);
    }

    void CreateEntities()
    {
        for (int i = 0; i < resourceCount; i++)
        {
            CreateResource();
        }

        entities = new ArenaEntity[entityCount];
        for (int i = 0; i < entities.Length; i++)
        {
            entities[i] = CreateEntity();
        }
    }

    public ArenaEntity CreateEntity()
    {
        return Instantiate(entityPrefab);
    }

    public ArenaResource CreateResource()
    {
        return Instantiate(resourcePrefab);
    }
}
