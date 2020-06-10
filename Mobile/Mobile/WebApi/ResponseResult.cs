using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile.WebApi
{
    public class ResponseResult<T>
    {
        public ResponseResult(bool isSuccessful, T content)
        {
            Content = content;
            IsSuccessful = isSuccessful;
        }

        public T Content { get; private set; }
        public bool IsSuccessful { get; private set; }
    }
    public class ResponseResult
    {
        public ResponseResult(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
        public bool IsSuccessful { get; private set; }
    }
}
