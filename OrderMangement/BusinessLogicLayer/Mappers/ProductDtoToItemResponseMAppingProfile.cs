using AutoMapper;
using OrderMangement.BusinessLogicLayer.DTO;
using OrderMangement.DataAccessLayer.Entities;

namespace OrderMangement.BusinessLogicLayer.Mappers;

public class ProductDtoToItemResponseMAppingProfile : Profile
{
  public ProductDtoToItemResponseMAppingProfile()
  {
    CreateMap<ProductDto, OrderItemResponse>()
      .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
      .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
  }
}