namespace BarberAndCustomers
{
    internal class Program
    {
        const int _numberOfFreeSeats = 3;
        static Semaphore _customerWait = new Semaphore(_numberOfFreeSeats, _numberOfFreeSeats);
        static Semaphore _barberSleep = new Semaphore(0, 1);
        //static bool _barberSleep = false;

        private static Queue<Customer> _customers = new Queue<Customer>();

        static void Main(string[] args)
        {
            var barber = new Barber();
            Thread.Sleep(4000);
            for (int i = 1; i <= 10; i++)
            {
                var customer = new Customer(i);
                Thread.Sleep(2000);
            }
        }

        class Barber
        {
            Thread _barberThread;

            public Barber()
            {
                _barberThread = new Thread(Work);
                _barberThread.Name = "Barber";
                _barberThread.Start();
            }

            public void Work()
            {
                while (true)
                {
                    if (_customers.Count > 0)
                    {
                        Customer customer = _customers.Dequeue();
                        _customerWait.Release();

                        Console.WriteLine("Barber is cutting hair of customer " + customer.Id);
                        Thread.Sleep(9000);
                        Console.WriteLine("Barber has finished cutting hair of customer " + customer.Id);
                    }
                    else
                    {
                        
                        Console.WriteLine("Barber is sleeping");
                        _barberSleep.WaitOne();
                        Console.WriteLine("Barber is woken up");
                        //_barberSleep = true;
                        Thread.Sleep(4000);
                    }
                }
            }

            //public void Join() { _barberThread.Join(); }
        }

        class Customer
        {
            public int Id { get; set; }

            Thread _customer;

            public Customer(int id)
            {
                Id = id;
                _customer = new Thread(EnterBarbershop);
                _customer.Name = $"Customer {id}";
                _customer.Start();
            }

            public void EnterBarbershop()
            {
                Console.WriteLine($"{Thread.CurrentThread.Name} enters the barbershop.");
                if (_customers.Count < _numberOfFreeSeats)
                {
                    _customerWait.WaitOne();
                    _customers.Enqueue(this);
                    if (!_barberSleep.WaitOne(0))
                    {
                        _barberSleep.Release();
                    }
                }
                else
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} leaves because there's no available waiting chair.");
                }
            }
        }
    }
}
