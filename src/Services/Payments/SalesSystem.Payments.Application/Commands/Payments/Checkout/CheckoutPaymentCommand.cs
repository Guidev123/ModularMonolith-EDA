﻿using SalesSystem.Payments.Application.DTOs;
using SalesSystem.SharedKernel.Abstractions;

namespace SalesSystem.Payments.Application.Commands.Payments.Checkout
{
    public record CheckoutPaymentCommand : Command<CheckoutPaymentResponse>
    {
        public CheckoutPaymentCommand(
            Guid orderId, decimal value,
            string orderCode, List<ProductDto> products
            )
        {
            AggregateId = orderId;
            OrderId = orderId;
            Value = value;
            OrderCode = orderCode;
            Products = products;
        }

        public Guid OrderId { get; }
        public decimal Value { get; }
        public string CustomerEmail { get; private set; } = string.Empty;
        public string OrderCode { get; } = string.Empty;
        public Guid CustomerId { get; private set; }
        public List<ProductDto> Products { get; } = [];

        public void SetCustomerCredentials(Guid customerId, string customerEmail)
        {
            CustomerEmail = customerEmail;
            CustomerId = customerId;
        }
    }
}