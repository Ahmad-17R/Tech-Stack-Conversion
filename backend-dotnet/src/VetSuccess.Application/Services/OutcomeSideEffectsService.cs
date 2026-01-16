using Microsoft.Extensions.Logging;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;

namespace VetSuccess.Application.Services;

public class OutcomeSideEffectsService : IOutcomeSideEffectsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OutcomeSideEffectsService> _logger;

    public OutcomeSideEffectsService(
        IUnitOfWork unitOfWork,
        ILogger<OutcomeSideEffectsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ProcessOutcomeChangesAsync(IEnumerable<Patient> patients, CancellationToken cancellationToken = default)
    {
        foreach (var patient in patients)
        {
            _logger.LogInformation("Processing outcome change for patient {PatientOduId}", patient.PatientOduId);
            
            // Update outcome timestamp (already handled in ClientService)
            // This service handles SMS follow-up logic
            
            // In Django, this triggers SMS follow-up by:
            // 1. Checking if the new outcome requires SMS follow-up
            // 2. Creating/updating SMS history records
            // 3. Potentially queuing SMS sending jobs
            
            // For now, we log the outcome change
            // The SMS aggregation job will pick up patients with outcome changes
            // during its regular run based on reminder due dates
            
            _logger.LogInformation(
                "Patient {PatientOduId} outcome changed to {OutcomeOduId} at {OutcomeAt}. " +
                "SMS follow-up will be handled by SMS aggregation job if reminders are due.",
                patient.PatientOduId,
                patient.OutcomeOduId,
                patient.OutcomeAt);
        }

        await Task.CompletedTask;
    }
}

