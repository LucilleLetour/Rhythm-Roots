using System.Collections;
using System.Collections.Generic;
using RhythmReader;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Transform leftPlayer;
    [SerializeField] private Transform rightPlayer;
    private float baselineY;

    public void SetBaselineY(float baseline)
    {
        baselineY = baseline;
    }

    void Update()
    {
        switch (InputDecoder.DecodeInput())
        {
            case LevelPlayer.ObstacleTypes.Center:
                leftPlayer.position = new Vector3(-0.25f, baselineY, 0f);
                rightPlayer.position = new Vector3(0.25f, baselineY, 0f);
                break;
            case LevelPlayer.ObstacleTypes.Left:
                leftPlayer.position = new Vector3(-1.25f, baselineY, 0f);
                rightPlayer.position = new Vector3(-0.75f, baselineY, 0f);
                break;
            case LevelPlayer.ObstacleTypes.Right:
                leftPlayer.position = new Vector3(0.75f, baselineY, 0f);
                rightPlayer.position = new Vector3(1.25f, baselineY, 0f);
                break;
            case LevelPlayer.ObstacleTypes.Split:
                leftPlayer.position = new Vector3(-1.25f, baselineY, 0f);
                rightPlayer.position = new Vector3(1.25f, baselineY, 0f);
                break;
        }
    }
}
