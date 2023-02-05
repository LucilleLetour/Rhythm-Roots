using UnityEngine;
using UnityEngine.VFX;

public class Root : MonoBehaviour
{

    [SerializeField] private VisualEffect vfx;
    
    [SerializeField] private VisualEffectAsset growMM; 
    [SerializeField] private VisualEffectAsset growML; 
    [SerializeField] private VisualEffectAsset growMR; 
    [SerializeField] private VisualEffectAsset growMS; 
    [SerializeField] private VisualEffectAsset growLM; 
    [SerializeField] private VisualEffectAsset growLL; 
    [SerializeField] private VisualEffectAsset growLR; 
    [SerializeField] private VisualEffectAsset growLS; 
    [SerializeField] private VisualEffectAsset growRM; 
    [SerializeField] private VisualEffectAsset growRL; 
    [SerializeField] private VisualEffectAsset growRR; 
    [SerializeField] private VisualEffectAsset growRS; 
    [SerializeField] private VisualEffectAsset growSM; 
    [SerializeField] private VisualEffectAsset growSL; 
    [SerializeField] private VisualEffectAsset growSR; 
    [SerializeField] private VisualEffectAsset growSS;

    public void StopVfx()
    {
        var position = transform.position;
        position = new Vector3(0, position.y, position.z);
        transform.position = position;
        transform.localScale = Vector3.one;
        vfx.Stop();
    }
    
    public void ConfigureVfx(LevelPlayer.ObstacleTypes from, LevelPlayer.ObstacleTypes to)
    {
        var visualEffectAsset = vfx.visualEffectAsset;
        switch (from)
        {
            case LevelPlayer.ObstacleTypes.Center:
                visualEffectAsset = to switch
                {
                    LevelPlayer.ObstacleTypes.Center => growMM,
                    LevelPlayer.ObstacleTypes.Left => growML,
                    LevelPlayer.ObstacleTypes.Right => growMR,
                    LevelPlayer.ObstacleTypes.Split => growMS,
                    _ => visualEffectAsset
                };
                break;
            case LevelPlayer.ObstacleTypes.Left:
                transform.position = new Vector3(-2, 0, 0);
                visualEffectAsset = to switch
                {
                    LevelPlayer.ObstacleTypes.Center => growLM,
                    LevelPlayer.ObstacleTypes.Left => growLL,
                    LevelPlayer.ObstacleTypes.Right => growLR,
                    LevelPlayer.ObstacleTypes.Split => growLS,
                    _ => visualEffectAsset
                };
                break;
            case LevelPlayer.ObstacleTypes.Right:
                transform.position = new Vector3(2, 0, 0);
                visualEffectAsset = to switch
                {
                    LevelPlayer.ObstacleTypes.Center => growRM,
                    LevelPlayer.ObstacleTypes.Left => growRL,
                    LevelPlayer.ObstacleTypes.Right => growRR,
                    LevelPlayer.ObstacleTypes.Split => growRS,
                    _ => visualEffectAsset
                };
                break;
            case LevelPlayer.ObstacleTypes.Split:
                visualEffectAsset = to switch
                {
                    LevelPlayer.ObstacleTypes.Center => growSM,
                    LevelPlayer.ObstacleTypes.Left => growSL,
                    LevelPlayer.ObstacleTypes.Right => growSR,
                    LevelPlayer.ObstacleTypes.Split => growSS,
                    _ => visualEffectAsset
                };
                break;
        }

        vfx.visualEffectAsset = visualEffectAsset;
    }
    
    public void StartVfx()
    {
        vfx.Reinit();
        vfx.Play();
    }
}
