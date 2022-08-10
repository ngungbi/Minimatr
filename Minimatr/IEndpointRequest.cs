using MediatR;
using Microsoft.AspNetCore.Http;

namespace Minimatr;

public interface IEndpointRequest : IRequest<IResult> { }

public interface IEndpointHandler<in TRequest> : IRequestHandler<TRequest, IResult> where TRequest : IEndpointRequest { }
// public interface IHttpRequest<TResponse> : IHttpRequest, IRequest<TResponse> { }
