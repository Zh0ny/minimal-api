namespace minimal_api.Dominio.Entidades
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Veiculo
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        private int _id = default!;
        [Required]
        [StringLength(150)]
        private string _nome = default!;
        [Required]
        [StringLength(100)]
        private string _marca = default!;
        [Required]
        private int _ano = default!;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
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