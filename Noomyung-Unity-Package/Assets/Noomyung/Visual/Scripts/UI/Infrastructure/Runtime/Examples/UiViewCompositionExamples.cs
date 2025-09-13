using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Infrastructure.Runtime.Views;

namespace Noomyung.UI.Infrastructure.Runtime.Examples
{
    /// <summary>
    /// UI 뷰 조합 사용 예제들을 보여주는 클래스입니다.
    /// 각 예제는 서로 다른 조합의 UI 뷰 컴포넌트들을 사용합니다.
    /// </summary>
    public class UiViewCompositionExamples : MonoBehaviour
    {
        [Header("Example GameObjects")]
        [SerializeField] private GameObject _exampleA; // UiVisibilityView만 있는 GameObject
        [SerializeField] private GameObject _exampleB; // UiVisibilityView + UiTransitionView
        [SerializeField] private GameObject _exampleC; // UiHoverView만 있는 GameObject
        [SerializeField] private GameObject _exampleD; // UiStateView가 있는 GameObject

        [Header("Test Controls")]
        [SerializeField] private KeyCode _toggleExampleA = KeyCode.Alpha1;
        [SerializeField] private KeyCode _toggleExampleB = KeyCode.Alpha2;
        [SerializeField] private KeyCode _testHoverExampleC = KeyCode.Alpha3;
        [SerializeField] private KeyCode _cycleStateExampleD = KeyCode.Alpha4;

        private UiState _currentStateForExampleD = UiState.Normal;

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(_toggleExampleA))
            {
                _ = ToggleExampleA();
            }

            if (Input.GetKeyDown(_toggleExampleB))
            {
                _ = ToggleExampleB();
            }

            if (Input.GetKeyDown(_testHoverExampleC))
            {
                _ = TestHoverExampleC();
            }

            if (Input.GetKeyDown(_cycleStateExampleD))
            {
                _ = CycleStateExampleD();
            }
        }

        /// <summary>
        /// 예제 A: UiVisibilityView만 있는 GameObject
        /// 직접 On/Off (SetActive) - 전환 효과 없음
        /// </summary>
        private async Task ToggleExampleA()
        {
            if (_exampleA == null) return;

            var visibilityView = _exampleA.GetComponent<IUiVisibilityView>();
            if (visibilityView == null)
            {
                Debug.LogWarning("Example A: UiVisibilityView component not found!");
                return;
            }

            if (visibilityView.IsVisible)
            {
                await visibilityView.HideAsync();
                Debug.Log("Example A: Hidden (immediate SetActive(false))");
            }
            else
            {
                await visibilityView.ShowAsync();
                Debug.Log("Example A: Shown (immediate SetActive(true))");
            }
        }

        /// <summary>
        /// 예제 B: UiVisibilityView + UiTransitionView
        /// VisibilityView가 요청하면, TransitionView가 애니메이션 + SetActive 처리
        /// </summary>
        private async Task ToggleExampleB()
        {
            if (_exampleB == null) return;

            var visibilityView = _exampleB.GetComponent<IUiVisibilityView>();
            if (visibilityView == null)
            {
                Debug.LogWarning("Example B: UiVisibilityView component not found!");
                return;
            }

            var transitionView = _exampleB.GetComponent<IUiTransitionView>();
            if (transitionView == null)
            {
                Debug.LogWarning("Example B: UiTransitionView component not found!");
                return;
            }

            if (visibilityView.IsVisible)
            {
                await visibilityView.HideAsync();
                Debug.Log("Example B: Hidden with transition effect");
            }
            else
            {
                await visibilityView.ShowAsync();
                Debug.Log("Example B: Shown with transition effect");
            }
        }

        /// <summary>
        /// 예제 C: UiHoverView만 있는 GameObject
        /// 호버 하이라이트 효과만
        /// </summary>
        private async Task TestHoverExampleC()
        {
            if (_exampleC == null) return;

            var hoverView = _exampleC.GetComponent<IUiHoverView>();
            if (hoverView == null)
            {
                Debug.LogWarning("Example C: UiHoverView component not found!");
                return;
            }

            if (hoverView.IsHovered)
            {
                await hoverView.HoverExitAsync();
                Debug.Log("Example C: Hover exit effect");
            }
            else
            {
                await hoverView.HoverEnterAsync();
                Debug.Log("Example C: Hover enter effect");
            }
        }

        /// <summary>
        /// 예제 D: UiStateView
        /// 여러 상태 관리 (Normal → Hovered → Pressed → Disabled → Selected → Normal)
        /// </summary>
        private async Task CycleStateExampleD()
        {
            if (_exampleD == null) return;

            var stateView = _exampleD.GetComponent<IUiStateView>();
            if (stateView == null)
            {
                Debug.LogWarning("Example D: UiStateView component not found!");
                return;
            }

            // 상태 순환
            _currentStateForExampleD = _currentStateForExampleD switch
            {
                UiState.Normal => UiState.Hovered,
                UiState.Hovered => UiState.Pressed,
                UiState.Pressed => UiState.Disabled,
                UiState.Disabled => UiState.Selected,
                UiState.Selected => UiState.Normal,
                _ => UiState.Normal
            };

            await stateView.ChangeStateAsync(_currentStateForExampleD);
            Debug.Log($"Example D: State changed to {_currentStateForExampleD}");
        }

        /// <summary>
        /// 모든 예제를 초기 상태로 리셋합니다.
        /// </summary>
        [ContextMenu("Reset All Examples")]
        public async Task ResetAllExamples()
        {
            // 예제 A 리셋
            if (_exampleA != null)
            {
                var visibilityViewA = _exampleA.GetComponent<IUiVisibilityView>();
                if (visibilityViewA != null && !visibilityViewA.IsVisible)
                {
                    await visibilityViewA.ShowAsync();
                }
            }

            // 예제 B 리셋
            if (_exampleB != null)
            {
                var visibilityViewB = _exampleB.GetComponent<IUiVisibilityView>();
                if (visibilityViewB != null && !visibilityViewB.IsVisible)
                {
                    await visibilityViewB.ShowAsync();
                }
            }

            // 예제 C 리셋
            if (_exampleC != null)
            {
                var hoverView = _exampleC.GetComponent<IUiHoverView>();
                if (hoverView != null && hoverView.IsHovered)
                {
                    await hoverView.HoverExitAsync();
                }
            }

            // 예제 D 리셋
            if (_exampleD != null)
            {
                var stateView = _exampleD.GetComponent<IUiStateView>();
                if (stateView != null)
                {
                    await stateView.ChangeStateAsync(UiState.Normal);
                    _currentStateForExampleD = UiState.Normal;
                }
            }

            Debug.Log("All examples reset to initial state");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("UI View Composition Examples", GUI.skin.box);
            GUILayout.Space(10);

            GUILayout.Label($"Press {_toggleExampleA} - Toggle Example A (Visibility only)");
            GUILayout.Label($"Press {_toggleExampleB} - Toggle Example B (Visibility + Transition)");
            GUILayout.Label($"Press {_testHoverExampleC} - Test Example C (Hover effect)");
            GUILayout.Label($"Press {_cycleStateExampleD} - Cycle Example D (State changes)");

            GUILayout.Space(10);
            if (GUILayout.Button("Reset All Examples"))
            {
                _ = ResetAllExamples();
            }

            GUILayout.EndArea();
        }
    }
}
