using Microsoft.EntityFrameworkCore;
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

    dbContext.Add(csharpCourse);
    dbContext.Add(efCore);
    dbContext.SaveChanges();
}

static void ReadData()
{
    using var dbContext = new ApplicationDbContext();

    // get all courses
    var courses = dbContext.Courses.ToList();

    Console.WriteLine(new String('-', 80));
    foreach (var course in courses)
    {
        Console.WriteLine($"Course Name: {course.Name} "+
            $"Qt: {course.LessonsQuantity}");
    }
    Console.WriteLine(new String('-', 80));
    var generatedSql = dbContext.Courses.ToQueryString();
    Console.WriteLine($"Genearted Sql: {generatedSql}.");
}

public class ApplicationDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;");
    }
}

public class Course 
{
    [Key]
    public int CourseId { get; set; }
    public string Name { get; set; }
    public int LessonsQuantity { get; set; }
}
