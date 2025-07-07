namespace ClearingAndForwarding.Helpers
{
    public static class NumberToWordsConverter
    {
        private static readonly string[] Units =
            { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
          "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
          "Eighteen", "Nineteen" };

        private static readonly string[] Tens =
            { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public static string ConvertAmountToWords(decimal amount)
        {
            if (amount == 0)
                return "Zero Taka Only";

            long taka = (long)amount;
            int poisha = (int)((amount - taka) * 100);

            string words = $"{ConvertNumberToWords(taka)} Taka";

            if (poisha > 0)
                words += $" and {ConvertNumberToWords(poisha)} Poisha";

            return words + " Only";
        }

        private static string ConvertNumberToWords(long number)
        {
            if (number < 20)
                return Units[number];
            if (number < 100)
                return Tens[number / 10] + (number % 10 > 0 ? " " + Units[number % 10] : "");
            if (number < 1000)
                return Units[number / 100] + " Hundred" + (number % 100 > 0 ? " " + ConvertNumberToWords(number % 100) : "");
            if (number < 100000)
                return ConvertNumberToWords(number / 1000) + " Thousand" + (number % 1000 > 0 ? " " + ConvertNumberToWords(number % 1000) : "");
            if (number < 10000000)
                return ConvertNumberToWords(number / 100000) + " Lakh" + (number % 100000 > 0 ? " " + ConvertNumberToWords(number % 100000) : "");

            return ConvertNumberToWords(number / 10000000) + " Crore" + (number % 10000000 > 0 ? " " + ConvertNumberToWords(number % 10000000) : "");
        }
    }

}
