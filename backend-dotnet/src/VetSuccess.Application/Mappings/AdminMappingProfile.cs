using AutoMapper;
using VetSuccess.Application.DTOs.Admin;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Application.Mappings;

public class AdminMappingProfile : Profile
{
    public AdminMappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.IsSuperuser, opt => opt.Ignore()) // Will be set from roles
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.EmailConfirmed))
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LockoutEnd))
            .ForMember(dest => dest.FirstName, opt => opt.Ignore())
            .ForMember(dest => dest.LastName, opt => opt.Ignore());

        CreateMap<User, UserListDto>()
            .ForMember(dest => dest.IsSuperuser, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.EmailConfirmed))
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LockoutEnd))
            .ForMember(dest => dest.FirstName, opt => opt.Ignore())
            .ForMember(dest => dest.LastName, opt => opt.Ignore());

        // Outcome mappings
        CreateMap<Outcome, OutcomeAdminDto>();

        // Question mappings
        CreateMap<Question, QuestionDto>();

        // Answer mappings
        CreateMap<Answer, AnswerDto>()
            .ForMember(dest => dest.PracticeName, opt => opt.MapFrom(src => src.Practice != null ? src.Practice.PracticeName : null))
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question.QuestionText));

        // Practice mappings
        CreateMap<Practice, PracticeAdminListDto>()
            .ForMember(dest => dest.ServerName, opt => opt.MapFrom(src => src.Server != null ? src.Server.ServerName : null))
            .ForMember(dest => dest.HasSettings, opt => opt.MapFrom(src => src.PracticeSettings != null));

        CreateMap<Practice, PracticeAdminDetailDto>()
            .ForMember(dest => dest.ServerName, opt => opt.MapFrom(src => src.Server != null ? src.Server.ServerName : null))
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.PracticeSettings))
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

        // Practice Settings mappings
        CreateMap<PracticeSettings, PracticeSettingsAdminDto>();

        // SMS Template mappings
        CreateMap<SMSTemplate, SMSTemplateDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));
    }
}
