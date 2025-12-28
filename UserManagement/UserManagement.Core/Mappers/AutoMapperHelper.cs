using AutoMapper;
using UserManagement.Core.DTO;
using UserManagement.Core.Entities;

namespace eCommerce.Core.Mappers;

public class AutoMapperHelper : Profile
{
  public AutoMapperHelper()
  {
    CreateMap<ApplicationUser, AuthenticationResponse>()
      .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID))
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
      .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.PersonName))
      .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
    .ForMember(dest => dest.Sucess, opt => opt.MapFrom(src => true)) 
      .ForMember(dest => dest.Token, opt => opt.Ignore()).ReverseMap();
      ;
  }
}
