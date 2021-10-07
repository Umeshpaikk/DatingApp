using System;

namespace API.Extentions
{
    public static class DateTimeExtentions
    {
        public static int CalculateAge(this DateTime DoB)
        {
            var today = DateTime.Today;
            var age = today.Year - DoB.Year;
            if(today > DoB.AddYears(-age)) 
                --age;

            return age;
        }
    }
}