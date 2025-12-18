namespace VTP_Induction
{
    public interface CDevice
    {
        bool m_bConnection { get; }
        string m_sName { get; }
        bool Connect();
        void Disconnect();
    }
}
