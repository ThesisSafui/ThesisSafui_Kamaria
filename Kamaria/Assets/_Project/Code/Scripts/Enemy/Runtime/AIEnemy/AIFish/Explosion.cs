using System;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.Fish
{
    public sealed class Explosion : MonoBehaviour
    {
        [SerializeField] private float lifeTime;

        private void OnEnable()
        {
            Invoke(nameof(Close), lifeTime);
        }

        private void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}