# RestService.Enrichers.RequestIdEnricher
Once I had the following problem:
I used the library SerilogWeb.Classic for logging of requests to a service and responses, but its standard HttpRequestIdEnricher generates its own Request Ids and these generated Request Ids are written into logs. But I do not need to generate Request Ids because Request Ids are set and contained in my requests. I need Request Ids in requests to the service to be in logs. To solve this problem I wrote my own Enricher.
