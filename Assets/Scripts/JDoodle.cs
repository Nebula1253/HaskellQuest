namespace JDoodle {
    public class JDoodleRequest {
        public string clientId = "7f33da00788efdd5ef493dd6bbbbd7d8";
        public string clientSecret = "bf6cca04cafecd7bec6293f792df632929de3c57c54e5c968332de26f2d58b9c";
        public string script;
        public string language = "haskell";
        public string versionIndex = "3";

        public JDoodleRequest(string script) {
            this.script = script;
        }
    }

    public class JDoodleCreditsRequest {
        public string clientId = "7f33da00788efdd5ef493dd6bbbbd7d8";
        public string clientSecret = "bf6cca04cafecd7bec6293f792df632929de3c57c54e5c968332de26f2d58b9c";
    }

    public class JDoodleResponse {
        public string output;
        public string statusCode;
        public string memory;
        public string cpuTime;

        public JDoodleResponse(string output, string statusCode, string memory, string cpuTime) {
            this.output = output;
            this.statusCode = statusCode;
            this.memory = memory;
            this.cpuTime = cpuTime;
        }
    }

    public class JDoodleCreditsResponse {
        public int used;
        public string error;
        public string statusCode;
        
        public JDoodleCreditsResponse(int used, string error, string statusCode) {
            this.used = used;
            this.error = error;
            this.statusCode = statusCode;
        }
    }
}
