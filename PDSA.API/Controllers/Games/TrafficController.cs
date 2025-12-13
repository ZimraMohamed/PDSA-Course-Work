using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDSA.API.Data;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.TrafficSimulation;
using PDSA.Core.Algorithms.TrafficSimulation;
using System.Collections.Generic;
using System.Linq;

namespace PDSA.API.Controllers.Games
{
    // Request model that matches React payload EXACTLY
    public class TrafficRequest
    {
        public List<TrafficEdge> Edges { get; set; } = new();
        public int PlayerAnswer { get; set; }
        public string PlayerName { get; set; }
    }

    [ApiController]
    [Route("api/traffic")]
    public class TrafficController : ControllerBase
    {
        private readonly TrafficGameService _trafficService = new();
        private readonly PDSADbContext _context;

        public TrafficController(PDSADbContext context)
        {
            _context = context;
        }

        [HttpPost("submit-answer")]
        public IActionResult SubmitAnswer([FromBody] TrafficRequest request)
        {
            try
            {
                // Validate player name
                if (string.IsNullOrWhiteSpace(request.PlayerName))
                {
                    return BadRequest(new { message = "Player name is required" });
                }

                // Build TrafficNetwork using edges sent from React
                var network = new TrafficNetwork
                {
                    Edges = request.Edges ?? new List<TrafficEdge>()
                };

                // Run calculation with both algorithms
                var result = _trafficService.CalculateMaxFlow(network, request.PlayerAnswer);

                // Only save to database if the player's answer is CORRECT
                if (result.Status == "Correct")
                {
                    // Get or create player
                    var player = _context.Players.FirstOrDefault(p => p.Name == request.PlayerName);
                    if (player == null)
                    {
                        player = new Player { Name = request.PlayerName };
                        _context.Players.Add(player);
                        _context.SaveChanges();
                    }

                    // Save round with correct answer
                    var round = new TrafficRound
                    {
                        PlayerID = player.PlayerID,
                        CorrectMaxFlow = result.CorrectAnswer,
                        DatePlayed = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    _context.TrafficRounds.Add(round);
                    _context.SaveChanges();

                    // Save capacities for this round
                    foreach (var edge in network.Edges)
                    {
                        var capacity = new TrafficCapacity
                        {
                            RoundID = round.RoundID,
                            RoadSegment = $"{edge.From}->{edge.To}",
                            Capacity_VehPerMin = edge.Capacity
                        };
                        _context.TrafficCapacities.Add(capacity);
                    }

                    // Save algorithm execution times
                    var edmondsKarpTime = new TrafficAlgoTime
                    {
                        RoundID = round.RoundID,
                        AlgorithmName = "Edmonds-Karp",
                        TimeTaken_ms = result.EdmondsKarpTime
                    };
                    _context.TrafficAlgoTimes.Add(edmondsKarpTime);

                    var dinicTime = new TrafficAlgoTime
                    {
                        RoundID = round.RoundID,
                        AlgorithmName = "Dinic",
                        TimeTaken_ms = result.DinicTime
                    };
                    _context.TrafficAlgoTimes.Add(dinicTime);

                    _context.SaveChanges();
                }

                // Return JSON that frontend expects
                return Ok(new
                {
                    playerAnswer = request.PlayerAnswer,
                    correctAnswer = result.CorrectAnswer,
                    edmondsKarpTime = result.EdmondsKarpTime,
                    dinicTime = result.DinicTime,
                    status = result.Status,
                    message = result.Status == "Correct" 
                        ? "Correct! Your answer has been saved." 
                        : $"Wrong! The correct answer is {result.CorrectAnswer}."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("game-stats")]
        public IActionResult GetGameStats()
        {
            try
            {
                var totalRounds = _context.TrafficRounds.Count();
                var uniquePlayers = _context.TrafficRounds
                    .Select(r => r.PlayerID)
                    .Distinct()
                    .Count();

                var avgEdmondsKarpTime = _context.TrafficAlgoTimes
                    .Where(t => t.AlgorithmName == "Edmonds-Karp")
                    .Average(t => (double?)t.TimeTaken_ms) ?? 0;

                var avgDinicTime = _context.TrafficAlgoTimes
                    .Where(t => t.AlgorithmName == "Dinic")
                    .Average(t => (double?)t.TimeTaken_ms) ?? 0;

                return Ok(new
                {
                    totalRounds,
                    uniquePlayers,
                    avgEdmondsKarpTime = Math.Round(avgEdmondsKarpTime, 2),
                    avgDinicTime = Math.Round(avgDinicTime, 2)
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
                    return Ok(new List<object>());
                }

                var rounds = _context.TrafficRounds
                    .Where(r => r.PlayerID == player.PlayerID)
                    .OrderByDescending(r => r.DatePlayed)
                    .ToList();

                var history = rounds.Select(round => new
                {
                    roundId = round.RoundID,
                    correctMaxFlow = round.CorrectMaxFlow,
                    datePlayed = round.DatePlayed,
                    algorithmTimes = _context.TrafficAlgoTimes
                        .Where(a => a.RoundID == round.RoundID)
                        .Select(a => new
                        {
                            algorithmName = a.AlgorithmName,
                            timeTakenMs = a.TimeTaken_ms
                        })
                        .ToList(),
                    capacities = _context.TrafficCapacities
                        .Where(c => c.RoundID == round.RoundID)
                        .Select(c => new
                        {
                            roadSegment = c.RoadSegment,
                            capacity = c.Capacity_VehPerMin
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

        [HttpGet("leaderboard")]
        public IActionResult GetLeaderboard([FromQuery] int top = 20)
        {
            try
            {
                var leaderboard = _context.Players
                    .Select(p => new
                    {
                        playerName = p.Name,
                        totalGames = _context.TrafficRounds.Count(r => r.PlayerID == p.PlayerID),
                        averageMaxFlow = _context.TrafficRounds
                            .Where(r => r.PlayerID == p.PlayerID)
                            .Average(r => (double?)r.CorrectMaxFlow) ?? 0,
                        bestMaxFlow = _context.TrafficRounds
                            .Where(r => r.PlayerID == p.PlayerID)
                            .Max(r => (double?)r.CorrectMaxFlow) ?? 0,
                        lastPlayed = _context.TrafficRounds
                            .Where(r => r.PlayerID == p.PlayerID)
                            .OrderByDescending(r => r.DatePlayed)
                            .Select(r => r.DatePlayed)
                            .FirstOrDefault()
                    })
                    .Where(x => x.totalGames > 0)
                    .OrderByDescending(x => x.bestMaxFlow)
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

        [HttpGet("algorithm-times")]
        public IActionResult GetAlgorithmTimes()
        {
            try
            {
                var times = _context.TrafficAlgoTimes
                    .Include(t => t.Round)
                    .OrderByDescending(t => t.RoundID)
                    .Take(20)
                    .GroupBy(t => t.RoundID)
                    .Select(g => new
                    {
                        roundId = g.Key,
                        datePlayed = g.First().Round!.DatePlayed,
                        edmondsKarpTime = g.FirstOrDefault(t => t.AlgorithmName == "Edmonds-Karp")!.TimeTaken_ms,
                        dinicTime = g.FirstOrDefault(t => t.AlgorithmName == "Dinic")!.TimeTaken_ms
                    })
                    .ToList();

                return Ok(new { times });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        // Legacy endpoint for backward compatibility
        [HttpPost("maxflow")]
        public IActionResult CalculateMaxFlow([FromBody] TrafficRequest request)
        {
            return SubmitAnswer(request);
        }
    }
}
