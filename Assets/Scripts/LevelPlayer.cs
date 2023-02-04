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
    [SerializeField] private OneShotPlayer oneShotPlayer;

    private float _levelBpm;
    private bool _started;
    private float _curLevelTime;
    private int _curObstacleTimestampId;
    private float _endFadeinTime;
    private int _curBeat;
    private Timestamp[] _timestamps;
    private readonly List<Timestamp> _gameFlowTimestamps = new();
    private readonly List<Timestamp> _obstacleTimestamps = new();
    private readonly List<(Timestamp, GameObject)> _nextLookaheadObstacles = new();
    
    void Start()
    {
        rbmReader.LoadLevel(levelRbmPath);
        _timestamps = rbmReader.GetRbmData().Timestamps;
        
        foreach (var timestamp in _timestamps)
        {
            switch (timestamp.BeatTrackId)
            {
                case (int)BeatTracks.GameControl:
                    _gameFlowTimestamps.Add(timestamp);
                    break;
                case (int)BeatTracks.Obstacles:
                    _obstacleTimestamps.Add(timestamp);
                    break;
            }
        }
        
        
        player.SetBaselineY(baselineY);
        
        Setup();
    }

    private void Update()
    {
        var mspb = ((60f / _levelBpm) * 1000f);
        if (!_started)
        {
            if (Input.anyKey)
            {
                _started = true;
                foreach (var obstacle in _nextLookaheadObstacles)
                {
                    obstacle.Item2.SetActive(true);
                }
                musicPlayer.Play();
                _endFadeinTime = (float)_gameFlowTimestamps.First(x => x.PrefabId == (int)GameControlTypes.EndIntro).Time;
            }
        }
        else
        {
            _curLevelTime += Time.deltaTime * 1000;
            introLowpass.cutoffFrequency = 21990f * lowpassCurve.Evaluate(Mathf.Lerp(0, 1, _curLevelTime/_endFadeinTime)) + 10f;
            DoBeatIndicator();
            if (_curObstacleTimestampId >= _obstacleTimestamps.Count)
            {
                // Do nothing, level over
            }
            else if (_curLevelTime > _obstacleTimestamps[_curObstacleTimestampId].Time + graceTimeMs)
            {
                var prefId = _obstacleTimestamps[_curObstacleTimestampId].PrefabId;
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

                if (_curObstacleTimestampId + obstacleLookahead < _obstacleTimestamps.Count)
                {
                    _nextLookaheadObstacles.Add((_obstacleTimestamps[_curObstacleTimestampId + obstacleLookahead],
                        _nextLookaheadObstacles[0].Item2));
                    _nextLookaheadObstacles[0].Item2.GetComponent<Obstacle>()
                        .SetType(_obstacleTimestamps[_curObstacleTimestampId + obstacleLookahead].PrefabId);
                }

                _nextLookaheadObstacles.RemoveAt(0);
                _curObstacleTimestampId++;
            }

            if (_curBeat < Mathf.Floor(_curLevelTime / mspb))
            {
                OnBeat();
                _curBeat++;
            }
            RenderObstacles();
        }
    }

    private void OnBeat()
    {
        oneShotPlayer.PlayRootGrowth();
    }

    private void DoBeatIndicator()
    {
        var timeBetweenBeats = _curLevelTime % ((60f / _levelBpm) * 1000f);
        var halfPoint = ((60f / _levelBpm) * 1000f) / 2f;
        var off = Mathf.Abs(timeBetweenBeats - halfPoint);
        beatIndicator.localScale = Vector3.one * Mathf.Lerp(0.5f, 1, off/halfPoint);
    }
    
    // TODO Temp
    private void RenderObstacles()
    {
        for (var i = 0; i < obstacleKeepBeforeCull + obstacleLookahead; i++)
        {
            if(i >= _nextLookaheadObstacles.Count || _nextLookaheadObstacles[i].Item1 == null) continue;
            var mspb = ((60f / _levelBpm) * 1000f);
            var beatsUntil = Mathf.Ceil((_nextLookaheadObstacles[i].Item1.Time - _curLevelTime) / mspb);
            var baseLoc = baselineY - (obstacleVisualSpacing * beatsUntil);
            var smoothLoc = baseLoc - (obstacleVisualSpacing * cameraLerp.Evaluate(
                (((_nextLookaheadObstacles[i].Item1.Time - _curLevelTime) / mspb) + obstacleKeepBeforeCull) % 1));
            _nextLookaheadObstacles[i].Item2.transform.position = new Vector3(0, smoothLoc, 0);
        }
    }
    
    private void CorrectInput()
    {
        
    }
    
    private void IncorrectInput()
    { 
        Hit();
    }
    
    private void Hit()
    {
        oneShotPlayer.PlayRockHit();
        StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        yield return null;
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
            _levelBpm = Mathf.Lerp(songBpm, songBpm / 2f, curLen / dieLen);
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
        _levelBpm = songBpm;
        InitializeObstacles();
    }

    private void InitializeObstacles()
    {
        foreach (var obstacle in _nextLookaheadObstacles)
        {
            Destroy(obstacle.Item2);
        }
        _nextLookaheadObstacles.Clear();
        for (var i = 0; i < obstacleKeepBeforeCull; i++)
        {
            _nextLookaheadObstacles.Add((null, Instantiate(obstaclePrefab)));
        }

        _nextLookaheadObstacles.AddRange(_obstacleTimestamps.Take(obstacleLookahead)
            .Select(x => (x, Instantiate(obstaclePrefab))).ToList());
        foreach (var nextLookaheadObstacle in _nextLookaheadObstacles)
        {
            if (nextLookaheadObstacle.Item1 == null) continue;
            nextLookaheadObstacle.Item2.GetComponent<Obstacle>().SetType(nextLookaheadObstacle.Item1.PrefabId);
            nextLookaheadObstacle.Item2.SetActive(false);
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
