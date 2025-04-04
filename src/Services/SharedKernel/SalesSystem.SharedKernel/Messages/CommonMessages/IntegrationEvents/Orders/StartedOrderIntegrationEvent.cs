﻿using SalesSystem.SharedKernel.Communication.DTOs;

namespace SalesSystem.SharedKernel.Messages.CommonMessages.IntegrationEvents.Orders
{
    public record StartedOrderIntegrationEvent : IntegrationEvent
    {
        public StartedOrderIntegrationEvent(Guid orderId, Guid customerId, decimal total, OrderProductsListDTO orderProductsList)
        {
            AggregateId = orderId;
            OrderId = orderId;
            CustomerId = customerId;
            Total = total;
            OrderProductsList = orderProductsList;
        }

        public Guid OrderId { get; }
        public Guid CustomerId { get; }
        public decimal Total { get; }
        public OrderProductsListDTO OrderProductsList { get; }
    }
}