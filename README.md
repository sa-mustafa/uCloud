# uCloud

uCloud is a cloud API service, supporting some tough machine vision and image processing tasks.

This is my side-project that I personally push some effort into to level up my coding knowledge outside the boundaries of everyday work. The project is still in progress, and I want to complete it in near future. The design is complete and the implementation and testing is still going.

The project is designed API-first, strictly following best practices to achieve best possible results.

The API is written in Swagger hosted at [SwaggerHub](https://app.swaggerhub.com/apis/sa-mustafa/ucloud/1.0.2). The project is implemented using ASP .net core for  services and C++ for low-level processing tasks following [microservice patterns](https://microservices.io/patterns/microservices.html). The C++ source code for the processor is not going to be published due to proprietary reasons. However, the code for backend services is published for review.

The backend services originally relied on [NancyFx](https://nancyfx.org/), [TopShelf](http://topshelf-project.com/) and [Aerospike](https://www.aerospike.com/) DB. Due to Nancy's reduction is activity and popularity and the rise in Microsoft .net core, I migrated the code to .net core omitting the topshelf too. The backend service receives API requests and pushes them onto [RabbitMQ](https://www.rabbitmq.com/) using [Masstransit](https://masstransit-project.com/) bus. Some requests are also retained in [MongoDB](https://www.mongodb.com/) for future reference. The C++ processor acts on requests in queue and responds with the processed results to the backend services through [masstransit_cpp](https://github.com/sa-mustafa/masstransit_cpp).
