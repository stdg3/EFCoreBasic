using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

CreateEmptyDb();
AddData();
ReadData();


static void CreateEmptyDb()
{
    using var dbContext = new ApplicationDbContext();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}

static void AddData()
{
    using var dbContext = new ApplicationDbContext();

    var csharpCourse = new Course
    {
        Name = "CSharp",
        LessonsQuantity = 7
    };

    var efCore = new Course
    {
        Name = "EF Core",
        LessonsQuantity = 10
    };

    var asp = new Course
    {
        Name = "ASP",
        LessonsQuantity = 5
    };

    dbContext.Add(csharpCourse);
    dbContext.Add(efCore);
    dbContext.Add(asp);
    dbContext.SaveChanges();
}

static void ReadData()
{
    using var dbContext = new ApplicationDbContext();

    // get all courses
    var course = dbContext
        .Courses
        .OrderByDescending(x => x.LessonsQuantity)
        .FirstOrDefault();

    Console.WriteLine(new String('-', 80));
        Console.WriteLine($"Course Name: {course.Name} " +
            $"Qt: {course.LessonsQuantity}");
    Console.WriteLine(new String('-', 80));
    //var generatedSql = dbContext.Courses.ToQueryString();
    //Console.WriteLine($"Genearted Sql: {generatedSql}.");
}

public class ApplicationDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;")
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .LogTo(
            Console.WriteLine,
            new[] { DbLoggerCategory.Database.Command.Name},
            LogLevel.Information);
    }
}

public class Course
{
    [Key]
    public int CourseId { get; set; }
    public string Name { get; set; }
    public int LessonsQuantity { get; set; }
}
