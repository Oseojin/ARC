
using System.Linq;
using UnityEngine;

public static class RoomCodeGenerator
{
    public static string GenerateCode(int length = 6)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789";
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[Random.Range(0, chars.Length)]).ToArray());
    }
}