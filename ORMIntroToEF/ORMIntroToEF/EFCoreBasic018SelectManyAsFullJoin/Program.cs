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
        LessonsQuantity = 5
    };

    var efcore = new Course
    {
        Name = "EF Core",
        LessonsQuantity = 7
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

    csharp.Author = artur;
    efcore.Author = artur;

    dbContext.Add(csharp);
    dbContext.Add(efcore);
    dbContext.Add(john);
    dbContext.SaveChanges();
}

static void ReadData()
{
    using var dbContext = new ApplicationDbContext();

    var joinedCoursesQueryble =
        dbContext.Courses
        .SelectMany(
            // her kursu yazarla birlestiriyourz cross join
            course => dbContext.Authors,
            // result
            (course, author) => new { Course = course, Author = author });

    var joinedCourses = joinedCoursesQueryble.ToList();

    Console.WriteLine(new String('*', 80));
    Console.WriteLine("Courses Info");
    foreach (var item in joinedCourses)
    {
        Console.WriteLine(
            $"CourseName: {item.Course.Name}, " +
            $"CourseId: {item.Course.CourseId}, " +
            $"Qt: {item.Course.LessonsQuantity}, " +
            $"Author: {item.Author.FirstName + " " + item.Author.LastName}"
            );
    }
    var generatedSql = joinedCoursesQueryble.ToQueryString();
    Console.WriteLine(new String('*', 80));
    Console.WriteLine($"Generated Sql Query:");
    Console.WriteLine(generatedSql);
}

public class ApplicationDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>()
            .HasMany(t => t.Courses)
            .WithOne(t => t.Author)
            .HasForeignKey(t => t.AuthorIdFK)
            .HasPrincipalKey(t => t.AuthorId);
    }

}


public class Course
{
    [Key]
    public int CourseId { get; set; }
    public string Name { get; set; }
    public int LessonsQuantity { get; set; }
    public int AuthorIdFK { get; set; }
    public Author Author { get; set; }
}


public class Author
{
    [Key]
    public int AuthorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Course> Courses { get; set; }
}