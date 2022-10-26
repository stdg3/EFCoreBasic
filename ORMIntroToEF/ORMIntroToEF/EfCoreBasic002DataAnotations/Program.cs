using EfCoreBasic002DataAnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

CreateEmptyDb();
//AddCourceToDb();


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
        .Cources
        //.Where(c => c.LessonQuantity > 8)
        .ToList();

    foreach (var cource in cources)
    {
        Console.WriteLine($"Cource Name {cource.Name}. Qt {cource.LessonQuantity}");
    }
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
        //Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;

        //Server=(localdb)\\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;MultipleActiveResultSets=true 
    }
}


//cource modeli
[Table("MyCourses")]
public class Cource
{
    [Key] // primary key varligin entitinin
    public int Id { get; set; }

    [Required] // zorunlu ve bos olamaz
    [MinLength(Consts.AuthorNameMinLength)]
    [MaxLength(500)]
    [Column("MyName")] //kolonun adi
    public string Name { get; set; }

    [Column("MyLessonQuantity")] //kolon adi
    public int LessonQuantity { get; set; }

    [Column("MyCreatedAt")]
    public DateTimeOffset CeatAt { get; set; }

    [Column(
        "MyPrice",
        TypeName = "money")]
    public decimal Price { get; set; } //kolon adi ve tipini elle girdik DB adini girdin, mssql de money baska db de fakli olabilir o efcorun dokumantasyonuna bakip check etmen gerekebilir bazen

    public int AuthorId { get; set; }

    //kursun yazari var
    [ForeignKey("AuthorId")] // Author ozelligi AuthorId yi FK olarak kullandigini belirtiyoruz

    //навигационное свойство, за счет которого получаем доступ к другой сущности
    public Author Author { get; set; }
}

[Table("MyAuthors")]
public class Author
{
    [Key] public int Id { get; set; }

    [Required]
    [Column("MyFirstName")] //zorunlu ve bos olamaz
    public string FirstName { get; set; }

    [Required]
    [Column("MyLastName")] //zorunlu ve bos olamaz
    public string LastName { get; set; }

    //[Required]
    //опционально
    public int? Age { get; set; } // istege bagli bu datayi girebilir

    //коллекция курсов у автора
    public ICollection<Cource> Cources { get; set; }
}