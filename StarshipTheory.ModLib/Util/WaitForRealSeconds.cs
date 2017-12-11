using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarshipTheory.ModLib.Util
{
    public sealed class WaitForRealSeconds : CustomYieldInstruction
    {
        private readonly float _endTime;

        public override bool keepWaiting
        {
            get { return _endTime > Time.realtimeSinceStartup; }
        }

        public WaitForRealSeconds(float seconds)
        {
            _endTime = Time.realtimeSinceStartup + seconds;
        }
    }
}
