using System.Text;

namespace Lacuna
{
class Program
{
    static async Task Main()
    {
        StartingPost startingPost = new StartingPost("chel0per","marcelo.smarques7@gmail.com");
        await startingPost.GetAcessToken();
    }
}

}