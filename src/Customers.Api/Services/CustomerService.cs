using Customers.Api.Database;
using Customers.Api.Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Customers.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _dbContext;

    public CustomerService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateAsync(Customer customer)
    {
        var existingUser = await _dbContext.Customers.FindAsync(customer.Id);
        if (existingUser is not null)
        {
            var message = $"A user with id {customer.Id} already exists";
            throw new ValidationException(message, GenerateValidationError(message));
        }

        await _dbContext.Customers.AddAsync(customer);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<Customer?> GetAsync(Guid id)
    {
        return await _dbContext.Customers.FindAsync(id);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _dbContext.Customers.ToListAsync();
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        _dbContext.Customers.Update(customer);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await GetAsync(id);
        if (customer is null)
        {
            return false;
        }

        _dbContext.Remove(customer);
        var changes = await _dbContext.SaveChangesAsync();
        return changes > 0;
    }

    private static ValidationFailure[] GenerateValidationError(string message)
    {
        return new []
        {
            new ValidationFailure(nameof(Customer), message)
        };
    }
}
