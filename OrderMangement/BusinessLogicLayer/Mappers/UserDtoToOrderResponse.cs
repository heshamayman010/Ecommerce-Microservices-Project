using AutoMapper;
using OrderMangement.BusinessLogicLayer.DTO;
using OrderMangement.DataAccessLayer.Entities;

namespace OrderMangement.BusinessLogicLayer.Mappers;

public class UserDtoToOrderResponse : Profile
{
  public UserDtoToOrderResponse()
  {
    CreateMap<UserDTO, OrderResponse>()
      .ForMember(dest => dest.UserNAme, opt => opt.MapFrom(src => src.PersonName))
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
  }
}