using System;

namespace vk_search_v3.API.Exceptions
{
    class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
