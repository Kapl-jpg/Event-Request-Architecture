using System;
using UnityEngine;

namespace ERA
{
    [Serializable]
    public class EventBinding
    {
        public GameObject owner;
        public Delegate Callback;
    }
}