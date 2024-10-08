using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


using BehaviourTree;

public class CollectWater : Node
{
    private PredatorBT predator;
    private NavMeshAgent agent;

    public CollectWater(PredatorBT pred, NavMeshAgent nav)
    {
        predator = pred;
        agent = nav;
    }
    public override NodeState Evaluate()
    {
        if (predator.IsThirsty())
        {
            if (agent != null)
            {
                if (predator.water.Count > 0)
                {
                    Resource temp = predator.water[0];
                    if (temp != null)
                    {
                        if (Vector3.Distance(predator.transform.position, temp.transform.position) > 1.5f)
                        {
                            if (agent.isActiveAndEnabled)
                            {
                                agent.SetDestination(temp.transform.position);
                            }
                            state = NodeState.RUNNING;
                            return state;
                        }
                        else
                        {
                            predator.Drink(temp);
                            predator.water.Remove(temp);

                            state = NodeState.SUCCESS;
                            return state;
                        }
                    }
                    else
                    {
                        predator.water.Remove(temp);
                        if (predator.water.Count < 1)
                        {
                            state = NodeState.FAILURE;
                            return state;
                        }
                        state = NodeState.RUNNING;
                        return state;
                    }
                }
            }
        }
        state = NodeState.FAILURE;
        return state;
    }
}