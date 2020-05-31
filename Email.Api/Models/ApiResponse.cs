namespace Email.Api.Models
{
    public class ApiResponse<T>
    {
		public ApiResponse(bool success = true)
		{
			Success = success;
		}

		public ApiResponse(T result, int? limit = null, int? offset = null, int? total = null, bool success = true)
		{
			Value = result;
			Success = success;
			Limit = limit;
			Offset = offset;
			Total = total;
		}

		public ApiResponse(Error error)
		{
			Error = error;
			Success = false;
		}

		public bool Success { get; set; }

        public T Value { get; set; }

		public int? Limit { get; set; }

		public int? Offset { get; set; }

		public int? Total { get; set; }

	    public Error Error { get; set; }
	}
}
