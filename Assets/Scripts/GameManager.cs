using System.Collections;
using System.Collections.Generic;
using RhythmReader;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RbmReader rbmReader;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SetLevel(RbmReader.Levels level)
    {
        rbmReader.SwitchLevel(level);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
