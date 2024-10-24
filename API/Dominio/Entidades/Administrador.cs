namespace minimal_api.Dominio.Entidades
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Dominio.Enums;

    public class Administrador
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        private int _id = default!;
        [Required]
        [StringLength(255)]
        private string _email = default!;
        [Required]
        [StringLength(50)]
        private string _senha= default!;
        [Required]
        [StringLength(10)]
        private string _perfil  = default!;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        public string Senha
        {
            get { return _senha; }
            set { _senha = value; }
        }
        public string Perfil
        {
            get { return _perfil; }
            set { _perfil = value; }
        }
    }
}