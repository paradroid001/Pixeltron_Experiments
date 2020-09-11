using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaNavigator : ArenaEntity
{
    
    public float minDist = 0.05f;

    int STR; //strength
    int DEX; //dex
    int INT; //intel
    int HP;  //health
    int CON; //const
    int TO;  //toughness

    int hpMax;
    float speed;
    float perception;
    float attackPower;
    float defence;
    float attackTime;
    float blockChance;

    ArenaEntity currentTarget;
    ArenaEntity previousTarget;
    Vector2 currentDestination;
    Vector2 previousDestination;

    bool alive = true;

    float timeBetweenDecisions;
    float decisionTimer;

    ArenaEntity[] neighbours;
    
    Material material;



    // Start is called before the first frame update
    void Start()
    {
        STR = 10;
        DEX = 10;
        INT = 20;
        CON = 10;
        TO = 10;
        HP = 3 * STR + 3 * CON;

        hpMax = HP;
        speed = DEX / 5.0f; //speed you move
        perception = INT / 2.0f; //range of scans
        timeBetweenDecisions = 1.0f / INT;

        transform.position = ArenaController.controller.RandomPoint();

        neighbours = new ArenaEntity[INT/3]; //we can only see this many neighbours.

        material = GetComponent<SpriteRenderer>().material;
    }

    public override void UpdateEntity()
    {
        if (alive)
        {
            decisionTimer += Time.deltaTime;
            if (decisionTimer > timeBetweenDecisions)
            {
                decisionTimer = 0;
                Think();
            }
            
        }
    }

    void LateUpdate()
    {
        if (!alive)
        {
            Die();
        }
        else if (uiEnabled)
        {
            DrawUI();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Resource")
        {
            ArenaResource r = col.GetComponent<ArenaResource>();
            Consume(r);
        }
        else if (col.tag == "Navigators")
        {
            ArenaEntity e = col.GetComponent<ArenaEntity>();
            Fight((ArenaNavigator)e);
        }
    }

    protected override void MoveTowardsCurrentDestination()
    {
        Vector3 delta = currentDestination - (Vector2)transform.position;

        transform.position += delta.normalized * speed * Time.deltaTime;
    }

    void MoveInDirection(Vector2 dir)
    {
        transform.position += (Vector3)dir * speed * Time.deltaTime;
    }

    void Think()
    {
        //If we have no destination.
        //if (currentDestination == Vector2.zero)
        //{
            ClearNeighbours();
            ScanNearbyArea();
            Vector2 dir = CalculateNewDirection();
            MoveInDirection(dir);
            
        //}

        /*
        else //we do have a destination
        {
            //Have we reached it?
            if (Vector2.Distance((Vector2)transform.position, currentDestination) > minDist )
            {
                MoveInCurrentDirection();
            }
            else //we now have no destination
            {
                currentDestination = Vector2.zero;
            }
        }
        */
    }

    void ScanNearbyArea()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, perception);
        
        foreach (Collider2D c in colliders)
        {
            if (c.tag == "Navigators" || c.tag == "Resources" && (c.gameObject != gameObject) )
            {
                ArenaEntity entity = c.GetComponent<ArenaEntity>();
                float value = Assess(entity);

                for (int i = 0; i < neighbours.Length; i++)
                {
                    if ( (neighbours[i] == null || Assess(neighbours[i]) < value) && entity != this)
                    {
                        neighbours[i] = entity;
                        break;
                    }
                }
            }

            /*
            else if (c.tag == "Resource")
            {
                //work out if we want to go for this resource.
                ArenaResource resource = c.GetComponent<ArenaResource>();
                float reward = resource.AssessValue();
                for (int i = 0; i < rewards.Length; i++)
                {
                    if (rewards[i] == null || rewards[i].AssessValue() < value)
                    {
                        rewards[i] = resource;
                        break;
                    }
                }
                
            }
            */
        }
    }

    float Assess(ArenaEntity entity)
    {
        //get value of entity
        float value = entity.Evaluate();

        //So here is where we involve some smarts about how
        //we behave.
        //Attack: if we are on high health
        //Heal: if we are on medium health
        //Low health: look for heals and go AWAY from threats.

        float healthPercent = HP/(float)hpMax * 100.0f;
        float attackWeighting = 1.0f;
        float healWeighting = 1.0f;

        if (healthPercent < 50)
        {
            attackWeighting = 0.5f;
        }
        else if (healthPercent < 25)
        {
            attackWeighting = -1.0f;
        }

        if (entity.tag == "Resources")
        {
            value *= healWeighting;
        }
        else if (entity.tag == "Navigators")
        {
            value *= attackWeighting;
        }

        //modulate by distance
        return value * (1.0f - (Distance(entity) / perception ) );
    }

    public override float Evaluate()
    {
        return 200.0f / (INT + STR + DEX + TO + HP + CON);
    }

    public void Heal(int amount)
    {
        HP += amount;
        if ( HP > hpMax )
        {
            HP = hpMax;
        }
        UpdateVisual();
    }

    Vector2 CalculateNewDirection()
    {
        Vector2 ret;

        if (neighbours.Length > 0)
        {
            //For each neighbour:
            //how much do we want to get closer to the neighbour?
            Vector2 sum = Vector2.zero;
            for (int i = 0; i < neighbours.Length; i++)
            {
                ArenaEntity e = neighbours[i];
                if (e != null)
                {
                    sum += VectorTo(e) * Assess(e); 
                }
            }
            ret = sum;
        }
        else
        {
            //if (threats[0] != null)
            //{
            //    currentDestination = threats[0].transform.position;
            //}
            //else
            //{
                ret = ArenaController.controller.RandomPoint();
            //}
        }
        return ret.normalized;
    }


    void Consume(ArenaResource resource)
    {
        float energy = resource.BeConsumed(this);
        Heal((int)energy);
    }

    public void Fight(ArenaNavigator entity)
    {
        entity.TakeDamage( STR, this);
    }

    public void TakeDamage( int amount, ArenaEntity damager)
    {
        HP -= amount;
        if (HP <= 0)
        {
            alive = false;
            Debug.Log("Killed");
        }
        UpdateVisual();
    }

    protected override void UpdateVisual()
    {
        Color c = material.color;
        c.g = HP * 1.0f / hpMax;
        c.r = 1.0f - c.g;
        c.b = 0;
        material.color = c;
    }

    void ClearNeighbours()
    {
        for (int i = 0; i < neighbours.Length; i++)
        {
            neighbours[i] = null;
        }
    }

    public override void DrawUI()
    {
        foreach (ArenaEntity entity in neighbours)
        {
            if (entity != null)
            {
                Debug.DrawRay(transform.position, VectorTo(entity), Color.red);
            }
        }
    }
}
