namespace SimpleBlog
{
    using System.Threading.Tasks;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class SimpleBlog
    {
        public static AppFunc App()
        {
            return env => Task.FromResult(0);
        }
    }
}