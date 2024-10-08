using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    //Store the current worldState
    [SerializeField]
    private WorldState worldState;

    //Get the size of the area the agent can move around in
    private float areaSize;

    //Store current and target positions + height offsets
    private Vector3 currentPos;
    public Vector3 targetPos;
    private float heightOffset = 1;

    //Store the NavMeshAgent
    private NavMeshAgent agent;

    //Create a random
    private System.Random rand = new System.Random();

    //List of resources
    public List<Resource> resources;

    //Store the predator to run from
    public List<PredatorBT> predators;

    //Agent Needs
    private int hunger;
    private int thirst;
    //If agent has a shelter modifier = 40
    private bool sheltered;
    private int shelterModifier;
    private GameObject shelter;
    //Place to store the agent's current discomfort levels
    private int discomfort;
    //Store what an agent is carrying
    private int woodAmount;
    private int stoneAmount;
    //Store if an agent can currently reproduce
    public bool reproduce = false;
    //Store time between reproductions
    public float reproduceTimer;
    //Store how much stamina the agent has left
    private float stamina;

    //Agent Stats
    public int speed = 8;
    public int maxStamina = 5;
    public int discomfortThreshold = 20;


    //Store the current target resource
    Resource targetResource;

    //how many seconds between each needs increase by 1
    private float needTick = 6.0f;


    //Agent Behaviours
    public enum agentGoal { Wander, CollectResource, GoTo, Run };
    public agentGoal currentGoal = agentGoal.Wander;


    private void Awake()
    {
        worldState = GameObject.FindObjectOfType<WorldState>();
        agent = this.gameObject.GetComponent<NavMeshAgent>();

        //Get the world size from the world state
        areaSize = worldState.worldSize;
        stamina = maxStamina;
        targetPos = new Vector3(rand.Next((int)(-areaSize / 2), (int)(areaSize / 2)), heightOffset, rand.Next((int)(-areaSize / 2), (int)(areaSize / 2)));
    }

    private void Update()
    {
        if (agent != null)
        {
            agent.speed = speed;
        }
        if (agent.destination != null)
        {
            Vector3 temp = agent.destination;
            temp.y = this.transform.position.y;
            this.transform.LookAt(temp);
        }

        currentPos = this.gameObject.transform.position;
        if (sheltered) shelterModifier = worldState.shelterMod;
        else shelterModifier = 0;

        switch(currentGoal)
        {
            //If wandering set a random target within the bounds of the arena for the agent to walk to
            case agentGoal.Wander:
                Debug.Log("Wandering");
                if (((currentPos.x < targetPos.x + 2) && (currentPos.x > targetPos.x - 2) && (currentPos.z < targetPos.z + 2) && (currentPos.z > targetPos.z - 2)) && (agent.velocity.magnitude < 0.1f))
                {
                    targetPos = new Vector3(rand.Next((int)(-areaSize / 2), (int)(areaSize / 2)), heightOffset, rand.Next((int)(-areaSize / 2), (int)(areaSize / 2)));
                }
                else
                {
                    agent.SetDestination(targetPos);
                }
                break;

            case agentGoal.CollectResource:
                Debug.Log("Getting Resource");
                //If the agent has reached the resource
                if (targetResource == null)
                {
                    currentGoal = agentGoal.Wander;
                }
                
                if (targetResource != null)
                {
                    if (Vector3.Distance(transform.position, targetResource.transform.position) <= 1.5f)
                    {
                        //Once in range of the agent the resource will be eaten/drunk
                        if (targetResource.resource == resourceType.Food) hunger -= targetResource.resourceVal;
                        else if (targetResource.resource == resourceType.Water) thirst -= targetResource.resourceVal;
                        else if ((targetResource.resource == resourceType.Wood) && (woodAmount < 5))
                        {
                            woodAmount += 1;
                            worldState.numWood -= 1;
                        }
                        else if ((targetResource.resource == resourceType.Stone) && (stoneAmount < 5))
                        {
                            stoneAmount += 1;
                            worldState.numStone -= 1;
                        }

                        //Once resource's effects have taken place remove it from the resource list
                        worldState.DeleteResource(targetResource);

                        //Set the agent back to wandering
                        currentGoal = agentGoal.Wander;
                    }
                }
                break;

            case agentGoal.GoTo:
                Debug.Log("Moving To");
                agent.SetDestination(targetPos);
                break;

            case agentGoal.Run:
                Debug.Log("Running");
                if(stamina > 0)
                {
                    stamina -= Time.deltaTime;

                    if (predators.Count > 0)
                    {
                        if (predators[0] != null)
                        {
                            Vector3 directionToRun = this.transform.position - predators[0].transform.position;
                            directionToRun = ((directionToRun.normalized * 8) + this.transform.position);
                            directionToRun.y = this.transform.position.y;
                            agent.SetDestination(directionToRun);
                            break;
                        }
                    }
                }
                targetPos = new Vector3(rand.Next((int)(-areaSize / 2), (int)(areaSize / 2)), heightOffset, rand.Next((int)(-areaSize / 2), (int)(areaSize / 2)));
                currentGoal = agentGoal.Wander;
                break;
        }

        //Ensure hunger and thirst don't drop bellow 0 when eating/drinking
        if (hunger < 0) hunger = 0;
        if (thirst < 0) thirst = 0;
        //Calculate discomfort of agent
        discomfort = (hunger * hunger) + (thirst * thirst) - shelterModifier;

        //If discomfort is bigger than the threshold or the agent has no shelter find the best action to take
        if ((discomfort >= discomfortThreshold) || (!sheltered))
        {
            if (currentGoal != agentGoal.Run)
            {
                targetResource = ChooseTarget();
                if (targetResource != null)
                {
                    agent.SetDestination(targetResource.transform.position);
                    currentGoal = agentGoal.CollectResource;
                }
            }
        }

        //If agent has enough food, water and has shelter allow to reproduce
        if ((discomfort < 0) && reproduceTimer <= 0)
        {
            reproduce = true;
        }

        //If agent gets too hungry or thirsty kill agent
        if ((hunger > 20) || (thirst > 20))
        {
            KillAgent();
        }

        //If agent has enough wood and stone create a shelter for the agent
        if (((woodAmount == 5) && (stoneAmount == 5)) && (!sheltered))
        {
            shelter = Instantiate(worldState.shelter, this.transform.position - new Vector3(0, 0.5f, 0), this.transform.rotation);
            worldState.shelters.Add(shelter);
            sheltered = true;
            woodAmount = 0;
            stoneAmount = 0;
        }

        //Ensure reproduction timer doesn't get too far past 0
        if (reproduceTimer < 0) reproduceTimer = 0;
        //Lower the agents reproduction timer
        reproduceTimer -= Time.deltaTime;
        //Lower the agents need tick
        needTick -= Time.deltaTime;
        //If the need tick is less than or 0 tick the agents needs up 1
        if ((stamina < maxStamina) && (currentGoal != agentGoal.Run))
        {
            stamina += Time.deltaTime;
        }
        if (needTick <= 0.0f)
        {
            hunger += 1;
            thirst += 1;
            needTick = 3f;
        }
    }

    public float GetStamina()
    {
        return stamina;
    }

    private Resource ChooseTarget()
    {
        targetResource = null;
        int bestScore = 0;
        //Calculate each found resource's score
        foreach (Resource r in resources)
        {
            if ((r.resource == resourceType.Food) || (r.resource == resourceType.Water) || ((r.resource == resourceType.Wood) && (woodAmount < 5)) || ((r.resource == resourceType.Stone) && (stoneAmount < 5)))
            {
                int tempScore = CalcResourceScore(r);
                if (tempScore > bestScore)
                {
                    bestScore = tempScore;
                    targetResource = r;
                }
            }
        }
        return targetResource;
    }

    private int CalcResourceScore(Resource resource)
    {
        //The higher the score the better
        int score = resource.resourceVal;  //Start score as the resource's base value

        //Work out distance to the resource
        float distance = Vector3.Distance(transform.position, resource.transform.position);

        //If the resource is food and the agent has a higher hunger than thirst double the resource's score
        if ((discomfort >= discomfortThreshold) && (resource.resource == resourceType.Food && hunger > thirst))
        {
            score *= 2;
        }
        //If the resource is water and the agent has a higher thirst than hunger double the resource's score
        else if ((discomfort >= discomfortThreshold) && (resource.resource == resourceType.Water && thirst > hunger))
        {
            score *= 2;
        }
        //If discomfort is less than discomfort threshold prioritise wood or stone collection
        else if ((discomfort < discomfortThreshold) && (resource.resource == resourceType.Wood || resource.resource == resourceType.Stone))
        {
            score *= 4;
        }
        //Half score if food or water at low discomfort level
        else if ((discomfort < discomfortThreshold) && (resource.resource == resourceType.Water || resource.resource == resourceType.Food))
        {
            score /= 2;
        }

        //Lower the resource's score based on how far away the resource is
        score -= Mathf.RoundToInt(distance);
        

        return score;
    }

    public void KillAgent()
    {
        if (worldState.agents.Contains(this.gameObject))
        {
            worldState.agents.Remove(this.gameObject);
        }
        if (sheltered)
        {
            worldState.shelters.Remove(shelter);
        }
        if (reproduce)
        {
            reproduce = false;
            reproduceTimer = worldState.reproduceTimer;
        }
        Destroy(shelter);
        Destroy(this.gameObject);
    }

    public void RemoveResource(Resource resource)
    {
        if (resources.Contains(resource))
        {
            resources.Remove(resource);
        }
    }
}