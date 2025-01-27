namespace JDoodle {
    public class JDoodleRequest {
        public string clientId = "190c8528c5ed8a609a6322fb00818260";
        public string clientSecret = "f61f2873be5015e976b49dc18c4f33ce45133494fc1dafeb60480fc20b9f40ac";
        public string script;
        public string language = "haskell";
        public string versionIndex = "3";

        public JDoodleRequest(string script) {
            this.script = script;
        }
    }

    public class JDoodleCreditsRequest {
        public string clientId = "190c8528c5ed8a609a6322fb00818260";
        public string clientSecret = "f61f2873be5015e976b49dc18c4f33ce45133494fc1dafeb60480fc20b9f40ac";
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
