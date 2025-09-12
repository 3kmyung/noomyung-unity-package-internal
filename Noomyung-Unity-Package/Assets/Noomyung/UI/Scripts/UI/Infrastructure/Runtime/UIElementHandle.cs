using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Infrastructure.Runtime
{
    /// <summary>
    /// Unity UI 요소에 대한 도메인 핸들 구현체입니다.
    /// </summary>
    public class UIElementHandle : IUIElementHandle
    {
        private readonly RectTransform _rectTransform;
        private readonly CanvasGroup _canvasGroup;
        private readonly Graphic[] _graphics;
        private readonly Material[] _materials;
        private Vector3Value _originalPosition;
        private bool _originalPositionStored;

        /// <summary>
        /// UIElementHandle의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="rectTransform">대상 RectTransform</param>
        /// <param name="canvasGroup">선택적 CanvasGroup</param>
        /// <param name="graphics">그래픽 컴포넌트들</param>
        public UIElementHandle(RectTransform rectTransform, CanvasGroup canvasGroup = null, Graphic[] graphics = null)
        {
            _rectTransform = rectTransform ?? throw new System.ArgumentNullException(nameof(rectTransform));
            _canvasGroup = canvasGroup;
            _graphics = graphics ?? _rectTransform.GetComponentsInChildren<Graphic>();
            _materials = _graphics?.Select(g => g.material).Where(m => m != null).ToArray() ?? new Material[0];
        }

        /// <inheritdoc />
        public float Alpha
        {
            get => _canvasGroup?.alpha ?? 1f;
            set
            {
                if (_canvasGroup != null)
                    _canvasGroup.alpha = Mathf.Clamp01(value);
            }
        }

        /// <inheritdoc />
        public Vector3Value AnchoredPosition
        {
            get
            {
                var pos = _rectTransform.anchoredPosition3D;
                return new Vector3Value(pos.x, pos.y, pos.z);
            }
            set => _rectTransform.anchoredPosition3D = new Vector3(value.X, value.Y, value.Z);
        }

        /// <inheritdoc />
        public Vector3Value LocalScale
        {
            get
            {
                var scale = _rectTransform.localScale;
                return new Vector3Value(scale.x, scale.y, scale.z);
            }
            set => _rectTransform.localScale = new Vector3(value.X, value.Y, value.Z);
        }

        /// <inheritdoc />
        public Vector3Value LocalPosition
        {
            get
            {
                var pos = _rectTransform.localPosition;
                return new Vector3Value(pos.x, pos.y, pos.z);
            }
            set => _rectTransform.localPosition = new Vector3(value.X, value.Y, value.Z);
        }

        /// <inheritdoc />
        public bool IsActive
        {
            get => _rectTransform.gameObject.activeInHierarchy;
            set => _rectTransform.gameObject.SetActive(value);
        }

        /// <inheritdoc />
        public IReadOnlyList<ColorValue> GetGraphicColors()
        {
            return _graphics?.Select(g => ConvertToDomainColor(g.color)).ToList() ?? new List<ColorValue>();
        }

        /// <inheritdoc />
        public void SetGraphicColor(int index, ColorValue color)
        {
            if (_graphics != null && index >= 0 && index < _graphics.Length)
            {
                _graphics[index].color = ConvertToUnityColor(color);
            }
        }

        /// <inheritdoc />
        public void SetAllGraphicColors(ColorValue color)
        {
            if (_graphics == null) return;

            var unityColor = ConvertToUnityColor(color);
            foreach (var graphic in _graphics)
            {
                graphic.color = unityColor;
            }
        }

        /// <inheritdoc />
        public float GetMaterialFloat(string propertyName)
        {
            return _materials?.FirstOrDefault()?.GetFloat(propertyName) ?? 0f;
        }

        /// <inheritdoc />
        public void SetMaterialFloat(string propertyName, float value)
        {
            if (_materials == null) return;

            foreach (var material in _materials)
            {
                if (material != null && material.HasProperty(propertyName))
                    material.SetFloat(propertyName, value);
            }
        }

        /// <inheritdoc />
        public ColorValue GetMaterialColor(string propertyName)
        {
            var color = _materials?.FirstOrDefault()?.GetColor(propertyName) ?? Color.white;
            return ConvertToDomainColor(color);
        }

        /// <inheritdoc />
        public void SetMaterialColor(string propertyName, ColorValue color)
        {
            if (_materials == null) return;

            var unityColor = ConvertToUnityColor(color);
            foreach (var material in _materials)
            {
                if (material != null && material.HasProperty(propertyName))
                    material.SetColor(propertyName, unityColor);
            }
        }

        /// <inheritdoc />
        public void StoreOriginalPosition()
        {
            _originalPosition = AnchoredPosition;
            _originalPositionStored = true;
        }

        /// <inheritdoc />
        public void RestoreOriginalPosition()
        {
            if (_originalPositionStored)
            {
                AnchoredPosition = _originalPosition;
            }
        }

        private static ColorValue ConvertToDomainColor(Color unityColor)
        {
            return new ColorValue(unityColor.r, unityColor.g, unityColor.b, unityColor.a);
        }

        private static Color ConvertToUnityColor(ColorValue domainColor)
        {
            return new Color(domainColor.R, domainColor.G, domainColor.B, domainColor.A);
        }
    }
}
