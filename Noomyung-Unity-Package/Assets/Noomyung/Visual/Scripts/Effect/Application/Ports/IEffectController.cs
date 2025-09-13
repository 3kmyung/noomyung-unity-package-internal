using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Effect.Domain.ValueObjects;

namespace _3kmyung.Effect.Application
{
    /// <summary>
    /// Interface for executing individual UI effects.
    /// </summary>
    public interface IEffectController
    {
        /// <summary>
        /// Executes the effect on the specified UI element asynchronously.
        /// </summary>
        /// <param name="target">Target UI element handle</param>
        /// <param name="effect">Effect to execute</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Completion task</returns>
        Task ExecuteAsync(IEffectElementPort target, IEffect effect, CancellationToken cancellationToken = default);
    }
}
