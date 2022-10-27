using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection.Emit;

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
    };

    var corseFinancialInfo = new FinancialCourseInfo
    {
        Id = course.Id,
        Price = 15M
    };

    course.FinancilaCourseInfo = corseFinancialInfo;

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
        $"Course Id:{course.Id}" +
        $"Lessons Qt: {course.LessonQuantity}. " +
        $"Price: {course.FinancilaCourseInfo?.Price ?? -1}."); // null ise -1 olarak al
    Console.WriteLine(
        new String('-',
        80));

    var financialCourse = dbContext.FinancialCourseInfos.First();
    Console.WriteLine(
        new String('-',
        80));
    Console.WriteLine("Finacial Infgormation Abouth Course");
    Console.WriteLine(
        $"Course id: {financialCourse.Id}. " +
        $"Price: {financialCourse.Price}"
        );
    Console.WriteLine(
        new String('-',
    80));
}

public class ApplicationDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; }

    public DbSet<FinancialCourseInfo> FinancialCourseInfos { get; set; }

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
        modelBuilder
            .Entity<Course>()
            // указываем что у курсаа есть одно навигационное свойство с фин инфой
            .HasOne(t => t.FinancilaCourseInfo)
            // указываем что у фин инфы есть навигационное свойство с курсом
            .WithOne(t => t.Course)
            // указываем что фин инфа связана с курсом через такой то внешний ключ
            .HasForeignKey<FinancialCourseInfo>(t => t.CourseId);
    }
}

public class FinancialCourseInfo
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    // 1-1 iliksi icin course tablosuu burada tanimlayip onun idsi iicn alan veriyorsun
    public Course Course { get; set; }
    public int CourseId { get; set; }
}

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LessonQuantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // ayni sekilde 1-1 icin id ve model tanimla
    public int FinancilaCourseInfoId { get; set; }
    public FinancialCourseInfo FinancilaCourseInfo { get; set; }
}