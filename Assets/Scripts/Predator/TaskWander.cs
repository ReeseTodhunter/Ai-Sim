using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using BehaviourTree;
public class TaskWander : Node
{
    //Store the NavMeshAgent
    private NavMeshAgent agent;
    //Store target location
    private Vector3 target;
    //Get the size of the area the agent can move around in
    private float areaSize;
    //Get the Height offset
    private float heightOffset;
    //Current agent position
    private Vector3 currentPos;
    //Create a random
    private System.Random rand = new System.Random();

    public TaskWander(NavMeshAgent _agent, float _areaSize, float height)
    {
        agent = _agent;
        areaSize = _areaSize;
        heightOffset = height;
        target = new Vector3(rand.Next((int)(-areaSize / 2), (int)(areaSize / 2)), heightOffset, rand.Next((int)(-areaSize / 2), (int)(areaSize / 2)));
    }

    public override NodeState Evaluate()
    {
        if (agent == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        else
        {
            currentPos = agent.gameObject.transform.position;

            if (((currentPos.x < target.x + 2) && (currentPos.x > target.x - 2) && (currentPos.z < target.z + 2) && (currentPos.z > target.z - 2)) && (agent.velocity.magnitude < 0.1f))
            {
                target = new Vector3(rand.Next((int)(-areaSize / 2), (int)(areaSize / 2)), heightOffset, rand.Next((int)(-areaSize / 2), (int)(areaSize / 2)));
            }
            else
            {
                if (agent.isActiveAndEnabled)
                {
                    agent.SetDestination(target);
                }
            }

            state = NodeState.RUNNING;
            return state;
        }
    }
}
