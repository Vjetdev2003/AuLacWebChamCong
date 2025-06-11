namespace AuLacChamCong.Services
{
    public interface IBaseRepository<T>
    {
        public T? Get(T entity);
        public IEnumerable<T> Gets(T entity);
        //public IEnumerable<T> Gets(SortConfig<T>? sortConfig, SearchConfig<T>? searchConfig);
        public IEnumerable<T> GetAll();
        //public IEnumerable<T> GetPage(int pageNumber, int pageSize, SortConfig<T>? sortConfig, SearchConfig<T>? searchConfig, out int totalRow);
        public string Create(T entity);
        public string Update(T entity);
        public bool Patch(T entity);
        public bool Delete(T entity);
    }
}
