using System.ComponentModel.DataAnnotations;

namespace IoT_SmartPlant_Portal.BindingModels {
    public class RegisterPostBindingModel {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}