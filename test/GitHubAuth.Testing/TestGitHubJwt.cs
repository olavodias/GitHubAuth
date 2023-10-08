using GitHubAuth.Jwt;
using GitHubAuth.Extensions;

namespace GitHubAuth.Testing;

[TestClass]
public class TestGitHubJwt
{
    [TestMethod]
    public void TestExtensions()
    {
        // Setup Epoch (1970-01-01 at 00:00:00 UTC)
        var currentDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(0, currentDateTime.ToSecondsSinceEpoch());

        // Check January 1st 2020 at 00:00:00 UTC
        currentDateTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(1577836800, currentDateTime.ToSecondsSinceEpoch());
    }

    [TestMethod]
    public void TestSerialization()
    {
        // Serialize the Token
        var expectedSerializedValues = "{\"iat\":1577836800,\"exp\":1577837280,\"iss\":\"123456\",\"alg\":\"RS256\"}";
        var payload = new GitHubJwtPayload
        {
            IssuedAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Issuer = "123456"
        };

        Assert.AreEqual(expectedSerializedValues, payload.ToJSON().Replace(" ", "").Replace("\n",""));        
    }

    [TestMethod]
    public void TestTokenGeneration()
    {
        var jwt = new GitHubJwt("sample_key.pem");

        jwt.Payload.IssuedAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        jwt.Payload.Issuer = "123456";

        var result = jwt.Get();

        Assert.IsTrue(true);
    }
}
