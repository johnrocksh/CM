namespace CastManager.Models.Desktops
{
    public enum DesktopStatus
    {
        NA, // service not found 
        Standby, // in pending state
        Active, // was active and now is active
        BecomeOnActive, // just become alive
        BecomeOnStandby, // was active and became standby
        Changed, // desktop config was changed on remote 
    }
}
