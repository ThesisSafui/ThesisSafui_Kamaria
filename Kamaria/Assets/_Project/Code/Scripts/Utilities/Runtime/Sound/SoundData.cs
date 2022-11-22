using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.Utilities
{
    [CreateAssetMenu(fileName = "New Sound", menuName = "Sound")]
    public sealed class SoundData : ScriptableObject
    {
        [SerializeField] private SoundTypes soundType;
        [SerializeField] private List<SoundClip> soundClips = new List<SoundClip>();
        
        public SoundTypes SoundTypes => soundType;
        public List<SoundClip> SoundClips => soundClips;
    }

}