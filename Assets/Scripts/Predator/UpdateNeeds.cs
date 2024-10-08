using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class UpdateNeeds : Node
{
    //how many seconds between each needs decrease/increase by 1
    private float needTick = 6.0f;
    private PredatorBT predator;

    public UpdateNeeds(PredatorBT pred)
    {
        predator = pred;
    }

    public override NodeState Evaluate()
    {
        needTick -= Time.deltaTime;

        if (needTick <= 0.0f)
        {
            needTick = 3f;

            predator.UpdateNeeds();

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }
}
