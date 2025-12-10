using Microsoft.AspNetCore.Mvc;
using PDSA.Core.Algorithms.TrafficSimulation;
using System.Collections.Generic;

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

        [HttpPost("maxflow")]
        public IActionResult CalculateMaxFlow([FromBody] TrafficRequest request)
        {
            // Build TrafficNetwork using edges sent from React
            var network = new TrafficNetwork
            {
                Edges = request.Edges ?? new List<TrafficEdge>()
            };

            // Run calculation (Make sure your service supports PlayerAnswer parameter)
            var result = _trafficService.CalculateMaxFlow(network, request.PlayerAnswer);

            // Return JSON that frontend expects
            return Ok(new
            {
                playerAnswer = request.PlayerAnswer,
                correctAnswer = result.CorrectAnswer,
                timeTaken = result.TimeTaken,
                status = result.Status
            });
        }
    }
}
