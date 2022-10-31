using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

CreateEntity();
AddData();
AddReleatedEntity();
updAtContext();
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

    var john = new Author
    {
        LastName = "Smith",
        FirstName = "John"
    };

    var arthur = new Author
    {
        FirstName = "Arthur",
        LastName = "Morgan"
    };

    cSharp.Author = john;
    efCore.Author = john;

    context.Add(arthur);
    context.Add(cSharp);
    context.Add(efCore);
    context.SaveChanges();
}

static void AddReleatedEntity()
{
    using var context = new ApplicationDbContext();

    var asp = new Course
    {
        Name = "asp",
        LessonQuantity = 7
    };

    var author = context.Authors
        .Include(x => x.Courses)
        .Single(x => x.FirstName == "Arthur");

    author.Courses.Add(asp);

    context.SaveChanges();
}

static void ReadData()
{
    using var dbContext = new ApplicationDbContext();

    var courses = dbContext
        .Courses
        .Include(c => c.Author)
        .ToList();

    foreach (var course in courses)
    {
        Console.WriteLine($"Course Name: {course.Name}. Qt: {course.LessonQuantity}." +
            $"Author: {course.Author.FirstName + " " + course.Author.LastName}");
    }

    var authors = dbContext.Authors.ToList();

    foreach (var author in authors)
    {
        Console.WriteLine($"Author: {author.FirstName + " " + author.LastName}");
    }
}

static void updAtContext()
{
    using var context = new ApplicationDbContext();

    var author = context.Authors.Single(x => x.FirstName == "Arthur");

    author.LastName = "Brown";

    context.SaveChanges();
}


public class ApplicationDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Author> Authors { get; set; }

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
    public Author Author { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Course> Courses { get; set; }
}