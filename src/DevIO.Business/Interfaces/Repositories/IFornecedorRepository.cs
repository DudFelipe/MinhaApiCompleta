using DevIO.Business.Models;

namespace DevIO.Business.Interfaces.Repositories
{
    public interface IFornecedorRepository : IRepository<Fornecedor>
    {
        Task<Fornecedor> ObterFornecedorEndereco(Guid id); //Retorna um fornecedor com seu endereço
        Task<Fornecedor> ObterFornecedorProdutosEndereco(Guid id); //Retorna um fornecedor com seus produtos e endereço
    }
}
