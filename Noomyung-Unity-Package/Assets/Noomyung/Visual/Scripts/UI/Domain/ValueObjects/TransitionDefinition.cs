using System;
using System.Collections.Generic;
using System.Linq;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// 특정 트리거에 대한 전환 정의를 나타내는 불변 값 객체입니다.
    /// </summary>
    public readonly struct TransitionDefinition : IEquatable<TransitionDefinition>
    {
        /// <summary>전환을 트리거하는 이벤트</summary>
        public EffectTrigger Trigger { get; }

        /// <summary>실행할 효과들</summary>
        public IReadOnlyList<Effect> Effects { get; }

        public TransitionDefinition(EffectTrigger trigger, IReadOnlyList<Effect> effects)
        {
            Trigger = trigger;
            Effects = effects ?? Array.Empty<Effect>();
        }

        public TransitionDefinition(EffectTrigger trigger, params Effect[] effects)
            : this(trigger, effects?.ToList() ?? new List<Effect>()) { }

        /// <summary>비어있는 전환 정의인지 확인합니다.</summary>
        public bool IsEmpty => Effects.Count == 0;

        public bool Equals(TransitionDefinition other) =>
            Trigger == other.Trigger &&
            Effects.SequenceEqual(other.Effects);

        public override bool Equals(object obj) => obj is TransitionDefinition other && Equals(other);

        public override int GetHashCode()
        {
            var hash = HashCode.Combine(Trigger);
            foreach (var effect in Effects)
            {
                hash = HashCode.Combine(hash, effect);
            }
            return hash;
        }

        public static bool operator ==(TransitionDefinition left, TransitionDefinition right) => left.Equals(right);
        public static bool operator !=(TransitionDefinition left, TransitionDefinition right) => !left.Equals(right);

        public override string ToString() => $"Trigger: {Trigger}, Effects: {Effects.Count}";
    }
}
