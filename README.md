# GitHubAuth



## Benchmarks

The method to generate the JWT in this library is very simplified and optimized. Comparing it with another Nuget package yielded the following results:

| Method              | Mean     | Error     | StdDev    | Rank | Gen0    | Gen1   | Allocated |
|-------------------- |---------:|----------:|----------:|-----:|--------:|-------:|----------:|
| TokenWithGitHubAuth | 1.921 ms | 0.0081 ms | 0.0075 ms |    1 |  3.9063 |      - |  29.83 KB |
| TokenWithGitHubJwt  | 2.514 ms | 0.0164 ms | 0.0146 ms |    2 | 70.3125 | 3.9063 | 329.65 KB |
