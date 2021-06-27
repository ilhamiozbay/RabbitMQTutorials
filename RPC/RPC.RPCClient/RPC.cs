using System;
using System.Threading.Tasks;

namespace RPC.RPCClient
{
    class RPC
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RPC Client");
            string n = args.Length > 0 ? args[0] : "30";
            Task task = InvokeAsync(n);
            task.Wait();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static async Task InvokeAsync(string n)
        {
            var rpcClient = new RPCClient();

            Console.WriteLine(" [x] Requesting fibonacci({0})", n);
            var response = await rpcClient.CallAsync(n.ToString());
            Console.WriteLine(" [.] Got '{0}'", response);

            rpcClient.Close();
        }

    }
}
