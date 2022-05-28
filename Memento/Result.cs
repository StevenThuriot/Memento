using Google.Protobuf;

namespace Memento;

partial class Result
{
    public static readonly Task<Result> Success = Task.FromResult(new Result() { Status = Status.Success });
    public static readonly Task<Result> NotFound = Task.FromResult(new Result() { Status = Status.NotFound });
    public static Result From(ByteString value) => new() { Status = Status.Success, Value = value };
    public static Result From(byte[] value) =>From(ByteString.CopyFrom(value));
    public static Task<Result> FromAsync(byte[] value) => Task.FromResult(From(value));
    public static Task<Result> FromAsync(ByteString value) => Task.FromResult(From(value));
    public static Result Fail(string message) => new() { Status = Status.Error, Error = message };
    public static Task<Result> FailAsync(string message) => Task.FromResult(Fail(message));
}
