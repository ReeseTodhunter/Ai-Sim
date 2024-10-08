using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField]
    private WorldState worldState;
    [SerializeField]
    private GameObject prey;
    [SerializeField]
    private GameObject pred;

    //Store number of agents to spawn
    private int numPrey;
    private int numPred;
    private float size;

    //List to store agents capable of reproducing
    public List<GameObject> capableReproducePrey;
    public List<GameObject> capableReproducePredator;

    //Create a random
    private System.Random rand = new System.Random();

    // Start is called before the first frame update
    private void Start()
    {
        numPrey = worldState.numPrey;
        numPred = worldState.numPred;
        size = worldState.worldSize;

        for (int i = 0; i < numPrey; i++)
        {
            GameObject temp = Instantiate(prey, new Vector3(rand.Next((int)(-size / 2), (int)(size / 2)), 1, rand.Next((int)(-size / 2), (int)(size / 2))), new Quaternion());
            temp.GetComponent<Agent>().speed = rand.Next(worldState.minAgentSpeed, worldState.maxAgentSpeed);
            temp.GetComponent<Agent>().maxStamina = rand.Next(worldState.minPreyStamina, worldState.maxPreyStamina);
            temp.GetComponent<Agent>().discomfortThreshold = rand.Next(worldState.minDiscomfortThreshold, worldState.maxDiscomfortThreshold);
            worldState.agents.Add(temp);
        }
        for (int i = 0; i < numPred; i++)
        {
            GameObject temp = Instantiate(pred, new Vector3(rand.Next((int)(-size / 2), (int)(size / 2)), 1, rand.Next((int)(-size / 2), (int)(size / 2))), new Quaternion());
            temp.GetComponent<PredatorBT>().hungerTolerance = rand.Next(worldState.minPredatorHungerTolerance, worldState.maxPredatorHungerTolerance);
            temp.GetComponent<PredatorBT>().fovRange = rand.Next(worldState.minPredatorFovRange, worldState.maxPredatorFovRange);
            temp.GetComponent<PredatorBT>().moveSpeed = rand.Next(worldState.minAgentSpeed, worldState.maxAgentSpeed);
            worldState.agents.Add(temp);
        }
    }

    private void Update()
    {
        Reproduce();
    }

    private void Reproduce()
    {
        for (int i = 0; i < worldState.agents.Count; i++)
        {
            if (worldState.agents[i].GetComponent<Agent>())
            {
                Agent temp = worldState.agents[i].GetComponent<Agent>();
                if (temp.reproduce)
                {
                    if (!capableReproducePrey.Contains(worldState.agents[i]))
                    {
                        capableReproducePrey.Add(worldState.agents[i]);
                    }
                }
                else
                {
                    if (capableReproducePrey.Contains(worldState.agents[i]))
                    {
                        capableReproducePrey.Remove(worldState.agents[i]);
                    }
                }
            }
            if (worldState.agents[i].GetComponent<PredatorBT>())
            {
                PredatorBT temp = worldState.agents[i].GetComponent<PredatorBT>();
                if (temp.reproduce)
                {
                    if (!capableReproducePredator.Contains(worldState.agents[i]))
                    {
                        capableReproducePredator.Add(worldState.agents[i]);
                    }
                }
                else
                {
                    if (capableReproducePredator.Contains(worldState.agents[i]))
                    {
                        capableReproducePredator.Remove(worldState.agents[i]);
                    }
                }
            }
        }

        if (capableReproducePrey.Count >= 2)
        {
            Agent temp1, temp2;
            temp1 = capableReproducePrey[0].GetComponent<Agent>();
            temp2 = capableReproducePrey[1].GetComponent<Agent>();

            temp1.currentGoal = Agent.agentGoal.GoTo;
            temp1.targetPos = temp2.transform.position;

            temp2.currentGoal = Agent.agentGoal.GoTo;
            temp2.targetPos = temp1.transform.position;

            if (Vector3.Distance(temp1.transform.position, temp2.transform.position) < 2f)
            {
                GameObject temp3 = Instantiate(prey, temp1.transform.position, temp1.transform.rotation);
                //New Prey gets random speed inherited from parents
                if (temp1.speed < temp2.speed)
                {
                    temp3.GetComponent<Agent>().speed = rand.Next(temp1.speed, temp2.speed);
                }
                else
                {
                    temp3.GetComponent<Agent>().speed = rand.Next(temp2.speed, temp1.speed);
                }
                //New Prey gets random stamina inherited from parents
                if (temp1.maxStamina < temp2.maxStamina)
                {
                    temp3.GetComponent<Agent>().maxStamina = rand.Next(temp1.maxStamina, temp2.maxStamina);
                }
                else
                {
                    temp3.GetComponent<Agent>().maxStamina = rand.Next(temp2.maxStamina, temp1.maxStamina);
                }
                //New Prey gets random discomfort inherited from parents
                if (temp1.discomfortThreshold < temp2.discomfortThreshold)
                {
                    temp3.GetComponent<Agent>().discomfortThreshold = rand.Next(temp1.discomfortThreshold, temp2.discomfortThreshold);
                }
                else
                {
                    temp3.GetComponent<Agent>().discomfortThreshold = rand.Next(temp2.discomfortThreshold, temp1.discomfortThreshold);
                }
                //Add new prey to the worldstate agents list
                worldState.agents.Add(temp3);

                //Reset parents reproduction variables
                temp1.reproduceTimer = worldState.reproduceTimer;
                temp2.reproduceTimer = worldState.reproduceTimer;
                temp1.reproduce = false;
                temp2.reproduce = false;
                temp1.currentGoal = Agent.agentGoal.Wander;
                temp2.currentGoal = Agent.agentGoal.Wander;
                capableReproducePrey.Remove(temp1.gameObject);
                capableReproducePrey.Remove(temp2.gameObject);
            }
        }

        if (capableReproducePredator.Count >= 2)
        {
            PredatorBT temp1, temp2;
            temp1 = capableReproducePredator[0].GetComponent<PredatorBT>();
            temp2 = capableReproducePredator[1].GetComponent<PredatorBT>();
            temp1.partner = temp2;
            temp2.partner = temp1;

            GameObject temp3 = Instantiate(pred, new Vector3(rand.Next((int)(-size / 2), (int)(size / 2)), 1, rand.Next((int)(-size / 2), (int)(size / 2))), new Quaternion());
            //New predator inherits a hungerTolerance from parents
            if (temp1.hungerTolerance < temp2.hungerTolerance)
            {
                temp3.GetComponent<PredatorBT>().hungerTolerance = rand.Next(temp1.hungerTolerance, temp2.hungerTolerance);
            }
            else
            {
                temp3.GetComponent<PredatorBT>().hungerTolerance = rand.Next(temp2.hungerTolerance, temp1.hungerTolerance);
            }
            //New predator inherits a moveSpeed from parents
            if (temp1.moveSpeed < temp2.moveSpeed)
            {
                temp3.GetComponent<PredatorBT>().moveSpeed = rand.Next(temp1.moveSpeed, temp2.moveSpeed);
            }
            else
            {
                temp3.GetComponent<PredatorBT>().moveSpeed = rand.Next(temp2.moveSpeed, temp1.moveSpeed);
            }
            //New predator inherits a fovRange from parents
            if (temp1.fovRange < temp2.fovRange)
            {
                temp3.GetComponent<PredatorBT>().fovRange = rand.Next(temp1.fovRange, temp2.fovRange);
            }
            else
            {
                temp3.GetComponent<PredatorBT>().fovRange = rand.Next(temp2.fovRange, temp1.fovRange);
            }
            //Add new predator to the worldstate agents list
            worldState.agents.Add(temp3);

            //Reset parents reproduction variables
            temp1.reproduceTimer = worldState.reproduceTimer;
            temp2.reproduceTimer = worldState.reproduceTimer;
            temp1.reproduce = false;
            temp2.reproduce = false;
            capableReproducePredator.Remove(temp1.gameObject);
            capableReproducePredator.Remove(temp2.gameObject);
            temp1.partner = null;
            temp2.partner = null;
        }
    }
}