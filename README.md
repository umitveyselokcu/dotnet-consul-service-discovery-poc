## Service discovery with Consul

Service discovery with Consul.  

 Run command `docker-compose up -d` than navigate to [http://localhost:8500/ui/dc1/services](http://localhost:8500/ui/dc1/services) on your local browser.

 You will find two service 'Tokyo' and 'London' running.  

This can be used with an api gateway like ocelot.

In this example, I will add a service proxy implementation that will allow microservices to find each other. 
