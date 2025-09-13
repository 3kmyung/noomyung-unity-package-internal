using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Application.Ports;

namespace Noomyung.UI.Infrastructure.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject that defines UI transition configurations for different triggers.
    /// </summary>
    public class UITransitionAsset : ScriptableObject, IUITransitionDefinition
    {
        [Header("전환 설정")]
        [SerializeField] private bool ignoreTimeScale = true;

        [Header("Show 전환")]
        [SerializeField] private UIEffectAsset[] showEffects = new UIEffectAsset[0];

        [Header("Hide 전환")]
        [SerializeField] private UIEffectAsset[] hideEffects = new UIEffectAsset[0];

        [Header("Hover Enter 전환")]
        [SerializeField] private UIEffectAsset[] hoverEnterEffects = new UIEffectAsset[0];

        [Header("Hover Exit 전환")]
        [SerializeField] private UIEffectAsset[] hoverExitEffects = new UIEffectAsset[0];

        /// <summary>Effects to play when showing the UI element.</summary>
        public IReadOnlyList<IUIEffectDefinition> ShowEffects => showEffects.Cast<IUIEffectDefinition>().ToList();

        /// <summary>Effects to play when hiding the UI element.</summary>
        public IReadOnlyList<IUIEffectDefinition> HideEffects => hideEffects.Cast<IUIEffectDefinition>().ToList();

        /// <summary>Effects to play when mouse enters the UI element.</summary>
        public IReadOnlyList<IUIEffectDefinition> HoverEnterEffects => hoverEnterEffects.Cast<IUIEffectDefinition>().ToList();

        /// <summary>Effects to play when mouse exits the UI element.</summary>
        public IReadOnlyList<IUIEffectDefinition> HoverExitEffects => hoverExitEffects.Cast<IUIEffectDefinition>().ToList();

        /// <summary>시간 스케일 무시 여부</summary>
        public bool IgnoreTimeScale => ignoreTimeScale;

        /// <summary>
        /// Converts this asset to a domain UITransitionSet object.
        /// </summary>
        /// <returns>Domain transition configuration</returns>
        public UITransitionSet ToDomain()
        {
            var show = CreateTransitionDefinition(EffectTrigger.Show, showEffects);
            var hide = CreateTransitionDefinition(EffectTrigger.Hide, hideEffects);
            var hoverEnter = CreateTransitionDefinition(EffectTrigger.HoverEnter, hoverEnterEffects);
            var hoverExit = CreateTransitionDefinition(EffectTrigger.HoverExit, hoverExitEffects);

            return new UITransitionSet(show, hide, hoverEnter, hoverExit);
        }

        private TransitionDefinition? CreateTransitionDefinition(EffectTrigger trigger, UIEffectAsset[] effects)
        {
            if (effects == null || effects.Length == 0)
                return null;

            var steps = effects
                .Where(effect => effect != null)
                .Select(effect => effect.ToDomain())
                .ToList();

            return steps.Count > 0 ? new TransitionDefinition(trigger, steps) : null;
        }

        /// <summary>
        /// Validates the configuration in the Inspector.
        /// </summary>
        private void OnValidate()
        {
            showEffects = showEffects?.Where(e => e != null).ToArray() ?? new UIEffectAsset[0];
            hideEffects = hideEffects?.Where(e => e != null).ToArray() ?? new UIEffectAsset[0];
            hoverEnterEffects = hoverEnterEffects?.Where(e => e != null).ToArray() ?? new UIEffectAsset[0];
            hoverExitEffects = hoverExitEffects?.Where(e => e != null).ToArray() ?? new UIEffectAsset[0];
        }

        /// <summary>
        /// Returns a summary of the transition configuration.
        /// </summary>
        /// <returns>Summary information</returns>
        public string GetSummary()
        {
            var summary = new List<string>();

            if (showEffects?.Length > 0)
                summary.Add($"Show: {showEffects.Length} effects");
            if (hideEffects?.Length > 0)
                summary.Add($"Hide: {hideEffects.Length} effects");
            if (hoverEnterEffects?.Length > 0)
                summary.Add($"HoverEnter: {hoverEnterEffects.Length} effects");
            if (hoverExitEffects?.Length > 0)
                summary.Add($"HoverExit: {hoverExitEffects.Length} effects");

            return summary.Count > 0 ? string.Join(", ", summary) : "No effects configured";
        }

        public override string ToString() => GetSummary();
    }
}
