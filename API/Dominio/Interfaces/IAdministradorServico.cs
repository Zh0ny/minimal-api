using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);
        Administrador? Incluir(Administrador administrador);
        List<Administrador> Todos(int? pagina);
        Administrador? BuscaPorId(int id);
        Administrador? Alterar(Administrador administrador);
        Administrador? Deletar(Administrador administrador);
    }
}