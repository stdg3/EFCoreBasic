using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection.Emit;

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
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}


//cource modeli
public class Cource
{
    public int Id { get; set; }

    public string Name { get; set; }
    
    public int LessonQuantity { get; set; }
    
    public DateTimeOffset CeatAt { get; set; }
    
    public decimal Price { get; set; }
    
    //навигационное свойство, за счет которого получаем доступ к другой сущности
    public Author Author { get; set; }
}


public class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Cource>
{
    public void Configure(EntityTypeBuilder<Cource> builder)
    {
        builder
            .ToTable("MyCourses");

        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .HasColumnName("MyName")
            .HasMaxLength(500)
            .IsRequired();

        builder
            .Property(c => c.LessonQuantity)
            .HasColumnName("MyCreatedAt");

        builder
            .Property(c => c.Price)
            .HasColumnName("MyPrice")
            .HasColumnType("money");


    }
}


public class Author
{
    public int Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    //опционально
    public int? Age { get; set; }

    //коллекция курсов у автора
    public ICollection<Cource> Cources { get; set; }
}


public class AuthorEntityTypeConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder
            .ToTable("MyAuthors");

        builder.HasKey(a => a.Id);

        builder
            .Property(a => a.FirstName)
            .HasColumnName("MyFirstName")
            .IsRequired();

        builder
            .Property(a => a.LastName)
            .HasColumnName("MyLastName")
            .IsRequired();

        builder
            .Property(a => a.Age)
            .HasColumnName("MyAge");


    }
}