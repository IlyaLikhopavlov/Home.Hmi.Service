namespace Home.Services.ConstantValues
{
    public static class DataPointIds
    {
        public const string LivingRoom = @"living";

        public const string BedRoom = @"bedroom";

        public const string MarinRoom = @"martin";

        public const string Study = @"study";

        public const string ControlSignal = @"ctrlsignal";

        public const string Outdoor = @"outdoor";

        public const string Hysteresis = @"hyst";

        public const string SetPoint = @"setpoint";

        public static readonly string[] MonitoringPoints = { LivingRoom, BedRoom, MarinRoom, Study, ControlSignal, Outdoor };

        public static readonly string[] Settings = { Hysteresis, SetPoint };
    }
}
