# sanity-client
.Net core client for Sanity CMS (https://www.sanity.io)

Implements the three main functions of the data API of Sanity
- Get to get a single document by Id
- Fetch to get a collection of documents using a GROQ query
- Mutate to create, modify or delete documents

First you need to declare your Document type. Document is a C# class with the same properties as your documents in Sanity CMS

Get Example:
```csharp
var client = new SanityClient(<project id>, <dataset name>, 
    null, false);
var result = await client.GetDocument<Document>(<document id>);
```

Fetch Example:
```csharp
var client = new SanityClient(<project id>, <dataset name>, 
    null, false);
var result = await client.Fetch<Document>("*[_type==\"<document type name>\"]");
```

To mutate data you need to first get a token. The auth token can be generated on the settings->api page of your Sanity manage page. The url to that page is: https://manage.sanity.io/projects/<your-project-id>/settings/api

Mutate Example:
```csharp
const string token = "<long random string>";
var mutations = new Mutations().AddCreate(new Document {Key = "testing"});
var client = new SanityClient("<your-project-id>", "<dataset name>", 
    token, false);
var result = await client.Mutate(mutations, true, true);
```

You can add multiple Mutations by chaning AddCreate, AddCreateOrReplace, AddCreateIfNotExists, AddDelete and AddPatch.

The SanityClient will create a HttpClient and the SanityClient is shareable, just as the HttpClient so you should make sure to only create one instance of the SanityClient per project. Usually you will not connect to more than one project in a single application so the SanityClient should be a Singleton. 
