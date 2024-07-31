namespace HangfireAPI.Models;

public class UserResponse
{
    public List<User> Users { get; set; } = new List<User>();

    public int TotalUsers { get; set; }

    public int Pages { get; set; }

    public int CurrentPage { get; set; }

    public int? PrevPage { get; set; }

    public int? NextPage { get; set; }
}
