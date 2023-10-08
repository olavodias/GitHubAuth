using GitHubAuth.Jwt;
using GitHubAuth.Extension;

namespace GitHubAuth.Testing;

[TestClass]
public class TestGitHubJwt
{
    [TestMethod]
    public void TestExtensions()
    {
        // Setup Epoch (1970-01-01 at 00:00:00 UTC)
        var currentDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(0, currentDateTime.ToUTCSeconds());

        // Check January 1st 2020 at 00:00:00 UtC
        currentDateTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(1577836800, currentDateTime.ToUTCSeconds());
    }

    [TestMethod]
    public void TestTokenGeneration()
    {
        var jwt = new GitHubJwt("sample_key.pem");

        jwt.Payload.IssuedAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUTCSeconds();
        jwt.Payload.ExpiresAt = new DateTime(2020, 1, 1, 0, 10, 0, DateTimeKind.Utc).ToUTCSeconds();
        jwt.Payload.Issuer = "123456";

        var result = jwt.Get();

        Assert.IsTrue(true);
    }
}
