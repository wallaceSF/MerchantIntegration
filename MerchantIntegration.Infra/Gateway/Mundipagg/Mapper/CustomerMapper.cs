using MerchantIntegration.Core.Entity;
using CustomerModelInfra = MerchantIntegration.Infra.Gateway.Mundipagg.Model.Customer;

namespace MerchantIntegration.Infra.Gateway.Mundipagg.Mapper
{
    public class CustomerMapper : AutoMapper.Profile
    {
        public CustomerMapper()
        {
            CreateMap<CustomerModelInfra, Customer>()
                .ForMember(
                    dest => dest.GatewayCustomerId,
                    origin => origin.MapFrom(customer => customer.Id)
                )
                .ForMember(
                    dest => dest.DocumentUser,
                    origin => origin.MapFrom(customer => customer.document)
                )
                .ForMember(
                    dest => dest.Id,
                    origin => origin.Ignore()
                );

            CreateMap<Customer, CustomerModelInfra>()
                .ForMember(
                    dest => dest.type,
                    origin => origin.MapFrom(_ => "individual")
                )
                .ForMember(
                    dest => dest.document,
                    origin => origin.MapFrom(customer => customer.DocumentUser)
                );
        }
    }
}