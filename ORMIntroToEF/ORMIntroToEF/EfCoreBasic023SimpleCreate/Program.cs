using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

CreateEntity();
AddData();
ReadData();

static void CreateEntity()
{
    using var dbContext = new ApplicationDbContext();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}

static void AddData()
{
    using var context = new ApplicationDbContext();

    var cSharp = new Course
    {
        Name = "CSharp",
        LessonQuantity = 5
    };

    var efCore = new Course
    {
        Name = "EF Core",
        LessonQuantity = 3
    };

    context.Add(cSharp);
    context.Add(efCore);
    context.SaveChanges();
}

static void ReadData()
{
    using var dbContext = new ApplicationDbContext();

    var courses = dbContext.Courses.ToList();

    foreach (var course in courses)
    {
        Console.WriteLine($"Course Name: {course.Name}. Qt: {course.LessonQuantity}");
    }
}


public class ApplicationDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;")
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .LogTo(
        Console.WriteLine,
        LogLevel.Information);
    }
}

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LessonQuantity { get; set; }
}