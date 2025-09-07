using UnityEditor;
using UnityEngine;
using Noomyung.UI.Infrastructure.ScriptableObjects;
using Noomyung.UI.Domain.Interfaces;

namespace Noomyung.UI.Infrastructure.Editor.CustomEditors
{
    /// <summary>
    /// Custom editor for UITransitionAsset with preview and utility features.
    /// </summary>
    [CustomEditor(typeof(UITransitionAsset))]
    public class UITransitionAssetEditor : UnityEditor.Editor
    {
        private SerializedProperty _ignoreTimeScale;
        private SerializedProperty _showEffects;
        private SerializedProperty _hideEffects;
        private SerializedProperty _hoverEnterEffects;
        private SerializedProperty _hoverExitEffects;

        private void OnEnable()
        {
            _ignoreTimeScale = serializedObject.FindProperty("ignoreTimeScale");
            _showEffects = serializedObject.FindProperty("showEffects");
            _hideEffects = serializedObject.FindProperty("hideEffects");
            _hoverEnterEffects = serializedObject.FindProperty("hoverEnterEffects");
            _hoverExitEffects = serializedObject.FindProperty("hoverExitEffects");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var asset = target as UITransitionAsset;

            DrawHeader(asset);
            DrawEffectsSection();
            DrawPreviewSection(asset);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(UITransitionAsset asset)
        {
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("UI Transition Asset", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space();

            var summary = asset?.GetSummary() ?? "No effects configured";
            EditorGUILayout.HelpBox(summary, MessageType.Info);

            EditorGUILayout.Space();
        }

        private void DrawEffectsSection()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("전환 효과 설정", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_ignoreTimeScale, new GUIContent("시간 스케일 무시", "Time.timeScale의 영향을 받지 않아 일시정지나 슬로우모드에서도 정상 속도로 실행"));
            EditorGUILayout.Space(5);

            DrawEffectArray("Show 전환", _showEffects);
            EditorGUILayout.Space(5);

            DrawEffectArray("Hide 전환", _hideEffects);
            EditorGUILayout.Space(5);

            DrawEffectArray("Hover Enter 전환", _hoverEnterEffects);
            EditorGUILayout.Space(5);

            DrawEffectArray("Hover Exit 전환", _hoverExitEffects);

            EditorGUILayout.EndVertical();
        }

        private void DrawEffectArray(string label, SerializedProperty property)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(property, new GUIContent("Effects"), true);

            if (property.arraySize > 1)
            {
                EditorGUILayout.Space(2);
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("▲ Move Up Selected", GUILayout.Width(120)))
                {
                }

                if (GUILayout.Button("▼ Move Down Selected", GUILayout.Width(120)))
                {
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DrawPreviewSection(UITransitionAsset asset)
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("미리보기 및 도구", EditorStyles.boldLabel);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("미리보기", EditorStyles.boldLabel);

            if (!UnityEngine.Application.isPlaying)
            {
                EditorGUILayout.HelpBox("미리보기 기능은 플레이 모드에서만 사용할 수 있습니다.", MessageType.Warning);
            }

            var uiView = FindFirstObjectByType<Runtime.UIView>();
            if (uiView == null)
            {
                EditorGUILayout.HelpBox("씬에 UIView 컴포넌트가 있는 GameObject가 필요합니다.", MessageType.Warning);
            }

            using (new EditorGUI.DisabledScope(!UnityEngine.Application.isPlaying || uiView == null))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Preview Show"))
                {
                    PreviewTransition(asset, "Show");
                }

                if (GUILayout.Button("Preview Hide"))
                {
                    PreviewTransition(asset, "Hide");
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Preview Hover Enter"))
                {
                    PreviewTransition(asset, "HoverEnter");
                }

                if (GUILayout.Button("Preview Hover Exit"))
                {
                    PreviewTransition(asset, "HoverExit");
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("유틸리티", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("복제하여 새 에셋 생성"))
            {
                DuplicateAsset(asset);
            }

            if (GUILayout.Button("기본 설정으로 리셋"))
            {
                ResetToDefaults(asset);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void PreviewTransition(UITransitionAsset asset, string triggerType)
        {
            if (asset == null) return;

            var uiView = FindFirstObjectByType<Runtime.UIView>();
            if (uiView != null)
            {
                Debug.Log($"Previewing {triggerType} transition for {uiView.name}");

                var originalAsset = uiView.TransitionAsset;
                uiView.SetTransitionAsset(asset);
                _ = SetupInitialStateAsync(uiView, triggerType);
                switch (triggerType)
                {
                    case "Show":
                        _ = ExecutePreviewAsync(() => uiView.ShowAsync());
                        break;
                    case "Hide":
                        _ = ExecutePreviewAsync(() => uiView.HideAsync());
                        break;
                    case "HoverEnter":
                        _ = ExecutePreviewAsync(() => uiView.HoverEnterAsync());
                        break;
                    case "HoverExit":
                        _ = ExecutePreviewAsync(() => uiView.HoverExitAsync());
                        break;
                }

                _ = RestoreOriginalAssetAsync(uiView, originalAsset);
            }
            else
            {
                Debug.LogWarning("씬에서 UIView 컴포넌트를 찾을 수 없습니다.");
            }
        }

        private async System.Threading.Tasks.Task ExecutePreviewAsync(System.Func<System.Threading.Tasks.Task> transitionFunc)
        {
            try
            {
                await transitionFunc();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Preview transition failed: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task SetupInitialStateAsync(Runtime.UIView uiView, string triggerType)
        {
            var rectTransform = uiView.GetComponent<RectTransform>();
            var canvasGroup = uiView.GetComponent<CanvasGroup>();

            if (rectTransform == null) return;

            var currentAsset = uiView.TransitionAsset;
            if (currentAsset != null)
            {
                if (triggerType == "Show")
                {
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 0f;
                    }
                    foreach (var effect in currentAsset.ShowEffects)
                    {
                        if (effect is MoveEffectAsset moveEffect)
                        {
                            rectTransform.anchoredPosition = new Vector2(moveEffect.FromPosition.x, moveEffect.FromPosition.y);
                            break;
                        }
                    }
                }
                else if (triggerType == "Hide")
                {
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 1f;
                    }
                    foreach (var effect in currentAsset.HideEffects)
                    {
                        if (effect is MoveEffectAsset moveEffect)
                        {
                            rectTransform.anchoredPosition = new Vector2(moveEffect.ToPosition.x, moveEffect.ToPosition.y);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (triggerType == "Show")
                {
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 0f;
                    }
                    rectTransform.anchoredPosition = Vector2.zero;
                }
                else if (triggerType == "Hide")
                {
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 1f;
                    }
                    rectTransform.anchoredPosition = new Vector2(200, 200);
                }
            }

            await System.Threading.Tasks.Task.Delay(100);
        }

        private async System.Threading.Tasks.Task RestoreOriginalAssetAsync(Runtime.UIView uiView, IUITransitionDefinition originalAsset)
        {
            await System.Threading.Tasks.Task.Delay(2000);
            uiView.SetTransitionAsset(originalAsset);
        }

        private void DuplicateAsset(UITransitionAsset asset)
        {
            if (asset == null) return;

            var path = AssetDatabase.GetAssetPath(asset);
            var newPath = AssetDatabase.GenerateUniqueAssetPath(path);

            if (AssetDatabase.CopyAsset(path, newPath))
            {
                AssetDatabase.SaveAssets();
                var newAsset = AssetDatabase.LoadAssetAtPath<UITransitionAsset>(newPath);
                Selection.activeObject = newAsset;
                EditorGUIUtility.PingObject(newAsset);
            }
        }

        private void ResetToDefaults(UITransitionAsset asset)
        {
            if (asset == null) return;

            if (EditorUtility.DisplayDialog("설정 리셋",
                "모든 효과 설정을 기본값으로 리셋하시겠습니까? 이 작업은 되돌릴 수 없습니다.",
                "리셋", "취소"))
            {
                Undo.RecordObject(asset, "Reset UI Transition Asset");

                _showEffects.ClearArray();
                _hideEffects.ClearArray();
                _hoverEnterEffects.ClearArray();
                _hoverExitEffects.ClearArray();

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(asset);
            }
        }
    }
}