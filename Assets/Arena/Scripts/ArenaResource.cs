using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaResource : ArenaEntity
{
    float energy = 15;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = ArenaController.controller.RandomPoint();
    }

    public float BeConsumed(ArenaEntity entity)
    {
        Destroy(gameObject);
        ArenaController.controller.CreateResource();
        return energy;
    }

    public override float Evaluate()
    {
        return energy/100.0f;
    }
}
