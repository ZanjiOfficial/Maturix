namespace MATURIXSHIFTPROJECT.Interfaces
{
    public interface Irepository<T> where T : class
    {
        void Create(T entity);
        T Read(object id);
        List<T> GetAll();
        void Update(T entity);
        void Delete(object id);
    }
}
