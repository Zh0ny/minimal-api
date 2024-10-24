using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Dominio.Entidades;
using minimal_api.Infraestrutura.Db;
using minimal_api.Dominio.Servicos;

namespace Test.Domain.Servicos;

[TestClass]
public class AdministradorServicoTeste
{
    private DbContexto CriarContextoDeTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }
    [TestMethod]
    public void TestandoSalvarAdministrador()
    {
        // Arrange
        var adm = new Administrador();
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "123456";
        adm.Perfil = "Administrador";
        var _contexto = CriarContextoDeTeste();
        var administradorServico = new AdministradorServico(_contexto);
        _contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        // Act
        administradorServico.Incluir(adm);
        // Assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count());
    }
    [TestMethod]
        public void TestandoAtualizarAdministrador()
        {
            // Arrange
            var adm = new Administrador
            {
                Id = 5,
                Email = "teste_Alterar@teste.com",
                Senha = "123456",
                Perfil = "Administrador"
            };
            var _contexto = CriarContextoDeTeste();
            var administradorServico = new AdministradorServico(_contexto);
            administradorServico.Incluir(adm);

            // Act
            adm.Email = "novoadminAlterado@teste.com";
            administradorServico.Alterar(adm);

            // Assert
            var admAtualizado = administradorServico.BuscaPorId(adm.Id);
            Assert.IsNotNull(admAtualizado);
            Assert.AreEqual("novoadminAlterado@teste.com", admAtualizado.Email);
        }

        [TestMethod]
        public void TestandoExcluirAdministrador()
        {
            // Arrange
            var adm = new Administrador
            {
                Id = 6,
                Email = "teste@teste.com",
                Senha = "123456",
                Perfil = "Administrador"
            };
            var _contexto = CriarContextoDeTeste();
            var administradorServico = new AdministradorServico(_contexto);
            var TabelaAdministradoresCount = administradorServico.Todos(1).Count();
            _contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
            administradorServico.Incluir(adm);

            // Act
            administradorServico.Deletar(adm);

            // Assert
            Assert.AreEqual(0, administradorServico.Todos(1).Count());
        }

        [TestMethod]
        public void TestandoBuscaPorId()
        {
            // Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var adm = new Administrador();
            adm.Email = "teste@teste.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";

            var administradorServico = new AdministradorServico(context);

            // Act
            administradorServico.Incluir(adm);
            var admDoBanco = administradorServico.BuscaPorId(adm.Id);

            // Assert
            Assert.IsNotNull(admDoBanco);
            Assert.AreEqual(1, admDoBanco.Id);
        }
}