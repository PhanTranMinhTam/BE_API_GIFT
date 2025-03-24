using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Models;

namespace WebAPI_PhanTranMinhTam_New.Mappings
{
    public class MappingGift : AutoMapper.Profile
    {
        public MappingGift()
        {
            CreateMap<CreateDTO, User>();
            CreateMap<CreateGiftDTO, Gift>();
            CreateMap<Gift, CreateGiftDTO>();
        }
    }
}
