using Microsoft.AspNetCore.Mvc;
using PDSA.API.Models;
using PDSA.API.Services;
using System;
using System.Linq;

namespace PDSA.API.Controllers.Games
{
    [Route("api/[controller]")]
    [ApiController]
    public class TOHPController : ControllerBase
    {
        private readonly TOHPService _service;

        public TOHPController()
        {
            _service = new TOHPService();
        }

        [HttpPost("check-moves")]
        public IActionResult CheckMoves([FromBody] TOHPRequest request)
        {
            if (request.NumDisks <= 0 || request.NumPegs <= 0)
                return BadRequest(new { message = "Number of pegs and disks must be positive." });

            try
            {
                // Call service to validate moves
                var response = _service.CheckUserMoves(request);

                // Ensure response contains correct format
                var result = new
                {
                    correctMoves = response.CorrectMoves,
                    correctSequence = response.CorrectSequence,
                    optimalMoves = response.OptimalMoves,
                    correctSequenceList = string.Join(", ", response.CorrectSequenceList)
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
