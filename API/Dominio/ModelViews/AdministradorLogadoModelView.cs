namespace minimal_api.Dominio.ModelViews
{
    public record AdministradorLogadoModelView{   
        private string _token = default!;
        public string Token
        {
            get { return _token; }
            set { _token = value; }
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