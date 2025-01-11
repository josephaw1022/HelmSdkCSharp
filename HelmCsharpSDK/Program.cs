using HelmClientLibrary;

class Program
{
    static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("KUBECONFIG", "/root/.kube/config");

        var helmClient = new HelmClient();

        // Add repos
        helmClient.EnsureRepoAdded("jetstack", "https://charts.jetstack.io");
        helmClient.EnsureRepoAdded("prometheus-community", "https://prometheus-community.github.io/helm-charts");

        // Update repos
        helmClient.EnsureReposUpdated();

        // Install cert-manager
        helmClient.UpgradeOrInstallRelease("cert-manager5", "jetstack/cert-manager", "cert-manager", "--set crds.enabled=true --version v1.16.2  --insecure-skip-tls-verify");

        Console.WriteLine("Hello, World!");
    }
}
