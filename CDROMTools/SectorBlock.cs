namespace CDROMTools
{
    public enum SectorBlock:byte
    {
        UserDataBlock=0,
        FourthRunInBlock =1,
        ThirdRunInBlock=2,
        SecondRunInBlock =3,
        FirstRunInBlock=4,
        LinkBlock =5,
        SecondRunOutBlock=6,
        FirstRunOutBlock= 7,
    }
}