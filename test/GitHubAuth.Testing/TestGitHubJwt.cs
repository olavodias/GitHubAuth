using GitHubAuth.Jwt;
using GitHubAuth.Extensions;

namespace GitHubAuth.Testing;

[TestClass]
public class TestGitHubJwt
{
    private static readonly string PrivateKeyFile = "sample_key.pem";
    private static readonly string AppID = "123456";

    [TestMethod]
    public void TestExtensions()
    {
        // Setup Epoch (1970-01-01 at 00:00:00 UTC)
        var currentDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(0, currentDateTime.ToSecondsSinceEpoch());

        // Check January 1st 2020 at 00:00:00 UTC
        currentDateTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(1577836800, currentDateTime.ToSecondsSinceEpoch());

        // Check May 1st 2022 at 00:00:00 UTC
        currentDateTime = new DateTime(2022, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(1651363200, currentDateTime.ToSecondsSinceEpoch());

        // Check May 1st 2022 at 00:10:00 UTC
        currentDateTime = new DateTime(2022, 5, 1, 0, 10, 0, DateTimeKind.Utc);
        Assert.AreEqual(1651363800, currentDateTime.ToSecondsSinceEpoch());
    }

    [TestMethod]
    public void TestSerialization()
    {
        // Serialize the Token
        var expectedSerializedValues = "{\"iat\":1651363200,\"exp\":1651363800,\"iss\":\"123456\"}";
        var payload = new GitHubJwtPayload
        {
            IssuedAt = new DateTime(2022, 5, 1, 0, 0, 0, DateTimeKind.Utc),
            Issuer = AppID
        };

        Assert.AreEqual(expectedSerializedValues, payload.ToJSON().Replace(" ", "").Replace("\n",""));        
    }

    [TestMethod]
    public void TestReadPrivateKey()
    {
        var expectedPrivateKey = "MIIEowIBAAKCAQEAr6vbYZMrer9g2WS6niFgonX8WlayxBIOHnTSi5fjsctJO/WOr9/BQLYwKlZMgdkryuhDXWd3zzIfazgbIglCwJUbygLiKEsYmC7zIeHxk5iFwQi6d0OklFr7icSLsuZQJRGxPQ7x/4NGhWXYz/JYYzdHqCNy49jSj+AppGLAIQ5gLzWHZknyaiyaPN61FWMrTFGofQEPSBPjuksSUGwK9zl4CAC9OuaC+GiTxXuO44zfvtVMGtkT2cMkzZD/yrcgOmQee4PIPQDAxKZC5mOCd9BymAwFmwFwLPzOzA/q/5bHz4pkDhPrLEzAOtHGxD+vmD2tSyfeMMdEHNa4EBFS9QIDAQABAoIBAD5NehnKAzKeay/OnKz2c3pK0/wKIY6ORLmifwWJEfT9fvSn6zoO5lAYDU8Gmk23AuQMqc+XoZM3WJNDK8RPeoAoodlsWl8l+wwGIq6SnoXVIyLKAK/JqrX+6pT6wvzo1+W9t5lLEqKnITywWUuuzJAri9ti7x1Fya/DNaGacD+IbbrHA9RLr27SOaY8GgtMrHuRtzBe5qw0s9gQoTVwMDH4gdQ7xu5fpu85NMnkHE9mdS5tZb+O7ngryqOKfFxrEHPLlgekOUEYbFmFPzNsw482IsvykukF9SXRAdAUTj53zESBGeKRhxXpq6FX/GCiRgVFPtVnE94DnlEO3cXCoeECgYEA5p5kYeumnqdN4NQhJoyK7nFHjHmwatlk1jBnxBvmRYqclEySe6iRlVMpGwtn4td/x2vCVURk9BEIFiTYiyvitqx2ndi6mJR4CCEagc35uHn+2dj8ByZylp7MlsZyw9V9QNQZAdneZVwQGE4lF+lepTdCpigoapsMmK0ShjKfhj0CgYEAwwFW6qZFI0sx5bazZhNREH/RnCN/Q68Ad+Sd2QaW6cIHvUB5VaBmvBKScMBpOiUcV9U4kHZhpGoVfB/Nov2fhjFJuac4xVYFLOHHpA4MpKpCI2mcet/p4RSdBReIT5EJZ9TB5zamT4Jb8OGs+8U6sBcsIKCMVyrl+/8+oQi8gxkCgYEAnazd5TVBDmhFDtr6b2jX9H7u9FSfEe7gyrD9wU9x+Un0FFDmNjXik73v9NWviaDddHNwwuuoOlcKuFko1L6Em3D/CJ0NtxSLnMNFZEp32nFOlZONfTYEUobwSoYUIWDt6k0Za/KR46s2Nef35IlidIN7xi7es0SfMCPk7mveNeECgYBvhxKYvWOJXfY5tRZBaQbR8uiE7mBY63vPFjeY6gXhx5D5kihS6pwXMGAEkGceVB8ztMTXCn2ptOp4xQ+tCnT0ILhTr+nuItZu5upxT3+3pZCSBFL0i5+NlWaAhQb2yrKgabREcqMHkjzXNZrjm9eG4pngRzI8oFJMn5zIKOD38QKBgGkH3UXUBAepruH5CTQ99wz1xUhhtHkdm8Ig0M62BL1C2gF4MyX8X/qOuiG4xsoBQ4UucfcL7mN3+wZnUJrj+GjqXigWqfdJuvTSuot4aIUvxKSSVp1iLNwV54EkTAjgp3iS75YhJVkz/ZolmDttEmZJ6e+3f+Ed6Me0DlMaqZ3F";
        var jwt = new GitHubJwt("sample_key.pem", AppID);

        Assert.AreEqual(expectedPrivateKey, jwt.ReadPrivateKey());
    }

    [TestMethod]
    public void TestTokenGeneration()
    {
        var jwt = new GitHubJwt(PrivateKeyFile, AppID);

        // Token 01 (issued at 2020-01-01, issuer 123456)
        var expectedJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE1Nzc4MzY4MDAsImV4cCI6MTU3NzgzNzQwMCwiaXNzIjoiMTIzNDU2In0.PQCqR9duu-3UETb07KWFuutV55ZM1XDmT2O_hVQoFeR16KC-NxJ3pkQzy3q6JqZlzqm8jojz-v1OBibsmMTZhuqsCa4_DL0meXNRsGGxRX2TKjTzKSiFcvdRLirjP-AXOQHIRzXwGauAYn9OaGS_PK5WRWPZN3tg9zfzZF9AesNFJzli3HJ9wLPVCOQIElnrbXqa6Q273PetsFdae5HKkHHBQarTvE_7QhVM1qKyKz5Dq5fn9Rnb7jRJ4OBcXKsnGU_BMhfW2H4jVLQkjcJ9BA0J25_ot7tTPa-Mu8yGK5KctVxaatYllPcq0Co696J1d0caj7dEMZYAglN2JX2kKQ";
        jwt.Payload.IssuedAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(expectedJwt, jwt.GenerateToken());

        // Token 02 (issued at 2022-05-01, issuer 123456)
        expectedJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE2NTEzNjMyMDAsImV4cCI6MTY1MTM2MzgwMCwiaXNzIjoiMTIzNDU2In0.XaSBPnArytB0B9enpJRZpeuJreNoCC-GlyMlqS8RQni6P2yz1dSdH-phJdqivFZP5hGA2QOn2mYRI2__idM81DKUeHJ5uTyOEV-uuR4jHbow87lblnp4his10je6AqTz5kuw6KHgLc6XaGnBLDHl_uRoH-kghQ9OQra3lymvCSBMJi-4pNT6mbNuWg5DT3xN-Ww9K573OxxubUrgjHfdb0I-EWAl5U6pZ2Bqisrk46tKIsYzN9x52tpqhu5S7AEbrRDR0W_gQD3lLdBE6SmbbX4b1a6uVchcOPPBFxcr5NubY7aBiiKJte2pVHYeLzd9gaCyiDZwXsW47TAKKah4DA";
        jwt.Payload.IssuedAt = new DateTime(2022, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(expectedJwt, jwt.GenerateToken());
    }

    [TestMethod]
    public void TestTokenRenewal()
    {
        var jwt = new GitHubJwt(PrivateKeyFile, AppID);

        // Token 01 (issued at 2020-01-01, issuer 123456)
        var expectedJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE1Nzc4MzY4MDAsImV4cCI6MTU3NzgzNzQwMCwiaXNzIjoiMTIzNDU2In0.PQCqR9duu-3UETb07KWFuutV55ZM1XDmT2O_hVQoFeR16KC-NxJ3pkQzy3q6JqZlzqm8jojz-v1OBibsmMTZhuqsCa4_DL0meXNRsGGxRX2TKjTzKSiFcvdRLirjP-AXOQHIRzXwGauAYn9OaGS_PK5WRWPZN3tg9zfzZF9AesNFJzli3HJ9wLPVCOQIElnrbXqa6Q273PetsFdae5HKkHHBQarTvE_7QhVM1qKyKz5Dq5fn9Rnb7jRJ4OBcXKsnGU_BMhfW2H4jVLQkjcJ9BA0J25_ot7tTPa-Mu8yGK5KctVxaatYllPcq0Co696J1d0caj7dEMZYAglN2JX2kKQ";
        jwt.Payload.IssuedAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(expectedJwt, jwt.GenerateToken());

        // Token 02 - Use the property "Token", which should force the generation of a new token since it expired
        var newToken = jwt.Token;
        Assert.IsNotNull(newToken);
        Assert.IsTrue(jwt.Payload.IssuedAt > DateTime.UtcNow.AddMinutes(-2));

        // Token 03 - Call the property again, it should have the same issued date/time
        var currentIssueDate = jwt.Payload.IssuedAt;
        _ = jwt.Token;
        Assert.AreEqual(currentIssueDate, jwt.Payload.IssuedAt);
    }
}
