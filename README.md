# Auth Token Refresh Demo

A project that demonstrates how to handle the scenario where an API call requires an auth token that expires periodically.  It will fetch a token and put it in a cache.  It then uses Polly to set up a resilience policy that looks for exceptions or error codes, then expires the cached token.  On retry it will fetch a new token and put it in the cache.  

## Components

- `AuthTokenProvider`: fetches a token for use in API calls
- `AuthenticatedHttpClient`: combines the above plus `IMemoryCache` to fetch a token that will be added to the auth header on every call.

For the resilience part, there are two ways to approach it:

1. Use `AddResilienceHandler` to add a message handler that will retry when it sees a 401 status code.  This will only work for `HttpClient`.  See `Things2Controller` for an example.  
2. Build a standalone resilience pipeline and a function wrapper that does the same as above, but is more flexible in handling other types of exceptions or things that use an SDK or anything else other than `HttpClient`.  See `AuthenticatedPipelineBuilder`, `ResilientClientWrapper`, and `ThingsController` for an example.  This way is also easier to unit test.  
