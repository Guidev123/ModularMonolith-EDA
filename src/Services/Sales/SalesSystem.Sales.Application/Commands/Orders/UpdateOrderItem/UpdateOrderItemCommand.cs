﻿using SalesSystem.SharedKernel.Abstractions;

namespace SalesSystem.Sales.Application.Commands.Orders.UpdateOrderItem
{
    public record UpdateOrderItemCommand : Command<UpdateOrderItemResponse>
    {
        public UpdateOrderItemCommand(Guid orderId, Guid productId, int quantity)
        {
            AggregateId = orderId;
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
        }

        public Guid CustomerId { get; private set; }
        public Guid OrderId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public void SetCustomerId(Guid customerId)
            => CustomerId = customerId;
    }
}