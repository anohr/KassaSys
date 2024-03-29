﻿using KassaSys.Register;

namespace KassaSys;

public class Program
{
    private static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("KASSA");
            Console.WriteLine("1. Ny kund");
            Console.WriteLine("2. Admin");
            Console.WriteLine("0. Avsluta");
            bool checkInput = int.TryParse(Console.ReadLine(), out int menuChoice);

            if (checkInput)
            {
                switch (menuChoice)
                {
                    case 0:
                        return;

                    case 1:
                        Register();
                        break;

                    case 2:
                        Admin();
                        break;
                }
            }
        }
    }

    public static void Register()
    {
        CashRegister CashRegister = new CashRegister();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("KASSA");

            CashRegister.PrintReceipt();

            Console.WriteLine("kommando:\n<productid> <antal>\nPAY");
            Console.Write("Kommando:");
            string inputToCheck = Console.ReadLine();

            if (string.Compare(inputToCheck.ToLower(), "pay", StringComparison.Ordinal) == 0)
            {
                CashRegister.SaveReceipt();
                CashRegister = new CashRegister();
                return;
            }
            else if (inputToCheck.Contains(' '))
            {
                string[] args = inputToCheck.Split(' ');

                if (int.TryParse(args[0], out int productID) && int.TryParse(args[1], out int productAmount))
                {
                    CashRegister.AddToReceipt(productID, productAmount);
                }
            }
        }
    }

    private static void Admin()
    {
        Product.Product Product = new Product.Product();
        Campaign.Campaign Campaign = new Campaign.Campaign();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
            Console.WriteLine(" Produkt:");
            Console.WriteLine("    1. Lägg till");
            Console.WriteLine("    2. Uppdatera");
            Console.WriteLine("    3. Ta bort");
            Console.WriteLine("\n Kampanj:");
            Console.WriteLine("    4. Lägg till");
            Console.WriteLine("    5. Uppdatera");
            Console.WriteLine("    6. Ta bort\n");
            Console.Write(" Gör ditt val: ");
            bool checkInput = int.TryParse(Console.ReadLine(), out int menuChoice);

            if (checkInput)
            {
                switch (menuChoice)
                {
                    case 0:
                        return;

                    case 1:
                        Product.AddProduct();
                        break;

                    case 2:
                        Product.UpdateProduct();
                        break;

                    case 3:
                        Product.RemoveProduct();
                        break;

                    case 4:
                        Campaign.AddCampaign();
                        break;

                    case 5:
                        Campaign.UpdateCampaign();
                        break;

                    case 6:
                        Campaign.RemoveCampaign();
                        break;
                }
            }
        }
    }

    public static string AskForInput(string str)
    {
        Console.Write(str);

        return Console.ReadLine().Trim();
    }

    public static void ErrorPrint(string str)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(str);
        Console.ResetColor();
    }
}