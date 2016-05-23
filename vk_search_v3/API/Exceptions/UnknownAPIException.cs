using System;

namespace vk_search_v3.API.Exceptions
{
    class UnknownAPIException : Exception
    {
        public UnknownAPIException(string message) : base(message)
        {
        }
    }
}
