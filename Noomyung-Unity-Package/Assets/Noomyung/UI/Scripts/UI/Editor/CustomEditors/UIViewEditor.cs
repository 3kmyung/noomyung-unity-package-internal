using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Noomyung.UI.Infrastructure.Runtime;
using Noomyung.UI.Infrastructure.ScriptableObjects;
using Noomyung.UI.Domain.Interfaces;

namespace Noomyung.UI.Infrastructure.Editor.CustomEditors
{
    /// <summary>
    /// Custom editor for UIView component with testing and validation features.
    /// </summary>
    [CustomEditor(typeof(UIView))]
    public class UIViewEditor : UnityEditor.Editor
    {
        private SerializedProperty _transitionAsset;
        private SerializedProperty _targetRectTransform;
        private SerializedProperty _canvasGroup;
        private SerializedProperty _graphics;

        private void OnEnable()
        {
            _transitionAsset = serializedObject.FindProperty("transitionAsset");
            _targetRectTransform = serializedObject.FindProperty("targetRectTransform");
            _canvasGroup = serializedObject.FindProperty("canvasGroup");
            _graphics = serializedObject.FindProperty("graphics");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var uiView = target as UIView;

            DrawHeader(uiView);
            DrawTransitionSettings();
            DrawComponentsSection(uiView);
            DrawTestingSection(uiView);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(UIView uiView)
        {
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("UI View Component", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space();
        }

        private void DrawTransitionSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(_transitionAsset, new GUIContent("전환 에셋", "사용할 전환 효과 에셋"));

            if (_transitionAsset.objectReferenceValue is UITransitionAsset asset)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox($"전환 정보: {asset.GetSummary()}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("전환 에셋이 할당되지 않았습니다.", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawComponentsSection(UIView uiView)
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(_targetRectTransform, new GUIContent("대상 RectTransform"));
            EditorGUILayout.PropertyField(_canvasGroup, new GUIContent("Canvas Group", "페이드 효과를 위한 CanvasGroup (선택사항)"));
            EditorGUILayout.PropertyField(_graphics, new GUIContent("Graphics", "색상/머티리얼 효과를 적용할 Graphic 컴포넌트들 (비워두면 자동 감지)"), true);

            // Graphics 필드에 대한 상세 정보 표시
            if (_graphics.arraySize > 0)
            {
                EditorGUILayout.Space(2);
                EditorGUILayout.HelpBox($"현재 {_graphics.arraySize}개의 Graphic 컴포넌트가 설정되어 있습니다.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.Space(2);
                EditorGUILayout.HelpBox("Graphics 배열이 비어있습니다. 자동 할당 버튼을 사용하거나 수동으로 추가하세요.", MessageType.Info);
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("자동 할당"))
            {
                AutoAssignComponents(uiView);
            }

            if (GUILayout.Button("Graphics 초기화"))
            {
                ClearGraphics(uiView);
            }

            if (GUILayout.Button("CanvasGroup 추가"))
            {
                AddCanvasGroup(uiView);
            }

            EditorGUILayout.EndHorizontal();

            DrawComponentValidation(uiView);

            EditorGUILayout.EndVertical();
        }

        private void DrawTestingSection(UIView uiView)
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("테스트 및 미리보기", EditorStyles.boldLabel);

            if (!UnityEngine.Application.isPlaying)
            {
                EditorGUILayout.HelpBox("전환 테스트는 플레이 모드에서만 가능합니다.", MessageType.Info);
            }

            if (_transitionAsset.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("전환 에셋이 할당되지 않았습니다.", MessageType.Warning);
            }

            using (new EditorGUI.DisabledScope(!UnityEngine.Application.isPlaying || _transitionAsset.objectReferenceValue == null))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Show"))
                {
                    _ = ExecuteTransitionSafely(() => uiView.ShowAsync());
                }

                if (GUILayout.Button("Hide"))
                {
                    _ = ExecuteTransitionSafely(() => uiView.HideAsync());
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Hover Enter"))
                {
                    _ = ExecuteTransitionSafely(() => uiView.HoverEnterAsync());
                }

                if (GUILayout.Button("Hover Exit"))
                {
                    _ = ExecuteTransitionSafely(() => uiView.HoverExitAsync());
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                if (GUILayout.Button("모든 전환 취소"))
                {
                    uiView.CancelAllTransitions();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void AutoAssignComponents(UIView uiView)
        {
            Undo.RecordObject(uiView, "Auto Assign Components");

            if (_targetRectTransform.objectReferenceValue == null)
            {
                _targetRectTransform.objectReferenceValue = uiView.GetComponent<RectTransform>();
            }

            if (_canvasGroup.objectReferenceValue == null)
            {
                _canvasGroup.objectReferenceValue = uiView.GetComponent<CanvasGroup>();
            }

            if (_graphics.arraySize == 0)
            {
                var graphicComponents = uiView.GetComponentsInChildren<Graphic>();
                _graphics.arraySize = graphicComponents.Length;

                for (int i = 0; i < graphicComponents.Length; i++)
                {
                    _graphics.GetArrayElementAtIndex(i).objectReferenceValue = graphicComponents[i];
                }
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(uiView);
        }

        private void AddCanvasGroup(UIView uiView)
        {
            if (uiView.GetComponent<CanvasGroup>() == null)
            {
                Undo.AddComponent<CanvasGroup>(uiView.gameObject);
                _canvasGroup.objectReferenceValue = uiView.GetComponent<CanvasGroup>();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(uiView);
            }
        }

        private void ClearGraphics(UIView uiView)
        {
            Undo.RecordObject(uiView, "Clear Graphics");
            _graphics.ClearArray();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(uiView);
        }

        private void DrawComponentValidation(UIView uiView)
        {
            EditorGUILayout.Space(5);

            var warnings = new System.Collections.Generic.List<string>();
            var infos = new System.Collections.Generic.List<string>();

            if (_targetRectTransform.objectReferenceValue == null)
                warnings.Add("RectTransform이 할당되지 않았습니다.");

            if (_transitionAsset.objectReferenceValue == null)
                warnings.Add("전환 에셋이 할당되지 않았습니다.");

            // Graphics 필드에 대한 상세 검증
            if (_graphics.arraySize == 0)
            {
                var availableGraphics = uiView.GetComponentsInChildren<Graphic>();
                if (availableGraphics.Length > 0)
                {
                    warnings.Add($"Graphic 컴포넌트가 할당되지 않았습니다. ({availableGraphics.Length}개의 Graphic이 자동 감지됨)");
                }
                else
                {
                    warnings.Add("Graphic 컴포넌트가 없습니다. (색상/머티리얼 효과를 사용하려면 Image, Text 등의 Graphic 컴포넌트가 필요)");
                }
            }
            else
            {
                // 할당된 Graphics 검증
                var validGraphics = 0;
                var nullGraphics = 0;

                for (int i = 0; i < _graphics.arraySize; i++)
                {
                    var graphic = _graphics.GetArrayElementAtIndex(i).objectReferenceValue as Graphic;
                    if (graphic == null)
                        nullGraphics++;
                    else
                        validGraphics++;
                }

                if (nullGraphics > 0)
                    warnings.Add($"{nullGraphics}개의 Graphic 참조가 null입니다.");

                if (validGraphics > 0)
                    infos.Add($"{validGraphics}개의 Graphic 컴포넌트가 설정되어 있습니다.");
            }

            // 전환 에셋에서 색상 효과 사용 여부 확인
            if (_transitionAsset.objectReferenceValue is UITransitionAsset asset)
            {
                var hasColorEffects = HasColorEffects(asset);
                if (hasColorEffects && _graphics.arraySize == 0)
                {
                    warnings.Add("전환 에셋에 색상 효과가 있지만 Graphic 컴포넌트가 설정되지 않았습니다.");
                }
                else if (hasColorEffects)
                {
                    infos.Add("전환 에셋에 색상 효과가 포함되어 있습니다.");
                }
            }

            if (warnings.Count > 0)
            {
                EditorGUILayout.HelpBox(
                    "검증 경고:\n• " + string.Join("\n• ", warnings),
                    MessageType.Warning);
            }

            if (infos.Count > 0)
            {
                EditorGUILayout.HelpBox(
                    "정보:\n• " + string.Join("\n• ", infos),
                    MessageType.Info);
            }

            if (warnings.Count == 0 && infos.Count == 0)
            {
                EditorGUILayout.HelpBox("모든 컴포넌트가 올바르게 설정되었습니다.", MessageType.Info);
            }
        }

        private bool HasColorEffects(UITransitionAsset asset)
        {
            var allEffects = asset.ShowEffects.AsEnumerable()
                .Concat(asset.HideEffects)
                .Concat(asset.HoverEnterEffects)
                .Concat(asset.HoverExitEffects);

            return allEffects.Any(effect => effect.EffectType == Domain.Enums.EffectType.Color);
        }

        private async System.Threading.Tasks.Task ExecuteTransitionSafely(System.Func<System.Threading.Tasks.Task> transitionFunc)
        {
            try
            {
                await transitionFunc();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Transition execution failed: {ex.Message}");
            }
        }
    }
}
