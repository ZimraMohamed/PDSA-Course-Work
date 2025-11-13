using Microsoft.AspNetCore.Mvc;
using PDSA.Core.Algorithms.TravelingSalesman;
using System.ComponentModel.DataAnnotations;

namespace PDSA.API.Controllers.Games
{
    [ApiController]
    [Route("api/[controller]")]
    public class TSPController : ControllerBase
    {
        private readonly ILogger<TSPController> _logger;

        public TSPController(ILogger<TSPController> logger)
        {
            _logger = logger;
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

    #endregion
}