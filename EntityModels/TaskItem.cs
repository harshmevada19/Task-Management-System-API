using System;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_System_API.EntityModels
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskStatus Status { get; set; }

        public string AssignedUserId { get; set; }
        public User AssignedUser { get; set; }
    }
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }
}
