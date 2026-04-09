using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var dbServer = builder.AddProject<SoapChat_DbServer>("dbserver");

var api = builder.AddProject<SoapChat_Api>("api")
    .WaitFor(dbServer);

builder.Build().Run();