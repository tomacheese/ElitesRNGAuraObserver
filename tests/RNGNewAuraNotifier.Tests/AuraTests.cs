using Xunit;
using RNGNewAuraNotifier;

namespace RNGNewAuraNotifier.Tests
{
  public class AuraTests
  {
    [Theory]
    [InlineData("00", "Common")]
    [InlineData("60", "Celebration")]
    [InlineData("62", "Denied")]
    public void GetAuraName_ValidId_ReturnsName(string id, string expected)
    {
      var name = Aura.GetAuraName(id);
      Assert.Equal(expected, name);
    }

    [Theory]
    [InlineData("99")]
    [InlineData("")]
    public void GetAuraName_InvalidId_ReturnsNull(string id)
    {
      var name = Aura.GetAuraName(id);
      Assert.Null(name);
    }

    [Fact]
    public void GetAuraName_NullId_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => Aura.GetAuraName(null));
    }
  }
}