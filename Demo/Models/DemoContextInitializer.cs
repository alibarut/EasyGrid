using System;
using System.Data.Entity;
using System.Text;

namespace Demo.Models
{
    public class DemoContextInitializer : DropCreateDatabaseAlways<DemoContext>
    {
        private string RandomString(int size)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                System.Threading.Thread.Sleep(10);
                builder.Append(ch);
            }
            return builder.ToString();
        }

        private int RandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        protected override void Seed(DemoContext context)
        {
            for (int i = 0; i < 100; i++)
            {
                var item = new Contact
                {
                    FirstName = RandomString(7) + i,
                    LastName = RandomString(6) + i,
                    PhoneNumber = RandomNumber(1000000, 9999999).ToString("N0"),
                };
                context.Contacts.Add(item);
            }
            context.SaveChanges();
            base.Seed(context);
        }
    }
}