using System;
using System.Collections.Generic;
using UnityEngine;
using Noomyung.UI.Application.Interfaces;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Infrastructure.ScriptableObjects;
using Noomyung.UI.Domain.Interfaces;

namespace Noomyung.UI.Infrastructure.Runtime
{
    /// <summary>
    /// ScriptableObject 기반 전환 리포지토리입니다.
    /// </summary>
    public class ScriptableTransitionRepository : IUITransitionRepository
    {
        private readonly Dictionary<string, IUITransitionDefinition> _transitionAssets;

        /// <summary>
        /// ScriptableTransitionRepository의 새 인스턴스를 초기화합니다.
        /// </summary>
        public ScriptableTransitionRepository()
        {
            _transitionAssets = new Dictionary<string, IUITransitionDefinition>();
        }

        /// <summary>
        /// 전환 에셋을 등록합니다.
        /// </summary>
        /// <param name="id">전환 ID</param>
        /// <param name="asset">전환 에셋</param>
        public void RegisterTransition(string id, IUITransitionDefinition asset)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID cannot be null or empty", nameof(id));

            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            _transitionAssets[id] = asset;
        }

        /// <summary>
        /// 전환 에셋을 제거합니다.
        /// </summary>
        /// <param name="id">전환 ID</param>
        public void UnregisterTransition(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                _transitionAssets.Remove(id);
            }
        }

        /// <inheritdoc />
        public UITransitionSet GetFor(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("Transition ID is null or empty. Returning empty transition set.");
                return new UITransitionSet();
            }

            if (_transitionAssets.TryGetValue(id, out var asset) && asset != null)
            {
                return asset.ToDomain();
            }

            Debug.LogWarning($"Transition asset with ID '{id}' not found. Returning empty transition set.");
            return new UITransitionSet();
        }

        /// <summary>
        /// 등록된 모든 전환 ID를 가져옵니다.
        /// </summary>
        /// <returns>전환 ID 컬렉션</returns>
        public IReadOnlyCollection<string> GetRegisteredIds()
        {
            return _transitionAssets.Keys;
        }

        /// <summary>
        /// 지정된 ID의 전환이 등록되어 있는지 확인합니다.
        /// </summary>
        /// <param name="id">확인할 전환 ID</param>
        /// <returns>등록 여부</returns>
        public bool IsRegistered(string id)
        {
            return !string.IsNullOrEmpty(id) && _transitionAssets.ContainsKey(id);
        }

        /// <summary>
        /// 모든 등록된 전환을 제거합니다.
        /// </summary>
        public void Clear()
        {
            _transitionAssets.Clear();
        }
    }
}
