using Core.Database;
using Core.UserDataProviders.Role;
using Core.UserDataProviders.Session;
using Core.UserDataProviders.Users;
using Core.UserServices.Role;
using Core.UserServices.Users;
using DataProviders.BatchProvider;
using DataProviders.DocumentProvider;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Services.BatchService;
using Services.DocumentService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
); 
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
SautinSoft.Pdf.PdfDocument.SetLicense("07/09/25o3eOOCFsy2u7Jiph4Pd7l8cc6gH+04tQ36");

#region Inject connection of database


var env = builder.Configuration.GetValue<string>("Environment");

    var ConnectionString = builder.Configuration.GetConnectionString($"{env}_CORE");
    builder.Services.AddDbContext<MyCoreContext>(options =>
        {
            options.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));
        }
    );

    var ConnectionString1 = builder.Configuration.GetConnectionString($"{env}_SERVER");
        builder.Services.AddDbContext<ExorabilisContext>(options =>
        {
            options.UseMySql(ConnectionString1, ServerVersion.AutoDetect(ConnectionString1));
        }
    );

#endregion

#region inject services batch 

  builder.Services.AddScoped<InterfaceBatchDataProvider, BatchDataProvider>();
  builder.Services.AddScoped<InterfaceBatchAppService, BatchAppService>();

#endregion

#region inject services document 

  builder.Services.AddScoped<InterfaceDocumentDataProvider, DocumentDataProvider>();
  builder.Services.AddScoped<InterfaceDocumentAppService, DocumentAppService>();

#endregion

#region inject User
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserDataProvider, UserDataProvider>();
builder.Services.AddScoped<ISessionDataProvider, SessionDataProvider>();
builder.Services.AddScoped<IRoleDataProvider, RoleDataProvider>();
builder.Services.AddScoped<IRoleService, RoleService>();
#endregion

builder.Services.AddSession();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
//app.UseAuthentication();

//app.MapControllerRoute(
//    name: "BatchLoad",
//    pattern: "EDMS/BatchLoad/{encryptedStep?}",
//    defaults: new { controller = "EDMS", action = "BatchLoad" }
//);

app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");


app.Run();
