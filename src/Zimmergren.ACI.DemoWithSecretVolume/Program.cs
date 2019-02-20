using System;
using System.IO;
using System.Threading;

namespace Zimmergren.ACI.DemoWithSecretVolume
{
    public class Program
    {
        // name: azure-resources-secret, path: /mounts/azure-resources-secrets
        private const string AzureSecretsMountPath = "/mounts/azure-resources-secrets";

        // name: app-secrets, path: /mounts/app-secrets
        private const string AppSecretsMountPath = "/mounts/app-secrets";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            Thread.Sleep(1000);
            while (true)
            {
               ReadSecretMountVolume(AzureSecretsMountPath);
               ReadSecretMountVolume(AppSecretsMountPath);
               Thread.Sleep(5000);
            }
        }
        
        private static void ReadSecretMountVolume(string mountPath)
        {
            Console.WriteLine($"Processing: {mountPath}");

           var secretFolders = Directory.GetDirectories(mountPath);
           foreach (var folder in secretFolders)
           {
               var allSecretFiles = Directory.GetFiles(folder);
               foreach (var f in allSecretFiles)
               {
                   Console.WriteLine($"Secret '{f}' has value '{File.ReadAllText(f)}'");
               }
            }
        }
    }
}
