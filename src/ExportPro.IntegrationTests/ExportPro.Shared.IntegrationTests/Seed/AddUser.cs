using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Interfaces;
using ExportPro.Auth.SDK.Models;
using ExportPro.Shared.IntegrationTests.MongoDbContext;
using MongoDB.Driver;
using Refit;

namespace ExportPro.Shared.IntegrationTests.Seed;

public static class AddUser
{
    private static readonly IMongoDbContext<User> MongoDbContext = new MongoDbContext<User>();
    private static readonly IAuth iauth = RestService.For<IAuth>("http://localhost:5000");

    public static async Task<string> AddSuperAdmin()
    {
        UserRegisterDto userDto = new()
        {
            Username = "SuperAdminTest",
            Email = "superadmintest@gmail.com",
            Password = "SuperAdminTest2@",
        };
        UserLoginDto userLoginDto = new() { Email = "superadmintest@gmail.com", Password = "SuperAdminTest2@" };
        await iauth.RegisterAsync(userDto);
        var filter = Builders<User>.Filter.Eq(u => u.Username, "SuperAdminTest");
        var update = Builders<User>.Update.Set(u => u.Role, UserRole.SuperAdmin);
        await MongoDbContext.Collection.UpdateOneAsync(filter, update);
        var responseDto = await iauth.LoginAsync(userLoginDto);
        return responseDto.AccessToken;
    }

    public static Task AddOwner()
    {
        User user = new()
        {
            Username = "OwnerUserTest",
            Role = UserRole.Owner,
            Email = "OwnerUserTest@gmail.com",
            PasswordHash = "OwnerUserTest2@",
        };
        return MongoDbContext.Collection.InsertOneAsync(user);
    }

    public static async Task<string> AddClientAdmin()
    {
        MongoDbContext.Collection.InsertOne(new User());
        UserRegisterDto userDto = new()
        {
            Username = "ClientAdminTest",
            Email = "ClientAdminTest@gmail.com",
            Password = "ClientAdminTest2@",
        };
        UserLoginDto userLoginDto = new() { Email = "ClientAdminTest@gmail.com", Password = "ClientAdminTest2@" };
        await iauth.RegisterAsync(userDto);
        await Task.Delay(10000);
        int attempts = 0;
        var userD = await MongoDbContext
            .Collection.Find(x => x.Email == "ClientAdminTest@gmail.com")
            .SingleOrDefaultAsync();
        while (userD == null && attempts < 5)
        {
            await Task.Delay(1000); // Wait for 1 second before retrying
            userD = await MongoDbContext
                .Collection.Find(x => x.Email == "ClientAdminTest@gmail.com")
                .FirstOrDefaultAsync();
            attempts++;
        }

        if (userD == null)
        {
            throw new Exception("the usern iot fgo");
        }
        await Task.Delay(1000);

        var filter = Builders<User>.Filter.Eq(u => u.Username, userDto.Username);
        var update = Builders<User>.Update.Set(u => u.Role, UserRole.SuperAdmin);
        await MongoDbContext.Collection.UpdateOneAsync(filter, update);
        await Task.Delay(1000);
        var responseDto = await iauth.LoginAsync(userLoginDto);
        await Task.Delay(1000);
        return responseDto.AccessToken;
    }

    public static Task AddOperator()
    {
        User user = new()
        {
            Username = "OperatorTest",
            Role = UserRole.Operator,
            Email = "AddOperatorTest@gmail.com",
            PasswordHash = "AddOperatorTest2@",
        };
        return MongoDbContext.Collection.InsertOneAsync(user);
    }
}
