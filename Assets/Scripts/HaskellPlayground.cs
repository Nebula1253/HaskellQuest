namespace HaskellPlayground {
    public class PlaygroundRequest
    {
        public string code;
        public string version = "8.10.7";
        public string opt = "O1";
        public string output = "run";

        public PlaygroundRequest(string code)
        {
            this.code = code;
        }
    }

    public class PlaygroundErrResponse
    {
        public string err;

        public PlaygroundErrResponse(string err)
        {
            this.err = err;
        }
    }

    public class PlaygroundResponse
    {
        public int ec;
        public string ghcout;
        public string sout;
        public string serr;
        public float timesecs;

        public PlaygroundResponse(int ec, string ghcout, string sout, string serr, float timesecs)
        {
            this.ec = ec;
            this.ghcout = ghcout;
            this.sout = sout;
            this.serr = serr;
            this.timesecs = timesecs;
        }
    }
}