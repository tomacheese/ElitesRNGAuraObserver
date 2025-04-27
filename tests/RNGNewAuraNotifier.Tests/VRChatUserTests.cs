using Xunit;
using RNGNewAuraNotifier;

namespace RNGNewAuraNotifier.Tests
{
  public class VRChatUserTests
  {
    [Fact]
    public void Properties_CanSetAndGet()
    {
      var user = new VRChatUser();
      user.UserName = "TestUser";
      user.UserId = "usr_12345";
      Assert.Equal("TestUser", user.UserName);
      Assert.Equal("usr_12345", user.UserId);

      // Allow empty and null
      user.UserName = null;
      user.UserId = string.Empty;
      Assert.Null(user.UserName);
      Assert.Equal(string.Empty, user.UserId);
    }
  }
}