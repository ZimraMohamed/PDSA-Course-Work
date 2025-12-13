using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDSA.API.Data;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.SnakeAndLadder;
using PDSA.Core.Algorithms.SnakeAndLadder;

namespace PDSA.API.Controllers.Games
{
    [Route("api/[controller]")]
    [ApiController]
    public class SALController : ControllerBase
    {
        private readonly SALGameService _service;
        private readonly PDSADbContext _context;

        public SALController(PDSADbContext context)
        {
            _service = new SALGameService();
            _context = context;
        }

        /// <summary>
        /// Generate a new random Snake and Ladder game round (for frontend)
        /// </summary>
        [HttpPost("new-game")]
        public IActionResult NewGame([FromBody] NewGameRequest request)
        {
            try
            {
                int boardSize = request.BoardSize > 0 ? request.BoardSize : 10;
                
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
        /// Solve a Snake and Ladder game using both algorithms (for frontend)
        /// </summary>
        [HttpPost("solve")]
        public IActionResult SolveGame([FromBody] SolveRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.GameId))
                    return BadRequest(new { message = "Game ID is required" });

                // Convert frontend format to SALRequest
                var salRequest = new SALRequest
                {
                    BoardSize = request.BoardSize,
                    Snakes = request.Snakes.Select(s => new PDSA.Core.Algorithms.SnakeAndLadder.Snake 
                    { 
                        Head = s.Head, 
                        Tail = s.Tail 
                    }).ToList(),
                    Ladders = request.Ladders.Select(l => new PDSA.Core.Algorithms.SnakeAndLadder.Ladder 
                    { 
                        Bottom = l.Bottom, 
                        Top = l.Top 
                    }).ToList()
                };

                var solution = _service.SolveGame(salRequest);
                
                return Ok(new
                {
                    gameId = request.GameId,
                    minimumThrows = solution.MinimumThrows,
                    algorithmResults = solution.AlgorithmResults
                });
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
        /// Validate user's answer and save to database
        /// </summary>
        [HttpPost("validate-answer")]
        public IActionResult ValidateAnswer([FromBody] ValidateAnswerRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.GameId))
                    return BadRequest(new { message = "Game ID is required" });

                if (string.IsNullOrWhiteSpace(request.PlayerName))
                    return BadRequest(new { message = "Player name is required" });

                bool isCorrect = request.UserAnswer == request.CorrectAnswer;

