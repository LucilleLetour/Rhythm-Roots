using System.Collections.Generic;
using UnityEngine;

public class OneShotPlayer : MonoBehaviour
{

    [SerializeField] private AudioSource player;
    [SerializeField] private List<AudioClip> rockHits;
    [SerializeField] private List<AudioClip> rootGrowths;

    public void PlayRockHit()
    {
        PlayFromList(rockHits);
    }
    
    public void PlayRootGrowth()
    {
        PlayFromList(rootGrowths);
    }

    private void PlayFromList(List<AudioClip> potentialClips)
    {
        player.pitch = Random.Range(0.8f, 1.2f);
        player.PlayOneShot(potentialClips[Random.Range(0, potentialClips.Count)]);
    }
}
