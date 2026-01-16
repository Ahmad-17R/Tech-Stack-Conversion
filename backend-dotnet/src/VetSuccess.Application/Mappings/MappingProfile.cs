using AutoMapper;
using VetSuccess.Application.DTOs.Client;
using VetSuccess.Application.DTOs.Outcome;
using VetSuccess.Application.DTOs.Practice;
using VetSuccess.Application.DTOs.SMSHistory;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Client mappings
        CreateMap<Client, ClientListDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => 
                src.Emails.FirstOrDefault(e => e.IsPreferred == true) != null 
                    ? src.Emails.First(e => e.IsPreferred == true).EmailAddress 
                    : src.Emails.FirstOrDefault() != null ? src.Emails.First().EmailAddress : null))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => 
                src.Phones.FirstOrDefault(p => p.IsPreferred == true) != null 
                    ? src.Phones.First(p => p.IsPreferred == true).PhoneNumber 
                    : src.Phones.FirstOrDefault() != null ? src.Phones.First().PhoneNumber : null))
            .ForMember(dest => dest.PracticeName, opt => opt.MapFrom(src => src.Practice != null ? src.Practice.PracticeName : null))
            .ForMember(dest => dest.PatientCount, opt => opt.MapFrom(src => src.ClientPatientRelationships.Count))
            .ForMember(dest => dest.LastContactedAt, opt => opt.Ignore())
            .ForMember(dest => dest.LastOutcome, opt => opt.Ignore());

        CreateMap<Client, ClientDetailDto>()
            .ForMember(dest => dest.PracticeName, opt => opt.MapFrom(src => src.Practice != null ? src.Practice.PracticeName : null))
            .ForMember(dest => dest.PracticeOduId, opt => opt.MapFrom(src => src.PracticeOduId));

        // Email mappings
        CreateMap<Email, EmailDto>();
        CreateMap<EmailUpdateDto, Email>()
            .ForMember(dest => dest.EmailOduId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ExtractorRemovedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DataSource, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore())
            .ForMember(dest => dest.ClientOduId, opt => opt.Ignore());

        // Phone mappings
        CreateMap<Phone, PhoneDto>();
        CreateMap<PhoneUpdateDto, Phone>()
            .ForMember(dest => dest.PhoneOduId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ExtractorRemovedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DataSource, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore())
            .ForMember(dest => dest.ClientOduId, opt => opt.Ignore());

        // Address mappings
        CreateMap<Address, AddressDto>();

        // Patient mappings
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.OutcomeName, opt => opt.MapFrom(src => src.Outcome != null ? src.Outcome.OutcomeName : null))
            .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.DeceasedDate, opt => opt.MapFrom(src => src.DeathDate));

        // Appointment mappings
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.PatientName : null));

        // Outcome mappings
        CreateMap<Outcome, OutcomeDto>();

        // Practice mappings
        CreateMap<Practice, PracticeDto>()
            .ForMember(dest => dest.ServerName, opt => opt.MapFrom(src => src.Server != null ? src.Server.ServerName : null))
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.PracticeSettings));

        CreateMap<PracticeSettings, PracticeSettingsDto>();

        // FAQ mappings
        CreateMap<Answer, FAQDto>()
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question != null ? src.Question.QuestionText : null))
            .ForMember(dest => dest.AnswerText, opt => opt.MapFrom(src => src.AnswerText))
            .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.Question != null ? src.Question.DisplayOrder : null));

        // SMS History mappings
        CreateMap<SMSHistory, ContactedClientDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.FullName : null))
            .ForMember(dest => dest.PracticeName, opt => opt.MapFrom(src => src.Practice != null ? src.Practice.PracticeName : null));
    }
}
