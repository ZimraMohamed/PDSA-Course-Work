using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDSA.API.Data;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.TSP;
using PDSA.Core.Algorithms.TravelingSalesman;
using System.ComponentModel.DataAnnotations;

namespace PDSA.API.Controllers.Games
{
    [ApiController]
    [Route("api/[controller]")]
    public class TSPController : ControllerBase
    {
        private readonly ILogger<TSPController> _logger;
        private readonly PDSADbContext _dbContext;

        public TSPController(ILogger<TSPController> logger, PDSADbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new TSP game round
        /// </summary>
        [HttpPost("new-game")]
        public ActionResult<TSPGameRoundDto> CreateNewGame()
        {
            try
            {
                var gameRound = TSPGameService.CreateNewGameRound();
                
                var dto = new TSPGameRoundDto
                {
                    GameId = Guid.NewGuid(),
                    HomeCityName = gameRound.HomeCity.Name,
                    HomeCityIndex = gameRound.HomeCity.Index,
                    DistanceMatrix = ConvertDistanceMatrixTo2DArray(gameRound.DistanceMatrix),
                    AllCities = TSPCities.AllCities.Select(c => new CityDto { Name = c.Name, Index = c.Index }).ToArray()
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new TSP game");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Solves TSP with selected cities using all algorithms
        /// </summary>
        [HttpPost("solve")]
        public ActionResult<TSPSolutionDto> SolveTSP([FromBody] TSPSolveRequest request)
        {
            try
            {
                // Validate input
                if (!TSPGameService.ValidateSelectedCities(request.SelectedCities))
                {
                    return BadRequest("Invalid city selection. Please select at least 2 unique cities from A-J.");
                }

                // Create game round from request
                var gameRound = new TSPGameRound
                {
                    DistanceMatrix = Convert2DArrayToDistanceMatrix(request.DistanceMatrix),
                    HomeCity = TSPCities.GetCityByName(request.HomeCityName)
                };

                // Solve with all algorithms
                var result = TSPGameService.SolveTSPWithAllAlgorithms(gameRound, request.SelectedCities);

                if (!result.Success)
                {
                    return StatusCode(500, result.ErrorMessage);
                }

                var solutionDto = new TSPSolutionDto
                {
                    GameId = request.GameId,
                    OptimalDistance = result.OptimalDistance,
                    OptimalRoute = result.OptimalRoute?.Cities.Select(c => c.Name).ToList() ?? new List<char>(),
                    AlgorithmResults = result.AlgorithmResults.Select(ar => new AlgorithmResultDto
                    {
                        AlgorithmName = ar.AlgorithmName,
                        Distance = ar.Route.TotalDistance,
                        ExecutionTimeMs = ar.ExecutionTimeMs,
                        Route = ar.Route.Cities.Select(c => c.Name).ToList(),
                        Complexity = ar.Complexity
                    }).ToList(),
                    ComplexityAnalysis = TSPGameService.AnalyzeComplexity(result.AlgorithmResults, request.SelectedCities.Count + 1),
                    DistanceMatrixDisplay = TSPGameService.GetDistanceMatrixDisplay(gameRound.DistanceMatrix, gameRound.SelectedCities)
                };

                return Ok(solutionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error solving TSP");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Validates user answer
        /// </summary>
        [HttpPost("validate-answer")]
        public ActionResult<ValidationResultDto> ValidateAnswer([FromBody] AnswerValidationRequest request)
        {
            try
            {
                bool isCorrect = TSPGameService.ValidateUserAnswer(request.UserAnswer, request.CorrectAnswer, request.TolerancePercentage);
                
                var result = new ValidationResultDto
                {
                    IsCorrect = isCorrect,
                    UserAnswer = request.UserAnswer,
                    CorrectAnswer = request.CorrectAnswer,
                    Difference = Math.Abs(request.UserAnswer - request.CorrectAnswer),
                    ToleranceUsed = request.TolerancePercentage
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating answer");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Saves TSP game round to database
        /// </summary>
        [HttpPost("save-game")]
        public async Task<ActionResult<SaveGameResultDto>> SaveGame([FromBody] SaveTSPGameRequest request)
        {
            try
            {
                // Get or create player
                var player = await _dbContext.Players
                    .FirstOrDefaultAsync(p => p.Name == request.PlayerName);

                if (player == null)
                {
                    player = new Player { Name = request.PlayerName };
                    _dbContext.Players.Add(player);
                    await _dbContext.SaveChangesAsync();
                }

                // Create TSP round
                var tspRound = new TSPRound
                {
                    PlayerID = player.PlayerID,
                    HomeCity = request.HomeCityName.ToString(),
                    SelectedCities = string.Join(",", request.SelectedCities),
                    ShortestRoute_Path = string.Join("->", request.OptimalRoute),
                    ShortestRoute_Distance = request.OptimalDistance,
                    DatePlayed = DateTime.UtcNow.ToString("o")
                };

                _dbContext.TSPRounds.Add(tspRound);
                await _dbContext.SaveChangesAsync();

                // Save distances for selected cities
                foreach (var distance in request.Distances)
                {
                    var tspDistance = new TSPDistance
                    {
                        RoundID = tspRound.RoundID,
                        City_A = distance.CityA.ToString(),
                        City_B = distance.CityB.ToString(),
                        Distance_km = distance.Distance
                    };
                    _dbContext.TSPDistances.Add(tspDistance);
                }

                // Save algorithm execution times
                foreach (var algoResult in request.AlgorithmResults)
                {
                    var algoTime = new TSPAlgoTime
                    {
                        RoundID = tspRound.RoundID,
                        AlgorithmName = algoResult.AlgorithmName,
                        TimeTaken_ms = algoResult.ExecutionTimeMs
                    };
                    _dbContext.TSPAlgoTimes.Add(algoTime);
                }

                await _dbContext.SaveChangesAsync();

                return Ok(new SaveGameResultDto
                {
                    Success = true,
                    RoundId = tspRound.RoundID,
                    PlayerId = player.PlayerID,
                    Message = "Game saved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving TSP game");
                return StatusCode(500, new SaveGameResultDto
                {
                    Success = false,
                    Message = "Failed to save game"
                });
            }
        }

        /// <summary>
        /// Gets player game history
        /// </summary>
        [HttpGet("player-history/{playerName}")]
        public async Task<ActionResult<List<TSPRoundHistoryDto>>> GetPlayerHistory(string playerName)
        {
            try
            {
                var player = await _dbContext.Players
                    .Include(p => p.TSPRounds)
                        .ThenInclude(r => r.Distances)
                    .Include(p => p.TSPRounds)
                        .ThenInclude(r => r.AlgorithmTimes)
                    .FirstOrDefaultAsync(p => p.Name == playerName);

                if (player == null)
                {
                    return Ok(new List<TSPRoundHistoryDto>());
                }

                var history = player.TSPRounds.Select(r => new TSPRoundHistoryDto
                {
                    RoundId = r.RoundID,
                    HomeCity = r.HomeCity,
                    SelectedCities = r.SelectedCities.Split(',').Select(c => c[0]).ToList(),
                    ShortestRoute = r.ShortestRoute_Path.Split("->").Select(c => c[0]).ToList(),
                    ShortestDistance = (int)r.ShortestRoute_Distance,
                    DatePlayed = DateTime.Parse(r.DatePlayed),
                    AlgorithmTimes = r.AlgorithmTimes.Select(at => new AlgorithmTimeDto
                    {
                        AlgorithmName = at.AlgorithmName,
                        TimeTakenMs = (long)at.TimeTaken_ms
                    }).ToList()
                }).OrderByDescending(r => r.DatePlayed).ToList();

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching player history");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets leaderboard (top players by average shortest distance)
        /// </summary>
        [HttpGet("leaderboard")]
        public async Task<ActionResult<List<LeaderboardEntryDto>>> GetLeaderboard([FromQuery] int top = 10)
        {
            try
            {
                var leaderboard = await _dbContext.Players
                    .Include(p => p.TSPRounds)
                    .Where(p => p.TSPRounds.Any())
                    .Select(p => new LeaderboardEntryDto
                    {
                        PlayerName = p.Name,
                        TotalGames = p.TSPRounds.Count,
                        AverageDistance = (int)p.TSPRounds.Average(r => r.ShortestRoute_Distance),
                        BestDistance = (int)p.TSPRounds.Min(r => r.ShortestRoute_Distance),
                        LastPlayed = DateTime.Parse(p.TSPRounds.OrderByDescending(r => r.DatePlayed).First().DatePlayed)
                    })
                    .OrderBy(l => l.AverageDistance)
                    .Take(top)
                    .ToListAsync();

                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching leaderboard");
                return StatusCode(500, "Internal server error");
            }
        }

        #region Helper Methods

        private static int[][] ConvertDistanceMatrixTo2DArray(int[,] matrix)
        {
            int size = 10;
            var result = new int[size][];
            for (int i = 0; i < size; i++)
            {
                result[i] = new int[size];
                for (int j = 0; j < size; j++)
                {
                    result[i][j] = matrix[i, j];
                }
            }
            return result;
        }

        private static int[,] Convert2DArrayToDistanceMatrix(int[][] array)
        {
            var result = new int[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    result[i, j] = array[i][j];
                }
            }
            return result;
        }

        #endregion
    }

    #region DTOs

    public class TSPGameRoundDto
    {
        public Guid GameId { get; set; }
        public char HomeCityName { get; set; }
        public int HomeCityIndex { get; set; }
        public int[][] DistanceMatrix { get; set; } = new int[10][];
        public CityDto[] AllCities { get; set; } = new CityDto[10];
    }

    public class CityDto
    {
        public char Name { get; set; }
        public int Index { get; set; }
    }

    public class TSPSolveRequest
    {
        public Guid GameId { get; set; }
        
        [Required]
        public char HomeCityName { get; set; }
        
        [Required]
        [MinLength(2, ErrorMessage = "At least 2 cities must be selected")]
        public List<char> SelectedCities { get; set; } = new List<char>();
        
        [Required]
        public int[][] DistanceMatrix { get; set; } = new int[10][];
    }

    public class TSPSolutionDto
    {
        public Guid GameId { get; set; }
        public int OptimalDistance { get; set; }
        public List<char> OptimalRoute { get; set; } = new List<char>();
        public List<AlgorithmResultDto> AlgorithmResults { get; set; } = new List<AlgorithmResultDto>();
        public AlgorithmComplexityAnalysis? ComplexityAnalysis { get; set; }
        public string DistanceMatrixDisplay { get; set; } = "";
    }

    public class AlgorithmResultDto
    {
        public string AlgorithmName { get; set; } = "";
        public int Distance { get; set; }
        public long ExecutionTimeMs { get; set; }
        public List<char> Route { get; set; } = new List<char>();
        public int Complexity { get; set; }
    }

    public class AnswerValidationRequest
    {
        public int UserAnswer { get; set; }
        public int CorrectAnswer { get; set; }
        public int TolerancePercentage { get; set; } = 5;
    }

    public class ValidationResultDto
    {
        public bool IsCorrect { get; set; }
        public int UserAnswer { get; set; }
        public int CorrectAnswer { get; set; }
        public int Difference { get; set; }
        public int ToleranceUsed { get; set; }
    }

    public class SaveTSPGameRequest
    {
        [Required]
        public string PlayerName { get; set; } = "";
        
        [Required]
        public char HomeCityName { get; set; }
        
        [Required]
        public List<char> SelectedCities { get; set; } = new List<char>();
        
        [Required]
        public List<char> OptimalRoute { get; set; } = new List<char>();
        
        public int OptimalDistance { get; set; }
        
        [Required]
        public List<DistanceDto> Distances { get; set; } = new List<DistanceDto>();
        
        [Required]
        public List<AlgorithmResultDto> AlgorithmResults { get; set; } = new List<AlgorithmResultDto>();
    }

    public class DistanceDto
    {
        public char CityA { get; set; }
        public char CityB { get; set; }
        public int Distance { get; set; }
    }

    public class SaveGameResultDto
    {
        public bool Success { get; set; }
        public int RoundId { get; set; }
        public int PlayerId { get; set; }
        public string Message { get; set; } = "";
    }

    public class TSPRoundHistoryDto
    {
        public int RoundId { get; set; }
        public string HomeCity { get; set; } = "";
        public List<char> SelectedCities { get; set; } = new List<char>();
        public List<char> ShortestRoute { get; set; } = new List<char>();
        public int ShortestDistance { get; set; }
        public DateTime DatePlayed { get; set; }
        public List<AlgorithmTimeDto> AlgorithmTimes { get; set; } = new List<AlgorithmTimeDto>();
    }

    public class AlgorithmTimeDto
    {
        public string AlgorithmName { get; set; } = "";
        public long TimeTakenMs { get; set; }
    }

    public class LeaderboardEntryDto
    {
        public string PlayerName { get; set; } = "";
        public int TotalGames { get; set; }
        public int AverageDistance { get; set; }
        public int BestDistance { get; set; }
        public DateTime LastPlayed { get; set; }
    }

    #endregion
}