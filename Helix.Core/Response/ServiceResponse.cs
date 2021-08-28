using Helix.Core.Abstractions;

namespace Helix.Core.Response
{
    public class ServiceResponse<TEntity> : IServiceResponse
    {
        public bool Success { get; }
        public IError? Errors { get; }
        public TEntity Entity { get; }

        private ServiceResponse(TEntity? entity, bool success, IError errors)
        {
            Entity = entity;
            Success = success;
            Errors = errors;
        }

        public static ServiceResponse<TEntity> Ok(TEntity entity) => new(entity, true, default);
        public static ServiceResponse<TEntity> Fail(IError errors) => new(default, false, errors);
    }

    public class ServiceResponse : IServiceResponse
    {
        public bool Success { get; }
        public IError? Errors { get; }

        private ServiceResponse(bool success, IError? errors)
        {
            Success = success;
            Errors = errors;
        }

        public static ServiceResponse Ok() => new(true, default);
        public static ServiceResponse Fail(IError errors) => new(false, errors);
    }
}