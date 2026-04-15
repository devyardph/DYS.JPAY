using System.Globalization;

namespace DYS.JPay.Common.Extensions
{
    public static class NumberExtensions
    {
        public static int ToInt32(this string input)
        {
            if (int.TryParse(input, out int output))
                return output;
            else
                return 0;
        }

        public static long ToLong(this string input)
        {
            if (long.TryParse(input, out long output))
                return output;
            else
                return 0;
        }

        public static decimal ToDecimal(this string? input)
        {
            if (decimal.TryParse(input, out decimal output))
                return output;
            else
                return 0;
        }

        public static double ToDouble(this decimal? input)
        {
            if (input.HasValue)
                return (double)input.Value;
            return 0;
        }

        public static double GetVariance(double? actual, double? plan)
        {
            var planCost = plan.HasValue ? plan.Value : 0;
            var actualCost = actual ?? 0;
            var variance = planCost > 0 ?
                ((actualCost / plan) - 1) * 100 :
                0;

            return variance ?? 0;
        }

        public static double DivideByMillion(double? actual)
        {
            if (actual.HasValue)
                return actual.Value / 1000000;
            return 0;
        }

        public static string GetVarianceString(double? actual, double? plan)
        {
            var planCost = plan.HasValue ? plan.Value : 0;
            var actualCost = actual ?? 0;
            var variance = planCost > 0 ?
                ((actualCost / plan) - 1) * 100 :
                0;

            return variance.HasValue ? $"{variance.Value.ToString("N")}%" : "0%";
        }

        public static double? GetSum(params double?[] inputs)
        {
            double sum = 0;
            foreach (var input in inputs)
            {
                var item = input.HasValue ? input.Value : 0;
                sum += item;
            }
            return sum;
        }

        public static decimal? GetSum(params decimal?[] inputs)
        {
            decimal sum = 0;
            foreach (var input in inputs)
            {
                var item = input.HasValue ? input.Value : 0;
                sum += item;
            }
            return sum;
        }

        public static string GetSumString(params double?[] inputs)
        {
            double sum = 0;
            foreach (var input in inputs)
            {
                var item = input.HasValue ? input.Value : 0;
                sum += item;
            }
            return sum == 0 ? "" : sum.ToString("N2");
        }

        public static string GetSumString(bool currency = false, params double?[] inputs)
        {
            double sum = 0;
            foreach (var input in inputs)
            {
                var item = input.HasValue ? input.Value : 0;
                sum += item;
            }
            return sum == 0 ? "" : (currency ? sum.ToString("C2") : sum.ToString("N2"));
        }


        public static string GetSumString(List<double?> inputs)
        {
            double sum = 0;
            foreach (var input in inputs)
            {
                var item = input.HasValue ? input.Value : 0;
                sum += item;
            }
            return sum == 0 ? "" : sum.ToString("N2");
        }

        public static double GetDifference(double? first, double? second)
        {
            var firstValue = first.HasValue ? first.Value : 0;
            var secondValue = second.HasValue ? second.Value : 0;
            return firstValue - secondValue;
        }

        public static decimal GetDifference(decimal? first, decimal? second)
        {
            var firstValue = first.HasValue ? first.Value : 0;
            var secondValue = second.HasValue ? second.Value : 0;
            return firstValue - secondValue;
        }

        public static string SetPlaces(this double? input, int max = 2, bool currency = false)
        {
            if (!input.HasValue) return string.Empty;

            if (currency)
                return input.Value.ToString($"c{max}");
            else
                return input.Value.ToString($"n{max}");
        }

        public static bool HasNoValue(this double? input)
        {
            return !input.HasValue;
        }
        public static string ToCurrency(double? value, int point = 0)
        {
            return value.HasValue ? value.Value.ToString($"C{point}", new CultureInfo("en-PH")) : string.Empty;
        }

        public static string ToSymbol(this double? value, int point = 0)
        {
            return value.HasValue ?
                   value.Value >= 0 ? $"${value.Value.ToString($"N{point}")}" : $"-${Math.Abs(value.Value).ToString($"N{point}")}"
                 : $"$0.00";
        }

        public static string ToCurrency(this decimal? value, int point = 0)
        {
            return value.HasValue ? value.Value.ToString($"C{point}", new CultureInfo("en-PH")) : string.Empty;
        }

        public static string ToSymbol(this decimal? value, int point = 0)
        {
            return value.HasValue ? $"${value.Value.ToString($"N{point}")}" : string.Empty;
        }

        public static bool Greater(double? first, double? second)
        {
            return first > second;
        }

