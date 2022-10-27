using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

CreateEmptyDb();
AddData();
ReadData();


static void CreateEmptyDb()
{
    using var dbContext = new ApplicationDbContext();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    dbContext.SaveChanges();
}

static void AddData()
{
    using var dbConte = new ApplicationDbContext();

    var course = new Course
    {
        Name = "Entity Framework Core Basic",
        LessonQuantity = 10,
        CreatedAt = new DateTimeOffset(2007,
            1,
            1,
            1,
            1,
            1,
            TimeSpan.Zero),
        FinancilaCourseInfo = new FinancialCourseInfo
        {
            Price = 15
        },
    };

    dbConte.Add(course);

    dbConte.SaveChanges();
}

static void ReadData()
{
    using var dbContext = new ApplicationDbContext();

    var course = dbContext.Courses.First();
    Console.WriteLine(
        new String('-',
        80));
    Console.WriteLine("Course Info: ");
    Console.WriteLine(
        $"Course Name: {course.Name}. " +
        $"Lessons Qt: {course.LessonQuantity}. " +
        $"Price: {course.FinancilaCourseInfo?.Price ?? -1}."); // null ise -1 olarak al
    Console.WriteLine(
        new String('-',
        80));

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
            new[] { DbLoggerCategory.Database.Command.Name },
            LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //explicit явно acik bir sekilde 2 varlik entitynin ayni tabloya ait oldugunu gosteriyoruz
        modelBuilder
            .Entity<Course>()
            // kursun finans varliginin var oldugunu belirtiyoruz
            .OwnsOne(
            t => t.FinancilaCourseInfo,
            t =>
            {
                // finansin sahibi var oldugunu belirtiyoruz
                t.WithOwner();
            });
    }
}

public class FinancialCourseInfo
{
    public decimal Price { get; set; }
}

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LessonQuantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public FinancialCourseInfo? FinancilaCourseInfo { get; set; }
}