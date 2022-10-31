using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

CreateEntity();
AddData();
//ReadData();

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

    var asp = new Course
    {
        Name = "asp",
        LessonQuantity = 9
    };

    //var john = new Author
    //{
    //    LastName = "Smith",
    //    FirstName = "John"
    //};

    //var arthur = new Author
    //{
    //    FirstName = "Arthur",
    //    LastName = "Morgan"
    //};

    //cSharp.Author = john;
    //efCore.Author = john;

    //context.Add(arthur);
    context.Add(cSharp);
    context.Add(efCore);
    context.Add(asp);

    Console.WriteLine(new String('*', 30));
    Console.WriteLine(context.ChangeTracker.DebugView.LongView == "" ? "Is empty!" : context.ChangeTracker.DebugView.LongView);
    Console.WriteLine(new String('*', 30));
    
    context.SaveChanges();

    Console.WriteLine(new String('*', 30));
    Console.WriteLine(context.ChangeTracker.DebugView.LongView == "" ? "Is empty!" : context.ChangeTracker.DebugView.LongView);
    Console.WriteLine(new String('*', 30));
}

static void ReadData()
{
    using var dbContext = new ApplicationDbContext();
    Console.WriteLine(new String('*', 30));
    Console.WriteLine(dbContext.ChangeTracker.DebugView.LongView == "" ? "Is empty!" : dbContext.ChangeTracker.DebugView.LongView);
    Console.WriteLine(new String('*', 30));


    var courses = dbContext
        .Courses
        //.AsNoTracking()
        .ToList();

    //foreach (var course in courses)
    //{
    //    Console.WriteLine($"Course Name: {course.Name}. Qt: {course.LessonQuantity}." +
    //        $"Author: {course.Author.FirstName + " " + course.Author.LastName}");
    //}

    //var authors = dbContext.Authors.ToList();

    //foreach (var author in authors)
    //{
    //    Console.WriteLine($"Author: {author.FirstName + " " + author.LastName}");
    //}
    Console.WriteLine(dbContext.ChangeTracker.DebugView.LongView == "" ? "Is empty!" : dbContext.ChangeTracker.DebugView.LongView);

}


public class ApplicationDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; }
    //public DbSet<Author> Authors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;")
            ;
    }
}

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LessonQuantity { get; set; }
    //public int AuthorId { get; set; }
    //public Author Author { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    //public ICollection<Course> Courses { get; set; }
}