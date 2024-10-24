using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Db
{
    public class DbContexto : DbContext
    {
        private readonly IConfiguration _configurationAppSettings;
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbContexto(IConfiguration configurationAppSettings)
        {
            _configurationAppSettings = configurationAppSettings;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador {Id = 1, Email = "administrador@teste.com", Senha = "123456", Perfil = "admin"}
            );
        }        // Add configurations for other entities as needed

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
                var connectionString = _configurationAppSettings.GetConnectionString("MySql")?.Replace("${MYSQL_PASSWORD}", mysqlPassword);
                if(!string.IsNullOrEmpty(connectionString)){
                    optionsBuilder.UseMySql(
                        connectionString, 
                        ServerVersion.AutoDetect(connectionString));
                }
            }
            
        }
    }
}