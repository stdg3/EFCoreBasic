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
    };

    var corseFinancialInfo = new FinancialCourseInfo
    {
        Id = course.Id,
        Name = course.Name,
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
        $"Course name: {financialCourse.Name}. " +
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
        //explicit явно acik bir sekilde 2 varlik entitynin ayni tabloya ait oldugunu gosteriyoruz
        modelBuilder
            .Entity<Course>()
            .ToTable("Cources");

        modelBuilder
            .Entity<FinancialCourseInfo>()
            .ToTable("Cources");

        // bu ayarlarin muhakkak yazilmasi lazim ki dogru ayrilim olsun
        modelBuilder
            .Entity<Course>()
            //kursta finansal veri oldugunu gosteriyoruz
            .HasOne(c => c.FinancilaCourseInfo)
            // one-to-one iliski oldugunu belirt
            .WithOne()
            //efcore anlayabilsin diye bunu da muhakkak giriyoruz
            .HasForeignKey<FinancialCourseInfo>(x => x.Id);

        // ortak parametrelerinin ayarlanmasi
        modelBuilder
            .Entity<Course>()
            .Property(x => x.Name)
            .HasColumnName("Name");
        modelBuilder
            .Entity<FinancialCourseInfo>()
            .Property(x => x.Name)
            .HasColumnName("Name");
    }
}

public class FinancialCourseInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LessonQuantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public FinancialCourseInfo FinancilaCourseInfo { get; set; }
}