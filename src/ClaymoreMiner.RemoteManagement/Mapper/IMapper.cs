namespace ClaymoreMiner.RemoteManagement.Mapper
{
    internal interface IMapper<in TSource, out TResult>
    {
        TResult Map(TSource source);
    }
}
