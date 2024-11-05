using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace Task_Management_System_API.EntityModels
{
    public class User : IdentityUser
    {
        public bool IsDeactivated { get; set; } = false;
        public ICollection<TaskItem> Tasks { get; set; }
    }
}
