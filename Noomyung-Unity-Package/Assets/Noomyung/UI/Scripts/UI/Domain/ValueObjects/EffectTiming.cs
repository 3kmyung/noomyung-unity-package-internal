using System;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// 효과의 타이밍 정보를 나타내는 불변 값 객체입니다.
    /// </summary>
    public readonly struct EffectTiming : IEquatable<EffectTiming>
    {
        /// <summary>효과의 지속 시간 (초)</summary>
        public float Duration { get; }
        
        /// <summary>효과 시작 전 지연 시간 (초)</summary>
        public float Delay { get; }
        
        /// <summary>반복 횟수 (-1은 무한 반복)</summary>
        public int Loops { get; }
        
        /// <summary>반복 유형</summary>
        public LoopType LoopType { get; }

        public EffectTiming(float duration, float delay = 0f, int loops = 0, LoopType loopType = LoopType.None)
        {
            Duration = Math.Max(0f, duration);
            Delay = Math.Max(0f, delay);
            Loops = Math.Max(-1, loops);
            LoopType = loopType;
        }

        /// <summary>기본 타이밍 설정 (1초 지속, 지연 없음, 반복 없음)</summary>
        public static EffectTiming Default => new(1f);

        public bool Equals(EffectTiming other) =>
            Math.Abs(Duration - other.Duration) < float.Epsilon &&
            Math.Abs(Delay - other.Delay) < float.Epsilon &&
            Loops == other.Loops &&
            LoopType == other.LoopType;

        public override bool Equals(object obj) => obj is EffectTiming other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Duration, Delay, Loops, LoopType);

        public static bool operator ==(EffectTiming left, EffectTiming right) => left.Equals(right);
        public static bool operator !=(EffectTiming left, EffectTiming right) => !left.Equals(right);

        public override string ToString() => $"Duration: {Duration}s, Delay: {Delay}s, Loops: {Loops}, Type: {LoopType}";
    }
}
