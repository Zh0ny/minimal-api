using minimal_api.Dominio.Enums;

namespace minimal_api.Dominio.ModelViews
{
    public record AdministradorModelView
    {   
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _email = default!;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        private string _perfil = default!;
        public string Perfil
        {
            get { return _perfil; }
            set { _perfil = value; }
        }
    }
}