using System.Collections.Generic;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Tests.Domain.Mocks
{
    /// <summary>
    /// 테스트용 IUIElementHandle 목(Mock) 구현체입니다.
    /// </summary>
    public class MockUIElementHandle : IUIElementHandle
    {
        private readonly List<ColorValue> _graphicColors = new();
        private readonly Dictionary<string, float> _materialFloats = new();
        private readonly Dictionary<string, ColorValue> _materialColors = new();
        private Vector3Value _originalPosition;
        private bool _originalPositionStored;

        /// <inheritdoc />
        public float Alpha { get; set; } = 1f;

        /// <inheritdoc />
        public Vector3Value AnchoredPosition { get; set; } = Vector3Value.Zero;

        /// <inheritdoc />
        public Vector3Value LocalScale { get; set; } = Vector3Value.One;

        /// <inheritdoc />
        public Vector3Value LocalPosition { get; set; } = Vector3Value.Zero;

        /// <inheritdoc />
        public bool IsActive { get; set; } = true;

        // 테스트용 검증 속성들
        public int SetGraphicColorCallCount { get; private set; }
        public int SetAllGraphicColorsCallCount { get; private set; }
        public int SetMaterialFloatCallCount { get; private set; }
        public int SetMaterialColorCallCount { get; private set; }
        public int StoreOriginalPositionCallCount { get; private set; }
        public int RestoreOriginalPositionCallCount { get; private set; }

        /// <summary>
        /// 그래픽 색상을 초기화합니다.
        /// </summary>
        /// <param name="colors">색상 배열</param>
        public void InitializeGraphicColors(params ColorValue[] colors)
        {
            _graphicColors.Clear();
            _graphicColors.AddRange(colors);
        }

        /// <inheritdoc />
        public IReadOnlyList<ColorValue> GetGraphicColors()
        {
            return _graphicColors.AsReadOnly();
        }

        /// <inheritdoc />
        public void SetGraphicColor(int index, ColorValue color)
        {
            SetGraphicColorCallCount++;

            while (_graphicColors.Count <= index)
            {
                _graphicColors.Add(ColorValue.White);
            }

            if (index >= 0 && index < _graphicColors.Count)
            {
                _graphicColors[index] = color;
            }
        }

        /// <inheritdoc />
        public void SetAllGraphicColors(ColorValue color)
        {
            SetAllGraphicColorsCallCount++;

            for (int i = 0; i < _graphicColors.Count; i++)
            {
                _graphicColors[i] = color;
            }
        }

        /// <inheritdoc />
        public float GetMaterialFloat(string propertyName)
        {
            return _materialFloats.TryGetValue(propertyName, out var value) ? value : 0f;
        }

        /// <inheritdoc />
        public void SetMaterialFloat(string propertyName, float value)
        {
            SetMaterialFloatCallCount++;
            _materialFloats[propertyName] = value;
        }

        /// <inheritdoc />
        public ColorValue GetMaterialColor(string propertyName)
        {
            return _materialColors.TryGetValue(propertyName, out var color) ? color : ColorValue.White;
        }

        /// <inheritdoc />
        public void SetMaterialColor(string propertyName, ColorValue color)
        {
            SetMaterialColorCallCount++;
            _materialColors[propertyName] = color;
        }

        /// <inheritdoc />
        public void StoreOriginalPosition()
        {
            StoreOriginalPositionCallCount++;
            _originalPosition = AnchoredPosition;
            _originalPositionStored = true;
        }

        /// <inheritdoc />
        public void RestoreOriginalPosition()
        {
            RestoreOriginalPositionCallCount++;

            if (_originalPositionStored)
            {
                AnchoredPosition = _originalPosition;
            }
        }

        /// <summary>
        /// 호출 횟수를 리셋합니다.
        /// </summary>
        public void ResetCallCounts()
        {
            SetGraphicColorCallCount = 0;
            SetAllGraphicColorsCallCount = 0;
            SetMaterialFloatCallCount = 0;
            SetMaterialColorCallCount = 0;
            StoreOriginalPositionCallCount = 0;
            RestoreOriginalPositionCallCount = 0;
        }
    }
}
