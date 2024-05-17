namespace AdeNote.Infrastructure.Utilities
{
    public class ActionResult<T> : ActionResult 
    {
        internal ActionResult() { }
        public T Data { get; set; }
        public static ActionResult<T> SuccessfulOperation(T data)
        {
            return new ActionResult<T>()
            {
                Data = data,
                StatusCode = 200,
                IsSuccessful = true,
                NotSuccessful = false,
                Errors = new List<string>()

            };
        }
        public static ActionResult<T> Failed(string error)
        {
            return new ActionResult<T>()
            {
                StatusCode = 500,
                IsSuccessful = false,
                NotSuccessful = true,
                Errors = new List<string>()
                {
                    error
                }

            };
        }
        public static ActionResult<T> Failed()
        {
            return new ActionResult<T>()
            {
                StatusCode = 500,
                IsSuccessful = false,
                NotSuccessful = true,
                Errors = new List<string>()

            };
        }
        public static ActionResult<T> Failed(string error, int code)
        {
            return new ActionResult<T>()
            {
                StatusCode = code,
                IsSuccessful = false,
                NotSuccessful = true,
                Errors = new List<string>()
                {
                    error
                }

            };
        }

        public static ActionResult<T> Failed(List<string> error, int statuscode)
        {
            return new ActionResult<T>()
            {
                StatusCode = statuscode,
                IsSuccessful = false,
                NotSuccessful = true,
                Errors = error
            };

        }

        public ActionResult<T> AddError(string error)
        {
            Errors.Add(error);
            return this;
        }

        public ActionResult<T> AddErrors(List<string> errors)
        {
            if (errors == null)
                return this;
            Errors.AddRange(errors);
            return this;
        }
    }

    public class ActionTokenResult<T> : ActionResult<T>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public static ActionTokenResult<T> SuccessfulOperation(T data, string accessToken,string refreshToken)
        {
            return new ActionTokenResult<T>()
            {
                Data = data,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                StatusCode = 200,
                IsSuccessful = true,
                NotSuccessful = false,
                Errors = new List<string>()

            };
        }
        public static ActionTokenResult<T> Failed(string error)
        {
            return new ActionTokenResult<T>
            {
                StatusCode = 500,
                IsSuccessful = false,
                NotSuccessful = true,
                Errors = new List<string>()
                {
                    error
                }

            };
        }
        public static ActionTokenResult<T> Failed(string error, int code)
        {
            return new ActionTokenResult<T>()
            {
                StatusCode = code,
                IsSuccessful = false,
                NotSuccessful = true,
                Errors = new List<string>()
                {
                    error
                }

            };
        }

    }


    public class ActionResult
    {
        internal ActionResult()
        {

        }
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; }
        public bool IsSuccessful { get; set; } = false;
        public bool NotSuccessful { get; set; } = false;

        public static ActionResult Successful()
        {
            return new ActionResult()
            {
                StatusCode = 200,
                IsSuccessful = true,
                NotSuccessful = false,
                Errors = new List<string>()
            };
        }
        public static ActionResult Failed(string error)
        {
            return new ActionResult()
            {
                StatusCode = 500,
                IsSuccessful = false,
                NotSuccessful = true,
                Errors = new List<string>()
                {
                    error
                }

            };
        }

        public static ActionResult Failed()
        {
            return new ActionResult()
            {
                StatusCode = 500,
                IsSuccessful = false,
                NotSuccessful = true,
                Errors = new List<string>()

            };
        }
        public static ActionResult Failed(string error,int code)
        {
            return new ActionResult()
            {
                StatusCode = code,
                IsSuccessful = false,
                NotSuccessful = true,
                Errors = new List<string>()
                {
                    error
                }

            };
        }

        public ActionResult AddError(string error)
        {
            Errors.Add(error);
            return this;
        }

        public ActionResult AddErrors(List<string> errors)
        {
            if (errors == null)
                return this;
            Errors.AddRange(errors);
            return this;
        }

    }

}