## Service discovery with Consul

Service discovery with Consul.  

Run command `docker-compose up -d` than navigate to [http://localhost:8500/ui/dc1/services](http://localhost:8500/ui/dc1/services) on your local browser.

You will find two service 'Tokyo' and 'London' running. 

Both applications register themself to Consul Services at application start up, with http and tcp health checks.  
You can find http and TCP health status in link [health-checks](http://localhost:8500/ui/dc1/services/London/instances/consul-server/London/health-checks) 


This can be used with an api gateway like ocelot.

In this example, I will add a service proxy implementation that will allow microservices to find each other. 


## Distributed Config

Our application uses the Consul Key/Value store to simplify distributed Config management.  

### Tokyo Weather
We have ConsulConfig param in response object which is targeting consul K/V store. 

[Tokyo Weather Api Basic Get request](http://localhost:60002/WeatherForecast)

You can change some settings on Consul Key/Value store and check api response to see how easy to change a setting on runtime.

You can check and Edit with link: [Tokyo Weather Api, Key/Value Store](http://localhost:8500/ui/dc1/kv/WeatherApi/Tokyo/appsettings.json/edit)


### Links for London Weather API and Key/Value Store


[London Weather Api, Key/Value Store](http://localhost:8500/ui/dc1/kv/WeatherApi/London/appsettings.json/edit)

[London Weather Api, Get](http://localhost:60001/WeatherForecast)

### Usage

We can use ```IOptions<T>``` to for configs you are not expecting to change, use ```IOptionsSnapsot<T>``` for configs to be consistent for the entirety of a request and use ```IOptionsMonitor<T>``` to get real time config values.
