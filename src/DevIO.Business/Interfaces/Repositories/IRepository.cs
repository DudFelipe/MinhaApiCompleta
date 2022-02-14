using DevIO.Business.Models;
using System.Linq.Expressions;

namespace DevIO.Business.Interfaces.Repositories
{
    //Definindo que para utilizar IRepository, obrigatóriamente a classe tem que ser filha de Entity
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity
    {
        Task Adicionar(TEntity entity);
        Task<TEntity> ObterPorId(Guid id);
        Task<List<TEntity>> ObterTodos();
        Task Atualizar(TEntity entity);
        Task Remover(Guid id);

        //Essa função receba uma função lambda que servirá como uma busca filtrada para retornar uma lista de objetos
        Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate);
        Task<int> SaveChanges();
    }
}
