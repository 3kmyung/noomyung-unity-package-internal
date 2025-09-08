using NUnit.Framework;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Tests.Domain.ValueObjects
{
    /// <summary>
    /// Vector3Value 값 객체에 대한 단위 테스트입니다.
    /// </summary>
    [TestFixture]
    public class Vector3ValueTests
    {
        [Test]
        public void Constructor_ValidValues_CreatesCorrectVector()
        {
            // Arrange
            const float x = 1.5f;
            const float y = -2.3f;
            const float z = 0.7f;

            // Act
            var vector = new Vector3Value(x, y, z);

            // Assert
            Assert.AreEqual(x, vector.X);
            Assert.AreEqual(y, vector.Y);
            Assert.AreEqual(z, vector.Z);
        }

        [Test]
        public void Zero_Property_ReturnsZeroVector()
        {
            // Act
            var zero = Vector3Value.Zero;

            // Assert
            Assert.AreEqual(0f, zero.X);
            Assert.AreEqual(0f, zero.Y);
            Assert.AreEqual(0f, zero.Z);
        }

        [Test]
        public void One_Property_ReturnsOneVector()
        {
            // Act
            var one = Vector3Value.One;

            // Assert
            Assert.AreEqual(1f, one.X);
            Assert.AreEqual(1f, one.Y);
            Assert.AreEqual(1f, one.Z);
        }

        [Test]
        public void Equals_SameValues_ReturnsTrue()
        {
            // Arrange
            var vector1 = new Vector3Value(1.5f, 2.3f, -0.7f);
            var vector2 = new Vector3Value(1.5f, 2.3f, -0.7f);

            // Act & Assert
            Assert.IsTrue(vector1.Equals(vector2));
            Assert.IsTrue(vector1 == vector2);
            Assert.IsFalse(vector1 != vector2);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            // Arrange
            var vector1 = new Vector3Value(1.5f, 2.3f, -0.7f);
            var vector2 = new Vector3Value(1.6f, 2.3f, -0.7f);

            // Act & Assert
            Assert.IsFalse(vector1.Equals(vector2));
            Assert.IsFalse(vector1 == vector2);
            Assert.IsTrue(vector1 != vector2);
        }

        [Test]
        public void Equals_VeryCloseValues_ReturnsTrue()
        {
            // Arrange
            var vector1 = new Vector3Value(1.0f, 2.0f, 3.0f);
            var vector2 = new Vector3Value(1.0f + float.Epsilon * 0.5f, 2.0f, 3.0f);

            // Act & Assert
            Assert.IsTrue(vector1.Equals(vector2));
        }

        [Test]
        public void GetHashCode_SameValues_ReturnsSameHash()
        {
            // Arrange
            var vector1 = new Vector3Value(1.5f, 2.3f, -0.7f);
            var vector2 = new Vector3Value(1.5f, 2.3f, -0.7f);

            // Act & Assert
            Assert.AreEqual(vector1.GetHashCode(), vector2.GetHashCode());
        }

        [Test]
        public void ToString_ValidVector_ReturnsFormattedString()
        {
            // Arrange
            var vector = new Vector3Value(1.234f, -5.678f, 0.0f);

            // Act
            var result = vector.ToString();

            // Assert
            Assert.IsTrue(result.Contains("1.23"));
            Assert.IsTrue(result.Contains("-5.68"));
            Assert.IsTrue(result.Contains("0.00"));
        }
    }
}
