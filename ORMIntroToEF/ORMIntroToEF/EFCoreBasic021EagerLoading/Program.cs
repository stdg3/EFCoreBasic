using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

CreateEmptyDb();
AddCourceToDb();
ReadCourceFromDb();


static void CreateEmptyDb()
{
    // подключение к дб исползуя EF Core, using olayina dikkat
    using var dbContext = new ApplicationDbContext();

    //DB silme islemi, bilerek yapiliyor ki elle silmek zorunlulugumuz olmasin
    dbContext.Database.EnsureDeleted();

    // create islemi, eger hali hazirda yoksa db olusturur yoksa bisi yapmaz
    dbContext.Database.EnsureCreated();
}

static void AddCourceToDb()
{
    using var dbContext = new ApplicationDbContext();

    var cSharpCource = new Course
    {
        Name = "C# Advanced",
        LessonQuantity = 7,
    };

    var efCoreCource = new Course
    {
        Name = "Entity Framework Basic",
        LessonQuantity = 10
    };

    var johnSmit = new Author
    {
        FirstName = "John",
        LastName = "Smit"
    };

    var arthurMorgan = new Author
    {
        FirstName = "Artur",
        LastName = "Morgan"
    };

    var connections = new[]
    {
        new AuthorCourse
        {
            Course = cSharpCource,
            Author = johnSmit,
        },
        new AuthorCourse
        {
            Course = cSharpCource,
            Author = arthurMorgan,
        },
        new AuthorCourse
        {
            Course = efCoreCource,
            Author = johnSmit,
        }
    };


    //ef corun contextine kurslari ekledik, fakat bu asamada db ye bir sorgu ger - cek - les - mi - yor
    dbContext.Add(cSharpCource);
    dbContext.Add(efCoreCource);
    dbContext.Add(arthurMorgan);
    dbContext.AddRange(connections);

    //db ye kayit komutu burada devreye giriyor
    dbContext.SaveChanges();
}

static void ReadCourceFromDb()
{
    using var dbContext = new ApplicationDbContext();

    // AuthorCourse DBSette olmamasina ragmen Set Parametresiyle erisim sagliyoruz kendisine burasi cok onemli
    var courseAuthors = dbContext
        .Set<AuthorCourse>()
        .Include(x => x.Author)
        .Include(x => x.Course)
        .ToList();
    /*
      SELECT [a].[CourseId], [a].[AuthorId], [a].[AuthorCourseId], [a0].[Id], [a0].[FirstName], [a0].[LastName], [c].[Id], [c].[LessonQuantity], [c].[Name]
    FROM [AuthorCourse] AS [a]                    
    INNER JOIN [Authors] AS [a0] ON [a].[AuthorId] = [a0].[Id]
    INNER JOIN [Courses] AS [c] ON [a].[CourseId] = [c].[Id] 
     */

    Console.WriteLine(new String('*', 80));
    Console.WriteLine("Info From Author Courses Tbl");
    foreach (var cource in courseAuthors)
    {
        Console.WriteLine(
            $"Course Name {cource.Course.Name}. " +
            $"Author: {cource.Author.FirstName + " " + cource.Author.LastName}"
            );
    }
    Console.WriteLine(new String('*', 80));
}

public class ApplicationDbContext : DbContext
{
    //абстракция таблицы курсов в бд
    public DbSet<Course> Courses { get; set; }
    public DbSet<Author> Authors { get; set; }

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
        // с использование промежуточнойсущности 
        // получается что настраиваем два отношения one-to-many
        // от отного курса к множеству промежуточных сущностей
        // от отного автора к множеству промежуточных сущностей

        // очень важно указать здесь составной ключ
        // иначе ефкоре не поймет что нужна связь many-to-many
        modelBuilder
            .Entity<AuthorCourse>()
            .HasKey(t => new { t.CourseId, t.AuthorId });

        modelBuilder
            .Entity<Author>()
            .HasMany(t => t.AuthorCourses)
            .WithOne(t => t.Author)
            .HasForeignKey(t => t.AuthorId)
            .HasPrincipalKey(t => t.Id);

        modelBuilder
            .Entity<Course>()
            .HasMany(t => t.AuthorCourses)
            .WithOne(t => t.Course)
            .HasForeignKey(t => t.CourseId)
            .HasPrincipalKey(t => t.Id);

    }
}


//cource modeli
public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LessonQuantity { get; set; }
    public ICollection<AuthorCourse> AuthorCourses { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    //у автора есть мновества курсов
    public ICollection<AuthorCourse> AuthorCourses { get; set; }
}

public class AuthorCourse
{
    [Key]
    public int AuthorCourseId { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }

    public int AuthorId { get; set; }
    public Author Author { get; set; }
}