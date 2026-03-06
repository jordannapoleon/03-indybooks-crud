using Microsoft.EntityFrameworkCore;
using IndyBooks.Services;

var builder = WebApplication.CreateBuilder(args);

//Enable MVC and DIJ Services for this application
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<Repository>(); //Adds the Repository Service to the DI Container
//Initializes the DBC Service for this application using the SQLite database
var connection = builder.Configuration.GetConnectionString("IndyBooks-Sqlite");
builder.Services.AddDbContext<IndyBooks.Services.IndyBooksDataContext>(options =>
    options.UseSqlite(connection));

var app = builder.Build();


/* Middleware in the HTTP Request Pipeline
 */

if (app.Environment.IsDevelopment())
{
    app.InitializeDb();    //custom extension method to seed the DB
}

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id}",
    defaults: new
    {
        controller = "Admin",
        action = "Index",
        id = 0 //TODO: set id default to 0 to show all books by default
    });

app.Run();
