# WebAlbum
"WebAlbum" is a photo album with users registration (email confirmation included), where you can create albums, upload photos, search for users or albums, view others' pictures and control users as an admin.

##  Technologies
* Onion Architechture;
* Entity Framework 6 as ORM (Code First development);
* ASP.NET Identity 2.x;
* Unit of Work + Generic Repository Patterns;
* Dependency Injection Using Ninject 3.x;
* AutoMapper;
* jQuery Unobtrusive Ajax;
* NSubstitute as a mocking framework.

##Running Locally
To run WebAlbum locally, complete the following steps:
1. Open Package Manager Console and set *WebAlbum.DataAccess* as a Default project
2. Run the *Add-Migration **MigrationName*** command and apply this pending migration to the database using *Update-Database* command
3. Run *WebAlbum.Web* project via your web browser. Tada!
4. Before registration of a new user you have to create *"C:\maildrop"* folder, then finish registration process and confirm your account by clicking a confirmation link in the mail sent to the following local directory.
5. You can also use Admin page that is created by default (see *WebAlbum.DataAccess.Seeding.SeedHelper.cs* for email and password)
