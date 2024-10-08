using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using BehaviourTree;

public class AttackPrey : Node
{
    private NavMeshAgent agent;
    private PredatorBT predator;

    public AttackPrey(NavMeshAgent _agent, PredatorBT pred)
    {
        agent = _agent;
        predator = pred;
    }

    public override NodeState Evaluate()
    {
        if(predator.IsHungry())
        {
            Transform target = (Transform)GetData("target");

            if (target == null)
            {
                ClearData("target");
                state = NodeState.FAILURE;
                return state;
            }

            if (agent != null)
            {
                if (Vector3.Distance(agent.transform.position, target.position) > 1.5f)
                {
                    if (agent.isActiveAndEnabled)
                    {
                        agent.SetDestination(target.position);
                    }
                    state = NodeState.RUNNING;
                    return state;
                }
                else
                {
                    target.gameObject.GetComponent<Agent>().KillAgent();
                    ClearData("target");
                    predator.Eat();
                    state = NodeState.SUCCESS;
                    return state;
                }
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}