using UnityEngine;

namespace RhythmReader
{
    public class InputDecoder
    {
        public static LevelPlayer.ObstacleTypes DecodeInput()
        {
            var right = Input.GetKey(KeyCode.RightArrow);
            var left = Input.GetKey(KeyCode.LeftArrow);
            if (!right && !left)
            {
                return LevelPlayer.ObstacleTypes.Center;
            }
            else if (!right && left)
            {
                return LevelPlayer.ObstacleTypes.Left;
            }
            else if (right && !left)
            {
                return LevelPlayer.ObstacleTypes.Right;
            }
            else 
            {
                return LevelPlayer.ObstacleTypes.Split; // right and left
            }
        }
    }
}