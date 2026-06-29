namespace RaizesDoNordeste.API.Application.Interfaces
{
    public interface IUserContext
    {
        int? ObterUsuarioLogadoId();
        bool IsInRole(string role);
    }
}