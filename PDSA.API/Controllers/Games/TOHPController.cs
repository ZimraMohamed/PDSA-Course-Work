using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDSA.API.Data;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.TowerOfHanoi;
using PDSA.API.Models;
using PDSA.API.Services;
using System;
using System.Linq;

namespace PDSA.API.Controllers.Games
{
    // Request model with player name for database integration
    public class TOHPSubmitRequest
    {
        public int NumPegs { get; set; }
        public int NumDisks { get; set; }
        public int UserMovesCount { get; set; }
        public string UserSequence { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TOHPController : ControllerBase
    {
        private readonly TOHPService _service;
        private readonly PDSADbContext _context;

        public TOHPController(PDSADbContext context)
        {
            _service = new TOHPService();
            _context = context;
        }

        [HttpPost("submit-answer")]
        public IActionResult SubmitAnswer([FromBody] TOHPSubmitRequest request)
        {
            if (request.NumDisks <= 0 || request.NumPegs <= 0)
                return BadRequest(new { message = "Number of pegs and disks must be positive." });

            if (string.IsNullOrWhiteSpace(request.PlayerName))
                return BadRequest(new { message = "Player name is required." });

            try
            {
                // Convert to TOHPRequest for the service
                var serviceRequest = new TOHPRequest
                {
                    NumPegs = request.NumPegs,
                    NumDisks = request.NumDisks,
                    UserMovesCount = request.UserMovesCount,
                    UserSequence = request.UserSequence
                };

                var response = _service.CheckUserMoves(serviceRequest);

                // Only save to database if BOTH moves and sequence are correct
                if (response.CorrectMoves && response.CorrectSequence)
                {
                    // Get or create player
                    var player = _context.Players.FirstOrDefault(p => p.Name == request.PlayerName);
                    if (player == null)
                    {
                        player = new Player { Name = request.PlayerName };
                        _context.Players.Add(player);
                        _context.SaveChanges();
                    }

                    // Save round
                    var round = new HanoiRound
                    {
                        PlayerID = player.PlayerID,
                        NumDisks_N = request.NumDisks,
                        NumPegs = request.NumPegs,
                        CorrectMoves_Count = response.OptimalMoves,
                        CorrectMoves_Sequence = string.Join(", ", response.CorrectSequenceList),
                        DatePlayed = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    _context.HanoiRounds.Add(round);
                    _context.SaveChanges();

                    // Save algorithm times for all benchmarked algorithms
                    foreach (var benchmark in response.BenchmarkTimings)
                    {
                        var algoTime = new HanoiAlgoTime
                        {
                            RoundID = round.RoundID,
                            AlgorithmName = benchmark.Key,
                            TimeTaken_ms = benchmark.Value
                        };
                        _context.HanoiAlgoTimes.Add(algoTime);
                    }
                    _context.SaveChanges();
                }

                var result = new
                {
                    correctMoves = response.CorrectMoves,
                    correctSequence = response.CorrectSequence,
                    optimalMoves = response.OptimalMoves,
                    userMoves = response.UserMoves,
                    correctSequenceList = string.Join(", ", response.CorrectSequenceList),
                    message = response.Message,
                    algorithmName = response.AlgorithmName,
                    algorithmTimeMs = response.AlgorithmTimeMs,
                    benchmarkTimings = response.BenchmarkTimings,
                    status = (response.CorrectMoves && response.CorrectSequence) ? "Correct" : "Wrong"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // Legacy endpoint for backward compatibility
        [HttpPost("check-moves")]
        public IActionResult CheckMoves([FromBody] TOHPRequest request)
        {
            if (request.NumDisks <= 0 || request.NumPegs <= 0)
                return BadRequest(new { message = "Number of pegs and disks must be positive." });

            try
            {
                var response = _service.CheckUserMoves(request);

                var result = new
                {
                    correctMoves = response.CorrectMoves,
                    correctSequence = response.CorrectSequence,
                    optimalMoves = response.OptimalMoves,
                    correctSequenceList = string.Join(", ", response.CorrectSequenceList),
                    algorithmName = response.AlgorithmName,
                    algorithmTimeMs = response.AlgorithmTimeMs,
                    benchmarkTimings = response.BenchmarkTimings
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("game-stats")]
        public IActionResult GetGameStats()
        {
            try
            {
                var totalRounds = _context.HanoiRounds.Count();
                var uniquePlayers = _context.HanoiRounds
                    .Select(r => r.PlayerID)
                    .Distinct()
                    .Count();

                var avgTime = _context.HanoiAlgoTimes
                    .Average(t => (double?)t.TimeTaken_ms) ?? 0;

                return Ok(new
                {
                    totalRounds,
                    uniquePlayers,
                    avgAlgorithmTime = Math.Round(avgTime, 2)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("player-history/{playerName}")]
        public IActionResult GetPlayerHistory(string playerName)
        {
            try
            {
                var player = _context.Players.FirstOrDefault(p => p.Name == playerName);
                if (player == null)
                {
                    return Ok(new { rounds = new System.Collections.Generic.List<object>() });
                }

                var rounds = _context.HanoiRounds
                    .Where(r => r.PlayerID == player.PlayerID)
                    .OrderByDescending(r => r.RoundID)
                    .Take(10)
                    .Select(r => new
                    {
                        roundId = r.RoundID,
                        numDisks = r.NumDisks_N,
                        numPegs = r.NumPegs,
                        correctMoves = r.CorrectMoves_Count,
                        datePlayed = r.DatePlayed
                    })
                    .ToList();

                return Ok(new { rounds });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}
