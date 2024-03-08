using System.ComponentModel.DataAnnotations;

namespace Customers.Api.Domain;

public class Customer
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    public string FullName { get; init; } = default!;

    public string Email { get; init; } = default!;

    public DateOnly DateOfBirth { get; init; } = default!;
}
