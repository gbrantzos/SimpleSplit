using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Features.Buildings
{
    public class Address : ValueObject
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        
        public override string ToString() => $"{Street} {Number}, {ZipCode}, {City}";
        
        protected Address() { }
        public Address(string street, string number, string city, string zipCode)
        {
            Street  = street  ?? throw new ArgumentNullException(nameof(street));
            Number  = number  ?? throw new ArgumentNullException(nameof(number));
            City    = city    ?? throw new ArgumentNullException(nameof(city));
            ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Street;
            yield return Number;
            yield return ZipCode;
            yield return City;
        }
    }
}