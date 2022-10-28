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


    var authorsQueryable = dbContext
        .Authors
        .Include(
            x => x.Cources
            .Where(x => x.LessonQuantity > 5)
            .OrderBy(x => x.LessonQuantity));

    var authors = authorsQueryable.ToList();

    Console.WriteLine(new String('*', 80));
    foreach (var author in authors)
    {
        Console.WriteLine(
            $"Author Name {author.FirstName + " " + author.LastName}");
        foreach (var cource in author.Cources)
        {
            Console.WriteLine(
                $"course name {cource.Name}. "+
                $"les qt: {cource.LessonQuantity}");
        }
    }
    var generatedsql = authorsQueryable.ToQueryString();
    Console.WriteLine(new String('*', 80));
    Console.WriteLine("generated sql\n" + generatedsql);
    Console.WriteLine(new String('*', 80));

}
/*
 Author Name John Smit
course name C# Advanced. les qt: 7
course name Entity Framework Basic. les qt: 10
Author Name Artur Morgan   
********************************************************************************  
generated sql
SELECT [a].[Id], [a].[FirstName], [a].[LastName], [t].[Id], [t].[AuthorId], [t].[LessonQuantity], [t].[Name]
FROM [Authors] AS [a]
LEFT JOIN ( 
    SELECT [c].[Id], [c].[AuthorId], [c].[LessonQuantity], [c].[Name]
    FROM [Cources] AS [c]
    WHERE [c].[LessonQuantity] > 5
) AS [t] ON [a].[Id] = [t].[AuthorId]
    ORDER BY [a].[Id], [t].[LessonQuantity]  
 */

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