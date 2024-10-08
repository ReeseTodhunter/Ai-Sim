using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using BehaviourTree;
public class Reproduce : Node
{
    PredatorBT partner;
    NavMeshAgent agent;

    public Reproduce(NavMeshAgent _agent, PredatorBT _partner)
    {
        agent = _agent;
        partner = _partner;
    }

    public override NodeState Evaluate()
    {
        if(partner != null)
        {
            if (Vector3.Distance(agent.transform.position, partner.transform.position) > 1.5f)
            {
                if (agent.isActiveAndEnabled)
                {
                    agent.SetDestination(partner.transform.position);
                    state = NodeState.RUNNING;
                    return state;
                }
            }
            else
            {
                state = NodeState.SUCCESS;
                return state;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }
}
