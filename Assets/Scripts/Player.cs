using RhythmReader;
using UnityEngine;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{

    [SerializeField] private Transform leftPlayer;
    [SerializeField] private Transform rightPlayer;
    [SerializeField] private VisualEffectAsset growM_M; 
    [SerializeField] private VisualEffectAsset growM_L; 
    [SerializeField] private VisualEffectAsset growM_R; 
    [SerializeField] private VisualEffectAsset growM_S; 
    [SerializeField] private VisualEffectAsset growL_M; 
    [SerializeField] private VisualEffectAsset growL_L; 
    [SerializeField] private VisualEffectAsset growL_R; 
    [SerializeField] private VisualEffectAsset growL_S; 
    [SerializeField] private VisualEffectAsset growR_M; 
    [SerializeField] private VisualEffectAsset growR_L; 
    [SerializeField] private VisualEffectAsset growR_R; 
    [SerializeField] private VisualEffectAsset growR_S; 
    [SerializeField] private VisualEffectAsset growS_M; 
    [SerializeField] private VisualEffectAsset growS_L; 
    [SerializeField] private VisualEffectAsset growS_R; 
    [SerializeField] private VisualEffectAsset growS_S;
    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private float branchSpacing;
    
    private float baselineY;
    private LevelPlayer.ObstacleTypes previousInput;
    private int _curBeat;

    public void SetBaselineY(float baseline)
    {
        baselineY = baseline;
    }

    public void OnBeat()
    {
        Debug.Log("PEEP");
        var branch = Instantiate(branchPrefab, this.transform);
        branch.transform.position = new Vector3(0, branchSpacing * _curBeat, 0);
        _curBeat++;
        var branchVfx = branch.GetComponent<VisualEffect>();
        var curInput = InputDecoder.DecodeInput();
        switch (curInput)
        {
            case LevelPlayer.ObstacleTypes.Center:
                switch (previousInput)
                {
                    case LevelPlayer.ObstacleTypes.Center:
                        branchVfx.visualEffectAsset = growM_M;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Left:
                        branchVfx.visualEffectAsset = growL_M;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Right:
                        branchVfx.visualEffectAsset = growR_M;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Split:
                        branchVfx.visualEffectAsset = growS_M;
                        branchVfx.Play();
                        break;
                }
                break;
            case LevelPlayer.ObstacleTypes.Left:
                switch (previousInput)
                {
                    case LevelPlayer.ObstacleTypes.Center:
                        branchVfx.visualEffectAsset = growM_L;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Left:
                        branchVfx.visualEffectAsset = growL_L;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Right:
                        branchVfx.visualEffectAsset = growR_L;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Split:
                        branchVfx.visualEffectAsset = growS_L;
                        branchVfx.Play();
                        break;
                }
                break;
            case LevelPlayer.ObstacleTypes.Right:
                switch (previousInput)
                {
                    case LevelPlayer.ObstacleTypes.Center:
                        branchVfx.visualEffectAsset = growM_R;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Left:
                        branchVfx.visualEffectAsset = growL_R;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Right:
                        branchVfx.visualEffectAsset = growR_R;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Split:
                        branchVfx.visualEffectAsset = growS_R;
                        branchVfx.Play();
                        break;
                }
                break;
            case LevelPlayer.ObstacleTypes.Split:
                switch (previousInput)
                {
                    case LevelPlayer.ObstacleTypes.Center:
                        branchVfx.visualEffectAsset = growM_S;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Left:
                        branchVfx.visualEffectAsset = growL_S;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Right:
                        branchVfx.visualEffectAsset = growR_S;
                        branchVfx.Play();
                        break;
                    case LevelPlayer.ObstacleTypes.Split:
                        branchVfx.visualEffectAsset = growS_S;
                        branchVfx.Play();
                        break;
                }
                break;
        }

        previousInput = curInput;
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
