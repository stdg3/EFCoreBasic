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

    cSharpCource.Authors = new List<Author>
    {
        johnSmit,
        arthurMorgan
    };
    efCoreCource.Authors = new List<Author>
    {
        johnSmit
    };


    //ef corun contextine kurslari ekledik, fakat bu asamada db ye bir sorgu ger - cek - les - mi - yor
    dbContext.Add(cSharpCource);
    dbContext.Add(efCoreCource);
    dbContext.Add(arthurMorgan);

    //db ye kayit komutu burada devreye giriyor
    dbContext.SaveChanges();
}

static void ReadCourceFromDb()
{
    using var dbContext = new ApplicationDbContext();

    // AuthorCourse DBSette olmamasina ragmen Set Parametresiyle erisim sagliyoruz kendisine burasi cok onemli
    var courseAuthors = dbContext
        .Courses
        .Include(x => x.Authors)
        .ToList();

    Console.WriteLine(new String('*', 80));
    Console.WriteLine("Info From Courses Tbl");
    foreach (var cource in courseAuthors)
    {
        Console.WriteLine(
            $"Course Name {cource.Name}. " +
            $"Authors Count: {cource.Authors.Count}");

        foreach (var author in cource.Authors)
        {
            Console.WriteLine($"Author: {author.FirstName + " " + author.LastName}");
        }
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
        // не используем промежуточную сущность, создастся не явно

        // глюент апи конфиг ниже не обязателен
        // по кконвенции еф и так поймет что связь many-to-many
       
        modelBuilder
            .Entity<Author>()
            // у автора есть множ сущ
            .HasMany(t => t.Courses)
            //у пром сущ есть один автор
            .WithMany(y => y.Authors);

        modelBuilder
            .Entity<Course>()
            //у автора есть множ сущ 
            .HasMany(t => t.Authors)
            //у пром сущ есть один курс
            .WithMany(t=>t.Courses);

    }
}


//cource modeli
public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LessonQuantity { get; set; }
    public ICollection<Author> Authors { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    //у автора есть мновества курсов
    public ICollection<Course> Courses { get; set; }
}
