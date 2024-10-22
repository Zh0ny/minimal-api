namespace minimal_api.Dominio.DTOs
{
    public class LoginDTO{
    private string _email;
    public string Email
    {
        get { return _email; }
        set { _email = value; }
    }
    private string _senha;
    public string Senha
    {
        get { return _senha; }
        set { _senha = value; } 
    }
    
}
}