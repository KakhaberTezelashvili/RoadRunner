namespace TDOC.Common.Utilities
{
    public static class FileDateUtilities
    {
        public static DateTime DosDateTimeToDateTime(int dosDateTime)
        {
            long date = (dosDateTime & 0xFFFF0000) >> 16;
            int time = dosDateTime & 0x0000FFFF;

            long year = (date >> 9) + 1980;
            long month = (date & 0x01e0) >> 5;
            long day = date & 0x1F;
            int hour = time >> 11;
            int minute = (time & 0x07e0) >> 5;
            int second = (time & 0x1F) * 2;

            return new DateTime((int)year, (int)month, (int)day, hour, minute, second);
        }
    }
}