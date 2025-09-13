using System.Threading;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// Represents a UI effect that can execute animations on UI elements.
    /// </summary>
    public interface IEffect
    {
        EffectType Type { get; }
        EffectTiming Timing { get; }
        EffectEasing Easing { get; }

        /// <summary>
        /// Executes the effect on the target UI element.
        /// </summary>
        /// <param name="target">Target UI element handle</param>
        /// <param name="reverse">Whether to execute in reverse direction</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Async task</returns>
        UniTask ExecuteAsync(IUIElementHandle target, bool reverse, CancellationToken cancellationToken = default);
    }
}
