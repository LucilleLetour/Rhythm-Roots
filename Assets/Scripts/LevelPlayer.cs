using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using RhythmReader;
using UnityEngine;

public class LevelPlayer : MonoBehaviour
{
    [SerializeField] private string levelRbmPath;
    [SerializeField] private int songBpm;
    [SerializeField] private RbmReader rbmReader;
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioLowPassFilter introLowpass;
    [SerializeField] private AnimationCurve lowpassCurve;
    [SerializeField] private Transform beatIndicator;
    [SerializeField] private float graceTimeMs;
    [SerializeField] private float obstacleVisualSpacing;
    [SerializeField] private AnimationCurve cameraLerp;
    [SerializeField] private int obstacleLookahead;
    [SerializeField] private int obstacleKeepBeforeCull;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float baselineY;
    [SerializeField] private Player player;

    private float levelBpm;
    private bool _started;
    private float _curLevelTime;
    private int _curObstacleTimestampId;
    private float endFadeinTime;
    private Timestamp[] timestamps;
    private List<Timestamp> gameFlowTimestamps = new();
    private List<Timestamp> obstacleTimestamps = new();
    private List<(Timestamp, GameObject)> nextLookaheadObstacles = new();
    
    void Start()
    {
        rbmReader.LoadLevel(levelRbmPath);
        timestamps = rbmReader.GetRbmData().Timestamps;
        
        foreach (var timestamp in timestamps)
        {
            switch (timestamp.BeatTrackId)
            {
                case (int)BeatTracks.GameControl:
                    gameFlowTimestamps.Add(timestamp);
                    break;
                case (int)BeatTracks.Obstacles:
                    obstacleTimestamps.Add(timestamp);
                    break;
            }
        }
        
        
        player.SetBaselineY(baselineY);
        
        Setup();
    }

    private void Update()
    {
        if (!_started)
        {
            if (Input.anyKey)
            {
                _started = true;
                musicPlayer.Play();
                Debug.Log(gameFlowTimestamps[0].PrefabId);
                endFadeinTime = (float)gameFlowTimestamps.First(x => x.PrefabId == (int)GameControlTypes.EndIntro).Time;
            }
        }
        else
        {
            _curLevelTime += Time.deltaTime * 1000;
            introLowpass.cutoffFrequency = 21990f * lowpassCurve.Evaluate(Mathf.Lerp(0, 1, _curLevelTime/endFadeinTime)) + 10f;
            DoBeatIndicator();
            if (_curLevelTime > obstacleTimestamps[_curObstacleTimestampId].Time + graceTimeMs)
            {
                var prefId = obstacleTimestamps[_curObstacleTimestampId].PrefabId;
                switch (prefId)
                {
                    case (int)ObstacleTypes.Center:
                    case (int)ObstacleTypes.Left:
                    case (int)ObstacleTypes.Right:
                    case (int)ObstacleTypes.Split:
                        if ((ObstacleTypes)prefId == InputDecoder.DecodeInput())
                        {
                            CorrectInput();
                        }
                        else
                        {
                            IncorrectInput();
                        }
                        break;
                    case (int)ObstacleTypes.SplitOrOneSide:
                        if (InputDecoder.DecodeInput() != ObstacleTypes.Center)
                        {
                            CorrectInput();
                        }
                        else
                        {
                            IncorrectInput();
                        }
                        break;
                }
                
                nextLookaheadObstacles.Add((obstacleTimestamps[_curObstacleTimestampId + obstacleLookahead], nextLookaheadObstacles[0].Item2));
                nextLookaheadObstacles[0].Item2.GetComponent<Obstacle>().SetType(obstacleTimestamps[_curObstacleTimestampId + obstacleLookahead].PrefabId);
                nextLookaheadObstacles.RemoveAt(0);
                _curObstacleTimestampId++;
            }
            RenderObstacles();
        }
    }

    private void DoBeatIndicator()
    {
        var timeBetweenBeats = _curLevelTime % ((60f / levelBpm) * 1000f);
        var halfPoint = ((60f / levelBpm) * 1000f) / 2f;
        var off = Mathf.Abs(timeBetweenBeats - halfPoint);
        beatIndicator.localScale = Vector3.one * Mathf.Lerp(0.5f, 1, off/halfPoint);
    }
    
    // TODO Temp
    private void RenderObstacles()
    {
        for (var i = 0; i < obstacleKeepBeforeCull + obstacleLookahead; i++)
        {
            if(nextLookaheadObstacles[i].Item1 == null) continue;
            var mspb = ((60f / levelBpm) * 1000f);
            var beatsUntil = Mathf.Ceil((nextLookaheadObstacles[i].Item1.Time - _curLevelTime) / mspb);
            var baseLoc = baselineY - (obstacleVisualSpacing * beatsUntil);
            var smoothLoc = baseLoc - (obstacleVisualSpacing * cameraLerp.Evaluate(
                (((nextLookaheadObstacles[i].Item1.Time - _curLevelTime) / mspb) + obstacleKeepBeforeCull) % 1));
            nextLookaheadObstacles[i].Item2.transform.position = new Vector3(0, smoothLoc, 0);
        }
    }
    
    private void CorrectInput()
    {
        Debug.Log("Good");
    }
    
    private void IncorrectInput()
    {
        Die();
    }

    private void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        var dieLen = 2f;
        var curLen = 0f;
        while (curLen < dieLen)
        {
            musicPlayer.pitch = Mathf.Lerp(1 , 0.5f, curLen / dieLen);
            levelBpm = Mathf.Lerp(songBpm, songBpm / 2f, curLen / dieLen);
            curLen += Time.deltaTime;
            yield return null;
        }
        Setup();
    }

    private void Setup()
    {
        _curObstacleTimestampId = 0;
        _curLevelTime = 0L;
        musicPlayer.pitch = 1f;
        musicPlayer.Stop();
        musicPlayer.time = 1.5f;
        _started = false;
        levelBpm = songBpm;
        InitializeObstacles();
    }

    private void InitializeObstacles()
    {
        foreach (var obstacle in nextLookaheadObstacles)
        {
            Destroy(obstacle.Item2);
        }
        nextLookaheadObstacles.Clear();
        for (var i = 0; i < obstacleKeepBeforeCull; i++)
        {
            nextLookaheadObstacles.Add((null, Instantiate(obstaclePrefab)));
        }

        nextLookaheadObstacles.AddRange(obstacleTimestamps.Take(obstacleLookahead)
            .Select(x => (x, Instantiate(obstaclePrefab))).ToList());
        foreach (var nextLookaheadObstacle in nextLookaheadObstacles)
        {
            if (nextLookaheadObstacle.Item1 == null) continue;
            nextLookaheadObstacle.Item2.GetComponent<Obstacle>().SetType(nextLookaheadObstacle.Item1.PrefabId);
        }
    }

    enum BeatTracks
    {
        GameControl = 1,
        Obstacles = 2,
        BpmControl = 3,
    }

    enum GameControlTypes
    {
        EndIntro = 5,
    }

    enum BPMControls
    {
        Double = 0,
        Halve = 1,
    }

    public enum ObstacleTypes
    {
        Center = 0,
        Left = 1,
        Right = 2,
        Split = 3,
        SplitOrOneSide = 4,
    }
}
