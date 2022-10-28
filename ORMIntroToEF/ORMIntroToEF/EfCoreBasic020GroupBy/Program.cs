using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

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

    var csharp = new Course
    {
        Name = "CSharp",
        isFree = false,
        LessonsQuantity = 5
    };

    var efcore = new Course
    {
        Name = "EF Core",
        isFree = false,
        LessonsQuantity = 7
    };

    var asp = new Course
    {
        Name = "asp",
        isFree = true,
        LessonsQuantity = 3
    };

    var john = new Author
    {
        FirstName = "John",
        LastName = "Smith"
    };

    var artur = new Author
    {
        FirstName = "Arthur",
        LastName = "Morgan"
    };

    //csharp.Author = john;
    //efcore.Author = john;

    dbContext.Add(csharp);
    dbContext.Add(efcore);
    dbContext.Add(asp);
    dbContext.SaveChanges();
}

static void ReadData()
{
    using var dbContext = new ApplicationDbContext();


    var groupedCoursesQueryble = dbContext.Courses
        // isfree ye gore gruplama
        .GroupBy(x => x.isFree)
        // her grup icin icceriisndeki kurslari say
        .Select(
            x => new
            {
                isFree = x.Key,
                TotalLessonsQuantity = x.Sum(y => y.LessonsQuantity)
            });

    var groupedCourses = groupedCoursesQueryble.ToList();

    Console.WriteLine(new String('*', 80));
    Console.WriteLine("Courses Info");
    foreach (var groupedCourse in groupedCourses)
    {
        Console.WriteLine(
            groupedCourse.isFree
                ? "Free Course"
                : "Not free courses" +
                $"Total course lesns: {groupedCourse.TotalLessonsQuantity}");
    }
    var generatedSql = groupedCoursesQueryble.ToQueryString();
    Console.WriteLine(new String('*', 80));
    Console.WriteLine($"Generated Sql Query:");
    Console.WriteLine(generatedSql);
}

public class ApplicationDbContext : DbContext
{
    //public DbSet<Author> Authors { get; set; }
    public DbSet<Course> Courses { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;")
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .LogTo(
            Console.WriteLine,
            new[] { DbLoggerCategory.Database.Command.Name },
        LogLevel.Information);
    }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<Course>();
    //}

}


public class Course
{
    [Key]
    public int CourseId { get; set; }
    public string Name { get; set; }
    public int LessonsQuantity { get; set; }
    public bool isFree { get; set; }
    //public int AuthorIdFK { get; set; }
    //public Author Author { get; set; }
}


public class Author
{
    [Key]
    public int AuthorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Course> Courses { get; set; }
}