using Microsoft.EntityFrameworkCore;
using System.Linq;


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
    //ef corun contextine kurslari ekledik, fakat bu asamada db ye bir sorgu ger - cek - les - mi - yor
    dbContext.Add(cSharpCource);
    dbContext.Add(efCoreCource);

    //db ye kayit komutu burada devreye giriyor
    dbContext.SaveChanges();
}

static void ReadCourceFromDb()
{
    using var dbContext = new ApplicationDbContext();


    var cources = dbContext.Cources.ToList();

    foreach (var cource in cources)
    {
        Console.WriteLine($"Cource Name {cource.Name}. Qt {cource.LessonQuantity}");
    }
}

//абстакция подключения к бд
public class ApplicationDbContext : DbContext
{
    //абстракция таблицы курсов в бд
    public DbSet<Cource> Cources { get; set; }

    //метод конгигурации подключения к бд
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //mssql baglanti stringi
        optionsBuilder
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;");
        //Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;

        //Server=(localdb)\\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;MultipleActiveResultSets=true 
    }
}


//cource modeli
public class Cource
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LessonQuantity { get; set; }
}