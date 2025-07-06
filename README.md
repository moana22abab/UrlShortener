# UrlShortener
Gabriela- UrlShortener

my project has a web api that is used to shorten URLs, supports custom aliases, expiration dates, analytics tracking, and automatic cleanup.


## Installation
You must have PostgreSQL installed to your computer
you will need to create a database with this commands in the NuGet Package Manager Console:
dotnet ef database update

Use the .NET CLI to build and run the project.

```bash
dotnet build
dotnet run

you will need to edit the appsettings.json file so it will match you PostgreSQL:
 "DefaultConnection": "Host=localhost;Port=5432;Database=urlshortener;Username=postgres;Password=yourpassword"

EndPoints:
1. POST /shorten
   you will need to enter the original url
   optional you choose a custom string
   expiratioDate by default a week you can change it according to your needs
2. GET /api/{code}
   string- you can enter the shorten version of the URL and you will get the original URL in return
3. GET /api/{code}/analytics
   string- you can enter the shorten version of the URL and the output will be how many times the shorthen version was used
