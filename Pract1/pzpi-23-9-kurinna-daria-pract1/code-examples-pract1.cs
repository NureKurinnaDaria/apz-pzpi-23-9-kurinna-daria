using System;

public interface IPaymentStrategy
{
    void Pay(int amount);
}

public class CreditCardPayment : IPaymentStrategy
{
    public void Pay(int amount)
    {
        Console.WriteLine("Оплата кредитною карткою на суму: " + amount);
    }
}

public class PayPalPayment : IPaymentStrategy
{
    public void Pay(int amount)
    {
        Console.WriteLine("Оплата через систему PayPal на суму: " + amount);
    }
}

public class PaymentContext
{
    private IPaymentStrategy strategy;

    public void SetStrategy(IPaymentStrategy strategy)
    {
        this.strategy = strategy;
    }

    public void ExecutePayment(int amount)
    {
        strategy.Pay(amount);
    }
}

class Program
{
    static void Main()
    {
        PaymentContext context = new PaymentContext();

        context.SetStrategy(new CreditCardPayment());
        context.ExecutePayment(100);

        context.SetStrategy(new PayPalPayment());
        context.ExecutePayment(200);
    }
}
