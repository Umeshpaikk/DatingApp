using System.Security.Claims;

namespace API.Extentions
{
    public static class ClaimsPrincipleExtention
    {
        
        public static string getUserName(this ClaimsPrincipal claim)
        {
            return claim.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static int getUserId(this ClaimsPrincipal claim)
        {
            return int.Parse(claim.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}