using System;

namespace Carbonate.Client
{
    public class RequestResponse
    {
        public bool accpeted;
        public string message;

        public RequestResponse(bool accpeted, string message) {
            this.accpeted = accpeted;
            this.message = message;
        }
    }
}