using System.Drawing;
using System.Numerics;

namespace _3kmyung.Effect.Application.Ports
{
    /// <summary>
    /// Abstract handle for UI elements that provides domain-level interface without Unity dependencies.
    /// </summary>
    public interface IEffectElementPort
    {
        bool IsActive { get; set; }

        Vector3 LocalPosition { get; set; }
        Vector3 LocalScale { get; set; }

        float Alpha { get; set; }
        Color Color { get; set; }

        float GetValue(string propertyName);
        void SetValue(string propertyName, float value);
        Color GetColor(string propertyName);
        void SetColor(string propertyName, Color value);

        void StoreOriginalTransform();
        void RestoreOriginalTransform();
    }
}
