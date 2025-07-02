using System;

namespace SAIM.Core.Utilities.MethodResult
{
    public class Response
    {
        public ResponseStatus Status { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
        public Exception? Exception { get; set; }
        public object? Data { get; set; }
    }

    public enum ResponseStatus
    {
        SUCCESS,
        ERROR,
        WARNING
    }
}