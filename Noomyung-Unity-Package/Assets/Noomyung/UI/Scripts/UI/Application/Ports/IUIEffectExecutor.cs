using System.Threading;
using System.Threading.Tasks;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Application.Ports
{
    /// <summary>
    /// 개별 효과를 실행하는 인터페이스입니다.
    /// </summary>
    public interface IUIEffectExecutor
    {
        /// <summary>
        /// 지정된 UI 요소에 대해 효과를 비동기적으로 실행합니다.
        /// </summary>
        /// <param name="target">대상 UI 요소 핸들</param>
        /// <param name="effect">실행할 효과</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>완료 작업</returns>
        Task ExecuteAsync(IUIElementHandle target, Effect effect, CancellationToken cancellationToken = default);
    }
}
