using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaEntity : MonoBehaviour
{
    
    protected bool uiEnabled = false;

    protected virtual void MoveTowardsCurrentDestination()
    {

    }

    protected virtual void UpdateVisual()
    {

    }

    public virtual void UpdateEntity()
    {

    }

    public virtual float Evaluate()
    {
        return Random.value;
    }

    public virtual float Distance(ArenaEntity entity)
    {
        return Vector2.Distance(transform.position, entity.transform.position);
    }

    public virtual Vector2 VectorTo(ArenaEntity entity)
    {
        return entity.transform.position - transform.position;
    }


    public virtual void Die()
    {
        if (gameObject != null)
            Destroy(gameObject);
    }

    public virtual void EnableUI(bool enable)
    {
        uiEnabled = enable;
    }

    public virtual void DrawUI()
    {

    }  
}
