namespace KassaSys;

public class Program
{
	static void Main()
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
				if (menuChoice == 0)
				{
					break;
				}
				else if (menuChoice == 1)
				{
					Register();
				}
				else if (menuChoice == 2)
				{
					Admin();
				}
			}
		}
	}

	static void Register()
	{
		Product product = new Product();
		CashRegister cashRegister = new CashRegister();

		while (true)
		{
			Console.Clear();
			Console.WriteLine("KASSA");

			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Red;

			Console.WriteLine($"KVITTO  #{cashRegister.FetchTotalReceipts():D4}  {DateTime.Now}");

			cashRegister.PrintReceipt();

			Console.WriteLine($"Total: {cashRegister.FetchTotal():F2}");

			Console.ResetColor();

			Console.WriteLine("kommando:\n<productid> <antal>\nPAY");
			Console.Write("Kommando:");
			var inputToCheck = Console.ReadLine();

			if (inputToCheck.ToLower() == "pay")
			{
				cashRegister.SaveReceipt();
				cashRegister = new CashRegister();
				return;
			}
			else if (inputToCheck.Contains(" "))
			{
				string[] args = inputToCheck.Split(' ');

				if (int.TryParse(args[0], out int productID) && int.TryParse(args[1], out int productAmount))
				{
					cashRegister.AddToReceipt(productID, productAmount);
				}
			}
		}
	}

	static void Admin()
	{
		Product product = new Product();
		ShopCampaine campaine = new ShopCampaine();

		while (true)
		{
			Console.Clear();
			Console.WriteLine("KASSA - admin - (0 - Gå Tillbaka)\n");
			Console.WriteLine(" Produkt:");
			Console.WriteLine("    1. Lägg till");
			Console.WriteLine("    2. Uppdatera");
			Console.WriteLine("    3. Ta bort\n");
			Console.WriteLine(" Kampanj:");
			Console.WriteLine("    4. Lätt till");
			Console.WriteLine("    5. Uppdatera");
			Console.WriteLine("    6. Ta bort\n");
			Console.Write(" Gör ditt val: ");
			bool checkInput = int.TryParse(Console.ReadLine(), out int menuChoice);

			if (checkInput)
			{
				if (menuChoice == 0)
				{
					break;
				}
				else if (menuChoice == 1)
				{
					product.AddProduct();
				}
				else if (menuChoice == 2)
				{
					product.UpdateProduct();
				}
				else if (menuChoice == 3)
				{
					product.RemoveProduct();
				}
				else if (menuChoice == 4)
				{
					campaine.AddCampaine();
				}
				else if (menuChoice == 5)
				{
					campaine.UpdateCampaine();
				}
				else if (menuChoice == 6)
				{
					campaine.RemoveCampaine();
				}
			}
		}
	}
}