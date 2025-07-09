namespace UserManagement.Domain.utils;

public class RandomCodeGenerator
{
    private static readonly Random _random = new Random();

    public static string GenerateRandomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int length = _random.Next(5, 9);
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[_random.Next(chars.Length)])
            .ToArray());
    }

}
