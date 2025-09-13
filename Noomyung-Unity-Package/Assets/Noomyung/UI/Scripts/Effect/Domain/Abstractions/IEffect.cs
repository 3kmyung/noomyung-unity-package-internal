using System.Threading;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// UI 효과를 나타내는 인터페이스입니다.
    /// 각 효과는 자신의 데이터를 직접 가지고 실행할 수 있습니다.
    /// </summary>
    public interface IEffect
    {
        /// <summary>효과 유형</summary>
        EffectType Type { get; }

        /// <summary>타이밍 정보</summary>
        EffectTiming Timing { get; }

        /// <summary>이징 정보</summary>
        EffectEasing Easing { get; }

        /// <summary>
        /// 효과를 실행합니다.
        /// </summary>
        /// <param name="target">대상 UI 요소 핸들</param>
        /// <param name="reverse">역방향 실행 여부</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        UniTask ExecuteAsync(IUIElementHandle target, bool reverse, CancellationToken cancellationToken = default);
    }
}
