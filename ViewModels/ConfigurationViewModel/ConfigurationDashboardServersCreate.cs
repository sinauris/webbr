using System.ComponentModel.DataAnnotations;

namespace Webbr.ViewModels.ConfigurationViewModel
{
    public class ConfigurationDashboardServersCreate
    {
        [Required] public string Name { get; set; }

        [Required] public string Ip { get; set; }

        [Required] public double CpuTreshold { get; set; }

        [Required] public double RamTreshold { get; set; }

        [Required] public string Comment { get; set; }

        [Required] public string Tag { get; set; }

        [Required] public int Enable { get; set; }
    }
}
