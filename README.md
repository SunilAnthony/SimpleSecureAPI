# SimpleSecureAPI
ASP.net Core 2  Authentication use JWT 

I've taken a .net core project from https://auth0.com/blog/asp-dot-net-core-authentication-tutorial/ and modified it to be compatible
.net core 2 changes.

All you have to do is is to modify the connection string and
run the following CLI Command:
<pre>
dotnet ef migrations add InitialCreate
dotnet ef database update
or in Visual Studio:
add-migration InitialCreate
update-database
</pre>
