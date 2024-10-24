using System.Collections.Generic;
using System.Threading.Tasks;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;

namespace Test.Mocks
{
    public class VeiculoServicoMock : IVeiculoServico
    {
        private static List<Veiculo> veiculos = new List<Veiculo>(){
            new Veiculo { Id = 1, Nome = "Corolla", Marca = "Toyota", Ano = 2020 },
            new Veiculo { Id = 2, Nome = "Civic", Marca = "Honda", Ano = 2019 },
            new Veiculo { Id = 3, Nome = "Focus", Marca = "Ford", Ano = 2018 }
        };

        public List<Veiculo>? Todos(int? pagina, string? nome = null, string? marca = null)
        {
            var query = veiculos.AsQueryable();

            int itemsPorPagina = 10; // Assuming a page size of 10
            if (pagina != null)
            {
                query = query.Skip((pagina.Value - 1) * itemsPorPagina).Take(itemsPorPagina);
            }

            return query.ToList();
        }

        public Veiculo? BuscaPorId(int id)
        {
            return veiculos.FirstOrDefault(v => v.Id == id);
        }

        public void Incluir(Veiculo veiculo)
        {
            veiculo.Id = veiculos.Count() + 1;
            veiculos.Add(veiculo);
        }

        public void Atualizar(Veiculo veiculo)
        {
            var existingVeiculo = veiculos.Find(v => v.Id == veiculo.Id);
            if (existingVeiculo != null)
            {
                existingVeiculo.Nome = veiculo.Nome;
                existingVeiculo.Marca = veiculo.Marca;
                existingVeiculo.Ano = veiculo.Ano;
            }
        }

        public void Apagar(Veiculo veiculo)
        {
            veiculos.Remove(veiculo);
        }
    }
}