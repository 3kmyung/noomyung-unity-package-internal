using System.Collections.Generic;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Application.Ports
{
    /// <summary>
    /// Defines a UI effect configuration that can be implemented by various sources.
    /// </summary>
    public interface IUIEffectDefinition
    {
        /// <summary>The type of effect this definition represents.</summary>
        EffectType EffectType { get; }

        /// <summary>Duration of the effect in seconds.</summary>
        float Duration { get; }

        /// <summary>Delay before the effect starts in seconds.</summary>
        float Delay { get; }

        /// <summary>Number of times to repeat the effect (-1 for infinite).</summary>
        int Loops { get; }

        /// <summary>Type of loop behavior.</summary>
        LoopType LoopType { get; }

        /// <summary>Easing type for the effect animation.</summary>
        EasingType EasingType { get; }

        /// <summary>Custom animation curve data for the effect.</summary>
        IReadOnlyList<KeyValuePair<float, float>> CustomCurveData { get; }

        /// <summary>
        /// Converts this effect definition to a domain EffectStep.
        /// </summary>
        /// <returns>Domain effect step</returns>
        EffectStep ToDomain();

        /// <summary>
        /// Gets effect-specific parameters as a dictionary.
        /// </summary>
        /// <returns>Effect parameters dictionary</returns>
        IReadOnlyDictionary<string, object> GetPayload();
    }
}
