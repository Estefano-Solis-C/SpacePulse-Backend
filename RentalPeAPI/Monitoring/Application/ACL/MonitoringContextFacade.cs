using System;
using System.Threading.Tasks;
using MediatR;

namespace RentalPeAPI.Monitoring.Application.ACL;

using CreateIoTDeviceCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.CreateIoTDeviceCommand;
using CreateWorkItemCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.CreateWorkItemCommand;
using TurnOffDevicesBySpaceIdCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.TurnOffDevicesBySpaceIdCommand;
using CreateNotificationCommand =
    RentalPeAPI.Monitoring.Application.Internal.CommandServices.CreateNotificationCommand;

/// <summary>
/// Facade Anti-Corruption Layer para Monitoring BC.
/// Proporciona métodos de acceso para otros BCs sin exponer detalles internos.
/// Alineado con SpaceId como llave foránea unificada.
/// </summary>
public class MonitoringContextFacade : IMonitoringContextFacade
{
    private readonly IMediator _mediator;

    public MonitoringContextFacade(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra un nuevo IoTDevice en un espacio (Space).
    /// Nota: UseCase previo para ACL, requiere system user ID.
    /// </summary>
    public async Task<int> RegisterIoTDeviceAsync(
        long spaceId,
        string name,
        string serialNumber,
        string type)
    {
        var systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        var command = new CreateIoTDeviceCommand(
            SpaceId: spaceId,
            CreatedByUserId: systemUserId,
            Type: type,
            Name: name,
            SerialNumber: serialNumber,
            CustomMetricName: null,
            CustomUnit: null,
            CustomMinThreshold: null,
            CustomMaxThreshold: null
        );
        var device = await _mediator.Send(command);
        return (int)device.Id;
    }

     /// <summary>
     /// Crea una WorkItem asociada a un espacio (Space).
     /// </summary>
     public async Task<int> CreateWorkItemForSpaceAsync(
         long spaceId,
         Guid createdByUserId,
         string title,
         string description,
         string? photoUrl = null,
         DateTime? plannedStartDate = null,
         DateTime? plannedEndDate = null)
     {
         var command = new CreateWorkItemCommand(
             SpaceId: spaceId,
             CreatedByUserId: createdByUserId,
             Title: title,
             Description: description,
             PhotoUrl: photoUrl,
             PlannedStartDate: plannedStartDate,
             PlannedEndDate: plannedEndDate
         );

         var workItemId = await _mediator.Send(command);
         return workItemId;
     }

      /// <summary>
      /// Apaga automáticamente todos los dispositivos IoT vinculados a un espacio.
      /// Se utiliza cuando un proyecto se cancela para optimizar recursos.
      /// </summary>
      public async Task DisableAllDevicesForSpaceAsync(long spaceId)
      {
          var command = new TurnOffDevicesBySpaceIdCommand(spaceId);
          await _mediator.Send(command);
      }

      /// <summary>
      /// Despacha una notificación a un usuario específico vinculada a un espacio.
      /// Se utiliza para comunicar eventos importantes (completación, cancelación, etc.) de forma reactiva.
      /// </summary>
      public async Task DispatchNotificationAsync(Guid userId, long spaceId, string title, string message)
      {
          var command = new CreateNotificationCommand(userId, spaceId, title, message);
          await _mediator.Send(command);
      }
}