using Microsoft.AspNetCore.Mvc;
using PDSA.Core.Algorithms.EightQueens;

namespace PDSA.API.Controllers.Games
{
	[ApiController]
	[Route("api/[controller]")]
	public class EQPController : ControllerBase
	{
		private readonly ILogger<EQPController> _logger;

		public EQPController(ILogger<EQPController> logger)
		{
			_logger = logger;
		}

		[HttpPost("new-game")]
		public ActionResult<EQPGameRoundDto> CreateNewGame()
		{
			try
			{
				var round = EQPGameService.CreateNewGameRound();
				var dto = new EQPGameRoundDto
				{
					GameId = round.GameId,
					BoardSize = round.BoardSize
				};
				return Ok(dto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating new EQP game round");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPost("solve")]
		public ActionResult<EQPSolveResultDto> Solve([FromBody] EQPSolveRequest request)
		{
			try
			{
				if (request.GameId == Guid.Empty) return BadRequest("Missing gameId");

				var result = EQPGameService.SolveAll(request.GameId);
				if (!result.Success)
				{
					return StatusCode(500, result.ErrorMessage);
				}

				var dto = new EQPSolveResultDto
				{
					GameId = request.GameId,
					TotalSolutionsSequential = result.SequentialSolutionsCount,
					TotalSolutionsThreaded = result.ThreadedSolutionsCount,
					AlgorithmResults = result.AlgorithmResults.Select(ar => new EQPAlgorithmResultDto
					{
						AlgorithmName = ar.AlgorithmName,
						SolutionsFound = ar.SolutionsFound,
						ExecutionTimeMs = ar.ExecutionTimeMs
					}).ToList()
				};

				return Ok(dto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error solving EQP");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPost("submit-solution")]
		public ActionResult<EQPSolutionSubmissionResultDto> SubmitSolution([FromBody] EQPSubmitRequest request)
		{
			try
			{
				if (request.GameId == Guid.Empty) return BadRequest("Missing gameId");
				if (request.QueenPositions == null) return BadRequest("Missing queen positions");

				bool isValid = EQPGameService.ValidateAndRecordPlayerSolution(request.GameId, request.PlayerName, request.QueenPositions);

				var dto = new EQPSolutionSubmissionResultDto
				{
					IsCorrect = isValid,
					Message = isValid ? "Solution accepted." : "Invalid solution."
				};

				return Ok(dto);
			}
			catch (ArgumentException aex)
			{
				return BadRequest(aex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error submitting EQP solution");
				return StatusCode(500, "Internal server error");
			}
		}
	}

	#region DTOs

	public class EQPGameRoundDto
	{
		public Guid GameId { get; set; }
		public int BoardSize { get; set; }
	}

	public class EQPSolveRequest
	{
		public Guid GameId { get; set; }
	}

	public class EQPAlgorithmResultDto
	{
		public string AlgorithmName { get; set; } = string.Empty;
		public int SolutionsFound { get; set; }
		public long ExecutionTimeMs { get; set; }
	}

	public class EQPSolveResultDto
	{
		public Guid GameId { get; set; }
		public int TotalSolutionsSequential { get; set; }
		public int TotalSolutionsThreaded { get; set; }
		public List<EQPAlgorithmResultDto> AlgorithmResults { get; set; } = new();
	}

	public class EQPSubmitRequest
	{
		public Guid GameId { get; set; }
		public string? PlayerName { get; set; }
		public List<QueenPosition>? QueenPositions { get; set; }
	}

	public class EQPSolutionSubmissionResultDto
	{
		public bool IsCorrect { get; set; }
		public string Message { get; set; } = string.Empty;
	}

	#endregion
}

