namespace PatientManagement.Domain.ValueObjects;

public class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string? Country { get; private set; }
    
    public Address(string street, string city, string state, string postalCode, string? country = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be empty", nameof(state));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));
        
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country ?? "India";
    }
    
    public string FullAddress => $"{Street}, {City}, {State} {PostalCode}, {Country}";
}
