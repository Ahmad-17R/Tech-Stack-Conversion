using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VetSuccess.Application.DTOs.Client;
using VetSuccess.Application.Interfaces;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Shared.Exceptions;

namespace VetSuccess.Application.Services;

public class ClientService : IClientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClientService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ClientListDto>> SearchClientsAsync(string? search, string? practiceOduId, CancellationToken cancellationToken = default)
    {
        var clients = await _unitOfWork.Clients.SearchClientsAsync(search, practiceOduId, cancellationToken);
        return _mapper.Map<List<ClientListDto>>(clients);
    }

    public async Task<ClientDetailDto> GetClientDetailsAsync(string clientOduId, CancellationToken cancellationToken = default)
    {
        var client = await _unitOfWork.Clients.GetClientWithDetailsAsync(clientOduId, cancellationToken);
        
        if (client == null)
        {
            throw new NotFoundException("Client", clientOduId);
        }

        return _mapper.Map<ClientDetailDto>(client);
    }

    public async Task<ClientDetailDto> UpdateClientAsync(string clientOduId, ClientUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var client = await _unitOfWork.Clients.GetClientWithDetailsAsync(clientOduId, cancellationToken);
            
            if (client == null)
            {
                throw new NotFoundException("Client", clientOduId);
            }

            // Update basic properties
            if (!string.IsNullOrWhiteSpace(updateDto.FullName))
            {
                client.FullName = updateDto.FullName;
                client.UpperFullName = updateDto.FullName.ToUpper();
            }

            // Update emails
            if (updateDto.Emails != null)
            {
                foreach (var emailDto in updateDto.Emails)
                {
                    if (emailDto.Uuid.HasValue)
                    {
                        // Update existing email
                        var email = client.Emails.FirstOrDefault(e => e.Uuid == emailDto.Uuid.Value);
                        if (email != null)
                        {
                            if (!string.IsNullOrWhiteSpace(emailDto.EmailAddress))
                                email.EmailAddress = emailDto.EmailAddress;
                            if (emailDto.IsPreferred.HasValue)
                                email.IsPreferred = emailDto.IsPreferred.Value;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(emailDto.EmailAddress))
                    {
                        // Add new email
                        var newEmail = new Email
                        {
                            EmailOduId = Guid.NewGuid().ToString(),
                            Uuid = Guid.NewGuid(),
                            EmailAddress = emailDto.EmailAddress,
                            IsPreferred = emailDto.IsPreferred ?? false,
                            ClientOduId = client.ClientOduId,
                            DataSource = "manual"
                        };
                        client.Emails.Add(newEmail);
                    }
                }
            }

            // Update phones
            if (updateDto.Phones != null)
            {
                foreach (var phoneDto in updateDto.Phones)
                {
                    if (phoneDto.Uuid.HasValue)
                    {
                        // Update existing phone
                        var phone = client.Phones.FirstOrDefault(p => p.Uuid == phoneDto.Uuid.Value);
                        if (phone != null)
                        {
                            if (!string.IsNullOrWhiteSpace(phoneDto.PhoneNumber))
                                phone.PhoneNumber = phoneDto.PhoneNumber;
                            if (!string.IsNullOrWhiteSpace(phoneDto.PhoneType))
                                phone.PhoneType = phoneDto.PhoneType;
                            if (phoneDto.IsPreferred.HasValue)
                                phone.IsPreferred = phoneDto.IsPreferred.Value;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(phoneDto.PhoneNumber))
                    {
                        // Add new phone
                        var newPhone = new Phone
                        {
                            PhoneOduId = Guid.NewGuid().ToString(),
                            Uuid = Guid.NewGuid(),
                            PhoneNumber = phoneDto.PhoneNumber,
                            PhoneType = phoneDto.PhoneType ?? "mobile",
                            IsPreferred = phoneDto.IsPreferred ?? false,
                            ClientOduId = client.ClientOduId,
                            DataSource = "manual"
                        };
                        client.Phones.Add(newPhone);
                    }
                }
            }

            // Update patient outcomes
            if (updateDto.Patients != null)
            {
                foreach (var patientDto in updateDto.Patients)
                {
                    var relationship = client.ClientPatientRelationships
                        .FirstOrDefault(r => r.Patient != null && r.Patient.Uuid == patientDto.Uuid);
                    
                    if (relationship?.Patient != null && !string.IsNullOrWhiteSpace(patientDto.OutcomeOduId))
                    {
                        var previousOutcome = relationship.Patient.OutcomeOduId;
                        relationship.Patient.OutcomeOduId = patientDto.OutcomeOduId;
                        
                        // Update outcome timestamp if outcome changed
                        if (previousOutcome != patientDto.OutcomeOduId)
                        {
                            relationship.Patient.OutcomeAt = DateTime.UtcNow;
                        }
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Reload client with updated data
            var updatedClient = await _unitOfWork.Clients.GetClientWithDetailsAsync(clientOduId, cancellationToken);
            return _mapper.Map<ClientDetailDto>(updatedClient!);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
