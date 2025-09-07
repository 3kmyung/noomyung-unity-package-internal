using UnityEditor;
using UnityEngine;
using Noomyung.UI.Infrastructure.ScriptableObjects;

namespace Noomyung.UI.Infrastructure.Editor.MenuItems
{
    /// <summary>
    /// UI 효과 에셋 생성을 위한 메뉴 아이템들입니다.
    /// </summary>
    public static class UIEffectMenuItems
    {
        private const string MenuRoot = "Assets/Create/Noomyung UI/";
        private const string EffectsPath = MenuRoot + "Effects/";
        
        [MenuItem(EffectsPath + "Fade Effect", false, 1)]
        public static void CreateFadeEffect()
        {
            CreateEffectAsset<FadeEffectAsset>("New Fade Effect");
        }
        
        [MenuItem(EffectsPath + "Scale Effect", false, 2)]
        public static void CreateScaleEffect()
        {
            CreateEffectAsset<ScaleEffectAsset>("New Scale Effect");
        }
        
        [MenuItem(EffectsPath + "Move Effect", false, 3)]
        public static void CreateMoveEffect()
        {
            CreateEffectAsset<MoveEffectAsset>("New Move Effect");
        }
        
        [MenuItem(EffectsPath + "Color Effect", false, 4)]
        public static void CreateColorEffect()
        {
            CreateEffectAsset<ColorEffectAsset>("New Color Effect");
        }
        
        [MenuItem(EffectsPath + "Material Float Effect", false, 5)]
        public static void CreateMaterialFloatEffect()
        {
            CreateEffectAsset<MaterialFloatEffectAsset>("New Material Float Effect");
        }
        
        [MenuItem(EffectsPath + "Material Color Effect", false, 6)]
        public static void CreateMaterialColorEffect()
        {
            CreateEffectAsset<MaterialColorEffectAsset>("New Material Color Effect");
        }
        
        [MenuItem(EffectsPath + "Shake Effect", false, 7)]
        public static void CreateShakeEffect()
        {
            CreateEffectAsset<ShakeEffectAsset>("New Shake Effect");
        }
        
        [MenuItem(MenuRoot + "UI Transition Asset", false, 100)]
        public static void CreateUITransitionAsset()
        {
            CreateEffectAsset<UITransitionAsset>("New UI Transition");
        }
        
        private static void CreateEffectAsset<T>(string defaultName) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!System.IO.Directory.Exists(path))
            {
                path = System.IO.Path.GetDirectoryName(path);
            }
            
            var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{defaultName}.asset");
            
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
