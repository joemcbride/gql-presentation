```gql
type Project {
  name: String
  tagline: String
  contributors: [User]
}

{
  project(name: "GraphQL") {
    tagline
  }
}
```

```json
{
  "project": {
    "tagline": "A query language for APIs"
  }
}
```
