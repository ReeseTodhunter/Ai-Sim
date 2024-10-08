using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awareness : MonoBehaviour
{
    private Agent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = transform.GetComponentInParent<Agent>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (agent != null)
        {
            if (other.gameObject.TryGetComponent<Resource>(out Resource resource))
            {
                //Debug.Log("Found resource: " + resource);

                if (!agent.resources.Contains(resource))
                {
                    agent.resources.Add(resource);
                }
            }

            if (other.gameObject.TryGetComponent<PredatorBT>(out PredatorBT predator))
            {
                if (!agent.predators.Contains(predator))
                {
                    agent.predators.Add(predator);
                }
                if (agent.GetStamina() > 2)
                {
                    agent.currentGoal = Agent.agentGoal.Run;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PredatorBT>(out PredatorBT predator))
        {
            if (agent.predators.Contains(predator))
            {
                agent.predators.Remove(predator);
            }
        }
    }
}