using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField]
    private WorldState worldState;

    [SerializeField]
    private GameObject food;
    [SerializeField]
    private GameObject water;
    [SerializeField]
    private GameObject wood;
    [SerializeField]
    private GameObject stone;
    

    //Available area size
    private int size;
    //How often to spawn a new resource
    private float resourceTimer;

    private int placed;
    List<GameObject> resources;

    //Create a random
    private System.Random rand = new System.Random();

    private void Start()
    {
        resourceTimer = worldState.resourceFrequency;
        size = worldState.worldSize;
    }

    private void Update()
    {
        resourceTimer -= Time.deltaTime;
        if (resourceTimer <= 0)
        {
            worldState.resources.Add(SpawnResource(food));
            worldState.resources.Add(SpawnResource(water));
            worldState.resources.Add(SpawnResource(water));
            worldState.resources.Add(SpawnResource(water));

            if (worldState.numWood < 5)
            {
                worldState.resources.Add(SpawnResource(wood));
                worldState.numWood += 1;
            }
            if (worldState.numStone < 5)
            {
                worldState.resources.Add(SpawnResource(stone));
                worldState.numStone += 1;
            }
            resourceTimer = worldState.resourceFrequency;
        }
    }

    private GameObject SpawnResource(GameObject resource)
    {
        GameObject tempObj = null;
        
        //While the resource hasn't been instantiated loop
        while (tempObj == null)
        {
            //Get a random position in the available area
            Vector3 temp = new Vector3(rand.Next((int)(-size / 2), (int)(size / 2)), 10, rand.Next((int)(-size / 2), (int)(size / 2)));

            if (!Physics.Raycast(temp, Vector3.down, 11.0f, LayerMask.NameToLayer("Obstacle")))
            {
                //Debug.Log("Found good spawn spot");
                tempObj = Instantiate(resource, new Vector3(temp.x, 1, temp.z), new Quaternion());
            }
        }

        return tempObj;
    }
}