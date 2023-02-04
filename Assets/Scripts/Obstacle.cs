using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    [SerializeField] private List<GameObject> types;

    public void SetType(uint type)
    {
        for (int i = 0; i < types.Count; i++)
        {
            if (i == (int)type)
            {
                types[i].SetActive(true);
            }
            else
            {
                types[i].SetActive(false);
            }
        }
    }
}
