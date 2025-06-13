using System;
using System.Collections.Generic;
using UnityEngine;

namespace ERA
{
    public class EventAutoUnsubscribeProxy : MonoBehaviour
    {
        private readonly List<Action> _unsubscribeActions = new();

        public void Register(Action unsubscribe)
        {
            _unsubscribeActions.Add(unsubscribe);
        }

        private void OnDestroy()
        {
            foreach (var unsubscribe in _unsubscribeActions)
                unsubscribe?.Invoke();

            _unsubscribeActions.Clear();
        }
    }
}