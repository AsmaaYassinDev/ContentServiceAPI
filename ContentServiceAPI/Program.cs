using ContentServiceAPI.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    // Retrieve the configuration values
    var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb");
    var cosmosDbAccount = cosmosDbConfig["Account"];
    var cosmosDbKey = cosmosDbConfig["Key"];

    return new CosmosClient(cosmosDbAccount, cosmosDbKey);
});
builder.Services.AddControllers();
builder.Services.AddScoped<IContentService, ContentService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
