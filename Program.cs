// See https://aka.ms/new-console-template for more information

using System;

Bank bancaIntesa = new Bank("Intesa San Paolo");

Console.WriteLine("Software Gestionale Bank {0}" + bancaIntesa.Nome);
Console.WriteLine();

bool running = true;

while (running)
{
    Console.WriteLine("Menu");
    Console.WriteLine("1. Aggiungi cliente");
    Console.WriteLine("2. Modifica cliente");
    Console.WriteLine("3. Ricerca cliente");
    Console.WriteLine("4. Ricerca prestito cliente");
    Console.WriteLine("5. Aggiungi un prestito");

    Console.WriteLine();
    Console.Write("Cosa vuoi fare?: ");
    int choice = Convert.ToInt32(Console.ReadLine());

    switch (choice)
    {
        case 1:

            Console.WriteLine("Inserisci il codice fiscale del cliente:");
            string codiceFiscale = Console.ReadLine();

           
            Client clienteEsistente = bancaIntesa.SearchClient(codiceFiscale);

            if (clienteEsistente != null)
            { 
                Console.WriteLine("Attenzione! Il cliente è già stato inserito!");

            }
            else
            {

                Client nuovoCLiente = new Client(codiceFiscale);
                bancaIntesa.AddClient(nuovoCLiente);
                Console.WriteLine("Il cliente è stato inserito correttamente");
            }
            

            break;
        case 2:

            Console.WriteLine("Inserisci il codice fiscale del cliente:");
            codiceFiscale = Console.ReadLine();

            clienteEsistente = bancaIntesa.SearchClient(codiceFiscale);

            if (clienteEsistente == null)
            {

                Console.WriteLine("Attenzione! non è stato trovato alcun cliente, cambia i criteri di ricerca");

            }
            else
            {
                Console.WriteLine("Inserisci il nome del cliente:");
                string nomeCliente = Console.ReadLine();
                Console.WriteLine("Inserisci il cognome del cliente:");
                string cognomeCliente = Console.ReadLine();

                clienteEsistente.Name = nomeCliente;
                clienteEsistente.Surname = cognomeCliente;

                Console.WriteLine("Il cliente è stato modificato correttamente");
            }



            break;
        case 3:

            Console.WriteLine("Inserisci il codice fiscale del cliente:");
            codiceFiscale = Console.ReadLine();

            clienteEsistente = bancaIntesa.SearchClient(codiceFiscale);

            if (clienteEsistente == null)
            {

                Console.WriteLine("Attenzione! non è stato trovato alcun cliente, cambia i criteri di ricerca");

            }
            else
            {
                Console.WriteLine("Client Trovato!");
                Console.WriteLine(clienteEsistente.ToString());
            }

            break;
        case 4:

            Console.WriteLine("Inserisci il codice fiscale del cliente:");
            codiceFiscale = Console.ReadLine();

            Client client = bancaIntesa.SearchClient(codiceFiscale);
            List<Loan> loans = bancaIntesa.SearchLoans(codiceFiscale);

            int totalAmount = 0;
           

            foreach(Loan loan in loans)
            {
                totalAmount += loan.Amount;
             
            }

            Console.WriteLine("Totale ammontare prestiti concessi: {0}", totalAmount);

            foreach (Loan loan in loans)
            {
                Console.WriteLine("Per il prestito {0}, rimangono {1} rate da pagare.", loan.ID, loan.PaymentLeft());

            }


            break;

        case 5:

            Console.WriteLine("Inserisci il codice fiscale del cliente:");
            codiceFiscale = Console.ReadLine();

            clienteEsistente = bancaIntesa.SearchClient(codiceFiscale);

            if (clienteEsistente == null)
            {

                Console.WriteLine("Attenzione! non è stato trovato alcun cliente, cambia i criteri di ricerca");

            }
            else
            {
                Console.WriteLine("Client trovato!");
                
                Console.WriteLine("Inserisci l'amount del prestito:");
                int ammontare = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Inserisci la payment del prestito:");
                int rata = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Inserisci la data di inizio prestito:");
                DateTime dataInizio = Convert.ToDateTime(Console.ReadLine());

                Loan newLoan = new Loan(clienteEsistente,ammontare,rata,dataInizio);

                bancaIntesa.AddLoan(newLoan);

            }



            break;
    }
}

