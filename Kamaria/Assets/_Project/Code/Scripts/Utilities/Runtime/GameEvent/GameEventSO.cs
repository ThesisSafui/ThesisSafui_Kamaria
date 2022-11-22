using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.Utilities.GameEvent
{
    [CreateAssetMenu(fileName = "New GameEvent", menuName = "ThesisSafui/Utilities/Game Event")]
    public sealed class GameEventSO : ScriptableObject
    {
        private List<GameEventListener> _listeners = new List<GameEventListener>();

        public void TriggerEvent()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventTriggered();
            }
        }

        public void AddListener(GameEventListener listener)
        {
            _listeners.Add(listener);
        }

        public void RemoveListener(GameEventListener listener)
        {
            _listeners.Remove(listener);
        }
    }
}