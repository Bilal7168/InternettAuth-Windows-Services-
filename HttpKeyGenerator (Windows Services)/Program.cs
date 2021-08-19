using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace InternetAuth__Windows_Services_
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<GenerateKey>(s =>
               {
                   s.ConstructUsing(GenerateKey => new GenerateKey());
                   s.WhenStarted(GenerateKey => GenerateKey.Start());
                   s.WhenStopped(GenerateKey => GenerateKey.Stop());
               });

                x.RunAsLocalSystem();

                x.SetServiceName("UserInternetControl");
                x.SetDisplayName("Internet Regulator"); //change name to display service as a hidden -ware
                x.SetDescription("Used to authenticate Internet access to User");
            });

        }
    }
}