                // Only save to database if answer is correct
                if (isCorrect)
                {
                    // Get or create player
                    var player = _context.Players.FirstOrDefault(p => p.Name == request.PlayerName);
                    if (player == null)
                    {
                        player = new Player { Name = request.PlayerName };
                        _context.Players.Add(player);
                        _context.SaveChanges();
                    }

                    // Create round record
                    var round = new SnakeLadderRound
                    {
                        PlayerID = player.PlayerID,
                        BoardSize_N = request.BoardSize,
                        NumLadders = request.NumLadders,
                        NumSnakes = request.NumSnakes,
                        CorrectMinThrows = request.CorrectAnswer,
                        DatePlayed = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    _context.SnakeLadderRounds.Add(round);
                    _context.SaveChanges();

                    // Save board configuration
                    foreach (var snake in request.Snakes)
                    {
                        var config = new SnakeLadderBoardConfig
                        {
                            RoundID = round.RoundID,
                            FeatureType = "Snake",
                            Start_Cell = snake.Head,
                            End_Cell = snake.Tail
                        };
                        _context.SnakeLadderBoardConfigs.Add(config);
                    }

                    foreach (var ladder in request.Ladders)
                    {
                        var config = new SnakeLadderBoardConfig
                        {
                            RoundID = round.RoundID,
                            FeatureType = "Ladder",
                            Start_Cell = ladder.Bottom,
                            End_Cell = ladder.Top
                        };
                        _context.SnakeLadderBoardConfigs.Add(config);
                    }

                    // Save algorithm execution times
                    foreach (var algoResult in request.AlgorithmResults)
                    {
                        var algoTime = new SnakeLadderAlgoTime
                        {
                            RoundID = round.RoundID,
                            AlgorithmName = algoResult.AlgorithmName,
                            TimeTaken_ms = algoResult.ExecutionTimeMs
                        };
                        _context.SnakeLadderAlgoTimes.Add(algoTime);
                    }

                    _context.SaveChanges();
                }

                return Ok(new
                {
                    isCorrect = isCorrect,
                    correctAnswer = request.CorrectAnswer
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// <summary>
        /// Solve a Snake and Ladder game using both algorithms (legacy endpoint)
        /// </summary>
        [HttpPost("solve-legacy")]
        public IActionResult SolveGameLegacy([FromBody] SALRequest request)
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

        /// <summary>
        /// Get player history
        /// </summary>
        [HttpGet("player-history/{playerName}")]
        public IActionResult GetPlayerHistory(string playerName)
        {
            try
            {
                var player = _context.Players.FirstOrDefault(p => p.Name == playerName);
                if (player == null)
                {
                    return Ok(new List<object>());
                }

                var rounds = _context.SnakeLadderRounds
                    .Where(r => r.PlayerID == player.PlayerID)
                    .OrderByDescending(r => r.DatePlayed)
                    .ToList();

                var history = rounds.Select(round => new
                {
                    roundId = round.RoundID,
                    boardSize = round.BoardSize_N,
                    numSnakes = round.NumSnakes,
                    numLadders = round.NumLadders,
                    minimumThrows = round.CorrectMinThrows,
                    datePlayed = round.DatePlayed,
                    algorithmTimes = _context.SnakeLadderAlgoTimes
                        .Where(a => a.RoundID == round.RoundID)
                        .Select(a => new
                        {
                            algorithmName = a.AlgorithmName,
                            timeTakenMs = a.TimeTaken_ms
                        })
                        .ToList(),
                    boardConfig = _context.SnakeLadderBoardConfigs
                        .Where(c => c.RoundID == round.RoundID)
                        .Select(c => new
                        {
                            featureType = c.FeatureType,
                            startCell = c.Start_Cell,
                            endCell = c.End_Cell
                        })
                        .ToList()
                }).ToList();

                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// <summary>
        /// Get leaderboard
        /// </summary>
        [HttpGet("leaderboard")]
        public IActionResult GetLeaderboard([FromQuery] int top = 20)
        {
            try
            {
                var leaderboard = _context.Players
                    .Select(p => new
                    {
                        playerName = p.Name,
                        totalGames = _context.SnakeLadderRounds.Count(r => r.PlayerID == p.PlayerID),
                        averageThrows = _context.SnakeLadderRounds
                            .Where(r => r.PlayerID == p.PlayerID)
                            .Average(r => (double?)r.CorrectMinThrows) ?? 0,
                        bestThrows = _context.SnakeLadderRounds
                            .Where(r => r.PlayerID == p.PlayerID)
                            .Min(r => (int?)r.CorrectMinThrows) ?? 0,
                        lastPlayed = _context.SnakeLadderRounds
                            .Where(r => r.PlayerID == p.PlayerID)
                            .OrderByDescending(r => r.DatePlayed)
                            .Select(r => r.DatePlayed)
                            .FirstOrDefault()
                    })
                    .Where(x => x.totalGames > 0)
                    .OrderBy(x => x.averageThrows)
                    .ThenByDescending(x => x.totalGames)
                    .Take(top)
                    .ToList();

                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }

    // Request models
    public class NewGameRequest
    {
        public int BoardSize { get; set; }
    }

    public class SnakeDto
    {
        public int Head { get; set; }
        public int Tail { get; set; }
    }

    public class LadderDto
    {
        public int Bottom { get; set; }
        public int Top { get; set; }
    }

    public class SALAlgorithmResultDto
    {
        public string AlgorithmName { get; set; } = string.Empty;
        public int MinimumThrows { get; set; }
        public double ExecutionTimeMs { get; set; }
    }

    public class SolveRequest
    {
        public string GameId { get; set; } = string.Empty;
        public int BoardSize { get; set; }
        public List<SnakeDto> Snakes { get; set; } = new List<SnakeDto>();
        public List<LadderDto> Ladders { get; set; } = new List<LadderDto>();
    }

    public class ValidateAnswerRequest
    {
        public string GameId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public int UserAnswer { get; set; }
        public int CorrectAnswer { get; set; }
        public int BoardSize { get; set; }
        public int NumSnakes { get; set; }
        public int NumLadders { get; set; }
        public List<SnakeDto> Snakes { get; set; } = new List<SnakeDto>();
        public List<LadderDto> Ladders { get; set; } = new List<LadderDto>();
        public List<SALAlgorithmResultDto> AlgorithmResults { get; set; } = new List<SALAlgorithmResultDto>();
    }

    // Request model for checking answers
    public class CheckAnswerRequest
    {
        public SALRequest GameData { get; set; } = new SALRequest();
        public int UserAnswer { get; set; }
    }
}
