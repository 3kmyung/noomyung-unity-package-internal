namespace Noomyung.UI.Infrastructure.Async
{
    /// <summary>
    /// 환경에 맞는 비동기 브리지를 생성하는 팩토리입니다.
    /// </summary>
    public static class AsyncBridgeFactory
    {
        /// <summary>
        /// 현재 환경에 맞는 비동기 브리지를 생성합니다.
        /// </summary>
        /// <returns>비동기 브리지 인스턴스</returns>
        public static IAsyncBridge Create()
        {
#if UNITASK_PRESENT
            return new UniTaskAsyncBridge();
#else
            return new StandardAsyncBridge();
#endif
        }
    }
}
