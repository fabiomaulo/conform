using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace ConfOrmTests
{
	public class PatternExtensionsTest
	{
		[Test]
		public void GetValueFirstMatchInListReverseOrder()
		{
			// the user can add IPatternApplier. His IPatternApplier have more priority than defaults.
			// The first IPatternApplier should be the last to check match.
			var patterns = new List<IPatternValueGetter<string, string>>();
			var firstAddedPattern = new Mock<IPatternValueGetter<string, string>>();
			firstAddedPattern.Setup(p => p.Match(It.IsAny<string>())).Returns(true);
			var lastAddedPattern = new Mock<IPatternValueGetter<string, string>>();
			lastAddedPattern.Setup(p => p.Match(It.IsAny<string>())).Returns(true);
			patterns.Add(firstAddedPattern.Object);
			patterns.Add(lastAddedPattern.Object);

			patterns.GetValueOfFirstMatch("pp");

			firstAddedPattern.Verify(p => p.Get(It.IsAny<string>()), Times.Never());
			lastAddedPattern.Verify(p => p.Get(It.IsAny<string>()), Times.Once());
		}

		[Test]
		public void WhenNoMatchNothingIsApplied()
		{
			var patterns = new List<IPatternValueGetter<string, string>>();
			var pattern = new Mock<IPatternValueGetter<string, string>>();
			patterns.Add(pattern.Object);
			pattern.Setup(p => p.Match(It.IsAny<string>())).Returns(false);
			
			patterns.GetValueOfFirstMatch("pp");

			pattern.Verify(p => p.Get(It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void WhenNullNothingHappen()
		{
			List<IPatternValueGetter<string, string>> patterns = null;

			ActionAssert.NotThrow(() => patterns.GetValueOfFirstMatch("pp"));
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

		[Test]
		public void WhenAddToNullHolderThenThrow()
		{
			ICollection<IPattern<MemberInfo>> holder = null;
			holder.Executing(h=> h.Add(x=> false)).Throws<ArgumentNullException>();
		}

		[Test]
		public void WhenAddPatternMatcherThenAdd()
		{
			ICollection<IPattern<MemberInfo>> holder = new List<IPattern<MemberInfo>>();
			holder.Add(x => false);
			holder.Count.Should().Be(1);
		}
	}
}