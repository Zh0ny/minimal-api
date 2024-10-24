using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;

namespace minimal_api.Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _db;
        public AdministradorServico(DbContexto db){
            _db = db;
        }

        public Administrador? Alterar(Administrador admin)
        {
            _db.Administradores.Update(admin);
            _db.SaveChanges();

            return admin;
        }

        public Administrador? BuscaPorId(int id)
        {
            return _db.Administradores.Where(a => a.Id == id).FirstOrDefault();
        }

        public Administrador? Deletar(Administrador admin)
        {
            _db.Administradores.Remove(admin);
            _db.SaveChanges();

            return admin;
        }

        public Administrador? Incluir(Administrador admin)
        {
            _db.Administradores.Add(admin);
            _db.SaveChanges();

            return admin;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            var admin = _db.Administradores.Where( a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return admin;
        }

        public List<Administrador> Todos(int? pagina)
        {
            var query = _db.Administradores.AsQueryable();

            int itemsPorPagina = 10;
            if (pagina != null)
            {
            query = query.Skip(((int)pagina - 1) * itemsPorPagina).Take(itemsPorPagina);
                
            }

            return query.ToList();
        }
    }

}