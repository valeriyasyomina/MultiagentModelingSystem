using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PersonSpeed
    {
        public static double GetMaxSpeed(int age, Sex sex)
        {
            double result = 0;
            if (age >= 6 && age <= 12)
                result = sex == Sex.MALE ? 2.151 : 1.987;
            else if (age >= 13 && age <= 19)
                result = sex == Sex.MALE ? 2.213 : 2.134;
            else if (age >= 20 && age <= 29)
                result = sex == Sex.MALE ? 2.533 : 2.467;
            else if (age >= 30 && age <= 39)
                result = sex == Sex.MALE ? 2.456 : 2.342;
            else if (age >= 40 && age <= 49)
                result = sex == Sex.MALE ? 2.462 : 2.123;
            else if (age >= 50 && age <= 59)
                result = sex == Sex.MALE ? 2.069 : 2.010;
            else if (age >= 60 && age <= 69)
                result = sex == Sex.MALE ? 1.933 : 1.774;
            else if (age >= 70 && age <= 79)
                result = sex == Sex.MALE ? 2.079 : 1.749;
            return result;
        }
        public static double GetComfortableSpeed(int age, Sex sex)
        {
            double result = 0;
            if (age >= 6 && age <= 12)
                result = sex == Sex.MALE ? 1.183 : 1.133;
            else if (age >= 13 && age <= 19)
                result = 1.217;
            else if (age >= 20 && age <= 29)
                result = sex == Sex.MALE ? 1.393 : 1.407;
            else if (age >= 30 && age <= 39)
                result = sex == Sex.MALE ? 1.458 : 1.414;
            else if (age >= 40 && age <= 49)
                result = sex == Sex.MALE ? 1.462 : 1.391;
            else if (age >= 50 && age <= 59)
                result = sex == Sex.MALE ? 1.393 : 1.395;
            else if (age >= 60 && age <= 69)
                result = sex == Sex.MALE ? 1.359 : 1.296;
            else if (age >= 70 && age <= 79)
                result = sex == Sex.MALE ? 1.330 : 1.272;
            return result;
        }
    }
}
