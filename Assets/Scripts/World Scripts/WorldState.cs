using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    //How large an area the agents have to explore
    [Range(50, 200)]
    public int worldSize = 50;

    //Number of prey agents to spawn
    [Range(0, 20)]
    public int numPrey = 2;

    //How much stamina a Prey should have
    public int minPreyStamina = 2;
    public int maxPreyStamina = 10;

    //How uncomfortable an agent can be before prioritising food and water collection
    public int minDiscomfortThreshold = 20;
    public int maxDiscomfortThreshold = 60;

    //Modifier for discomfort removed by shelter
    [Range(0, 100)]
    public int shelterMod = 40;

    //Number of predator agents to spawn
    [Range(0, 20)]
    public int numPred = 0;

    //How long predators will go before eating
    public int minPredatorHungerTolerance = 10;
    public int maxPredatorHungerTolerance = 20;

    //How far predators can see
    public int minPredatorFovRange = 3;
    public int maxPredatorFovRange = 8;

    //How fast agents can be
    public int minAgentSpeed = 5;
    public int maxAgentSpeed = 12;

    //How long between reproductions
    [Range(0, 120)]
    public float reproduceTimer = 50;

    //How long between resource spawns (lower = quicker)
    [Range(2, 10)]
    public float resourceFrequency = 2;

    //Store the shelter prefab
    public GameObject shelter;

    public List<GameObject> agents;
    public List<GameObject> resources;
    public List<GameObject> shelters;

    public int numWood = 0;
    public int numStone = 0;

    public void DeleteResource(Resource resource)
    {
        if(resource != null)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].GetComponent<Agent>())
                {
                    agents[i].GetComponent<Agent>().RemoveResource(resource);
                }
                else if (agents[i].GetComponent<PredatorBT>())
                {
                    agents[i].GetComponent<PredatorBT>().RemoveResource(resource);
                }
            }
            if (resources.Contains(resource.gameObject))
            {
                //Remove the resource from the world's resource list
                resources.Remove(resource.gameObject);
                //Destroy GameObject in world space
                GameObject.Destroy(resource.gameObject);
            }
        }
    }
}