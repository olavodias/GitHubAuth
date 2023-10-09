using GitHubAuth.Jwt;
using GitHubAuth.Extensions;

namespace GitHubAuth.Testing;

[TestClass]
public class TestGitHubJwt
{
    /// <summary>
    /// The default AppID for calculating the token
    /// </summary>
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
    }

    [TestMethod]
    public void TestSerialization()
    {
        // Serialize the Token
        var expectedSerializedValues = "{\"iat\":1577836800,\"exp\":1577837280,\"iss\":\"123456\",\"alg\":\"RS256\"}";
        var payload = new GitHubJwtPayload
        {
            IssuedAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
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
        var expectedJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOiAxNTc3ODM2ODAwLCAiZXhwIjogMTU3NzgzNzI4MCwgImlzcyI6ICIxMjM0NTYifQ.ltu6VL2wMmsp5pT9M2ForRaD6SGntwk00T4cTSnAnU0v0JygF2LMWslgO98mXmvM6IlgjiziA2eUBf87TyA5puEbBzL1opvSMF6gpnOJ0LlsUDekrmcFZbceVM0B2cp4T2Oy6wJiOmxOlBQkAstJvuQJ94evtAjIF3goo_2VWKUqlcEB_n5_JXcWo_Ftrd6BQiKluFQII9_b_FeddfEOdo_O1HnYvPfyMQ-CL_9wmaI9RrNb3ZqQR9WAxM9r10mikCU_g1fVxcwGW97BtRVpkeWrOqXqC6aj3vHOVbWosAlzB-teyQZ0U03NILj8xdbYbAkM2rYwWF4jdU846BF0AA";
        var jwt = new GitHubJwt("sample_key.pem", AppID);



        //var s2 = GitHubJwt.Base64UrlDecode("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9");
        //s2 = GitHubJwt.Base64UrlDecode("eyJpYXQiOiAxNTc3ODM2ODAwLCAiZXhwIjogMTU3NzgzNzI4MCwgImlzcyI6ICIxMjM0NTYifQ");
        //s2 = GitHubJwt.Base64UrlDecode("eyJpYXQiOjE1Nzc4MzY4MDAsImV4cCI6MTU3NzgzNzI4MCwiaXNzIjoiMTIzNDU2In0");


        jwt.Payload.IssuedAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var token = jwt.GenerateToken();

        Assert.AreEqual(expectedJwt, jwt.GenerateToken());

        Assert.IsTrue(true);
    }
}
