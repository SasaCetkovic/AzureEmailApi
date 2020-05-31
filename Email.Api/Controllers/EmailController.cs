using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Email.Api.Filters;
using Email.Api.Models;
using Email.Api.Models.Requests;
using Email.Api.Services;
using System.Threading.Tasks;

namespace Email.Api.Controllers
{
	[Produces("application/json")]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
		private readonly IRabbitService _rabbitService;

		public EmailController(IRabbitService rabbitService)
		{
			_rabbitService = rabbitService;
		}

		/// <summary>
		/// Transfers email request to queue
		/// </summary>
		/// <param name="request">Email data</param>
		[HttpPost]
		[Authorize]
		[ValidateModel]
		[ProducesResponseType(typeof(ApiResponse<long>), 200)]
		public async Task<IActionResult> AddToQueue([FromBody]SendEmailRequest request)
		{
			var response = await _rabbitService.QueueAsync(User.Identity.Name, request);

			if (!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}
	}
}
