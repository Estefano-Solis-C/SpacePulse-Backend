using System;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Application.ACL;

/// <summary>
/// Anti-Corruption Layer para el Bounded Context de Monitoring.
/// Define el contrato de acceso para otros BCs.
/// </summary>
public interface IMonitoringContextFacade
{
    /// <summary>
    /// Registra un dispositivo IoT en un espacio específico.
    /// </summary>
    Task<int> RegisterIoTDeviceAsync(
        long spaceId,
        string name,
        string serialNumber,
        string type);


    /// <summary>
    /// Crea una tarea de trabajo vinculada a un espacio.
    /// </summary>
    Task<int> CreateWorkItemForSpaceAsync(
        long spaceId,
        Guid createdByUserId,
        string title,
        string description,
        string? photoUrl = null,
        DateTime? plannedStartDate = null,
        DateTime? plannedEndDate = null);

    /// <summary>
    /// Apaga automáticamente todos los dispositivos IoT vinculados a un espacio.
    /// Se utiliza cuando un proyecto se cancela para optimizar recursos.
    /// </summary>
    Task DisableAllDevicesForSpaceAsync(long spaceId);

    /// <summary>
    /// Despacha una notificación a un usuario específico vinculada a un espacio.
    /// Se utiliza para comunicar eventos importantes (completación, cancelación, etc.) de forma reactiva.
    /// </summary>
    Task DispatchNotificationAsync(Guid userId, long spaceId, string title, string message);
}

