using System;
using System.Collections.Generic;

namespace MortgageCalculator
{
    class Program
    {
        private const double AnnualInterestRate = 0.05; // 5%
        private const double MonthlyInterestRate = AnnualInterestRate / 12;
        private const int MaxAdministrativeFee = 10000;
        private const double AdministrativeFeeRate = 0.01; // 1%
        private const double APRThreshold = 0.4;
        
        static void Main(string[] args)
        {
            var loanAmount = Int32.Parse(args[0]);
            var duration = Int32.Parse(args[1]) * 12;

            var monthlyCostCoefficient = GetMonthlyCostCoefficient(duration, MonthlyInterestRate);

            var monthlyCost = loanAmount * monthlyCostCoefficient;
            var totalInterestPaid = monthlyCost * duration - loanAmount;
            var administrativeFee = GetAdministrativeFee(loanAmount);
            var apr = GetAPR(loanAmount, duration, monthlyCost, administrativeFee);

            System.Console.WriteLine($"APR: {Math.Round(apr, 4)}");
            System.Console.WriteLine($"Monthly cost: {Math.Round(monthlyCost, 2)}");
            System.Console.WriteLine($"Total interest paid: {Math.Round(totalInterestPaid, 2)}");
            System.Console.WriteLine($"Administrative fee: {Math.Round(administrativeFee, 2)}");
        }

        static double GetAdministrativeFee(int loanAmount) 
        {
            var administrativeFee = loanAmount * AdministrativeFeeRate;

            return administrativeFee < MaxAdministrativeFee ? administrativeFee : MaxAdministrativeFee;
        }

        static double GetMonthlyCostCoefficient(int duration, double monthlyInterestRate) 
        {
            return monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, duration) / (Math.Pow(1 + monthlyInterestRate, duration) - 1);
        }

        static double GetAPR(int loanAmount, int duration, double monthlyCost, double administrativeFee)
        {
            var possibleInterests = new List<double>();

            for (var i = AnnualInterestRate; i < AnnualInterestRate * 2; i += 0.0001)
            {
                possibleInterests.Add(i);
            }

            return BinarySearchAPR(possibleInterests, loanAmount - administrativeFee, duration, monthlyCost);
        }

        static double BinarySearchAPR(List<double> input, double loanAmount, int duration, double monthlyCost)
        {
            if (input.Count == 1)
            {
                return input[0];
            }

            var center = input.Count / 2;
            var interest = input[center];

            var monthlyCostCoefficient = GetMonthlyCostCoefficient(duration, interest / 12);
            var effectiveMonthlyCost = loanAmount * monthlyCostCoefficient;

            if (Math.Abs(monthlyCost - effectiveMonthlyCost) < APRThreshold)
            {
                return interest;
            }
            else if (monthlyCost > effectiveMonthlyCost)
            {
                return BinarySearchAPR(input.GetRange(center, center), loanAmount, duration, monthlyCost);
            }
            else
            {
                return BinarySearchAPR(input.GetRange(0, center), loanAmount, duration, monthlyCost);
            }
        }
    }
}
