using System.Collections.Generic;
using ConfOrm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class PatternExtensionsTest
	{
		[Test]
		public void ApplyFirstMatchInListReverseOrder()
		{
			// the user can add IPatternApplier. His IPatternApplier have more priority than defaults.
			// The first IPatternApplier should be the last to check match.
			var patterns = new List<IPatternApplier<string, string>>();
			var firstAddedPattern = new Mock<IPatternApplier<string, string>>();
			firstAddedPattern.Setup(p => p.Match(It.IsAny<string>())).Returns(true);
			var lastAddedPattern = new Mock<IPatternApplier<string, string>>();
			lastAddedPattern.Setup(p => p.Match(It.IsAny<string>())).Returns(true);
			patterns.Add(firstAddedPattern.Object);
			patterns.Add(lastAddedPattern.Object);

			patterns.ApplyFirstMatch("pp");

			firstAddedPattern.Verify(p => p.Apply(It.IsAny<string>()), Times.Never());
			lastAddedPattern.Verify(p => p.Apply(It.IsAny<string>()), Times.Once());
		}

		[Test]
		public void WhenNoMatchNothingIsApplied()
		{
			var patterns = new List<IPatternApplier<string, string>>();
			var pattern = new Mock<IPatternApplier<string, string>>();
			patterns.Add(pattern.Object);
			pattern.Setup(p => p.Match(It.IsAny<string>())).Returns(false);
			
			patterns.ApplyFirstMatch("pp");

			pattern.Verify(p => p.Apply(It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void WhenNullNothingHappen()
		{
			List<IPatternApplier<string, string>> patterns = null;

			ActionAssert.NotThrow(() => patterns.ApplyFirstMatch("pp"));
		}

		[Test]
		public void WhenHaveOneMatchThenMatch()
		{
			var patterns = new List<IPattern<string>>();
			var firstAddedPattern = new Mock<IPattern<string>>();
			firstAddedPattern.Setup(p => p.Match(It.IsAny<string>())).Returns(true);
			var lastAddedPattern = new Mock<IPattern<string>>();
			lastAddedPattern.Setup(p => p.Match(It.IsAny<string>())).Returns(false);
			patterns.Add(firstAddedPattern.Object);
			patterns.Add(lastAddedPattern.Object);

			patterns.Match("").Should().Be.True();
		}

		[Test]
		public void WhenHaveNoMatchThenNoMatch()
		{
			var patterns = new List<IPattern<string>>();
			var firstAddedPattern = new Mock<IPattern<string>>();
			firstAddedPattern.Setup(p => p.Match(It.IsAny<string>())).Returns(false);
			var lastAddedPattern = new Mock<IPattern<string>>();
			lastAddedPattern.Setup(p => p.Match(It.IsAny<string>())).Returns(false);
			patterns.Add(firstAddedPattern.Object);
			patterns.Add(lastAddedPattern.Object);

			patterns.Match("").Should().Be.False();
		}

		[Test]
		public void WhenEmptyThenNoMatch()
		{
			var patterns = new List<IPattern<string>>();
			patterns.Match("").Should().Be.False();
		}

		[Test]
		public void WhenNullThenNoMatch()
		{
			List<IPattern<string>> patterns = null;
			patterns.Match("").Should().Be.False();
		}
	}
}