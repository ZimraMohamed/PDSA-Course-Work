using Microsoft.AspNetCore.Mvc;
using PDSA.Core.Algorithms.SnakeAndLadder;

namespace PDSA.API.Controllers.Games
{
    [Route("api/[controller]")]
    [ApiController]
    public class SALController : ControllerBase
    {
        private readonly SALGameService _service;

        public SALController()
        {
            _service = new SALGameService();
        }

        /// <summary>
        /// Generate a new random Snake and Ladder game round
        /// </summary>
        [HttpPost("generate")]
        public IActionResult GenerateGame([FromQuery] int boardSize = 10)
        {
            try
            {
                if (boardSize < 2 || boardSize > 20)
                    return BadRequest(new { message = "Board size must be between 2 and 20" });

                // Generate board with size-2 snakes and ladders (reasonable density)
                int numSnakes = boardSize - 2;
                int numLadders = boardSize - 2;
                
                var gameRound = _service.GenerateRandomBoard(boardSize, numSnakes, numLadders);
                
                return Ok(new
                {
                    gameId = Guid.NewGuid().ToString(),
                    boardSize = gameRound.BoardSize,
                    snakes = gameRound.Snakes,
                    ladders = gameRound.Ladders
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// <summary>
        /// Solve a Snake and Ladder game using both algorithms
        /// </summary>
        [HttpPost("solve")]
        public IActionResult SolveGame([FromBody] SALRequest request)
        {
            try
            {
                if (request.BoardSize <= 0)
                    return BadRequest(new { message = "Invalid board size" });

                var solution = _service.SolveGame(request);
                
                return Ok(solution);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// <summary>
        /// Check user's answer
        /// </summary>
        [HttpPost("check-answer")]
        public IActionResult CheckAnswer([FromBody] CheckAnswerRequest request)
        {
            try
            {
                var solution = _service.SolveGame(request.GameData);
                bool isCorrect = _service.CheckUserAnswer(request.UserAnswer, solution.MinimumThrows);
                
                return Ok(new
                {
                    correct = isCorrect,
                    correctAnswer = solution.MinimumThrows,
                    algorithmResults = solution.AlgorithmResults,
                    comparison = _service.GetAlgorithmComparison(solution.AlgorithmResults)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// <summary>
        /// Get algorithm comparison for a specific board
        /// </summary>
        [HttpPost("algorithm-comparison")]
        public IActionResult GetAlgorithmComparison([FromBody] SALRequest request)
        {
            try
            {
                var solution = _service.SolveGame(request);
                var comparison = _service.GetAlgorithmComparison(solution.AlgorithmResults);
                
                return Ok(new
                {
                    minimumThrows = solution.MinimumThrows,
                    algorithms = solution.AlgorithmResults,
                    comparison = comparison
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }

    // Request model for checking answers
    public class CheckAnswerRequest
    {
        public SALRequest GameData { get; set; } = new SALRequest();
        public int UserAnswer { get; set; }
    }
}
