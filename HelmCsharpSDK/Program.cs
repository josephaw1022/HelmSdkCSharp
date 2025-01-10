using HelmClientLibrary;

class Program
{
    static void Main(string[] args)
    {

        var helmClient = new HelmClient();

        // Add repos

        // add cert-manager
        helmClient.EnsureRepoAdded("jetstack", "https://charts.jetstack.io");

        // add prometheus-community
        helmClient.EnsureRepoAdded("prometheus-community", "https://prometheus-community.github.io/helm-charts");


        // Update repos
        helmClient.EnsureReposUpdated();


        Console.WriteLine("Hello, World!");
    }
}
