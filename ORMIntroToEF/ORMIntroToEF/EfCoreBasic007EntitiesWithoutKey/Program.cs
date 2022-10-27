using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

CreateEmptyDb();
CreateView();
AddCourceToDb();
ReadCourceFromDb();

// key olmadigi icin cud gibi islemler yapilamaz, rawsql kullanmak zorunda


static void CreateEmptyDb()
{
    // подключение к дб исползуя EF Core, using olayina dikkat
    using var dbContext = new ApplicationDbContext();

    //DB silme islemi, bilerek yapiliyor ki elle silmek zorunlulugumuz olmasin
    dbContext.Database.EnsureDeleted();

    // create islemi, eger hali hazirda yoksa db olusturur yoksa bisi yapmaz
    dbContext.Database.EnsureCreated();
}

static void CreateView()
{
    var dbContext = new ApplicationDbContext();

    var createViewCommand = @$"
        CREATE VIEW LongCourses AS
            SELECT c.Name as CourseName
            FROM Courses c
            WHERE c.LessonQuantity > 5";

    // raw sql ile db de view olusturuyoruz
    dbContext.Database.ExecuteSqlRaw(createViewCommand);
}


static void AddCourceToDb()
{
    using var dbContext = new ApplicationDbContext();
    var cSharpCource = new Course
    {
        Name = "C# Advanced",
        LessonQuantity = 3,
    };

    var efCoreCource = new Course
    {
        Name = "Entity-Framework Basic",
        LessonQuantity = 10
    };
    //ef corun contextine kurslari ekledik, fakat bu asamada db ye bir sorgu ger - cek - les - mi - yor
    dbContext.Add(cSharpCource);
    dbContext.Add(efCoreCource);

    //db ye kayit komutu burada devreye giriyor
    dbContext.SaveChanges();
}

static void ReadCourceFromDb()
{
    using var dbContext = new ApplicationDbContext();


    var cources = dbContext
        .LongCourses
        //.Where(c => c.LessonQuantity > 8)
        .First();

    Console.WriteLine(
        new String('-',
        80));
    Console.WriteLine("Course Info: ");
    Console.WriteLine($"Cource Name {cources.CourseName}.");
    Console.WriteLine(
    new String('-',
        80));

}

public class ApplicationDbContext : DbContext
{
    //абстракция таблицы курсов в бд
    public DbSet<Course> Courses { get; set; }

    // view olsa da contextten cagirabilmen icin tanimlamis olman gerek
    public DbSet<LongCourse> LongCourses { get; set; }

    //метод конгигурации подключения к бд
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //mssql baglanti stringi
        optionsBuilder
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;")
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .LogTo(
                Console.WriteLine,
                LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // испол'зуется ссхаблон строитель
        // refactoring.guru/ru/design-patterns/builder
        // позволяет наглядно смоделировать сущность
        modelBuilder
            .Entity<LongCourse>()
            .HasNoKey();

        modelBuilder
            .Entity<LongCourse>()
            .ToView("LongCourses");


    }
}


//cource modeli
public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LessonQuantity { get; set; }
}

//keysiz entity
public class LongCourse
{
    public string CourseName { get; set; }
}