using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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

    var cSharpCource = new Cource
    {
        Name = "C# Advanced",
        LessonQuantity = 7,
    };

    var efCoreCource = new Cource
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

    cSharpCource.Author = johnSmit;
    efCoreCource.Author = johnSmit;


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


    var cources = dbContext
        .Cources
        .Include(x => x.Author)
        .ToList();
    Console.WriteLine(new String('*', 80));
    foreach (var cource in cources)
    {
        Console.WriteLine(
            $"Course Name {cource.Name}. " +
            $"Course Id {cource.Id}. "+
            $"Qt {cource.LessonQuantity}. "+
            $"Author: {cource.Author.FirstName + " " + cource.Author.LastName}"
            );
    }
    Console.WriteLine(new String('*', 80));
    var authors = dbContext.Authors.ToList();
    foreach (var author in authors)
    {
        Console.WriteLine($"Author: {author.Id + " " + author.FirstName + " " + author.LastName}");
    }
    Console.WriteLine(new String('*', 80));

}

public class ApplicationDbContext : DbContext
{
    //абстракция таблицы курсов в бд
    public DbSet<Cource> Cources { get; set; }
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
        modelBuilder
            .Entity<Author>()
            // настраиваем то что у автора есть множества курсов
            .HasMany(t => t.Cources)
            // настраиваем что у курса есть один аврор
            .WithOne(t => t.Author)
            // задаем внешний ключ курсов
            .HasForeignKey(k => k.AuthorId)
            // указываем главный ключ в сущности автор
            .HasPrincipalKey(k => k.Id);


    }
}


//cource modeli
public class Cource
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LessonQuantity { get; set; }
    public int AuthorId { get; set; }
    // у курса есть автор
    public Author Author { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    //у автора есть мновества курсов
    public ICollection<Cource> Cources { get; set; }
}