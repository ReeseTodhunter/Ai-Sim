using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class ResourceInRange : Node
{
    private static int preyLayer = 1 << 8;
    private static int resourceLayer = 1 << 7;

    private Transform position;
    private float fov;
    private PredatorBT predator;

    public ResourceInRange(Transform pos, float range, PredatorBT pred)
    {
        position = pos;
        fov = range;
        predator = pred;
    }

    public override NodeState Evaluate()
    {
        //If the predator is more hungry than it's hunger tolerance
        if (predator.IsHungry())
        {
            object t = GetData("target");
            if (t == null)
            {
                Collider[] colliders = Physics.OverlapSphere(position.position, fov, preyLayer);
                if (colliders.Length > 0)
                {
                    parent.parent.SetData("target", colliders[0].transform);

                    state = NodeState.SUCCESS;
                    return base.Evaluate();
                }

                state = NodeState.FAILURE;
                return state;
            }
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(position.position, fov, resourceLayer);
            if (colliders.Length > 0)
            {
                foreach (Collider obj in colliders)
                {
                    if (obj.gameObject.GetComponent<Resource>().resource == resourceType.Water)
                    {
                        if (!predator.water.Contains(obj.gameObject.GetComponent<Resource>()))
                        {
                            predator.water.Add(obj.gameObject.GetComponent<Resource>());
                        }
                    }
                }
            }
        }
        state = NodeState.SUCCESS;
        return state;
    }
}
