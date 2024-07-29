namespace Qgen.Contracts.Services
{
    public interface SetProvider
    {
        IQueryable<T> Get<T>() where T : class;
    }
}
