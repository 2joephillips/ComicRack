using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicRack.Core.Models
{
    [Table("Settings")]
    public class Setting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Key { get; set; } // Stores the string representation of ApplicationSettingKey

        [Required]
        public string Value { get; set; }


        public bool IsValidKey()
        {
            return Enum.TryParse(typeof(ApplicationSettingKey), Key, true, out _);
        }

        public ApplicationSettingKey? GetKeyEnum()
        {
            if (Enum.TryParse(typeof(ApplicationSettingKey), Key, true, out var result))
            {
                return (ApplicationSettingKey)result;
            }
            return null;
        }
    }
}
