﻿using MediatR;
using SalesSystem.Catalog.Domain.Interfaces.Services;
using SalesSystem.SharedKernel.Messages.CommonMessages.IntegrationEvents.Orders;

namespace SalesSystem.Catalog.Domain.Events.CanceledOrder
{
    public sealed class OrderProcessingCanceledIntegrationEventHandler(IStockService stockService) : INotificationHandler<OrderProcessingCanceledIntegrationEvent>
    {
        private readonly IStockService _stockService = stockService;

        public async Task Handle(OrderProcessingCanceledIntegrationEvent notification, CancellationToken cancellationToken)
        {
            await _stockService.AddListStockAsync(notification.OrderProducts);
        }
    }
}