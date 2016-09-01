using System.ComponentModel.DataAnnotations;

namespace SecurityGuard.ViewModels
{
    public class ActionViewModel
    {
        [Required(ErrorMessage="Action Name is required.")]
        public string ActionName { get; set; }
        public string Description { get; set; }
    }
}
