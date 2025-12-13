using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDSA.API.Data;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.EightQueens;
using PDSA.Core.Algorithms.EightQueens;

namespace PDSA.API.Controllers.Games
{
	[ApiController]
	[Route("api/[controller]")]
	public class EQPController : ControllerBase
	{
		private readonly ILogger<EQPController> _logger;
		private readonly PDSADbContext _dbContext;

		public EQPController(ILogger<EQPController> logger, PDSADbContext dbContext)
		{
			_logger = logger;
			_dbContext = dbContext;
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
		public async Task<ActionResult<EQPSolveResultDto>> Solve([FromBody] EQPSolveRequest request)
		{
			try
			{
				if (request.GameId == Guid.Empty) return BadRequest("Missing gameId");

				var result = EQPGameService.SolveAll(request.GameId);
				if (!result.Success)
				{
					return StatusCode(500, result.ErrorMessage);
				}

				// Save algorithm execution times to database
				var roundNumber = await GetNextRoundNumber();
				
				foreach (var algoResult in result.AlgorithmResults)
				{
					var algoTime = new EQPAlgoTime
					{
						DateExecuted = DateTime.UtcNow.ToString("o"),
						AlgorithmType = algoResult.AlgorithmName,
						TimeTaken_ms = algoResult.ExecutionTimeMs,
						RoundNumber = roundNumber
					};
					_dbContext.EQPAlgoTimes.Add(algoTime);
				}

				await _dbContext.SaveChangesAsync();

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
					}).ToList(),
					RoundNumber = roundNumber
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
		public async Task<ActionResult<EQPSolutionSubmissionResultDto>> SubmitSolution([FromBody] EQPSubmitRequest request)
		{
			try
			{
				if (request.GameId == Guid.Empty) return BadRequest("Missing gameId");
				if (request.QueenPositions == null) return BadRequest("Missing queen positions");
				if (string.IsNullOrWhiteSpace(request.PlayerName)) return BadRequest("Player name is required");

				// Validate the solution
				bool isValid = EQPGameService.ValidateSolution(request.QueenPositions, 8);
				
				if (!isValid)
				{
					return Ok(new EQPSolutionSubmissionResultDto
					{
						IsCorrect = false,
						Message = "Invalid solution. Queens are attacking each other."
					});
				}

				// Convert solution to canonical text representation
				var solutionText = ConvertToSolutionText(request.QueenPositions);

				// Find the matching solution in the database
				var solution = await _dbContext.EQPSolutions
					.FirstOrDefaultAsync(s => s.Solution_Text == solutionText);

				if (solution == null)
				{
					// This shouldn't happen if all 92 solutions are seeded, but just in case
					return Ok(new EQPSolutionSubmissionResultDto
					{
						IsCorrect = false,
						Message = "Solution not found in database. This is unusual - please contact admin."
					});
				}

				// Check if this solution has already been found
				if (solution.IsFound)
				{
					var foundSolutions = await _dbContext.EQPSolutions.CountAsync(s => s.IsFound);
					var totalSolutions = await GetTotalPossibleSolutions();
					
					return Ok(new EQPSolutionSubmissionResultDto
					{
						IsCorrect = true,
						IsAlreadyFound = true,
						Message = $"This solution has already been found! Try another one. ({foundSolutions}/{totalSolutions} solutions found)",
						FoundSolutionsCount = foundSolutions,
						TotalSolutionsCount = totalSolutions
					});
				}

				// Get or create player
				var player = await _dbContext.Players
					.FirstOrDefaultAsync(p => p.Name == request.PlayerName);

				if (player == null)
				{
					player = new Player { Name = request.PlayerName };
					_dbContext.Players.Add(player);
					await _dbContext.SaveChangesAsync();
				}

				// Mark solution as found
				solution.IsFound = true;
				solution.PlayerID = player.PlayerID;
				solution.DateFound = DateTime.UtcNow.ToString("o");
				
				await _dbContext.SaveChangesAsync();

				var foundSolutionsCount = await _dbContext.EQPSolutions.CountAsync(s => s.IsFound);
				var totalSolutionsCount = await GetTotalPossibleSolutions();

				// Check if all solutions have been found
				if (foundSolutionsCount >= totalSolutionsCount)
				{
					// All solutions found - reset for new game
					await ResetAllSolutions();
					
					return Ok(new EQPSolutionSubmissionResultDto
					{
						IsCorrect = true,
						IsAlreadyFound = false,
						Message = $"🎉 Congratulations! You found the last solution! All {totalSolutionsCount} solutions have been discovered. The game has been reset for new players.",
						FoundSolutionsCount = 0,
						TotalSolutionsCount = totalSolutionsCount,
						AllSolutionsFound = true
					});
				}

				return Ok(new EQPSolutionSubmissionResultDto
				{
					IsCorrect = true,
					IsAlreadyFound = false,
					Message = $"Correct! New solution accepted. ({foundSolutionsCount}/{totalSolutionsCount} solutions found)",
					FoundSolutionsCount = foundSolutionsCount,
					TotalSolutionsCount = totalSolutionsCount
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error submitting EQP solution");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("game-stats")]
		public async Task<ActionResult<EQPGameStatsDto>> GetGameStats()
		{
			try
			{
				var totalSolutions = await GetTotalPossibleSolutions();
				var foundSolutions = await _dbContext.EQPSolutions.CountAsync(s => s.IsFound);
				var uniquePlayers = await _dbContext.EQPSolutions
					.Where(s => s.IsFound && s.PlayerID.HasValue)
					.Select(s => s.PlayerID)
					.Distinct()
					.CountAsync();

				return Ok(new EQPGameStatsDto
				{
					TotalSolutionsCount = totalSolutions,
					FoundSolutionsCount = foundSolutions,
					RemainingSolutionsCount = totalSolutions - foundSolutions,
					UniquePlayers = uniquePlayers
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting game stats");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("leaderboard")]
		public async Task<ActionResult<List<EQPLeaderboardEntryDto>>> GetLeaderboard([FromQuery] int top = 10)
		{
			try
			{
				var leaderboard = await _dbContext.Players
					.Include(p => p.EQPSolutions)
					.Where(p => p.EQPSolutions.Any(s => s.IsFound))
					.Select(p => new EQPLeaderboardEntryDto
					{
						PlayerName = p.Name,
						SolutionsFound = p.EQPSolutions.Count(s => s.IsFound),
						FirstSolutionDate = p.EQPSolutions
							.Where(s => s.IsFound)
							.OrderBy(s => s.DateFound)
							.Select(s => s.DateFound)
							.FirstOrDefault() ?? ""
					})
					.OrderByDescending(l => l.SolutionsFound)
					.ThenBy(l => l.FirstSolutionDate)
					.Take(top)
					.ToListAsync();

				return Ok(leaderboard);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting leaderboard");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("player-solutions/{playerName}")]
		public async Task<ActionResult<List<EQPPlayerSolutionDto>>> GetPlayerSolutions(string playerName)
		{
			try
			{
				var player = await _dbContext.Players
					.Include(p => p.EQPSolutions)
					.FirstOrDefaultAsync(p => p.Name == playerName);

				if (player == null)
				{
					return Ok(new List<EQPPlayerSolutionDto>());
				}

				var solutions = player.EQPSolutions
					.Where(s => s.IsFound)
					.OrderBy(s => s.DateFound)
					.Select(s => new EQPPlayerSolutionDto
					{
						SolutionID = s.SolutionID,
						DateFound = s.DateFound,
						Solution_Text = s.Solution_Text
					})
					.ToList();

				return Ok(solutions);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting player solutions");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("algorithm-times")]
		public async Task<ActionResult<List<EQPAlgorithmTimeDto>>> GetAlgorithmTimes([FromQuery] int count = 10)
		{
			try
			{
				var times = await _dbContext.EQPAlgoTimes
					.OrderByDescending(a => a.DateExecuted)
					.Take(count)
					.Select(a => new EQPAlgorithmTimeDto
					{
						AlgorithmType = a.AlgorithmType,
						TimeTaken_ms = a.TimeTaken_ms,
						RoundNumber = a.RoundNumber,
						DateExecuted = a.DateExecuted
					})
					.ToListAsync();

				return Ok(times);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting algorithm times");
				return StatusCode(500, "Internal server error");
			}
		}

		#region Helper Methods

		private string ConvertToSolutionText(List<QueenPosition> positions)
		{
			// Sort by row to ensure canonical representation
			var sorted = positions.OrderBy(p => p.Row).ToList();
			return string.Join(",", sorted.Select(p => $"{p.Row}-{p.Col}"));
		}

		private async Task<int> GetTotalPossibleSolutions()
		{
			// For 8x8 board, there are exactly 92 solutions
			return await Task.FromResult(92);
		}

		private async Task<int> GetNextRoundNumber()
		{
			var maxRound = await _dbContext.EQPAlgoTimes
				.Select(a => (int?)a.RoundNumber)
				.MaxAsync();
			
			return (maxRound ?? 0) + 1;
		}

		private async Task ResetAllSolutions()
		{
			// Reset all IsFound flags to false instead of deleting records
			var allSolutions = await _dbContext.EQPSolutions.ToListAsync();
			foreach (var solution in allSolutions)
			{
				solution.IsFound = false;
				solution.PlayerID = null;
				solution.DateFound = null;
			}
			await _dbContext.SaveChangesAsync();
			
			_logger.LogInformation("All EQP solutions have been reset. New game started.");
		}

		#endregion
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
		public int RoundNumber { get; set; }
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
		public bool IsAlreadyFound { get; set; }
		public int FoundSolutionsCount { get; set; }
		public int TotalSolutionsCount { get; set; }
		public bool AllSolutionsFound { get; set; }
	}

	public class EQPGameStatsDto
	{
		public int TotalSolutionsCount { get; set; }
		public int FoundSolutionsCount { get; set; }
		public int RemainingSolutionsCount { get; set; }
		public int UniquePlayers { get; set; }
	}

	public class EQPLeaderboardEntryDto
	{
		public string PlayerName { get; set; } = string.Empty;
		public int SolutionsFound { get; set; }
		public string FirstSolutionDate { get; set; } = string.Empty;
	}

	public class EQPPlayerSolutionDto
	{
		public int SolutionID { get; set; }
		public string DateFound { get; set; } = string.Empty;
		public string Solution_Text { get; set; } = string.Empty;
	}

	public class EQPAlgorithmTimeDto
	{
		public string AlgorithmType { get; set; } = string.Empty;
		public double TimeTaken_ms { get; set; }
		public int RoundNumber { get; set; }
		public string DateExecuted { get; set; } = string.Empty;
	}

	#endregion
}

