namespace HangfireAPI;

public sealed class Timeservice
{
    public void DisplayTime()
    {
        Console.WriteLine($"The current time is: {DateTime.Now.ToString()}");
    }
}