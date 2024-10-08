using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using BehaviourTree;

public class PredatorBT : BehaviourTree.Tree
{
    private NavMeshAgent agent;
    private WorldState worldState;

    //Predator Stats
    public int fovRange = 5;
    public int moveSpeed = 8;
    public int hungerTolerance = 20;

    public float heightOffset = 1f;
    public bool reproduce = false;
    public float reproduceTimer;
    public PredatorBT partner;

    private int numEaten = 0;

    //Agent Needs
    private int hunger;
    private int thirst;

    //List of resources
    public List<Resource> water;

    //Create a random
    private System.Random rand = new System.Random();

    private void Awake()
    {
        worldState = GameObject.FindObjectOfType<WorldState>();
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        reproduceTimer = worldState.reproduceTimer;
    }

    //Create Behaviour Tree
    protected override Node SetupTree()
    {
        Node root = new Sequence(new List<Node>
        {
            new UpdateNeeds(this),
            new Selector(new List<Node>
            {
                new Reproduce(agent, partner),
                new Sequence(new List<Node>
                {
                    new ResourceInRange(this.transform, fovRange, this),
                    new AttackPrey(agent, this),
                }),
                new CollectWater(this, agent),
                new TaskWander(agent, worldState.worldSize, heightOffset),
            })
        });
        return root;
    }

    public void UpdateNeeds()
    {
        if (water.Count > 0)
        {
            foreach (object r in water)
            {
                if (r == null)
                {
                    water.Remove((Resource)r);
                }
            }
        }

        agent.speed = moveSpeed;
        if ((hunger > 30) || (thirst > 20))
        {
            KillPredator();
        }
        if (reproduceTimer > 0)
        {
            reproduceTimer -= 5 * numEaten;
            partner = null;
        }
        else if ((reproduceTimer <= 0) && (thirst < 10))
        {
            reproduce = true;
            numEaten = 0;
            reproduceTimer = 0;
        }
        hunger += 1;
        thirst += 1;
    }

    public int GetHunger()
    {
        return hunger;
    }

    public int GetThirst()
    {
        return thirst;
    }

    public bool IsHungry()
    {
        bool temp;
        if (hunger > hungerTolerance)
        {
            temp = true;
        }
        else
        {
            temp = false;
        }
        return temp;
    }

    public bool IsThirsty()
    {
        bool temp;
        if (thirst > 10)
        {
            temp = true;
        }
        else
        {
            temp = false;
        }
        return temp;
    }

    public void Drink(Resource drink)
    {
        thirst -= drink.resourceVal;
        worldState.DeleteResource(drink);
        if (thirst < 0) thirst = 0;
    }

    public void Eat()
    {
        hunger -= 10;
        if (hunger < 0) hunger = 0;
        numEaten =+ 1;
    }

    public void RemoveResource(Resource resource)
    {
        if (water.Contains(resource))
        {
            water.Remove(resource);
        }
    }

    public void KillPredator()
    {
        if (worldState.agents.Contains(this.gameObject))
        {
            worldState.agents.Remove(this.gameObject);
        }
        if (reproduce)
        {
            reproduceTimer = worldState.reproduceTimer;
            reproduce = false;
        }

        Destroy(this.gameObject);
        Destroy(this);
    }
}