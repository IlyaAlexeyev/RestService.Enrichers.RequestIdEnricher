# RestService.Enrichers.RequestIdEnricher
Once I had the following problem: I used the library SerilogWeb.Classic for logging of requests and responses to a site, but its standard HttpRequestIdEnricher generates the Request Ids, but Request Id is set in my requests and my Request Ids must be in logs, so I wrote my Enricher.
