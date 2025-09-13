using System.Threading;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Application.Ports
{
    /// <summary>
    /// UI 효과를 실행하는 포트 인터페이스입니다.
    /// </summary>
    public interface IEffectPort
    {
        /// <summary>
        /// 효과를 실행합니다.
        /// </summary>
        /// <param name="target">대상 UI 요소 핸들</param>
        /// <param name="step">효과 단계 정의</param>
        /// <param name="reverse">역방향 실행 여부</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        UniTask ExecuteAsync(IUIElementHandle target, EffectStep step, bool reverse, CancellationToken cancellationToken = default);
    }
}
