using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IoT_SmartPlant_Portal.Middleware.Errors {
    public class CustomException : Exception {

        public HttpStatusCode Code { get; }
        public string Error { get; }

        public CustomException(HttpStatusCode code, string error = null) {
            Code = code;
            Error = error;
        }

    }
}