public class Bank
{
    public string Nome { get; set;}

    List<Loan> Loans;

    List<Client> Clients;

    public Bank(string nome)
    {
        
        this.Nome = nome;
        Loans = new List<Loan>();
        Clients = new List<Client>();


        //fake db
        Client fc1 = new Client("ABCD");
        Loan fl1 = new Loan(fc1, 2000, 100, DateTime.Parse("01/01/2022"));

        Client fc2 = new Client("EFGH");
        Loan fl2 = new Loan(fc2, 4000, 100, DateTime.Parse("01/01/2021"));
        Loan fl3 = new Loan(fc2, 1000, 50, DateTime.Parse("01/05/2022"));

        Client fc3 = new Client("ILMN");
        Loan fl4 = new Loan(fc3, 1000, 30, DateTime.Parse("01/01/2021"));
        Loan fl5 = new Loan(fc3, 1000, 25, DateTime.Parse("01/05/2022"));
        Loan fl6 = new Loan(fc3, 1000, 70, DateTime.Parse("01/07/2022"));

        Clients.Add(fc1);
        Clients.Add(fc2);
        Clients.Add(fc3);

        Loans.Add(fl1);
        Loans.Add(fl2);
        Loans.Add(fl3);
        Loans.Add(fl4);
        Loans.Add(fl5);
        Loans.Add(fl6);

    }
 
    public void AddClient(Client cliente)
    {
        this.Clients.Add(cliente);
    }

    public Client SearchClient(string codiceFiscale)
    {
        foreach(Client cliente in Clients)
        {
            if(cliente.FiscalCode == codiceFiscale)
            {
                return cliente;
            }
        }

        return null;

    }

    public void AddLoan(Loan newLoan)
    {
        this.Loans.Add(newLoan);
    }

    public List<Loan> SearchLoans(string codiceFiscale)
    {
        List<Loan> loans = new List<Loan>();
        foreach(Loan loan in Loans)
        {
            if(loan.Holder.FiscalCode == codiceFiscale)
            {
                loans.Add(loan);
            }
        }

        return loans;

    }
}


public class Loan
{
    private static int LastID { get; set; } = 0;

    public int ID { get; set;}

    public Client Holder { get; set; }
    public int Amount { get; set;}
    public int Payment { get; set;}

    public DateTime Start;

    public DateTime End;

    public Loan( Client holder, int amount, int payment, DateTime start)
    {
        //gestione virtuale degli id
        LastID++;
        ID = LastID;
       
        Holder = holder;
        Amount = amount;
        Payment = payment;
        Start = start;

        End = ResolveLastDate();
    }

    private DateTime ResolveLastDate()
    {
        int numberOfRates = (int)(Amount / Payment);

        End = Start.AddMonths(numberOfRates);

        return End;
    }

    internal int PaymentLeft()
    {
       
        TimeSpan span = End.Subtract(DateTime.Today);

        return (int)(span.Days / (365.25 / 12));
    }
}

public class Client
{
    public string Name { get; set;}
     public string Surname { get; set;}
     public string FiscalCode { get; set;}
     public int Salary { get; set;}

    public Client(string codiceFiscale)
    {
        
        FiscalCode = codiceFiscale;
        Salary = 0;
        Name = null;
        Surname = null;
    }
    public Client(string nome, string cognome, string codiceFiscale, int stipendio)
    {
        Name = nome;
        Surname = cognome;
        FiscalCode = codiceFiscale;
        Salary = stipendio;
    }


    public override string ToString()
    {
       
        return $"{this.Name}\t{this.Surname}\t{this.FiscalCode}\t{this.Salary}";
    }

}