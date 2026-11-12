using AutoMapper;
using CustomerManagement.Core.DTOs;
using CustomerManagement.Core.Models;

namespace CustomerManagement.Core.Mapping
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            // DTO to Customer
            CreateMap<CustomerCreateDto, Customer>();

            CreateMap<CustomerUpdateDto, Customer>();

            // Customer to Response DTO
            CreateMap<Customer, CustomerResponseDto>();
        }
    }
}