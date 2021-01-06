using Newtonsoft.Json;

namespace IoT_SmartPlant_Portal.Middleware.Models {
    public class ErrorResponse {
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }
        [JsonProperty("response")]
        public string Response { get; set; }
        public ErrorResponse(int statusCode, string response) {
            StatusCode = statusCode;
            Response = response;
        }

        public ErrorResponse(int statusCode) {
            StatusCode = statusCode;
        }

        public override string ToString() {
            if (Response == null || Response == "") {
                return "StatusCode: " + StatusCode;
            }
            return "StatusCode: " + StatusCode + " Response: " + Response;
        }
    }

}
