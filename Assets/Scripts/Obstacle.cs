using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                foreach (var child in types[i].GetComponentsInChildren<Transform>(true).Skip(1))
                {
                    child.eulerAngles = new Vector3(Random.Range(0f, 360f), 0, 0);
                }
            }
            else
            {
                types[i].SetActive(false);
            }
        }
    }
}
