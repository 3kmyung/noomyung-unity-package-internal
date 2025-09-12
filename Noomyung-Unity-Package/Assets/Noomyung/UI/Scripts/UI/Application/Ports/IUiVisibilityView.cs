using System.Threading;
using System.Threading.Tasks;

namespace Noomyung.UI.Application.Ports
{
    /// <summary>
    /// UI 요소의 가시성을 관리하는 인터페이스입니다.
    /// GameObject의 active/inactive 상태를 제어합니다.
    /// </summary>
    public interface IUiVisibilityView
    {
        /// <summary>
        /// UI 요소를 표시합니다.
        /// UiTransitionView가 연결되어 있으면 전환 효과를 사용하고, 없으면 즉시 활성화합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        Task ShowAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// UI 요소를 숨깁니다.
        /// UiTransitionView가 연결되어 있으면 전환 효과를 사용하고, 없으면 즉시 비활성화합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        Task HideAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// UI 요소가 현재 표시되어 있는지 여부를 확인합니다.
        /// </summary>
        bool IsVisible { get; }
    }
}
