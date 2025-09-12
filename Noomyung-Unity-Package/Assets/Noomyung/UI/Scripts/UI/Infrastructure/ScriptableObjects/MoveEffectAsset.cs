using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Infrastructure.ScriptableObjects
{
    /// <summary>
    /// 이동 효과의 공간 유형을 정의합니다.
    /// </summary>
    public enum MoveSpace
    {
        /// <summary>로컬 좌표계</summary>
        Local,

        /// <summary>앵커 기준 좌표계</summary>
        Anchored
    }

    /// <summary>
    /// 이동 효과 에셋입니다.
    /// </summary>
    public class MoveEffectAsset : UIEffectAsset
    {
        [Header("이동 설정")]
        [SerializeField] private Vector3 fromPosition = Vector3.zero;
        [SerializeField] private Vector3 toPosition = Vector3.zero;
        [SerializeField] private MoveSpace space = MoveSpace.Anchored;

        /// <inheritdoc />
        public override EffectType EffectType => EffectType.Move;

        /// <summary>시작 위치</summary>
        public Vector3 FromPosition => fromPosition;

        /// <summary>끝 위치</summary>
        public Vector3 ToPosition => toPosition;

        /// <summary>좌표 공간</summary>
        public MoveSpace Space => space;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, object> GetPayload()
        {
            return new Dictionary<string, object>
            {
                { "From", new Vector3(fromPosition.x, fromPosition.y, fromPosition.z) },
                { "To", new Vector3(toPosition.x, toPosition.y, toPosition.z) },
                { "Space", space.ToString() }
            };
        }
    }
}
