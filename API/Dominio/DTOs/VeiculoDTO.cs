using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Dominio.DTOs
{
    public record VeiculoDTO{
        private string _nome = default!;
        private string _marca = default!;
        private int _ano = default!;
        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }
        public string Marca
        {
            get { return _marca; }
            set { _marca = value; }
        }
        public int Ano
        {
            get { return _ano; }
            set { _ano = value; }
        }
    
    }
}