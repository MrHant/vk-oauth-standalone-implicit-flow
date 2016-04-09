using VkOAuthStandaloneImplicitFlow;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = AuthorizeImplicitFlow.Authorize("000000");
        }
    }
}
