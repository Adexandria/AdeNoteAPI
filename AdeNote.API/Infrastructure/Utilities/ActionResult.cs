

using Twilio.Base;

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
                NotSuccessful = false
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
                NotSuccessful = true
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
            Errors = errors;
            return this;
        }
    }

    public class ActionResult
    {
        internal ActionResult()
        {

        }
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public bool IsSuccessful { get; set; } = false;
        public bool NotSuccessful { get; set; } = false;

        public static ActionResult SuccessfulOperation()
        {
            return new ActionResult()
            {
                StatusCode = 200,
                IsSuccessful = true,
                NotSuccessful = false
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
                NotSuccessful = true
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
            Errors = errors;
            return this;
        }
    }


    public class PaginatedResponse<T> 
    {
        public PaginatedResponse(int pageNumber, int pageSize, IEnumerable<T> entities)
        {
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalCount = entities.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            Entities = entities;
        }
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public IEnumerable<T> Entities { get; private set; }

    }

}