using AutoMapper;
using property_price_purchase_service.Models;

namespace property_price_purchase_service.Profiles;

public class OrderProfile: Profile
{
    public OrderProfile()
    {
        CreateMap<OrderRequest, Order>();
    }
}