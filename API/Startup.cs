using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enums;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Infraestrutura.Db;
using minimal_api.Dominio.Servicos;
public class Startup{

    public Startup(IConfiguration configuration){
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }
    public IConfiguration Configuration { get; set; } = default!;
    private string key { get; set; }

    public void ConfigureServices(IServiceCollection services){
        services.AddAuthentication(option => {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option => {
            option.TokenValidationParameters = new TokenValidationParameters{
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();    

        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>{
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT aqui:"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        });

        var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
        if (string.IsNullOrEmpty(mysqlPassword))
        {
            throw new InvalidOperationException("The environment variable MYSQL_PASSWORD is not set.");
        }

        var connectionString = Configuration.GetConnectionString("MySql");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("The connection string 'mysql' is not set in the configuration.");
        }

        connectionString = connectionString.Replace("${MYSQL_PASSWORD}", mysqlPassword);

        services.AddDbContext<DbContexto>(options => {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env){
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            #region Endpoints

            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

            #region Endpoints ADMINISTRADOR

            string GerarTokenJwt(Administrador administrador){
                if (string.IsNullOrEmpty(key))  
                {
                    return string.Empty;
                }
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>{
                    new Claim("Email", administrador.Email),
                    new Claim("Perfil", administrador.Perfil),
                    new Claim(ClaimTypes.Role, administrador.Perfil)
                };
                var token = new JwtSecurityToken(
                    claims: claims,
                    //issuer: "minimal-api",
                    //audience: "minimal-api",
                    expires: DateTime.Now.AddHours(12),
                    signingCredentials: credentials
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            ErrosDeValidacao validaAdministradorDTO(AdministradorDTO administradorDTO){
                var validacao = new ErrosDeValidacao{
                    Mensagens = new List<string>()
                };

                if (string.IsNullOrEmpty(administradorDTO.Email))
                {
                    validacao.Mensagens.Add("O email não pode ser vazio");
                }
                if (string.IsNullOrEmpty(administradorDTO.Senha))
                {
                    validacao.Mensagens.Add("A senha não pode ser vazia");
                }
                if (administradorDTO.Perfil == null)
                {
                    validacao.Mensagens.Add("O perfil não pode ser vazio");
                }
                return validacao;
            }

            endpoints.MapPost("/Administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
                var administrador = administradorServico.Login(loginDTO);
                if (administrador != null){
                    string token = GerarTokenJwt(administrador);
                    return Results.Ok(new AdministradorLogadoModelView{
                        Token = token,
                        Email = administrador.Email,
                        Perfil = administrador.Perfil
                    });
                }
                else{
                    return Results.Unauthorized();
                }
            })
            .AllowAnonymous()
            .WithTags("Administradores");

            endpoints.MapGet("/Administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) => {
                var admsModelView = new List<AdministradorModelView>();
                var administradores = administradorServico.Todos(pagina);

                foreach (var adm in administradores)
                {
                    admsModelView.Add(new AdministradorModelView{
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });
                }

                return Results.Ok(admsModelView);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Administrador"})
            .WithTags("Administradores");

            endpoints.MapGet("/Administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) => {
                var administrador = administradorServico.BuscaPorId(id);

                if (administrador == null)
                {
                    return Results.NotFound();
                }
                
                var adminModelView = new AdministradorModelView{
                            Id = administrador.Id,
                            Email = administrador.Email,
                            Perfil = administrador.Perfil
                        };
                return Results.Ok(adminModelView);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Administrador"})
            .WithTags("Administradores");

            endpoints.MapPut("/Administradores/{id}", ([FromRoute] int id, AdministradorDTO administradorDTO, IAdministradorServico administradorServico) => {
                var administrador = administradorServico.BuscaPorId(id);

                if (administrador == null)
                {
                    return Results.NotFound();
                }

                var validacao = validaAdministradorDTO(administradorDTO);

                if (validacao.Mensagens.Count > 0)
                {
                    return Results.BadRequest(validacao);
                }
                administrador.Email = administradorDTO.Email;
                administrador.Senha = administradorDTO.Senha;
                administrador.Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString();
                
                administradorServico.Alterar(administrador);

                return Results.Ok(administrador);

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Administrador"})
            .WithTags("Administradores");

            endpoints.MapPost("/Administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) => {
                var validacao = validaAdministradorDTO(administradorDTO);

                if (validacao.Mensagens.Count > 0)
                {
                    return Results.BadRequest(validacao);
                }
                var administrador = new Administrador{
                        Email = administradorDTO.Email,
                        Senha = administradorDTO.Senha,
                        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
                    };
                administradorServico.Incluir(administrador);

                var adminModelView = new AdministradorModelView{
                            Id = administrador.Id,
                            Email = administrador.Email,
                            Perfil = administrador.Perfil
                        };

                return Results.Created($"/Administrador/{adminModelView.Id}", adminModelView);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Administrador"})
            .WithTags("Administradores");

            endpoints.MapDelete("/Administradores/{id}", (int id, IAdministradorServico administradorServico) => {
                var adminstrador = administradorServico.BuscaPorId(id);
                if (adminstrador == null)
                {
                    return Results.NotFound();
                }

                administradorServico.Deletar(adminstrador);
                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Administrador"})
            .WithTags("Administradores");

            #endregion

            #region Endpoints VEICULO
            ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO){
                var validacao = new ErrosDeValidacao{
                    Mensagens = new List<string>()
                };

                if (string.IsNullOrEmpty(veiculoDTO.Nome))
                {   
                    validacao.Mensagens.Add("O nome não pode ser vazio");
                }

                if (string.IsNullOrEmpty(veiculoDTO.Marca))
                {   
                    validacao.Mensagens.Add("A marca não pode estar em branco");
                }

                if (veiculoDTO.Ano < 1950)
                {   
                    validacao.Mensagens.Add("Veículo muito antigo, é somente aceito veículos cujo ano seja maior que 1950");
                }
                return validacao;
            }

            endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
                var validacao = validaDTO(veiculoDTO);

                if(validacao.Mensagens.Count > 0){
                    return Results.BadRequest(validacao);
                }

                var veiculo = new Veiculo{
                    Nome = veiculoDTO.Nome,
                    Marca = veiculoDTO.Marca,
                    Ano = veiculoDTO.Ano
                };
                veiculoServico.Incluir(veiculo);

                return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Administrador, Editor"})
            .WithTags("Veiculos");

            endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) => {
                var veiculos = veiculoServico.Todos(pagina);
                return Results.Ok(veiculos);
            })
            .RequireAuthorization()
            .WithTags("Veiculos");

            endpoints.MapGet("/veiculos/{id}", (int id, IVeiculoServico veiculoServico) => {
                var veiculo = veiculoServico.BuscaPorId(id);
                if (veiculo == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Administrador, Editor"})
            .WithTags("Veiculos");

            endpoints.MapPut("/veiculos/{id}", ([FromRoute]int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
                var veiculo = veiculoServico.BuscaPorId(id);
                if (veiculo == null)
                {
                    return Results.NotFound();
                }

                var validacao = validaDTO(veiculoDTO);

                if(validacao.Mensagens.Count > 0){
                    return Results.BadRequest(validacao);
                }

                veiculo.Nome = veiculoDTO.Nome;
                veiculo.Marca = veiculoDTO.Marca;
                veiculo.Ano = veiculoDTO.Ano;
                veiculoServico.Atualizar(veiculo);

                return Results.Ok(veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Administrador"})
            .WithTags("Veiculos");

            endpoints.MapDelete("/veiculos/{id}", (int id, IVeiculoServico veiculoServico) => {
                var veiculo = veiculoServico.BuscaPorId(id);
                if (veiculo == null)
                {
                    return Results.NotFound();
                }

                veiculoServico.Apagar(veiculo);
                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Administrador"})
            .WithTags("Veiculos");

            #endregion

            #endregion
        });
    }

}