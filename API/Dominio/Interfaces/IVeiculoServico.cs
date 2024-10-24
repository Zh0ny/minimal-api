using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces
{
    public interface IVeiculoServico
    {
        List<Veiculo>? Todos(int? pagina, string? nome = null, string? Marca = null);
        Veiculo? BuscaPorId(int id);
        void Incluir(Veiculo veiculo);

        void Atualizar(Veiculo veiculo);

        void Apagar(Veiculo veiculo);
    }
}