using System.Threading;
using System.Threading.Tasks;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Application.Ports
{
    /// <summary>
    /// 전환 정의를 실행하는 인터페이스입니다.
    /// </summary>
    public interface IUITransitionRunner
    {
        /// <summary>
        /// 지정된 UI 요소에 대해 전환을 비동기적으로 실행합니다.
        /// </summary>
        /// <param name="target">대상 UI 요소 핸들</param>
        /// <param name="transition">실행할 전환 정의</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>완료 작업</returns>
        Task RunAsync(IUIElementHandle target, TransitionDefinition transition, CancellationToken cancellationToken = default);
    }
}
