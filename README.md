# Patronus

## Overview
A test project integrating a number of interesting frameworks and methodologies:
- NET 6
- Blazor
  - MudBlazor Component Library: https://mudblazor.com/
- FluentValidation https://docs.fluentvalidation.net/en/latest/
- REST Api
- EntityFramework
- Refit Package Pattern https://github.com/reactiveui/refit
- xUnit
  - AutoMoq
  - AutoFixture https://github.com/AutoFixture/AutoFixture
  - Fluent Assertions https://fluentassertions.com/
  - EF In-Memory DB
  
## Installation
Required tools:
 - Visual Studio 2022 w/ASP.NET workload
 
1. Clone the repository from Github.
2. Open the .sln file in Visual Studio 2022.
3. Rebuild solution to force package installs.
4. Set Patronus.API and Patronus.Client as the startup projects:

 ![image](https://user-images.githubusercontent.com/842330/177230796-e1936fa8-8371-478c-9938-c9006f6fc0d2.png)

5. Run the solution.

## Operation
The Blazor WebAssembly app displays a table with three pre-populated Contacts. The table lists the Name, Email and Phone of the Contacts.
The buttons at the bottom of the table refresh the data, and allow the user to add another Contact.
Clicking on a row in the table pops up a dialog that allows the user to edit that contact, or delete it.

## Explanations
### Refit Package Pattern
The Refit framework allows us to create an interface of an REST API, and then inject that interface (in this case, via Nuget package) into other projects that can use it. This lends itself tremendously to a microservices architecture, since it gives us a very easy way to package and consume REST APIs from downstream clients.
The Patronus.Api.Client references the Patronus.Api.Models project, but crucially does not reference the Api itself. It contains only an interface that uses the Refit conventions to describe the API. Referencing the Api project would bloat the package, and could cause dependency overlaps.
The Patronus.Api.Models project should be a source of shared DTOs and other utility classes that are needed for the calling client.

### FluentValidation
This was new to me for this project, but I wanted to experiment with a quick way to share my validation across both client and server. In the past I would have done it by hand, but this will be a framework I adopt going forward. 

### MudBlazor
Having only played with Blazor briefly in the past, I opted for a pre-built component library. There were some nice advantages here that I found along the way: integration with FluentValidation was a big one, as well as simple snackbar, table, and dialog support.

### Unit Testing
I like unit testing though my experience so far with enterprise development has never given me the luxury of a pace of development that would allow for extensive unit testing. So, I test what I can. The unit tests in the Patronus.Api.Test project are not meant to be exhaustive, or even complete. They are less than I would write for a real project, but do provide some examples of mocking, autofixture customizations (including the EF In-Memory provider) for DI, and fluent assertions.

### General Architecture
I opted to keep my REST API in a separate project to benefit from the Refit pattern explained above. In a true enterprise environment it would be in an entirely separate repository, with it's own build/deploy processes. This is in keeping with the microservices pattern.

The Patronus.API project contains the ContactService class, which is the next layer down from the Controllers. We separate the service from the controller so that the service gives us a tidy place to base our business logic surrounding Contacts, should we have any. The Controller should try to be light on validation and just handle appropriate responses to the calling client.
The service is also the only place we call our EF DbContext, the PatronusContext. This gives us a leg up for unit testing the actions of the service, since the context can be easily manipulated in tests via the In-Memory EF provider.

I did not use a repository pattern here, since EntityFramework (and most other ORMs) essentially ARE a repository pattern, with the context being the Unit-of-Work and the DbSets acting as repositories. It is injectable and testable entirely without the repository layer.

