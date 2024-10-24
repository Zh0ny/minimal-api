namespace minimal_api.Dominio.DTOs
{
    public class LoginDTO{
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
    
}
}