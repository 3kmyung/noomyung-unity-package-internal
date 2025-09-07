using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Noomyung.UI.Infrastructure.Runtime;
using Noomyung.UI.Infrastructure.ScriptableObjects;

namespace Noomyung.UI.Tests.PlayMode
{
    /// <summary>
    /// UIView 컴포넌트에 대한 PlayMode 테스트입니다.
    /// </summary>
    [TestFixture]
    public class UIViewPlayModeTests
    {
        private GameObject _testGameObject;
        private UIView _uiView;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private UITransitionAsset _testTransitionAsset;

        [SetUp]
        public void SetUp()
        {
            // 테스트용 GameObject 생성
            _testGameObject = new GameObject("TestUIView");
            _rectTransform = _testGameObject.AddComponent<RectTransform>();
            _canvasGroup = _testGameObject.AddComponent<CanvasGroup>();
            _uiView = _testGameObject.AddComponent<UIView>();

            // 기본 이미지 컴포넌트 추가
            var image = _testGameObject.AddComponent<Image>();
            image.color = Color.white;

            // 테스트용 전환 에셋 생성
            _testTransitionAsset = CreateTestTransitionAsset();
            
            // UIView 설정
            _uiView.SetTransitionAsset(_testTransitionAsset);
        }

        [TearDown]
        public void TearDown()
        {
            if (_testGameObject != null)
            {
                Object.DestroyImmediate(_testGameObject);
            }

            if (_testTransitionAsset != null)
            {
                Object.DestroyImmediate(_testTransitionAsset);
            }
        }

        [UnityTest]
        public IEnumerator ShowAsync_FadeEffect_ChangesAlpha()
        {
            // Arrange
            _canvasGroup.alpha = 0f;
            
            // Act
            var showTask = _uiView.ShowAsync();
            
            // Wait for completion with timeout
            yield return WaitForTask(showTask, 3f);
            
            // Assert
            Assert.Greater(_canvasGroup.alpha, 0.5f, "Alpha should have increased after show transition");
        }

        [UnityTest]
        public IEnumerator HideAsync_FadeEffect_ChangesAlpha()
        {
            // Arrange
            _canvasGroup.alpha = 1f;
            
            // Act
            var hideTask = _uiView.HideAsync();
            
            // Wait for completion with timeout
            yield return WaitForTask(hideTask, 3f);
            
            // Assert
            Assert.Less(_canvasGroup.alpha, 0.5f, "Alpha should have decreased after hide transition");
        }

        [UnityTest]
        public IEnumerator HoverEnterAsync_ScaleEffect_ChangesScale()
        {
            // Arrange
            _rectTransform.localScale = Vector3.one;
            
            // Act
            var hoverTask = _uiView.HoverEnterAsync();
            
            // Wait for completion with timeout
            yield return WaitForTask(hoverTask, 3f);
            
            // Assert
            Assert.Greater(_rectTransform.localScale.x, 1.0f, "Scale should have increased after hover enter");
        }

        [UnityTest]
        public IEnumerator CancelAllTransitions_DuringTransition_StopsTransition()
        {
            // Arrange
            _canvasGroup.alpha = 0f;
            
            // Act
            var showTask = _uiView.ShowAsync();
            yield return new WaitForSeconds(0.1f); // 약간 기다림
            _uiView.CancelAllTransitions();
            
            // Wait a bit more
            yield return new WaitForSeconds(0.5f);
            
            // Assert
            // 전환이 취소되었으므로 알파값이 완전히 1에 도달하지 않아야 함
            Assert.Less(_canvasGroup.alpha, 1f, "Transition should have been cancelled");
        }

        private UITransitionAsset CreateTestTransitionAsset()
        {
            var asset = ScriptableObject.CreateInstance<UITransitionAsset>();
            
            // Show 효과: Fade In
            var fadeInEffect = ScriptableObject.CreateInstance<FadeEffectAsset>();
            SetPrivateField(fadeInEffect, "duration", 1f);
            SetPrivateField(fadeInEffect, "fromAlpha", 0f);
            SetPrivateField(fadeInEffect, "toAlpha", 1f);
            
            // Hide 효과: Fade Out
            var fadeOutEffect = ScriptableObject.CreateInstance<FadeEffectAsset>();
            SetPrivateField(fadeOutEffect, "duration", 1f);
            SetPrivateField(fadeOutEffect, "fromAlpha", 1f);
            SetPrivateField(fadeOutEffect, "toAlpha", 0f);
            
            // Hover Enter 효과: Scale Up
            var scaleUpEffect = ScriptableObject.CreateInstance<ScaleEffectAsset>();
            SetPrivateField(scaleUpEffect, "duration", 0.5f);
            SetPrivateField(scaleUpEffect, "fromScale", Vector3.one);
            SetPrivateField(scaleUpEffect, "toScale", Vector3.one * 1.1f);
            
            // Hover Exit 효과: Scale Down
            var scaleDownEffect = ScriptableObject.CreateInstance<ScaleEffectAsset>();
            SetPrivateField(scaleDownEffect, "duration", 0.5f);
            SetPrivateField(scaleDownEffect, "fromScale", Vector3.one * 1.1f);
            SetPrivateField(scaleDownEffect, "toScale", Vector3.one);
            
            // 전환 에셋에 효과들 할당
            SetPrivateField(asset, "showEffects", new UIEffectAsset[] { fadeInEffect });
            SetPrivateField(asset, "hideEffects", new UIEffectAsset[] { fadeOutEffect });
            SetPrivateField(asset, "hoverEnterEffects", new UIEffectAsset[] { scaleUpEffect });
            SetPrivateField(asset, "hoverExitEffects", new UIEffectAsset[] { scaleDownEffect });
            
            return asset;
        }

        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            field?.SetValue(obj, value);
        }

        private IEnumerator WaitForTask(Task task, float timeoutSeconds)
        {
            float elapsed = 0f;
            
            while (!task.IsCompleted && elapsed < timeoutSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            
            if (!task.IsCompleted)
            {
                Assert.Fail($"Task did not complete within {timeoutSeconds} seconds");
            }
            
            if (task.IsFaulted)
            {
                Assert.Fail($"Task failed with exception: {task.Exception?.GetBaseException()?.Message}");
            }
        }
    }
}
