using UnityEngine;
using UnityEngine.Events;

namespace Kamaria.Utilities.GameEvent
{
    public sealed class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEventSO gameEventSO;
        [SerializeField] private UnityEvent onEventTriggered;

        private void OnEnable()
        {
            gameEventSO.AddListener(this);
        }

        private void OnDisable()
        {
            gameEventSO.RemoveListener(this);
        }

        public void OnEventTriggered()
        {
            onEventTriggered.Invoke();
        }
    }
}