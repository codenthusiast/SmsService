# Overview
This microservice is implemented to act as an abstarction layer over a 3rd party SMS service. 

## Project structure
The solution comprises of two .NET projects:

- SmsService.Core - A .NET Standard project which contains the core business logic, contracts and entities.
- SmsService.Test - An xUnit test project

## Setup
### Console
Run the `dotnet restore` command in the root of the solution. 

### Visual Studio
- Navigate to the root folder and open the `SmsService.sln` to load the project in Visual Studio.
- Navigate to the Test Explorer to run the tests.


### Application entry point
The test project to serve as the entry point in order to test the application logic(`SmsService.Core.CommandHandlers.SendSmsCommandHandler`). The following assumptions were made:

- A consumer already exists in form of a hosted worker service(Sample included) or console which is listening on the queue for incoming `SendSmsCommand` message.
- Upon receiving the `SendSmsCommand` message from the queue, the consumer invokes `SendSmsCommandHandler.Handle` which is where the business logic is executed. 
- This handler either throws an exception or returns a `boolean` result to determine if the request succeeded or failed. The queue consumer may then use the result to decide whether to positively or negatively acknowledge the message.

 ### Resilience
 In order to achieve some resilience while calling the SMS service via HTTP, it was assumed that the implementation of the `ISmsGateway.SendAsync` would cater for implementing a retry policy in the event of transient network failures.
 
### Idempotency
SendSmsCommand contains an idempotency-key i.e, `SessionId` which will be used to query the database to check if the request has been previously treated. 