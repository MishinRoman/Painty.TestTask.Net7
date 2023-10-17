using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Painty.TestTask.Net7.Data.Models
{
    public class AppUser:IdentityUser
    {
        private List<MyImage> _images;

        [NotMapped]
        public IReadOnlyCollection<MyImage> Images
        {
            get
            {
                return _images;
            }
        }

        public AppUser()
        {
            _images = new List<MyImage>();
            
            Friends = new List<Friend>();

        }

        
        [Required]        
        public string Name { get; set; }

        [Required]        
        [EmailAddress]
        public string Email { get; set; }

        [Required]        
        [DataType(DataType.Password)]
        public string Password { get; set; }

       
        public string PhotoUrl { get; set; }

        [NotMapped]
        public ICollection<Friend> Friends { get; set; }

       
        public void AddImage(MyImage image)
        {
            _images.Add(image);
        }

       
        public void RemoveImage(MyImage image)
        {
            _images.Remove(image);
        }

        public void AddFrend(Friend friend)
        {
            Friends.Add(friend);
        }


        public void RemoveFriend(Friend friend)
        {
            Friends.Remove(friend);
        }



    }
}


