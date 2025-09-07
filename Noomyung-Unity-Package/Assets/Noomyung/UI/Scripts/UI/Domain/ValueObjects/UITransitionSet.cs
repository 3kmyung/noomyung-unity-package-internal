using System;
using System.Linq;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// UI 요소의 모든 전환 정의를 포함하는 불변 값 객체입니다.
    /// </summary>
    public readonly struct UITransitionSet : IEquatable<UITransitionSet>
    {
        /// <summary>Show 트리거 전환</summary>
        public TransitionDefinition? Show { get; }
        
        /// <summary>Hide 트리거 전환</summary>
        public TransitionDefinition? Hide { get; }
        
        /// <summary>HoverEnter 트리거 전환</summary>
        public TransitionDefinition? HoverEnter { get; }
        
        /// <summary>HoverExit 트리거 전환</summary>
        public TransitionDefinition? HoverExit { get; }

        public UITransitionSet(
            TransitionDefinition? show = null,
            TransitionDefinition? hide = null,
            TransitionDefinition? hoverEnter = null,
            TransitionDefinition? hoverExit = null)
        {
            Show = show;
            Hide = hide;
            HoverEnter = hoverEnter;
            HoverExit = hoverExit;
        }

        /// <summary>지정된 트리거에 해당하는 전환 정의를 가져옵니다.</summary>
        public TransitionDefinition? GetTransition(EffectTrigger trigger)
        {
            return trigger switch
            {
                EffectTrigger.Show => Show,
                EffectTrigger.Hide => Hide,
                EffectTrigger.HoverEnter => HoverEnter,
                EffectTrigger.HoverExit => HoverExit,
                _ => null
            };
        }

        /// <summary>지정된 트리거에 해당하는 전환이 존재하는지 확인합니다.</summary>
        public bool HasTransition(EffectTrigger trigger) => GetTransition(trigger).HasValue;

        /// <summary>모든 전환이 비어있는지 확인합니다.</summary>
        public bool IsEmpty => !Show.HasValue && !Hide.HasValue && !HoverEnter.HasValue && !HoverExit.HasValue;

        public bool Equals(UITransitionSet other) =>
            Equals(Show, other.Show) &&
            Equals(Hide, other.Hide) &&
            Equals(HoverEnter, other.HoverEnter) &&
            Equals(HoverExit, other.HoverExit);

        public override bool Equals(object obj) => obj is UITransitionSet other && Equals(other);
        
        public override int GetHashCode() => HashCode.Combine(Show, Hide, HoverEnter, HoverExit);

        public static bool operator ==(UITransitionSet left, UITransitionSet right) => left.Equals(right);
        public static bool operator !=(UITransitionSet left, UITransitionSet right) => !left.Equals(right);

        public override string ToString()
        {
            var transitions = new[]
            {
                Show.HasValue ? "Show" : null,
                Hide.HasValue ? "Hide" : null,
                HoverEnter.HasValue ? "HoverEnter" : null,
                HoverExit.HasValue ? "HoverExit" : null
            };
            
            return $"Transitions: [{string.Join(", ", transitions.Where(t => t != null))}]";
        }
    }
}
