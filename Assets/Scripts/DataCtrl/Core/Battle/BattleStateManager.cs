using System;
using UnityEngine;
using Zenject;

namespace Ebonor.DataCtrl
{
    public enum BattleState
    {
        NotStarted,
        Running,
        Paused,
        Ended
    }

    public class BattleStateManager : ITickable
    {
        public event Action<BattleState> StateChanged;
        public event Action<float> TimeChanged;

        public BattleState State { get; private set; } = BattleState.NotStarted;
        public float ElapsedSeconds { get; private set; }

        public void SetState(BattleState state)
        {
            if (State == state)
            {
                return;
            }

            State = state;
            StateChanged?.Invoke(State);
        }

        public void ResetTime()
        {
            ElapsedSeconds = 0f;
            TimeChanged?.Invoke(ElapsedSeconds);
        }

        public string GetFormattedTime()
        {
            var totalSeconds = Mathf.Max(0, Mathf.FloorToInt(ElapsedSeconds));
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }

        public void Tick()
        {
            if (State != BattleState.Running)
            {
                return;
            }

            ElapsedSeconds += Time.deltaTime;
            TimeChanged?.Invoke(ElapsedSeconds);
        }
    }
}
