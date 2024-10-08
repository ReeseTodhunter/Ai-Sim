using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made by following: https://www.youtube.com/watch?v=aR6wt5BlE-E

namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {

        private Node _root = null;

        protected void Start()
        {
            _root = SetupTree();
        }

        private void Update()
        {
            if (_root != null)
                _root.Evaluate();
        }

        protected abstract Node SetupTree();

    }
}