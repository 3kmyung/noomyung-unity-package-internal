using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Application.Interfaces
{
    /// <summary>
    /// UI 전환 설정을 관리하는 리포지토리 인터페이스입니다.
    /// </summary>
    public interface IUITransitionRepository
    {
        /// <summary>
        /// 지정된 ID에 해당하는 UI 전환 설정을 가져옵니다.
        /// </summary>
        /// <param name="id">전환 설정 ID</param>
        /// <returns>UI 전환 설정</returns>
        UITransitionSet GetFor(string id);
    }
}
