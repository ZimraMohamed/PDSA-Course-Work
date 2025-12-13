using Microsoft.EntityFrameworkCore;
using PDSA.API.Data;
using PDSA.API.Data.Models.EightQueens;
using PDSA.Core.Algorithms.TravelingSalesman;
using PDSA.Core.Algorithms.TrafficSimulation;
using PDSA.Core.Algorithms.EightQueens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add Entity Framework
builder.Services.AddDbContext<PDSADbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=pdsa.db"));

// Add our services
builder.Services.AddScoped<TSPGameService>();
builder.Services.AddScoped<TrafficGameService>();


// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "http://localhost:3003")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PDSADbContext>();
    
    Console.WriteLine("Creating database schema...");
    context.Database.EnsureCreated();
    Console.WriteLine("Database schema created successfully.");
    
    // Log all tables
    var connection = context.Database.GetDbConnection();
    connection.Open();
    var command = connection.CreateCommand();
    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
    var reader = command.ExecuteReader();
    Console.WriteLine("Database tables:");
    while (reader.Read())
    {
        Console.WriteLine($"  - {reader.GetString(0)}");
    }
    reader.Close();
    connection.Close();
    
    // Seed all 92 Eight Queens solutions if not already seeded
    SeedEightQueensSolutions(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");

app.UseAuthorization();
app.MapControllers();

app.Run();

static void SeedEightQueensSolutions(PDSADbContext context)
{
    // Check if solutions already exist
    if (context.EQPSolutions.Any())
    {
        return; // Already seeded
    }

    Console.WriteLine("Seeding Eight Queens solutions...");
    Console.WriteLine("Running Sequential Backtracking algorithm...");

    // Run the Sequential algorithm to get all 92 solutions and measure time
    var (sequentialSolutions, sequentialTime) = EQPSolver.SolveSequential(8);
    Console.WriteLine($"Sequential completed: {sequentialSolutions.Count} solutions in {sequentialTime}ms");

    Console.WriteLine("Running Threaded Backtracking algorithm...");
    
    // Run the Threaded algorithm and measure time
    var (threadedSolutions, threadedTime) = EQPSolver.SolveThreaded(8);
    Console.WriteLine($"Threaded completed: {threadedSolutions.Count} solutions in {threadedTime}ms");

    // Calculate speedup
    var speedup = sequentialTime > 0 ? ((double)(sequentialTime - threadedTime) / threadedTime * 100) : 0;
    Console.WriteLine($"Speedup: Threaded was {speedup:F1}% faster");

    // Save algorithm execution times to database
    var roundNumber = 1; // Initial seed round
    
    var seqAlgoTime = new EQPAlgoTime
    {
        DateExecuted = DateTime.UtcNow.ToString("o"),
        AlgorithmType = "Sequential Backtracking",
        TimeTaken_ms = sequentialTime,
        RoundNumber = roundNumber
    };
    context.EQPAlgoTimes.Add(seqAlgoTime);

    var threadAlgoTime = new EQPAlgoTime
    {
        DateExecuted = DateTime.UtcNow.ToString("o"),
        AlgorithmType = "Threaded Backtracking",
        TimeTaken_ms = threadedTime,
        RoundNumber = roundNumber
    };
    context.EQPAlgoTimes.Add(threadAlgoTime);

    // Save all 92 solutions to database (both algorithms find the same solutions)
    foreach (var solution in sequentialSolutions)
    {
        // Convert int[] solution to text format
        var positions = new List<string>();
        for (int row = 0; row < solution.Length; row++)
        {
            positions.Add($"{row}-{solution[row]}");
        }
        var solutionText = string.Join(",", positions);

        // Add solution to database (not yet found by any player)
        var eqpSolution = new EQPSolution
        {
            Solution_Text = solutionText,
            IsFound = false,
            PlayerID = null,
            DateFound = null
        };

        context.EQPSolutions.Add(eqpSolution);
    }

    context.SaveChanges();
    Console.WriteLine($"Successfully seeded {sequentialSolutions.Count} Eight Queens solutions and recorded algorithm execution times.");
}
