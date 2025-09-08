#if UNITASK_PRESENT
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Noomyung.UI.Infrastructure.Async
{
    /// <summary>
    /// UniTask 기반 비동기 브리지 구현체입니다.
    /// </summary>
    public class UniTaskAsyncBridge : IAsyncBridge
    {
        /// <inheritdoc />
        public Task ToTask(object uniTask)
        {
            return uniTask switch
            {
                UniTask ut => ut.AsTask(),
                UniTask<bool> utBool => utBool.AsTask(),
                UniTask<int> utInt => utInt.AsTask(),
                UniTask<float> utFloat => utFloat.AsTask(),
                UniTask<string> utString => utString.AsTask(),
                _ => Task.CompletedTask
            };
        }

        /// <inheritdoc />
        public object ToUniTask(Task task)
        {
            return task.AsUniTask();
        }
    }
}
#endif
