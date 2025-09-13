namespace Feed_Bridge.Models.Enums
{
    public enum OrderStatus
    {
        Pending,    // في انتظار موافقة الأدمن
        Approved,   // تمت الموافقة من الأدمن
        Rejected,   // مرفوض
        Assigned,   // استلمه الدليفري
        Delivered   // تم التوصيل
    }
}
