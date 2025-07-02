# Docker container to create and manage websockify connections
This container fetches dynamic websockify configuration from an API,
and proxies requests via [YARP](https://dotnet.github.io/yarp/) towards dynamically created [websockify](https://github.com/novnc/websockify) instances.

The request proxy flow is the following:
1. Client issues request towards `wss://dockerContainer/proxy/<host>`
2. YARP proxies this request to the correct websockify instance running in the container
3. Websockify proxies the request as websocket RDP to the same host as specified in the initial request.

The queried API must return the following data format (json):

```json
[
  {
    "uid": "686cf7ab-c937-4aa4-879c-766412ce348e",
    "websockifyHost": "target",
    "websockifyPort": 123
  }
]
```
All properties are optional, but only entries with both `uid` and `websockifyHost` as well as `websockifyPort` > 0 will be used.

## Running the service
To run the service, simply create the following docker-compose, create a corresponding .env file.

#### Compose
```yaml
services:
  websockifyManager:
    image: ghcr.io/infsoft-locaware/Docker.WebsockifyManager
    environment:
      - CONFIG_RELOAD_INTERVALL // only if you wish to differ from the default 3600
      - CONFIG_API_URL
      - CONFIG_API_KEY
    ports:
      - 443:443
    volumes:
      - ./nginx/self.pem:/ssl/cert.pem:ro
      - ./nginx/self.key:/ssl/cert.key:ro

```

#### Env file
```
CONFIG_RELOAD_INTERVALL=3600 // default
CONFIG_API_URL=<YourApiUrl>
CONFIG_API_KEY=<YourApiKey>
```

### Further information
#### Container configuration
When running the container manually you have the following options:

##### Environment variables
| Env Var | Required | Default Value | Description |
| -- | -- | -- | -- |
| CONFIG_RELOAD_INTERVALL | false | 3600 | Interval in which the config is queried and refreshed at (in seconds) |
| CONFIG_API_KEY | true | | ApiKey required to call the api |
| CONFIG_API_URL | true | | Url to query in order to fetch configuration |

##### Mounts
Mount a SSL certificate (named cert.pem and cert.key) in the following path: `/ssl/cert.pem` and `/ssl/cert.key`
