using AutoMapper;
using MyFirstApi.Models;
using MyFirstApi.DTOs;

namespace MyFirstApi.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Map Product → ProductReadDto
            CreateMap<Product, ProductReadDto>();

            // Map ProductCreateDto → Product
            CreateMap<ProductCreateDto, Product>();
        }
    }
}