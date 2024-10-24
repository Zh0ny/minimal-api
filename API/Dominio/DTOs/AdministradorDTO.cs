using minimal_api.Dominio.Enums;

namespace minimal_api.Dominio.DTOs
{
    public class AdministradorDTO{
        private string _email = default!;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        private string _senha = default!;
        public string Senha
        {
            get { return _senha; }
            set { _senha = value; } 
        }
        private Perfil? _perfil = default!;
        public Perfil? Perfil
        {
            get { return _perfil; }
            set { _perfil = value; }
        }
    }
}