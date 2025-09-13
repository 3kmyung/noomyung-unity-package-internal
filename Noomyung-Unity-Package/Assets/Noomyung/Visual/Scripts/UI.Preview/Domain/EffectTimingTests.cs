using NUnit.Framework;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Tests.Domain.ValueObjects
{
    /// <summary>
    /// EffectTiming 값 객체에 대한 단위 테스트입니다.
    /// </summary>
    [TestFixture]
    public class EffectTimingTests
    {
        [Test]
        public void Constructor_ValidValues_CreatesCorrectTiming()
        {
            // Arrange
            const float duration = 2.5f;
            const float delay = 0.5f;
            const int loops = 3;
            const LoopType loopType = LoopType.Yoyo;

            // Act
            var timing = new EffectTiming(duration, delay, loops, loopType);

            // Assert
            Assert.AreEqual(duration, timing.Duration);
            Assert.AreEqual(delay, timing.Delay);
            Assert.AreEqual(loops, timing.Loops);
            Assert.AreEqual(loopType, timing.LoopType);
        }

        [Test]
        public void Constructor_NegativeDuration_ClampsToZero()
        {
            // Arrange & Act
            var timing = new EffectTiming(-1.0f);

            // Assert
            Assert.AreEqual(0f, timing.Duration);
        }

        [Test]
        public void Constructor_NegativeDelay_ClampsToZero()
        {
            // Arrange & Act
            var timing = new EffectTiming(1.0f, -0.5f);

            // Assert
            Assert.AreEqual(0f, timing.Delay);
        }

        [Test]
        public void Constructor_InvalidLoops_ClampsToMinusOne()
        {
            // Arrange & Act
            var timing = new EffectTiming(1.0f, 0f, -5);

            // Assert
            Assert.AreEqual(-1, timing.Loops);
        }

        [Test]
        public void Default_Property_ReturnsCorrectValues()
        {
            // Act
            var defaultTiming = EffectTiming.Default;

            // Assert
            Assert.AreEqual(1f, defaultTiming.Duration);
            Assert.AreEqual(0f, defaultTiming.Delay);
            Assert.AreEqual(0, defaultTiming.Loops);
            Assert.AreEqual(LoopType.None, defaultTiming.LoopType);
        }

        [Test]
        public void Equals_SameValues_ReturnsTrue()
        {
            // Arrange
            var timing1 = new EffectTiming(1.5f, 0.2f, 2, LoopType.Restart);
            var timing2 = new EffectTiming(1.5f, 0.2f, 2, LoopType.Restart);

            // Act & Assert
            Assert.IsTrue(timing1.Equals(timing2));
            Assert.IsTrue(timing1 == timing2);
            Assert.IsFalse(timing1 != timing2);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            // Arrange
            var timing1 = new EffectTiming(1.5f, 0.2f, 2, LoopType.Restart);
            var timing2 = new EffectTiming(2.0f, 0.2f, 2, LoopType.Restart);

            // Act & Assert
            Assert.IsFalse(timing1.Equals(timing2));
            Assert.IsFalse(timing1 == timing2);
            Assert.IsTrue(timing1 != timing2);
        }

        [Test]
        public void GetHashCode_SameValues_ReturnsSameHash()
        {
            // Arrange
            var timing1 = new EffectTiming(1.5f, 0.2f, 2, LoopType.Restart);
            var timing2 = new EffectTiming(1.5f, 0.2f, 2, LoopType.Restart);

            // Act & Assert
            Assert.AreEqual(timing1.GetHashCode(), timing2.GetHashCode());
        }

        [Test]
        public void ToString_ValidTiming_ReturnsFormattedString()
        {
            // Arrange
            var timing = new EffectTiming(2.5f, 0.5f, 3, LoopType.Yoyo);

            // Act
            var result = timing.ToString();

            // Assert
            Assert.IsTrue(result.Contains("2.5"));
            Assert.IsTrue(result.Contains("0.5"));
            Assert.IsTrue(result.Contains("3"));
            Assert.IsTrue(result.Contains("Yoyo"));
        }
    }
}
