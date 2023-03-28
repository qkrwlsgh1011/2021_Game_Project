using UnityEngine;
using System;

namespace PolyPerfect
{
    [Serializable]
    public class FeastState : AIState
    {
        public float minStateTime = 20f;
        public float maxStateTime = 40f;
    }
}