        public static string GetQuotientString(double? first, int? second, int? third)
        {
            double quotient = 0;
            if (!first.HasValue && !second.HasValue && !third.HasValue) return quotient.ToString("N");

            if (first.HasValue && second.HasValue && second.Value > 0)
                quotient = first.Value / second.Value;

            if (quotient > 0 && third.Value > 0)
                quotient = quotient / third.Value;

            return quotient.ToString("N2");

        }

        public static double GetQuotient(double? first, int? second, int? third)
        {
            double quotient = 0;
            if (!first.HasValue && !second.HasValue && !third.HasValue) return quotient;

            if (first.HasValue && second.HasValue && second.Value > 0)
                quotient = first.Value / second.Value;

            if (quotient > 0 && third.Value > 0)
                quotient = quotient / third.Value;

            return quotient;
        }

        public static decimal? GetQuotient(decimal? first, int? second, int? third)
        {
            decimal quotient = 0;
            if (!first.HasValue && !second.HasValue && !third.HasValue) return quotient;

            if (first.HasValue && second.HasValue && second.Value > 0)
                quotient = first.Value / second.Value;

            if (quotient > 0 && third.HasValue && third.Value > 0)
                quotient = quotient / third.Value;

            return quotient;
        }

        public static string GetQuotientString(double? first, double? second = 1000000, string suffix = "m")
        {
            double quotient = 0;
            if (!first.HasValue) return $"{quotient:C2}{suffix}";

            if (first.HasValue && second.HasValue)
                quotient = first.Value / second.Value;

            return $"{quotient:C2}{suffix}";
        }

        public static string GetProductString(double? first, double? second, bool currency = false)
        {
            if (first.HasValue && second.HasValue)
            {
                var result = first.Value * second.Value;
                return currency ? result.ToString("C2") : result.ToString("N2");
            }
            else
                return "";
        }

        public static double GetProduct(double? first, double? second)
        {
            if (first.HasValue && second.HasValue)
            {
                var result = first.Value * second.Value;
                return result;
            }
            else
                return 0;
        }

        public static decimal? GetAverage(IEnumerable<decimal?> first, int point = 1)
        {
            if (first.HasAny())
            {
                var result = first.Sum(query => query) / first.Count();
                return Math.Round(result ?? 0, point);
            }
            else
                return 0;
        }

        public static bool ValidateNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
                number = "0";

            if (Double.TryParse(number, out double dateValue))
                return true;
            else return false;
        }

        public static double Round(double input, int? point = 2)
        {
            return Math.Round(input, point.Value);
        }

        public static bool IsWholeNumber(double? input)
        {
            return input.HasValue ? input.Value % 1 == 0 : false;
        }


        public static string? IsWholeNumber(this double? input, int point)
        {
            if (!input.HasValue) return string.Empty;
            var isWholeNumber = input.HasValue ? input.Value % 1 == 0 : false;
            return isWholeNumber ? input.ToString() : input?.ToString($"N{point}");
        }

        public static string? IsWholeNumber(this decimal? input, int point)
        {
            if (!input.HasValue) return string.Empty;
            var isWholeNumber = input.HasValue ? input.Value % 1 == 0 : false;
            return isWholeNumber ? input.ToString() : input?.ToString($"N{point}");
        }


        public static string GetInThousand(double? input, int point = 2)
        {
            if (!input.HasValue) return string.Empty;
            var number = input.Value / 1000;
            return number.ToString($"N{point}");
        }

        public static string GetForecastString(this double? input)
        {
            var value = input < 1000 ? $"${input.Value.ToString("N0")}k" : $"${NumberExtensions.IsWholeNumber(NumberExtensions.GetQuotient(input.Value, 1000, 0), 2)}m";
            return value;
        }

        public static string GetForecastString(this decimal? input)
        {
            var value = input < 1000 ? $"${input.Value.ToString("N0")}k" : $"${NumberExtensions.IsWholeNumber(NumberExtensions.GetQuotient(input.Value, 1000, 0), 2)}m";
            return value;
        }

        public static decimal Convert(double? actual)
        {
            if (actual.HasValue)
                return (decimal)actual.Value;
            return 0;
        }

        public static float Convert(decimal? actual)
        {
            if (actual.HasValue)
                return (float)actual.Value;
            return 0;
        }

        public static decimal ToKilometers(this decimal? input)
        {
            if (input.HasValue) return input.Value > 0 ? input.Value / 1000 : 0;
            return 0;
        }

        public static decimal? ToPercentage(this decimal? input, decimal? percent)
        {
            if (input == null) return 0;
            var percentage = percent / 100;
            var total = input.Value * percentage;
            return total;
        }
    }
}
