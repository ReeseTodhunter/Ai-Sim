using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum resourceType
{
    Food,
    Water,
    Wood,
    Stone,
    Shelter
};

public class Resource : MonoBehaviour
{
    public resourceType resource;
    public int resourceVal = 10;
}