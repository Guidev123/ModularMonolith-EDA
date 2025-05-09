﻿using SalesSystem.Registers.Domain.Entities;
using SalesSystem.SharedKernel.Data;

namespace SalesSystem.Registers.Domain.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByIdAsync(Guid id);

        Task<Customer?> GetCustomerAddressByIdAsync(Guid id);

        Task<Address?> GetAddressByCustomerIdAsync(Guid customerId);

        Task<Customer?> GetByEmailAsync(string email);

        Task<bool> AlreadyExists(string document);

        void Create(Customer customer);

        void Update(Customer customer);

        void CreateAddress(Address address);
    }
}