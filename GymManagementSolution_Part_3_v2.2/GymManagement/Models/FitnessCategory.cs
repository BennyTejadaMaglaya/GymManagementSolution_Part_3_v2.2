using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models
{
    public class FitnessCategory : Auditable, IValidatableObject
    {
        public int ID { get; set; }

        [Display(Name = "Category")]
        public string Summary => (ExerciseCategories.Count()==0) ? Category
            : Category + " (" + ExerciseCategories.Count() + " Exercises)";

        [Required(ErrorMessage = "You cannot leave the category name blank.")]
        [StringLength(50, ErrorMessage = "Category name cannot be more than 50 characters long.")]
        public string Category { get; set; } = "";

        [Display(Name = "Group Classes")]
        public ICollection<GroupClass> GroupClasses { get; set; } = new HashSet<GroupClass>();

        [Display(Name = "Exercises")]
        public ICollection<ExerciseCategory> ExerciseCategories { get; set; } = new HashSet<ExerciseCategory>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Category.Contains("Macbeth"))//I know this is silly, but it is just for fun
            {
                yield return new ValidationResult("You should never enter the name of The Scotish Play!", new[] { "Category" });
            }
            if (Category.Length == 7)
            {
                yield return new ValidationResult("You cannot have a Category with length 7.", new[] { "Category" });
            }
        }
    }
}
