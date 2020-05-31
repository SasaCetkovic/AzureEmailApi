using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Email.Api.Models;
using Email.Api.Services;
using System.Threading.Tasks;

namespace Email.Api.Controllers
{
	[Produces("application/json")]
    [Route("api/email/status")]
    public class StatusController : ControllerBase
    {
		private readonly IStatusService _service;

		public StatusController(IStatusService service)
		{
			_service = service;
		}

		/// <summary>
		/// Retrieves the current status of a previously received email request
		/// </summary>
		/// <param name="trailId">ID of the email trail</param>
		[HttpGet]
		[Route("{trailId}")]
		[Authorize]
		[ProducesResponseType(typeof(ApiResponse<string>), 200)]
		public async Task<IActionResult> Get(long trailId)
		{
			var response = await _service.GetEmailStatusAsync(User.Identity.Name, trailId);

			if (!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}
	}
}
