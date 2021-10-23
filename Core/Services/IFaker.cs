namespace Core.Services
{
    public interface IFaker
    {
        public T Create<T>();
    }
}