using System.Reflection;
using MediatR;
using Minimatr;
using Minimatr.Extensions;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddMinimatr(Assembly.GetExecutingAssembly());
services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapAllRequests();

app.Run();
