using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add HttpClient Service.
builder.Services.AddHttpClient();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Rainfall Api",
        Version = "v1",
        Description = "An API which provides rainfall data",
        Contact = new OpenApiContact
        {
            Name = "Sorted",
            Url = new Uri("https://www.sorted.com"),
        },
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rainfall Api v1");
});
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    app.MapControllers();
});


app.Run();
