using System.Collections.Generic;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Application.Ports
{
    /// <summary>
    /// Defines a UI transition configuration that can be implemented by various sources.
    /// </summary>
    public interface IUITransitionDefinition
    {
        /// <summary>Effects to play when showing the UI element.</summary>
        IReadOnlyList<IUIEffectDefinition> ShowEffects { get; }

        /// <summary>Effects to play when hiding the UI element.</summary>
        IReadOnlyList<IUIEffectDefinition> HideEffects { get; }

        /// <summary>Effects to play when mouse enters the UI element.</summary>
        IReadOnlyList<IUIEffectDefinition> HoverEnterEffects { get; }

        /// <summary>Effects to play when mouse exits the UI element.</summary>
        IReadOnlyList<IUIEffectDefinition> HoverExitEffects { get; }

        /// <summary>시간 스케일 무시 여부</summary>
        bool IgnoreTimeScale { get; }

        /// <summary>
        /// Converts this transition definition to a domain UITransitionSet object.
        /// </summary>
        /// <returns>Domain transition configuration</returns>
        UITransitionSet ToDomain();

        /// <summary>
        /// Returns a summary of the transition configuration.
        /// </summary>
        /// <returns>Summary information</returns>
        string GetSummary();
    }
}
